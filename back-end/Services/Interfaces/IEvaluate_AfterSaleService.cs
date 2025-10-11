using BackEnd.DTOs.AfterSaleApplication;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 售后申请评估服务接口
    /// </summary>
    public interface IEvaluate_AfterSaleService
    {
        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<GetAfterSaleApplicationInfo>> GetApplicationsForAdminAsync(int adminId);

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<SetAfterSaleApplicationResponse> UpdateAfterSaleApplicationAsync(SetAfterSaleApplicationInfo request);
    }
}
