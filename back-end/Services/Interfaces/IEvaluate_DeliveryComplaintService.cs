using BackEnd.DTOs.DeliveryComplaint;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送投诉评估服务接口
    /// </summary>
    public interface IEvaluate_DeliveryComplaintService
    {
        /// <summary>
        /// 获取管理员的配送投诉列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>配送投诉列表</returns>
        Task<IEnumerable<GetComplaintInfo>> GetComplaintsForAdminAsync(int adminId);

        /// <summary>
        /// 更新配送投诉处理结果
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<SetComplaintInfoResponse> UpdateComplaintAsync(SetComplaintInfo request);
    }
}
