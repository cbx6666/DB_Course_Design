using BackEnd.DTOs.Cart;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Order;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackEnd.Services
{
    /// <summary>
    /// 订单服务实现（商家侧）
    /// </summary>
    public class MerchantOrderService : IMerchantOrderService
    {
        private readonly IFoodOrderRepository _orderRepo;
        private readonly IShoppingCartItemRepository _cartItemRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MerchantOrderService>? _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantOrderService(
            IFoodOrderRepository orderRepo,
            IShoppingCartItemRepository cartItemRepo,
            ICouponRepository couponRepo,
            IServiceProvider serviceProvider,
            ILogger<MerchantOrderService>? logger = null)
        {
            _orderRepo = orderRepo;
            _cartItemRepo = cartItemRepo;
            _couponRepo = couponRepo;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        public async Task<IEnumerable<MerchantOrderViewDto>> GetOrdersAsync(int? sellerId, int? storeId)
        {
            var orders = await _orderRepo.GetAllAsync();

            // 筛选逻辑
            if (sellerId.HasValue)
                orders = orders.Where(o => o.Store.SellerID == sellerId.Value);
            if (storeId.HasValue)
                orders = orders.Where(o => o.StoreID == storeId.Value);

            var result = new List<MerchantOrderViewDto>();
            
            foreach (var o in orders)
            {
                // 获取购物车项目
                var cartItems = o.CartID.HasValue ? await GetCartItemsAsync(o.CartID.Value) : Enumerable.Empty<ShoppingCartItemDto>();
                
                // 获取订单使用的优惠券信息
                OrderCouponInfoDto? usedCoupon = null;
                if (o.Coupons != null && o.Coupons.Any())
                {
                    var coupon = o.Coupons.FirstOrDefault();
                    if (coupon != null && coupon.CouponManager != null)
                    {
                        usedCoupon = new OrderCouponInfoDto
                        {
                            CouponId = coupon.CouponID,
                            CouponName = coupon.CouponManager.CouponName,
                            Description = coupon.CouponManager.Description,
                            DiscountType = coupon.CouponManager.CouponType == CouponType.Fixed ? "fixed" : "discount",
                            DiscountValue = coupon.CouponManager.Value,
                            ValidFrom = coupon.CouponManager.ValidFrom.ToString("o"),
                            ValidTo = coupon.CouponManager.ValidTo.ToString("o"),
                            IsUsed = coupon.CouponState == CouponState.Used
                        };
                    }
                }
                
                result.Add(new MerchantOrderViewDto
                {
                    OrderId = o.OrderID,
                    PaymentTime = o.PaymentTime?.ToString("o") ?? string.Empty,
                    Remarks = o.Remarks,
                    CartId = o.CartID ?? 0,
                    StoreId = o.StoreID,
                    SellerId = o.Store.SellerID,
                    OrderState = o.FoodOrderState,
                    DeliveryTaskId = o.DeliveryTask?.TaskID,
                    DeliveryStatus = o.DeliveryTask != null ? (int)o.DeliveryTask.Status : -1,
                    Items = cartItems,
                    DeliveryAddress = o.DeliveryInfo?.Address,
                    DeliveryName = o.DeliveryInfo?.Name,
                    DeliveryPhone = o.DeliveryInfo?.PhoneNumber,
                    DeliveryFee = o.DeliveryFee,
                    UsedCoupon = usedCoupon
                });
            }
            
            return result;
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        public async Task<OrderDecisionDto> AcceptOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("订单不存在");

            // 修改订单状态为备菜中
            order.FoodOrderState = FoodOrderState.Preparing;
            await _orderRepo.UpdateAsync(order);

            return new OrderDecisionDto
            {
                OrderId = orderId,
                Decision = "accepted",
                DecidedAt = DateTime.Now.ToString("o")
            };
        }

        /// <summary>
        /// 标记为已出餐
        /// </summary>
        public async Task<OrderDecisionDto> MarkAsReadyAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("订单不存在");

            // 修改订单状态为已出餐
            order.FoodOrderState = FoodOrderState.Completed;
            await _orderRepo.UpdateAsync(order);

            // 更新店铺月销量
            try
            {
                var merchantRepo = _serviceProvider.GetService<Repositories.Interfaces.IMerchantRepository>();
                if (merchantRepo != null)
                {
                    await merchantRepo.IncrementStoreMonthlySalesAsync(order.StoreID);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "更新店铺月销量时发生错误，订单ID: {OrderId}, 店铺ID: {StoreId}", orderId, order.StoreID);
            }

            return new OrderDecisionDto
            {
                OrderId = orderId,
                Decision = "completed",
                DecidedAt = DateTime.Now.ToString("o")
            };
        }

        /// <summary>
        /// 获取购物车项
        /// </summary>
        public async Task<IEnumerable<ShoppingCartItemDto>> GetCartItemsAsync(int cartId)
        {
            var items = await _cartItemRepo.GetByCartIdAsync(cartId);
            return items.Select(it => new ShoppingCartItemDto
            {
                ItemId = it.ItemID,
                Quantity = it.Quantity,
                TotalPrice = it.TotalPrice,
                DishId = it.DishID,
                CartId = it.CartID,
                Dish = it.Dish != null ? new CartItemDishRefDto
                {
                    DishId = it.Dish.DishID,
                    DishName = it.Dish.DishName,
                    Price = it.Dish.Price,
                    Description = it.Dish.Description,
                    IsSoldOut = (int)it.Dish.IsSoldOut
                } : null
            });
        }

        /// <summary>
        /// 获取订单优惠券信息
        /// </summary>
        public async Task<IEnumerable<OrderCouponInfoDto>> GetOrderCouponsAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("订单不存在");

            return order.Coupons?.Select(c => new OrderCouponInfoDto
            {
                CouponId = c.CouponID,
                CouponName = $"优惠券{c.CouponID}",
                Description = $"满{c.CouponManager.MinimumSpend}减{c.CouponManager.Value}元",
                DiscountType = "fixed",
                DiscountValue = c.CouponManager.Value,
                ValidFrom = c.CouponManager.ValidFrom.ToString("o"),
                ValidTo = c.CouponManager.ValidTo.ToString("o"),
                IsUsed = true
            }) ?? Enumerable.Empty<OrderCouponInfoDto>();
        }
    }
}
