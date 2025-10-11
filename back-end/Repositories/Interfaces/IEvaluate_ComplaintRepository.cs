using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 投诉评价仓储接口
    /// </summary>
    public interface IEvaluate_ComplaintRepository
    {
        /// <summary>
        /// 获取所有投诉评价
        /// </summary>
        /// <returns>投诉评价列表</returns>
        Task<IEnumerable<Evaluate_Complaint>> GetAllAsync();

        /// <summary>
        /// 根据管理员与投诉ID获取投诉评价
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="complaintId">投诉ID</param>
        /// <returns>投诉评价</returns>
        Task<Evaluate_Complaint?> GetByIdAsync(int adminId, int complaintId);

        /// <summary>
        /// 添加投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        Task AddAsync(Evaluate_Complaint evaluateComplaint);

        /// <summary>
        /// 更新投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Evaluate_Complaint evaluateComplaint);

        /// <summary>
        /// 删除投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Evaluate_Complaint evaluateComplaint);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}