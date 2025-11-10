using BackEnd.DTOs.Comment;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 评论服务实现（管理员侧）
    /// </summary>
    public class AdminCommentService : IAdminCommentService
    {
        private readonly ICommentRepository _commentRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminCommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// 获取管理员的评论列表
        /// </summary>
        public async Task<IEnumerable<AdminCommentDetailDto>> GetCommentsForAdminAsync(int adminId)
        {
            var commentsFromDb = await _commentRepository.GetByAdminIdAsync(adminId);

            if (commentsFromDb == null || !commentsFromDb.Any())
            {
                return Enumerable.Empty<AdminCommentDetailDto>();
            }

            return commentsFromDb.Select(comment => new AdminCommentDetailDto
            {
                ReviewId = comment.CommentID.ToString(),
                Username = comment.Commenter?.User?.Username ?? "未知用户",
                Content = comment.Content,
                Image = comment.CommentImage,
                Type = GetCommentTypeString(comment.CommentType),
                Rating = comment.Rating ?? 0,
                SubmitTime = comment.PostedAt.ToString("yyyy-MM-dd HH:mm"),
                Status = GetCommentStatusString(comment.CommentState)
            });
        }

        /// <summary>
        /// 更新评论
        /// </summary>
        public async Task<UpdateCommentReviewResponseDto> UpdateCommentAsync(UpdateCommentReviewDto request)
        {
            try
            {
                if (request == null)
                {
                    return new UpdateCommentReviewResponseDto
                    {
                        Success = false,
                        Message = "请求数据不能为空"
                    };
                }

                if (!int.TryParse(request.ReviewId, out int commentId))
                {
                    return new UpdateCommentReviewResponseDto
                    {
                        Success = false,
                        Message = "无效的评论编号格式"
                    };
                }

                var existingComment = await _commentRepository.GetByIdAsync(commentId);
                if (existingComment == null)
                {
                    return new UpdateCommentReviewResponseDto
                    {
                        Success = false,
                        Message = "未找到指定的评论"
                    };
                }

                var newState = request.Status switch
                {
                    "待处理" => CommentState.Pending,
                    "已完成" => CommentState.Completed,
                    "违规" => CommentState.Illegal,
                    _ => (CommentState?)null
                };

                if (newState == null)
                {
                    return new UpdateCommentReviewResponseDto
                    {
                        Success = false,
                        Message = "无效的状态值，只能是：待处理、已完成、违规"
                    };
                }

                if (existingComment.CommentState == CommentState.Completed ||
                    existingComment.CommentState == CommentState.Illegal)
                {
                    return new UpdateCommentReviewResponseDto
                    {
                        Success = false,
                        Message = "该评论已经处理完成，无法重复处理"
                    };
                }

                existingComment.CommentState = newState.Value;

                await _commentRepository.UpdateAsync(existingComment);

                var updatedCommentDto = new AdminCommentDetailDto
                {
                    ReviewId = existingComment.CommentID.ToString(),
                    Username = existingComment.Commenter.User.Username,
                    Content = existingComment.Content,
                    Image = existingComment.CommentImage,
                    Type = GetCommentTypeString(existingComment.CommentType),
                    Rating = existingComment.Rating ?? 0,
                    SubmitTime = existingComment.PostedAt.ToString("yyyy-MM-dd HH:mm"),
                    Status = GetCommentStatusString(existingComment.CommentState)
                };

                return new UpdateCommentReviewResponseDto
                {
                    Success = true,
                    Message = "评论审核完成",
                    Data = updatedCommentDto
                };
            }
            catch (Exception ex)
            {
                return new UpdateCommentReviewResponseDto
                {
                    Success = false,
                    Message = $"处理评论时发生错误：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取评论类型字符串
        /// </summary>
        private string GetCommentTypeString(CommentType commentType)
        {
            return commentType switch
            {
                CommentType.Comment => "回复评论",
                CommentType.Store => "店铺评价",
                CommentType.FoodOrder => "订单评价",
                _ => "未知类型"
            };
        }

        /// <summary>
        /// 获取评论状态字符串
        /// </summary>
        private string GetCommentStatusString(CommentState commentState)
        {
            return commentState switch
            {
                CommentState.Pending => "待处理",
                CommentState.Completed => "已完成",
                CommentState.Illegal => "违规",
                _ => "未知状态"
            };
        }
    }
}
