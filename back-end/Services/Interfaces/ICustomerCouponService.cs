using BackEnd.DTOs.Coupon;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 优惠券服务接口（消费者侧）
    /// </summary>
    public interface ICustomerCouponService
    {
        /// <summary>
        /// 获取用户优惠券列表（用于结账页面）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>优惠券列表</returns>
        Task<List<CustomerCouponDto>> GetUserCouponsAsync(int userId);

        /// <summary>
        /// 获取用户优惠券信息（用于我的优惠券页面）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>优惠券列表</returns>
        Task<IEnumerable<CouponDto>> GetUserCouponListAsync(int userId);

        /// <summary>
        /// 获取所有可领取的优惠券
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>可领取优惠券列表</returns>
        Task<List<AvailableCouponDto>> GetAvailableCouponsAsync(int userId);

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="couponManagerId">优惠券管理ID</param>
        /// <returns>领取结果</returns>
        Task<bool> ClaimCouponAsync(int userId, int couponManagerId);
    }
}
