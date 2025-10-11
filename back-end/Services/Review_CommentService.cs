using BackEnd.DTOs.Comment;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 评论审核服务
    /// </summary>
    public class Review_CommentService : IReview_CommentService
    {
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="administratorRepository">管理员仓储</param>
        public Review_CommentService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取管理员处理的评论列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>评论信息列表</returns>
        public async Task<IEnumerable<GetCommentInfo>> GetCommentsForAdminAsync(int adminId)
        {
            var commentsFromDb = await _administratorRepository.GetReviewCommentsByAdminIdAsync(adminId);

            if (commentsFromDb == null || !commentsFromDb.Any())
            {
                return Enumerable.Empty<GetCommentInfo>();
            }

            var commentDtos = commentsFromDb.Select(comment => new GetCommentInfo
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

            return commentDtos;
        }

        /// <summary>
        /// 更新评论状态
        /// </summary>
        /// <param name="request">设置评论信息请求</param>
        /// <returns>更新结果</returns>
        public async Task<SetCommentInfoResponse> UpdateCommentAsync(SetCommentInfo request)
        {
            try
            {
                if (request == null)
                {
                    return new SetCommentInfoResponse
                    {
                        Success = false,
                        Message = "请求数据不能为空"
                    };
                }

                if (!int.TryParse(request.ReviewId, out int commentId))
                {
                    return new SetCommentInfoResponse
                    {
                        Success = false,
                        Message = "无效的评论编号格式"
                    };
                }

                var existingComment = await _administratorRepository.GetReviewCommentByIdAsync(commentId);
                if (existingComment == null)
                {
                    return new SetCommentInfoResponse
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
                    return new SetCommentInfoResponse
                    {
                        Success = false,
                        Message = "无效的状态值，只能是：待处理、已完成、违规"
                    };
                }

                if (existingComment.CommentState == CommentState.Completed ||
                    existingComment.CommentState == CommentState.Illegal)
                {
                    return new SetCommentInfoResponse
                    {
                        Success = false,
                        Message = "该评论已经处理完成，无法重复处理"
                    };
                }

                existingComment.CommentState = newState.Value;

                bool updateSuccess = await _administratorRepository.UpdateReviewCommentAsync(existingComment);
                if (!updateSuccess)
                {
                    return new SetCommentInfoResponse
                    {
                        Success = false,
                        Message = "更新评论状态失败，请稍后重试"
                    };
                }

                var updatedCommentDto = new GetCommentInfo
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

                return new SetCommentInfoResponse
                {
                    Success = true,
                    Message = "评论审核完成",
                    Data = updatedCommentDto
                };
            }
            catch (Exception ex)
            {
                return new SetCommentInfoResponse
                {
                    Success = false,
                    Message = $"处理评论时发生错误：{ex.Message}"
                };
            }
        }

        /// <summary>
        /// 获取评论类型字符串
        /// </summary>
        /// <param name="commentType">评论类型</param>
        /// <returns>评论类型字符串</returns>
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
        /// <param name="commentState">评论状态</param>
        /// <returns>评论状态字符串</returns>
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

