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
        /// 获取推荐商家
        /// </summary>
        /// <returns>推荐商家信息</returns>
        Task<HomeRecmDto> GetRecommendedStoresAsync();

        /// <summary>
        /// 搜索商家和菜品
        /// </summary>
        /// <param name="searchDto">搜索请求</param>
        /// <returns>商家和菜品搜索结果</returns>
        Task<(IEnumerable<ShowStoreDto> Stores, IEnumerable<ShowStoreDto> Dishes)> SearchAsync(HomeSearchDto searchDto);

        /// <summary>
        /// 获取订单历史
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单历史列表</returns>
        Task<List<CustomerOrderViewDto>> GetOrderHistoryAsync(int userId);

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
        /// 获取所有店铺
        /// </summary>
        /// <returns>店铺列表</returns>
        Task<StoresResponseDto> GetAllStoresAsync();
    }
}
