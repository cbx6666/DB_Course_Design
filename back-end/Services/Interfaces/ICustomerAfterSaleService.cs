using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 售后申请服务接口（消费者侧）
    /// </summary>
    public interface ICustomerAfterSaleService
    {
        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request">创建申请请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        Task<CreateAfterSaleApplicationResponseDto> CreateApplicationAsync(CreateAfterSaleApplicationDto request, int userId);

        /// <summary>
        /// 获取用户的售后申请列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>售后申请列表</returns>
        Task<List<CustomerAfterSaleListItemDto>> GetMyAfterSalesAsync(int userId);
    }
}
