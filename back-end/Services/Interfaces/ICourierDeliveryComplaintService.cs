using BackEnd.DTOs.DeliveryComplaint;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送投诉服务接口（骑手侧）
    /// </summary>
    public interface ICourierDeliveryComplaintService
    {
        /// <summary>
        /// 获取骑手的投诉列表
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <returns>投诉列表</returns>
        Task<IEnumerable<CourierComplaintDto>> GetComplaintsAsync(int courierId);
    }
}
