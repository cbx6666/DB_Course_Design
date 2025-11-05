using BackEnd.DTOs.Penalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺举报惩罚服务接口（消费者侧）
    /// </summary>
    public interface ICustomerPenaltyService
    {
        /// <summary>
        /// 举报店铺
        /// </summary>
        /// <param name="dto">举报请求</param>
        /// <returns>举报任务</returns>
        Task SubmitStoreReportAsync(ReportStoreDto dto);
    }
}
