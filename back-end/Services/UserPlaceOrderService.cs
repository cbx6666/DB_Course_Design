using BackEnd.Models;
using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using BackEnd.Repositories.Interfaces;
using BackEnd.Models.Enums;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户下单服务
    /// </summary>
    public class UserPlaceOrderService : IUserPlaceOrderService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IFoodOrderRepository _foodOrderRepository;
        private readonly ICouponRepository _couponRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cartRepository">购物车仓储</param>
        /// <param name="foodOrderRepository">订单仓储</param>
        /// <param name="couponRepository">优惠券仓储</param>
        public UserPlaceOrderService(
            IShoppingCartRepository cartRepository, 
            IFoodOrderRepository foodOrderRepository,
            ICouponRepository couponRepository)
        {
            _cartRepository = cartRepository;
            _foodOrderRepository = foodOrderRepository;
            _couponRepository = couponRepository;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>响应结果</returns>
        public async Task<ResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var cart = await _cartRepository.GetByIdAsync(dto.CartId);
            if (cart == null || cart.ShoppingCartItems?.Count == 0)
            {
                return await Task.FromResult(new ResponseDto
                {
                    Success = false,
                    Message = "购物车为空，无法生成订单"
                });
            }

            // 计算订单总金额（商品总价）
            var orderTotal = cart.ShoppingCartItems?.Sum(item => item.TotalPrice) ?? 0;
            
            // 处理优惠券（如果提供了）
            Coupon? usedCoupon = null;
            if (dto.CouponId.HasValue && dto.CouponId.Value > 0)
            {
                // 获取优惠券
                usedCoupon = await _couponRepository.GetByIdAsync(dto.CouponId.Value);
                
                if (usedCoupon == null)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券不存在"
                    };
                }

                // 验证优惠券属于当前用户
                if (usedCoupon.CustomerID != dto.CustomerId)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券不属于当前用户"
                    };
                }

                // 验证优惠券状态
                if (usedCoupon.CouponState != CouponState.Unused)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券已使用或无效"
                    };
                }

                // 验证优惠券是否过期
                if (usedCoupon.IsExpired)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券已过期"
                    };
                }

                // 验证优惠券是否在有效期内
                var now = DateTime.Now;
                if (now < usedCoupon.CouponManager.ValidFrom || now > usedCoupon.CouponManager.ValidTo)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券不在有效期内"
                    };
                }

                // 验证优惠券是否属于当前店铺
                if (usedCoupon.CouponManager.StoreID != dto.StoreId)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = "优惠券不属于当前店铺"
                    };
                }

                // 验证最低消费金额
                if (orderTotal < usedCoupon.CouponManager.MinimumSpend)
                {
                    return new ResponseDto
                    {
                        Success = false,
                        Message = $"订单金额未达到优惠券使用门槛（需满{usedCoupon.CouponManager.MinimumSpend:F2}元）"
                    };
                }
            }

            var foodOrder = new FoodOrder
            {
                CustomerID = dto.CustomerId,
                CartID = dto.CartId,
                StoreID = dto.StoreId,
                DeliveryInfoID = dto.DeliveryInfoID,
                DeliveryFee = dto.DeliveryFee,
                OrderTime = DateTime.Now,
                PaymentTime = dto.PaymentTime,   // 下单时传入
                Remarks = dto.Remarks,
                FoodOrderState = FoodOrderState.Pending
            };

            await _foodOrderRepository.AddAsync(foodOrder);
            await _foodOrderRepository.SaveAsync();

            // 如果使用了优惠券，更新优惠券状态并关联订单
            if (usedCoupon != null)
            {
                usedCoupon.OrderID = foodOrder.OrderID;
                usedCoupon.CouponState = CouponState.Used;
                await _couponRepository.UpdateAsync(usedCoupon);
                await _couponRepository.SaveAsync();
            }

            // 下单成功后将购物车状态修改为已完成
            cart.ShoppingCartState = ShoppingCartState.Done;
            cart.LastUpdatedTime = DateTime.UtcNow;
            await _cartRepository.UpdateAsync(cart);

            return new ResponseDto
            {
                Success = true,
                Message = "订单创建成功"
            };
        }
    }
}