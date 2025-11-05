using BackEnd.DTOs.Penalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺举报惩罚服务接口（管理员侧）
    /// </summary>
    public interface IAdminPenaltyService
    {
        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        Task<IEnumerable<AdminPenaltyDetailDto>> GetViolationPenaltiesForAdminAsync(int adminId);

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<UpdatePenaltyResponseDto> UpdateViolationPenaltyAsync(UpdatePenaltyDto request);
    }
}
