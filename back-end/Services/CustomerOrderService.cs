using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 订单服务实现（消费者侧）
    /// </summary>
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IFoodOrderRepository _orderRepo;
        private readonly IShoppingCartRepository _cartRepo;
        private readonly ICouponRepository _couponRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerOrderService(
            IFoodOrderRepository orderRepo,
            IShoppingCartRepository cartRepo,
            ICouponRepository couponRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _couponRepo = couponRepo;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        public async Task<ApiResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var cart = await _cartRepo.GetByIdAsync(dto.CartId);
            if (cart == null || cart.ShoppingCartItems?.Count == 0)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 400,
                    Message = "购物车为空，无法生成订单"
                };
            }

            var orderTotal = cart.ShoppingCartItems?.Sum(item => item.TotalPrice) ?? 0;
            
            Coupon? usedCoupon = null;
            if (dto.CouponId.HasValue && dto.CouponId.Value > 0)
            {
                usedCoupon = await _couponRepo.GetByIdAsync(dto.CouponId.Value);
                
                if (usedCoupon == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 404,
                        Message = "优惠券不存在"
                    };
                }

                if (usedCoupon.CustomerID != dto.CustomerId)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 403,
                        Message = "优惠券不属于当前用户"
                    };
                }

                if (usedCoupon.CouponState != CouponState.Unused)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券已使用或无效"
                    };
                }

                if (usedCoupon.IsExpired)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券已过期"
                    };
                }

                var now = DateTime.Now;
                if (now < usedCoupon.CouponManager.ValidFrom || now > usedCoupon.CouponManager.ValidTo)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券不在有效期内"
                    };
                }

                if (usedCoupon.CouponManager.StoreID != dto.StoreId)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 403,
                        Message = "优惠券不属于当前店铺"
                    };
                }

                if (orderTotal < usedCoupon.CouponManager.MinimumSpend)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
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
                PaymentTime = dto.PaymentTime,
                Remarks = dto.Remarks,
                FoodOrderState = FoodOrderState.Pending
            };

            await _orderRepo.AddAsync(foodOrder);
            await _orderRepo.SaveAsync();

            if (usedCoupon != null)
            {
                usedCoupon.OrderID = foodOrder.OrderID;
                usedCoupon.CouponState = CouponState.Used;
                await _couponRepo.UpdateAsync(usedCoupon);
                await _couponRepo.SaveAsync();
            }

            cart.ShoppingCartState = ShoppingCartState.Done;
            cart.LastUpdatedTime = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "订单创建成功"
            };
        }
    }
}
