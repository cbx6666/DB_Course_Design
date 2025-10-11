using BackEnd.DTOs.User;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户调试服务接口
    /// </summary>
    public interface IUserDebugService
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        Task<UserInfoResponseDto> GetUserInfoAsync(int userId);

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="dto">提交订单请求</param>
        /// <returns>提交任务</returns>
        Task SubmitOrderAsync(SubmitOrderRequestDto dto);

        /// <summary>
        /// 获取用户ID
        /// </summary>
        /// <param name="dto">获取用户ID请求</param>
        /// <returns>用户ID响应</returns>
        Task<GetUserIdResponseDto> GetUserIdAsync(GetUserIdRequestDto dto);

        /// <summary>
        /// 使用优惠券
        /// </summary>
        /// <param name="couponId">优惠券ID</param>
        /// <returns>使用任务</returns>
        Task UseCouponAsync(int couponId);
    }
}