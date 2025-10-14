using BackEnd.DTOs.User;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户下单服务接口
    /// </summary>
    public interface IUserPlaceOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建结果</returns>
        Task<ResponseDto> CreateOrderAsync(CreateOrderDto dto);

        // 账户与地址相关接口已迁移至 IUserProfileService
    }
}