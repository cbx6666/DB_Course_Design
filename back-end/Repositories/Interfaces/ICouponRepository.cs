using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 优惠券仓储接口
    /// </summary>
    public interface ICouponRepository
    {
        /// <summary>
        /// 获取所有优惠券
        /// </summary>
        /// <returns>优惠券列表</returns>
        Task<IEnumerable<Coupon>> GetAllAsync();

        /// <summary>
        /// 根据ID获取优惠券
        /// </summary>
        /// <param name="id">优惠券ID</param>
        /// <returns>优惠券</returns>
        Task<Coupon?> GetByIdAsync(int id);

        /// <summary>
        /// 根据客户ID获取优惠券
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>优惠券列表</returns>
        Task<IEnumerable<Coupon>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        Task AddAsync(Coupon coupon);

        /// <summary>
        /// 更新优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Coupon coupon);

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Coupon coupon);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}