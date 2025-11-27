using BackEnd.Data;
using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Dish;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 评论服务实现（消费者侧）
    /// </summary>
    public class CustomerCommentService : ICustomerCommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IReview_CommentRepository _reviewCommentRepository;
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerCommentService(
            ICommentRepository commentRepository,
            IReview_CommentRepository reviewCommentRepository,
            IAdministratorRepository administratorRepository)
        {
            _commentRepository = commentRepository;
            _reviewCommentRepository = reviewCommentRepository;
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        public async Task<List<CustomerCommentDto>> GetCommentListAsync(int storeId)
        {
            // 只显示审核通过的评论（Completed状态）
            var comments = (await _commentRepository.GetAllAsync())
                .Where(c => c.StoreID == storeId && c.CommentState == CommentState.Completed)
                .OrderByDescending(c => c.PostedAt);

            return comments.Select(c =>
            {
                // 只获取已通过的商家回复（CommentState = Completed）
                var merchantReply = c.CommentReplies?
                    .FirstOrDefault(r => r.CommentType == CommentType.Comment && r.CommentState == CommentState.Completed);

                return new CustomerCommentDto
                {
                    Id = c.CommentID,
                    Username = c.Commenter?.User?.Username ?? "匿名用户",
                    Rating = c.Rating,
                    Date = c.PostedAt,
                    Content = c.Content,
                    Avatar = c.Commenter?.User?.Avatar ?? "/images/user/default.png",
                    Images = string.IsNullOrWhiteSpace(c.CommentImage)
                            ? Array.Empty<string>()
                            : c.CommentImage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    MerchantReply = merchantReply?.Content,
                    MerchantReplyTime = merchantReply?.PostedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();
        }

        /// <summary>
        /// 获取评论状态
        /// </summary>
        public async Task<CommentStateDto> GetCommentStateAsync(int storeId)
        {
            // 只统计审核通过的评论（Completed状态）
            var comments = (await _commentRepository.GetAllAsync())
                .Where(c => c.StoreID == storeId && c.CommentState == CommentState.Completed)
                .Select(c => c.Rating);

            int perfect = comments.Count(r => r == 5);
            int good = comments.Count(r => r == 4);
            int normal = comments.Count(r => r == 3);
            int bad = comments.Count(r => r == 2);
            int awful = comments.Count(r => r == 1);

            return new CommentStateDto
            {
                Status = new List<int> { perfect, good, normal, bad, awful }
            };
        }

        /// <summary>
        /// 提交评论
        /// </summary>
        public async Task SubmitCommentAsync(CreateCommentDto dto)
        {
            // 检查该订单是否已有评论（一个订单只能发起一次评论）
            if (dto.OrderId.HasValue)
            {
                var existingComments = await _commentRepository.GetByOrderIdAsync(dto.OrderId.Value);
                if (existingComments.Any())
                {
                    throw new InvalidOperationException("该订单已有评论，一个订单只能发起一次评论");
                }
            }

            var comment = new Comment
            {
                Content = dto.Content,
                PostedAt = DateTime.UtcNow,
                Rating = dto.Rating,
                CommentImage = dto.Images,
                CommentType = CommentType.Store,
                CommentState = CommentState.Pending,
                StoreID = dto.StoreId,
                FoodOrderID = dto.OrderId,
                CommenterID = dto.UserId
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveAsync();

            var admin = await PickCommentAdminAsync();
            if (admin == null)
                throw new InvalidOperationException("没有可用的管理员");

            var review = new Review_Comment
            {
                AdminID = admin.UserID,
                CommentID = comment.CommentID,
                ReviewTime = DateTime.UtcNow
            };
            await _reviewCommentRepository.AddAsync(review);
            await _reviewCommentRepository.SaveAsync();
        }

        /// <summary>
        /// 获取用户的评论列表
        /// </summary>
        public async Task<List<CustomerMyCommentListItemDto>> GetMyCommentsAsync(int userId)
        {
            var comments = await _commentRepository.GetByCommenterIdAsync(userId);

            var result = comments.Select(comment =>
            {
                // 只获取已通过的商家回复（CommentState = Completed）
                var merchantReply = comment.CommentReplies?
                    .FirstOrDefault(r => r.CommentType == CommentType.Comment && r.CommentState == CommentState.Completed);

                return new CustomerMyCommentListItemDto
                {
                    CommentId = comment.CommentID,
                    OrderId = comment.FoodOrderID,
                    StoreId = comment.StoreID ?? 0,
                    StoreName = comment.Store?.StoreName ?? "未知店铺",
                    Rating = comment.Rating ?? 0,
                    Content = comment.Content,
                    Images = string.IsNullOrWhiteSpace(comment.CommentImage)
                        ? Array.Empty<string>()
                        : comment.CommentImage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                    PostedAt = comment.PostedAt,
                    Status = comment.CommentState.ToString(),
                    DishDetails = comment.FoodOrder?.Cart?.ShoppingCartItems?
                        .Select(item => new OrderDishDto
                        {
                            DishName = item.Dish?.DishName ?? "未知菜品",
                            DishImage = item.Dish?.DishImage ?? "",
                            Quantity = item.Quantity,
                            Price = item.Dish?.Price ?? 0m
                        })
                        .ToList() ?? new List<OrderDishDto>(),
                    MerchantReply = merchantReply?.Content,
                    MerchantReplyTime = merchantReply?.PostedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };
            }).ToList();

            return result;
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
