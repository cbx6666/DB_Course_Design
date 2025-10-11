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

        /// <summary>
        /// 更新账户信息
        /// </summary>
        /// <param name="dto">更新账户请求</param>
        /// <returns>更新结果</returns>
        Task<ResponseDto> UpdateAccountAsync(UpdateAccountDto dto);

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="file">头像文件</param>
        /// <returns>头像URL</returns>
        Task<string> UpdateUserAvatarAsync(int userId, IFormFile file);

        /// <summary>
        /// 保存或更新地址
        /// </summary>
        /// <param name="dto">地址请求</param>
        /// <returns>保存结果</returns>
        Task<ResponseDto> SaveOrUpdateAddressAsync(SaveAddressDto dto);
    }
}