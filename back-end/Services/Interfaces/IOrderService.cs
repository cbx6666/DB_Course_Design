using BackEnd.DTOs.Cart;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Order;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 订单服务接口
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>订单列表</returns>
        Task<IEnumerable<FoodOrderDto>> GetOrdersAsync(int? sellerId, int? storeId);

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单详情</returns>
        Task<FoodOrderDto?> GetOrderByIdAsync(int orderId);

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单决策结果</returns>
        Task<OrderDecisionDto> AcceptOrderAsync(int orderId);

        /// <summary>
        /// 标记订单为已准备
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单决策结果</returns>
        Task<OrderDecisionDto> MarkAsReadyAsync(int orderId);

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="reason">拒绝原因</param>
        /// <returns>订单决策结果</returns>
        Task<OrderDecisionDto> RejectOrderAsync(int orderId, string? reason);

        /// <summary>
        /// 获取购物车商品
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车商品列表</returns>
        Task<IEnumerable<ShoppingCartItemDto>> GetCartItemsAsync(int cartId);

        /// <summary>
        /// 获取订单优惠券信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>优惠券信息列表</returns>
        Task<IEnumerable<OrderCouponInfoDto>> GetOrderCouponsAsync(int orderId);
    }
}