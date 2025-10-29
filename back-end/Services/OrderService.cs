using BackEnd.DTOs.Cart;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Order;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BackEnd.Services
{
    /// <summary>
    /// 订单服务
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IFoodOrderRepository _orderRepo;
        private readonly IShoppingCartItemRepository _cartItemRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderService>? _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="orderRepo">订单仓储</param>
        /// <param name="cartItemRepo">购物车项仓储</param>
        /// <param name="storeRepo">店铺仓储</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="logger">日志记录器</param>
        public OrderService(IFoodOrderRepository orderRepo,
                           IShoppingCartItemRepository cartItemRepo,
                           IStoreRepository storeRepo,
                           IServiceProvider serviceProvider,
                           ILogger<OrderService>? logger = null)
        {
            _orderRepo = orderRepo;
            _cartItemRepo = cartItemRepo;
            _storeRepo = storeRepo;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>订单列表</returns>
        public async Task<IEnumerable<FoodOrderDto>> GetOrdersAsync(int? sellerId, int? storeId)
        {
            var orders = await _orderRepo.GetAllAsync();

            // 筛选逻辑
            if (sellerId.HasValue)
                orders = orders.Where(o => o.Store.SellerID == sellerId.Value);
            if (storeId.HasValue)
                orders = orders.Where(o => o.StoreID == storeId.Value);

            var result = new List<FoodOrderDto>();
            
            foreach (var o in orders)
            {
                // 获取购物车项目
                var cartItems = o.CartID.HasValue ? await GetCartItemsAsync(o.CartID.Value) : Enumerable.Empty<ShoppingCartItemDto>();
                
                // 获取订单使用的优惠券信息
                OrderCouponInfoDto? usedCoupon = null;
                if (o.Coupons != null && o.Coupons.Any())
                {
                    var coupon = o.Coupons.FirstOrDefault(); // 一个订单通常只有一个优惠券
                    if (coupon != null && coupon.CouponManager != null)
                    {
                        usedCoupon = new OrderCouponInfoDto
                        {
                            CouponId = coupon.CouponID,
                            CouponName = coupon.CouponManager.CouponName,
                            Description = coupon.CouponManager.Description,
                            DiscountType = coupon.CouponManager.CouponType == Models.Enums.CouponType.Fixed ? "fixed" : "discount",
                            DiscountValue = coupon.CouponManager.Value,
                            ValidFrom = coupon.CouponManager.ValidFrom.ToString("o"),
                            ValidTo = coupon.CouponManager.ValidTo.ToString("o"),
                            IsUsed = coupon.CouponState == Models.Enums.CouponState.Used
                        };
                    }
                }
                
                result.Add(new FoodOrderDto
                {
                    OrderId = o.OrderID,
                    PaymentTime = o.PaymentTime?.ToString("o") ?? string.Empty,
                    Remarks = o.Remarks,
                    CustomerId = o.CustomerID,
                    CartId = o.CartID ?? 0,
                    StoreId = o.StoreID,
                    SellerId = o.Store.SellerID,
                    OrderState = o.FoodOrderState,
                    DeliveryTaskId = o.DeliveryTask?.TaskID,
                    DeliveryStatus = o.DeliveryTask != null ? (int)o.DeliveryTask.Status : -1,
                    Items = cartItems,
                    // 配送信息
                    DeliveryAddress = o.DeliveryInfo?.Address,
                    DeliveryName = o.DeliveryInfo?.Name,
                    DeliveryPhone = o.DeliveryInfo?.PhoneNumber,
                    DeliveryFee = o.DeliveryFee,
                    // 优惠券信息
                    UsedCoupon = usedCoupon
                });
            }
            
            return result;
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单决策结果</returns>
        public async Task<OrderDecisionDto> AcceptOrderAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("订单不存在");

            // 修改订单状态为备菜中
            order.FoodOrderState = FoodOrderState.Preparing;
            await _orderRepo.UpdateAsync(order); // 保存修改

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
        /// <param name="orderId">订单ID</param>
        /// <returns>订单决策结果</returns>
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
                // 记录日志但不影响主要业务流程
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
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车项列表</returns>
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
        /// <param name="orderId">订单ID</param>
        /// <returns>优惠券信息列表</returns>
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