using BackEnd.DTOs.Review;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 评价服务
    /// </summary>
    public class ReviewService : IReviewService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IFoodOrderRepository _orderRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="commentRepository">评论仓储</param>
        /// <param name="orderRepository">订单仓储</param>
        public ReviewService(ICommentRepository commentRepository, IFoodOrderRepository orderRepository)
        {
            _commentRepository = commentRepository;
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="keyword">关键词</param>
        /// <returns>评价分页结果</returns>
        public async Task<RPageResultDto<ReviewDto>> GetReviewsAsync(int sellerId, int page, int pageSize, string? keyword)
        {
            var comments = await _commentRepository.GetBySellerAsync(sellerId);

            // 应用搜索过滤
            if (!string.IsNullOrEmpty(keyword))
            {
                comments = comments.Where(c =>
                    c.Content.Contains(keyword) ||
                    c.CommentID.ToString().Contains(keyword))
                    .ToList();
            }

            // 分页处理
            var total = comments.Count();
            var paginatedComments = comments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 转换为DTO
            var reviewDtos = paginatedComments.Select(c => new ReviewDto
            {
                Id = c.CommentID,
                OrderNo = $"ORD{c.StoreID}",  //对店铺评论，这里使用店铺ID
                User = new RUserInfoDto
                {
                    Name = c.Commenter?.User?.Username ?? "未知用户",
                    Phone = c.Commenter?.User?.PhoneNumber.ToString() ?? "未知电话",
                    Avatar = c.Commenter?.User?.Avatar ?? ""
                },
                Content = c.Content ?? "无评论内容",
                CreatedAt = c.PostedAt.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return new RPageResultDto<ReviewDto>
            {
                List = reviewDtos,
                Total = total
            };
        }

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="id">评论ID</param>
        /// <param name="replyDto">回复请求</param>
        /// <returns>回复响应</returns>
        public async Task<ReplyResponseDto> ReplyToReviewAsync(int id, ReplyDto replyDto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return new ReplyResponseDto
                {
                    Success = false,
                    Message = "评论不存在"
                };
            }

            // 更新原评论的回复数
            comment.Replies++;
            await _commentRepository.UpdateAsync(comment);

            return new ReplyResponseDto
            {
                Success = true,
                Message = "回复成功"
            };
        }
    }
}