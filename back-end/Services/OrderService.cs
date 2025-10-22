using BackEnd.DTOs.Cart;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Order;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

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

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="orderRepo">订单仓储</param>
        /// <param name="cartItemRepo">购物车项仓储</param>
        /// <param name="storeRepo">店铺仓储</param>
        public OrderService(IFoodOrderRepository orderRepo,
                           IShoppingCartItemRepository cartItemRepo,
                           IStoreRepository storeRepo)
        {
            _orderRepo = orderRepo;
            _cartItemRepo = cartItemRepo;
            _storeRepo = storeRepo;
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

            return orders.Select(o => new FoodOrderDto
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
                DeliveryStatus = o.DeliveryTask != null ? (int)o.DeliveryTask.Status : -1
            });
        }

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单信息</returns>
        public async Task<FoodOrderDto?> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderRepo.GetByIdAsync(orderId);
            if (order == null) return null;

            return new FoodOrderDto
            {
                OrderId = order.OrderID,
                PaymentTime = order.PaymentTime?.ToString("o") ?? string.Empty,
                Remarks = order.Remarks,
                CustomerId = order.CustomerID,
                CartId = order.CartID ?? 0,
                StoreId = order.StoreID,
                SellerId = order.Store.SellerID,
                OrderState = order.FoodOrderState,
                DeliveryTaskId = order.DeliveryTask?.TaskID,
                DeliveryStatus = order.DeliveryTask != null ? (int)order.DeliveryTask.Status : -1
            };
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

            return new OrderDecisionDto
            {
                OrderId = orderId,
                Decision = "completed",
                DecidedAt = DateTime.Now.ToString("o")
            };
        }

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="reason">拒绝原因</param>
        /// <returns>订单决策结果</returns>
        public async Task<OrderDecisionDto> RejectOrderAsync(int orderId, string? reason)
        {
            var order = await _orderRepo.GetByIdAsync(orderId)
                ?? throw new KeyNotFoundException("订单不存在");

            return new OrderDecisionDto
            {
                OrderId = orderId,
                Decision = "rejected",
                DecidedAt = DateTime.Now.ToString("o"),
                Reason = reason
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