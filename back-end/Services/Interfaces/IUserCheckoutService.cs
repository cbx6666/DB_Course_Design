using BackEnd.DTOs.User;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户结账服务接口
    /// </summary>
    public interface IUserCheckoutService
    {
        /// <summary>
        /// 获取用户优惠券列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>优惠券列表</returns>
        Task<List<UserCouponDto>> GetUserCouponsAsync(int userId);

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>购物车信息</returns>
        Task<CartResponseDto> GetShoppingCartAsync(int userId, int storeId);

        /// <summary>
        /// 更新购物车商品
        /// </summary>
        /// <param name="dto">更新请求</param>
        /// <returns>更新任务</returns>
        Task UpdateCartItemAsync(UpdateCartItemDto dto);

        /// <summary>
        /// 移除购物车商品
        /// </summary>
        /// <param name="dto">移除请求</param>
        /// <returns>移除任务</returns>
        Task RemoveCartItemAsync(RemoveCartItemDto dto);
    }
}