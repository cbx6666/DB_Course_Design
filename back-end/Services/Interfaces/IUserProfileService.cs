using BackEnd.Models;
using BackEnd.DTOs.User;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户档案服务接口
    /// </summary>
    public interface IUserProfileService
    {
        /// <summary>
        /// 获取用户档案
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户档案</returns>
        Task<UserProfileDto?> GetUserProfileAsync(int userId);

        /// <summary>
        /// 获取用户地址
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户地址</returns>
        Task<UserAddressDto?> GetUserAddressAsync(int userId);
    }
}


