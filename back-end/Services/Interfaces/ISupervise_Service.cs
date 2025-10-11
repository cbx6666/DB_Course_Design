using BackEnd.DTOs.ViolationPenalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 监督服务接口
    /// </summary>
    public interface ISupervise_Service
    {
        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        Task<IEnumerable<GetViolationPenaltyInfo>> GetViolationPenaltiesForAdminAsync(int adminId);

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<SetViolationPenaltyInfoResponse> UpdateViolationPenaltyAsync(SetViolationPenaltyInfo request);
    }
}
