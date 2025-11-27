using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 售后申请服务接口（商家侧）
    /// </summary>
    public interface IMerchantAfterSaleService
    {
        /// <summary>
        /// 获取售后申请列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="field">筛选字段（content | user.name | orderNo）</param>
        /// <returns>售后申请列表</returns>
        Task<PageResultDto<AfterSaleApplicationListItemDto>> GetAfterSalesAsync(int sellerId, int page, int pageSize, string? keyword, string? field);

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <returns>售后申请详情</returns>
        Task<AfterSaleApplicationListItemDto?> GetAfterSaleByIdAsync(int id);

        /// <summary>
        /// 商家在待处理状态下提交回复（状态将置为商家反馈）
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <param name="replyDto">商家回复内容</param>
        /// <returns>处理结果</returns>
        Task<ApiResponseDto> SubmitMerchantReplyAsync(int id, MerchantReplyDto replyDto);
    }
}
