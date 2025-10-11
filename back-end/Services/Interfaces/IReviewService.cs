using BackEnd.DTOs.Review;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 评价服务接口
    /// </summary>
    public interface IReviewService
    {
        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>评价列表</returns>
        Task<RPageResultDto<ReviewDto>> GetReviewsAsync(int sellerId, int page, int pageSize, string? keyword);

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="id">评价ID</param>
        /// <param name="replyDto">回复请求</param>
        /// <returns>回复结果</returns>
        Task<ReplyResponseDto> ReplyToReviewAsync(int id, ReplyDto replyDto);
    }
}