using BackEnd.DTOs.AuthRequest;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 注册服务接口
    /// </summary>
    public interface IRegisterService
    {
        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>注册结果</returns>
        Task<RegisterResult> RegisterAsync(RegisterRequest request);
    }
}
