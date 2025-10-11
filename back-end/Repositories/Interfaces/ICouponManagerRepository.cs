using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 优惠券管理仓储接口
    /// </summary>
    public interface ICouponManagerRepository
    {
        /// <summary>
        /// 获取所有优惠券管理
        /// </summary>
        /// <returns>优惠券管理列表</returns>
        Task<IEnumerable<CouponManager>> GetAllAsync();

        /// <summary>
        /// 根据ID获取优惠券管理
        /// </summary>
        /// <param name="id">优惠券管理ID</param>
        /// <returns>优惠券管理</returns>
        Task<CouponManager?> GetByIdAsync(int id);

        /// <summary>
        /// 添加优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        Task AddAsync(CouponManager couponManager);

        /// <summary>
        /// 更新优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        Task UpdateAsync(CouponManager couponManager);

        /// <summary>
        /// 删除优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        Task DeleteAsync(CouponManager couponManager);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();

        /// <summary>
        /// 根据店铺ID分页获取优惠券
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>优惠券列表和总数</returns>
        Task<(IEnumerable<CouponManager> coupons, int total)> GetByStoreIdAsync(int storeId, int page, int pageSize);

        /// <summary>
        /// 根据店铺ID获取优惠券统计信息
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>统计数据</returns>
        Task<(int total, int active, int expired, int upcoming, int totalUsed, decimal totalDiscountAmount)> GetStatsByStoreIdAsync(int storeId);

        /// <summary>
        /// 根据店铺和优惠券ID获取优惠券
        /// </summary>
        /// <param name="id">优惠券管理ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>优惠券管理</returns>
        Task<CouponManager?> GetByIdAndStoreIdAsync(int id, int storeId);

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="ids">优惠券ID集合</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>删除数量</returns>
        Task<int> BatchDeleteAsync(IEnumerable<int> ids, int storeId);
    }
}