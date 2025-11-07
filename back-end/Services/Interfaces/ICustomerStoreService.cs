using BackEnd.DTOs.Menu;
using BackEnd.DTOs.DishCategory;
using BackEnd.DTOs.Store;
using BackEnd.DTOs.Customer;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺服务接口（消费者侧）
    /// </summary>
    public interface ICustomerStoreService
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
        /// 获取所有店铺
        /// </summary>
        /// <returns>店铺列表</returns>
        Task<StoresResponseDto> GetAllStoresAsync();

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>店铺信息</returns>
        Task<StoreResponseDto?> GetStoreInfoAsync(int storeId);

        /// <summary>
        /// 获取店铺的菜品种类列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>菜品种类列表</returns>
        Task<List<CategoryResponseDto>> GetStoreCategoriesAsync(int storeId);

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>菜单列表</returns>
        Task<List<MenuResponseDto>> GetMenuAsync(int storeId);
    }
}
