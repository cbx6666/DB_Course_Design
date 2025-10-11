using BackEnd.DTOs.User;
using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 店铺仓储接口
    /// </summary>
    public interface IStoreRepository
    {
        /// <summary>
        /// 获取所有店铺（谨慎使用）
        /// </summary>
        /// <returns>店铺列表</returns>
        Task<IEnumerable<Store>> GetAllAsync();

        /// <summary>
        /// 根据ID获取店铺（谨慎使用）
        /// </summary>
        /// <param name="id">店铺ID</param>
        /// <returns>店铺</returns>
        Task<Store?> GetByIdAsync(int id);

        /// <summary>
        /// 根据商家ID获取店铺
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺</returns>
        Task<Store?> GetBySellerIdAsync(int sellerId);

        /// <summary>
        /// 根据商家ID获取店铺ID
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺ID</returns>
        Task<int?> GetStoreIdBySellerIdAsync(int sellerId);

        /// <summary>
        /// 获取用户端展示用的店铺信息（无关联）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>店铺</returns>
        Task<Store?> GetStoreInfoForUserAsync(int storeId);

        /// <summary>
        /// 根据店铺获取菜品
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<Dish>> GetDishesByStoreIdAsync(int storeId);

        /// <summary>
        /// 添加店铺
        /// </summary>
        /// <param name="store">店铺</param>
        /// <returns>任务</returns>
        Task AddAsync(Store store);

        /// <summary>
        /// 更新店铺
        /// </summary>
        /// <param name="store">店铺</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Store store);

        /// <summary>
        /// 删除店铺
        /// </summary>
        /// <param name="store">店铺</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Store store);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();

        /// <summary>
        /// 首页推荐：获取评分最高的店铺
        /// </summary>
        /// <param name="takeCount">数量</param>
        /// <returns>店铺展示DTO集合</returns>
        Task<IEnumerable<ShowStoreDto>> GetTopRatedStoresForHomepageAsync(int takeCount);

        /// <summary>
        /// 按店铺名称搜索
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>店铺展示DTO集合</returns>
        Task<IEnumerable<HomeSearchGetDto>> SearchStoresByNameAsync(string keyword);

        /// <summary>
        /// 按菜品名称搜索店铺
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns>店铺展示DTO集合</returns>
        Task<IEnumerable<HomeSearchGetDto>> SearchStoresByDishNameAsync(string keyword);

        /// <summary>
        /// 获取运营中的店铺
        /// </summary>
        /// <returns>店铺展示DTO集合</returns>
        Task<IEnumerable<ShowStoreDto>> GetOperationalStoresAsync();
    }
}