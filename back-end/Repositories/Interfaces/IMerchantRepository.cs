using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 商家仓储接口
    /// </summary>
    public interface IMerchantRepository
    {
        /// <summary>
        /// 根据商家ID获取店铺信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺</returns>
        Task<Store?> GetStoreBySellerIdAsync(int sellerId);

        /// <summary>
        /// 根据商家ID获取商家信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>商家</returns>
        Task<Seller?> GetSellerByIdAsync(int sellerId);

        /// <summary>
        /// 根据商家ID获取用户信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>用户</returns>
        Task<User?> GetUserBySellerIdAsync(int sellerId);

        /// <summary>
        /// 更新店铺信息
        /// </summary>
        /// <param name="store">店铺</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateStoreAsync(Store store);

        /// <summary>
        /// 获取店铺评分（预留）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评分</returns>
        Task<decimal> GetStoreRatingAsync(int storeId);

        /// <summary>
        /// 获取店铺月销量
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>销量</returns>
        Task<int> GetStoreMonthlySalesAsync(int storeId);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}