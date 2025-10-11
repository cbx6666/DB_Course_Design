using System.Collections.Generic;
using System.Threading.Tasks;
using BackEnd.DTOs.User;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户首页服务接口
    /// </summary>
    public interface IUserHomepageService
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
        Task<(IEnumerable<HomeSearchGetDto> Stores, IEnumerable<HomeSearchGetDto> Dishes)>
            SearchAsync(HomeSearchDto searchDto);

        /// <summary>
        /// 获取订单历史
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单历史列表</returns>
        Task<List<HistoryOrderDto>> GetOrderHistoryAsync(int userId);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        Task<UserInfoResponse?> GetUserInfoAsync(int userId);

        /// <summary>
        /// 获取用户优惠券信息
        /// </summary>
        /// <param name="userIdDto">用户ID请求</param>
        /// <returns>优惠券列表</returns>
        Task<IEnumerable<CouponDto>> GetUserCouponsAsync(UserIdDto userIdDto);

        /// <summary>
        /// 获取所有店铺
        /// </summary>
        /// <returns>店铺列表</returns>
        Task<StoresResponseDto> GetAllStoresAsync();
    }
}
