using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.DeliveryTask;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 订单服务接口（消费者侧）
    /// </summary>
    public interface ICustomerOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建结果</returns>
        Task<ApiResponseDto> CreateOrderAsync(CreateOrderDto dto);

        /// <summary>
        /// 获取订单历史
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单历史列表</returns>
        Task<List<CustomerOrderViewDto>> GetOrderHistoryAsync(int userId);

        /// <summary>
        /// 获取订单配送信息（消费者侧）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送信息</returns>
        Task<OrderDeliveryInfoDto> GetOrderDeliveryInfoAsync(int orderId);
    }
}
