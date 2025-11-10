using BackEnd.DTOs.Penalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺举报服务接口（消费者侧）
    /// </summary>
    public interface ICustomerStoreReportService
    {
        /// <summary>
        /// 举报店铺
        /// </summary>
        /// <param name="dto">举报请求</param>
        /// <returns>举报任务</returns>
        Task SubmitStoreReportAsync(ReportStoreDto dto);

        /// <summary>
        /// 获取用户的店铺举报列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>店铺举报列表</returns>
        Task<List<CustomerStoreReportListItemDto>> GetMyReportsAsync(int userId);
    }
}

