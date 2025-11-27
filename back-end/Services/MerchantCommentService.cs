using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 评论服务实现（商家侧）
    /// </summary>
    public class MerchantCommentService : IMerchantCommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReview_CommentRepository _reviewCommentRepository;
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantCommentService(
            ICommentRepository commentRepository,
            IReview_CommentRepository reviewCommentRepository,
            IAdministratorRepository administratorRepository)
        {
            _commentRepository = commentRepository;
            _reviewCommentRepository = reviewCommentRepository;
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取评价列表
        /// </summary>
        public async Task<PageResultDto<MerchantCommentDto>> GetReviewsAsync(int sellerId, int page, int pageSize, string? keyword, string? field)
        {
            var comments = (await _commentRepository.GetBySellerAsync(sellerId))?.ToList()
                ?? new List<Comment>();

            // 应用搜索过滤
            if (!string.IsNullOrEmpty(keyword))
            {
                // 如果指定了字段，仅在该字段过滤；否则多个字段 OR 匹配
                string normalized = keyword;
                if (field == "orderNo")
                {
                    normalized = new string(keyword.Where(char.IsDigit).ToArray());
                }

                if (!string.IsNullOrWhiteSpace(field))
                {
                    comments = field switch
                    {
                        "content" => comments.Where(c => (c.Content ?? "").Contains(keyword)).ToList(),
                        "orderNo" => comments.Where(c => (c.FoodOrderID?.ToString() ?? "").Contains(normalized)).ToList(),
                        "user.name" => comments.Where(c => (c.Commenter?.User?.Username ?? "").Contains(keyword)).ToList(),
                        _ => comments
                    };
                }
                else
                {
                    comments = (comments ?? new List<Comment>()).Where(c =>
                        (c.Content ?? "").Contains(keyword) ||
                        c.CommentID.ToString().Contains(keyword) ||
                        (c.Commenter?.User?.Username ?? "").Contains(keyword) ||
                        (c.FoodOrderID.HasValue && c.FoodOrderID.Value.ToString().Contains(normalized)) ||
                        (c.StoreID.HasValue && c.StoreID.Value.ToString().Contains(keyword)))
                        .ToList();
                }
            }

            // 分页处理
            var total = comments.Count();
            var paginatedComments = comments
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // 转换为DTO
            var reviewDtos = paginatedComments.Select(c =>
            {
                // 查找商家回复（CommentType = Comment 且 ReplyToCommentID = 当前评论ID）
                var merchantReply = c.CommentReplies?.FirstOrDefault(r => r.CommentType == CommentType.Comment);

                // 获取回复状态的中文描述
                string? replyStatus = null;
                if (merchantReply != null)
                {
                    replyStatus = merchantReply.CommentState switch
                    {
                        CommentState.Pending => "待审核",
                        CommentState.Completed => "已通过",
                        CommentState.Illegal => "违规",
                        _ => null
                    };
                }

                return new MerchantCommentDto
                {
                    Id = c.CommentID,
                    OrderNo = c.FoodOrderID.HasValue ? $"ORD{c.FoodOrderID.Value}" : $"STORE{c.StoreID}",
                    OrderId = c.FoodOrderID,
                    User = new UserProfileDto
                    {
                        Name = c.Commenter?.User?.Username ?? "未知用户",
                        PhoneNumber = c.Commenter?.User?.PhoneNumber ?? 0,
                        Avatar = c.Commenter?.User?.Avatar
                    },
                    Content = c.Content ?? "无评论内容",
                    Rating = c.Rating,
                    Images = string.IsNullOrWhiteSpace(c.CommentImage)
                        ? Array.Empty<string>()
                        : c.CommentImage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    CreatedAt = c.PostedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    DishDetails = c.FoodOrder?.Cart?.ShoppingCartItems?
                        .Where(item => item != null)
                        .Select(item => new DTOs.Dish.OrderDishDto
                        {
                            DishName = item!.Dish?.DishName ?? "未知菜品",
                            DishImage = item!.Dish?.DishImage ?? "",
                            Quantity = item!.Quantity,
                            Price = item!.Dish?.Price ?? 0m
                        })
                        .ToList() ?? new List<DTOs.Dish.OrderDishDto>(),
                    MerchantReply = merchantReply?.Content,
                    MerchantReplyTime = merchantReply?.PostedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    MerchantReplyStatus = replyStatus,
                    Replies = c.Replies
                };
            }).ToList();

            return new PageResultDto<MerchantCommentDto>
            {
                List = reviewDtos,
                Total = total
            };
        }

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="id">原始评论ID（用户对订单的评论ID）</param>
        /// <param name="replyDto">回复内容</param>
        /// <returns>回复结果</returns>
        public async Task<ApiResponseDto> ReplyToReviewAsync(int id, ReplyCommentDto replyDto)
        {
            // id 是原始评论ID（用户评论的ID）
            var originalComment = await _commentRepository.GetByIdAsync(id);
            if (originalComment == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "评论不存在"
                };
            }

            // 检查是否已经回复过该评论（通过仓储层查询）
            // 查找 ReplyToCommentID == id 且 CommentType == CommentType.Comment 的回复
            var existingReply = await _commentRepository.GetReplyByCommentIdAsync(id);
            
            if (existingReply != null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 400,
                    Message = "已回复过该评论，不能重复回复"
                };
            }

            // 验证关联数据
            if (originalComment.Store == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 400,
                    Message = "评论关联的店铺信息不存在"
                };
            }

            if (originalComment.Store.Seller == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 400,
                    Message = "店铺关联的商家信息不存在"
                };
            }

            // 创建商家回复评论
            // 注意：回复不应该设置 FoodOrderID，因为唯一约束 UX_Comment_Order_Once 要求每个订单只能有一条评论
            // 回复是对评论的回复，不是对订单的评论，所以 FoodOrderID 应该为 null
            // CommenterID 使用原评论的 CommenterID（消费者的ID），因为商家和消费者是不同的模型
            var replyComment = new Comment
            {
                Content = replyDto.Content,
                PostedAt = DateTime.Now,
                CommentType = CommentType.Comment,  // 回复类型
                CommentState = CommentState.Pending,  // 待审核
                ReplyToCommentID = id,
                StoreID = originalComment.StoreID,
                FoodOrderID = null,  // 回复不设置订单ID，避免违反唯一约束
                CommenterID = originalComment.CommenterID  // 使用原评论的消费者ID
            };

            await _commentRepository.AddAsync(replyComment);
            await _commentRepository.SaveAsync();

            // 分配管理员审核回复
            var admin = await PickCommentAdminAsync();
            if (admin != null)
            {
                var review = new Review_Comment
                {
                    AdminID = admin.UserID,
                    CommentID = replyComment.CommentID,
                    ReviewTime = DateTime.Now
                };
                await _reviewCommentRepository.AddAsync(review);
                await _reviewCommentRepository.SaveAsync();
            }

            // 更新原评论的回复数
            originalComment.Replies++;
            await _commentRepository.UpdateAsync(originalComment);
            await _commentRepository.SaveAsync();

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "回复成功，等待管理员审核"
            };
        }

        /// <summary>
        /// 选择一个评论管理员
        /// </summary>
        private async Task<Administrator?> PickCommentAdminAsync()
        {
            var admins = await _administratorRepository.GetAdministratorsByManagedEntityAsync("评论审核");
            if (admins == null || !admins.Any())
                return null;

            var random = new Random();
            return admins.ElementAt(random.Next(admins.Count()));
        }
    }
}
