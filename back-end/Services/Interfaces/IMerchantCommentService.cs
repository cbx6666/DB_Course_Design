using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 评论服务接口（商家侧）
    /// </summary>
    public interface IMerchantCommentService
    {
        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="field">筛选字段（content | orderNo | user.name）</param>
        /// <returns>评价列表</returns>
        Task<PageResultDto<MerchantCommentDto>> GetReviewsAsync(int sellerId, int page, int pageSize, string? keyword, string? field);

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="id">评价ID</param>
        /// <param name="replyDto">回复请求</param>
        /// <returns>回复结果</returns>
        Task<ApiResponseDto> ReplyToReviewAsync(int id, ReplyCommentDto replyDto);
    }
}
