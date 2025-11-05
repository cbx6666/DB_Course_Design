using BackEnd.DTOs.DeliveryTask;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送任务服务接口（商家侧）
    /// </summary>
    public interface IMerchantDeliveryTaskService
    {
        /// <summary>
        /// 发布配送任务
        /// </summary>
        /// <param name="dto">发布任务请求</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>发布结果（前端不需要返回数据，只返回成功即可）</returns>
        Task<bool> PublishDeliveryTaskAsync(
            CreateDeliveryTaskDto dto, int sellerId);

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单配送信息</returns>
        Task<OrderDeliveryInfoDto> GetOrderDeliveryInfoAsync(int orderId);
    }
}
