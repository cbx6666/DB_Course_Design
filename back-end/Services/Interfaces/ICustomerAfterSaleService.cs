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

        /// <summary>
        /// 提交（或更新）售后申请评分（仅已完成的申请）
        /// </summary>
        /// <param name="applicationId">申请ID</param>
        /// <param name="userId">用户ID</param>
        /// <param name="score">评分（0-5）</param>
        /// <returns>操作结果</returns>
        Task<ApiResponseDto> SubmitAfterSaleRatingAsync(int applicationId, int userId, int score);
    }
}
