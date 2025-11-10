using BackEnd.DTOs.Penalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺举报服务接口（骑手侧）
    /// </summary>
    public interface ICourierStoreReportService
    {
        /// <summary>
        /// 举报店铺
        /// </summary>
        /// <param name="dto">举报请求</param>
        /// <returns>举报任务</returns>
        Task SubmitStoreReportAsync(ReportStoreDto dto);

        /// <summary>
        /// 获取骑手的店铺举报列表
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <returns>店铺举报列表</returns>
        Task<List<CustomerStoreReportListItemDto>> GetMyReportsAsync(int courierId);
    }
}

