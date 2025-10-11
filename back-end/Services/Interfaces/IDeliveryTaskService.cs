using BackEnd.DTOs.Delivery;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送任务服务接口
    /// </summary>
    public interface IDeliveryTaskService
    {
        /// <summary>
        /// 发布配送任务
        /// </summary>
        /// <param name="dto">发布任务请求</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>配送任务和发布信息</returns>
        Task<(DeliveryTaskDto? DeliveryTask, PublishTaskDto? Publish)> PublishDeliveryTaskAsync(
            PublishDeliveryTaskDto dto, int sellerId);

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单配送信息</returns>
        Task<OrderDeliveryInfoDto> GetOrderDeliveryInfoAsync(int orderId);
    }
}