using BackEnd.DTOs.AuthRequest;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 登录服务接口
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request">登录请求</param>
        /// <returns>登录结果</returns>
        Task<LoginResult> LoginAsync(LoginRequest request);

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>登出任务</returns>
        Task LogoutAsync(int userId);
    }
}
