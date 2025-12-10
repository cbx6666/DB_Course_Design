using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Store;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 客户信息服务接口（整合用户首页和用户档案功能）
    /// </summary>
    public interface ICustomerInfoService
    {
        /// <summary>
        /// 获取用户档案
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户档案</returns>
        Task<UserProfileDto?> GetUserProfileAsync(int userId);

        /// <summary>
        /// 获取用户全部收货地址列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>收货地址列表</returns>
        Task<List<UserDeliveryInfoDto>> GetUserAddressesAsync(int userId);

        /// <summary>
        /// 更新账户信息（姓名、头像）
        /// </summary>
        Task<ApiResponseDto> UpdateAccountAsync(UpdateAccountDto dto);

        /// <summary>
        /// 新建收货地址
        /// </summary>
        Task<ApiResponseDto> CreateAddressAsync(int userId, CreateAddressDto dto);

        /// <summary>
        /// 更新收货地址
        /// </summary>
        Task<ApiResponseDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto);

        /// <summary>
        /// 删除收货地址
        /// </summary>
        Task<ApiResponseDto> DeleteAddressAsync(int userId, int addressId);

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        Task<ApiResponseDto> SetDefaultAddressAsync(int userId, int addressId);

        /// <summary>
        /// 获取用户的收藏夹列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>收藏夹列表</returns>
        Task<List<FavoritesFolderDto>> GetFavoritesFoldersAsync(int userId);
    }
}
