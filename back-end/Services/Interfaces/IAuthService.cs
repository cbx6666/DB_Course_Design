using BackEnd.DTOs.AuthRequest;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 认证服务接口（登录和注册）
    /// </summary>
    public interface IAuthService
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

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>注册结果</returns>
        Task<ApiResponseDto> RegisterAsync(RegisterRequest request);
    }
}
