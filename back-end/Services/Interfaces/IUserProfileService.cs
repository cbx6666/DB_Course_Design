using BackEnd.DTOs.User;
using Microsoft.AspNetCore.Http;

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

        /// <summary>
        /// 获取用户全部收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>收货地址列表</returns>
        Task<List<UserDeliveryInfoDto>> GetUserAddressesAsync(int userId);

        /// <summary>
        /// 更新账户信息（姓名、头像）
        /// </summary>
        Task<ResponseDto> UpdateAccountAsync(UpdateAccountDto dto);

        /// <summary>
        /// 更新用户头像（供内部复用）
        /// </summary>
        Task<string> UpdateUserAvatarAsync(int userId, IFormFile file);

        /// <summary>
        /// 保存或更新默认收货地址
        /// </summary>
        Task<ResponseDto> SaveOrUpdateAddressAsync(SaveAddressDto dto);

        /// <summary>
        /// 新建收货地址
        /// </summary>
        Task<ResponseDto> CreateAddressAsync(int userId, CreateAddressDto dto);

        /// <summary>
        /// 更新收货地址
        /// </summary>
        Task<ResponseDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto);

        /// <summary>
        /// 删除收货地址
        /// </summary>
        Task<ResponseDto> DeleteAddressAsync(int userId, int addressId);

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        Task<ResponseDto> SetDefaultAddressAsync(int userId, int addressId);
    }
}


