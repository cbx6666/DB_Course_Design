using BackEnd.DTOs.Menu;
using BackEnd.DTOs.DishCategory;
using BackEnd.DTOs.Store;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺服务接口（消费者侧）
    /// </summary>
    public interface ICustomerStoreService
    {
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
