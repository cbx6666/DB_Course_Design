using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 订单服务接口（消费者侧）
    /// </summary>
    public interface ICustomerOrderService
    {
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建结果</returns>
        Task<ApiResponseDto> CreateOrderAsync(CreateOrderDto dto);
    }
}
