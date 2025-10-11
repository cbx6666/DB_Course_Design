using BackEnd.DTOs.DeliveryComplaint;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 创建配送投诉服务接口
    /// </summary>
    public interface ICreateComplaintService
    {
        /// <summary>
        /// 创建配送投诉
        /// </summary>
        /// <param name="request">创建投诉请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        Task<CreateComplaintResult> CreateComplaintAsync(CreateComplaintDto request, int userId);
    }
}
