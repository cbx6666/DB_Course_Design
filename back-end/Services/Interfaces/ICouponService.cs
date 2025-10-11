using BackEnd.DTOs.Merchant;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 优惠券服务接口
    /// </summary>
    public interface ICouponService
    {
        /// <summary>
        /// 获取优惠券列表（分页）
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>优惠券列表</returns>
        Task<CouponListResponseDto> GetCouponsAsync(int sellerId, int page, int pageSize);

        /// <summary>
        /// 获取优惠券统计信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>统计信息</returns>
        Task<CouponStatsDto> GetStatsAsync(int sellerId);

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">创建请求</param>
        /// <returns>创建结果</returns>
        Task<CreateCouponResponseDto> CreateCouponAsync(int sellerId, CreateCouponRequestDto request);

        /// <summary>
        /// 更新优惠券
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新任务</returns>
        Task UpdateCouponAsync(int sellerId, UpdateCouponRequestDto request);

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="couponId">优惠券ID</param>
        /// <returns>删除任务</returns>
        Task DeleteCouponAsync(int sellerId, int couponId);

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">批量删除请求</param>
        /// <returns>批量删除结果</returns>
        Task<BatchDeleteResponseDto> BatchDeleteCouponsAsync(int sellerId, BatchDeleteCouponsRequestDto request);

        /// <summary>
        /// 检查数据库连接状态
        /// </summary>
        /// <returns>数据库健康状态</returns>
        Task<bool> CheckDatabaseHealthAsync();
    }
}
