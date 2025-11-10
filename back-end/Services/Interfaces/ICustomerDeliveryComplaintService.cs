using BackEnd.DTOs.DeliveryComplaint;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送投诉服务接口（消费者侧）
    /// </summary>
    public interface ICustomerDeliveryComplaintService
    {
        /// <summary>
        /// 创建配送投诉
        /// </summary>
        /// <param name="request">创建投诉请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        Task<CreateDeliveryComplaintResponseDto> CreateComplaintAsync(CreateDeliveryComplaintDto request, int userId);

        /// <summary>
        /// 获取用户的配送投诉列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>配送投诉列表</returns>
        Task<List<CustomerDeliveryComplaintListItemDto>> GetMyComplaintsAsync(int userId);
    }
}
