using System.Threading.Tasks;
using BackEnd.DTOs.AfterSale;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 售后服务接口
    /// </summary>
    public interface IAfterSaleService
    {
        /// <summary>
        /// 获取售后申请列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>售后申请列表</returns>
        Task<APageResultDto<AfterSaleApplicationDto>> GetAfterSalesAsync(int sellerId, int page, int pageSize, string? keyword);

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <returns>售后申请详情</returns>
        Task<AfterSaleApplicationDto?> GetAfterSaleByIdAsync(int id);

        /// <summary>
        /// 处理售后申请
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <param name="processDto">处理请求</param>
        /// <returns>处理结果</returns>
        Task<ProcessResponseDto> ProcessAfterSaleAsync(int id, ProcessAfterSaleDto processDto);
    }
}