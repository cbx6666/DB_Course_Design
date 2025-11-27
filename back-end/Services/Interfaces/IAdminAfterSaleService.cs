using BackEnd.DTOs.AfterSaleApplication;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 售后申请服务接口（管理员侧）
    /// </summary>
    public interface IAdminAfterSaleService
    {
        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<AfterSaleApplicationDetailDto>> GetApplicationsForAdminAsync(int adminId);

        /// <summary>
        /// 根据申请ID获取售后详情
        /// </summary>
        /// <param name="applicationId">申请ID</param>
        /// <returns>售后详情</returns>
        Task<AfterSaleApplicationDetailDto?> GetApplicationDetailAsync(int applicationId);

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<UpdateAfterSaleApplicationResponseDto> UpdateAfterSaleApplicationAsync(UpdateAfterSaleApplicationDto request);
    }
}
