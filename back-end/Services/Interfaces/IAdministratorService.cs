using BackEnd.DTOs.Administrator;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 管理员服务接口
    /// </summary>
    public interface IAdministratorService
    {
        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>管理员信息</returns>
        Task<GetAdminInfo?> GetAdministratorInfoAsync(int adminId);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<SetAdminInfoResponse> UpdateAdministratorInfoAsync(int adminId, SetAdminInfo request);
    }
}
