using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantCommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        /// <summary>
        /// 获取评价列表
        /// </summary>
        public async Task<PageResultDto<MerchantCommentDto>> GetReviewsAsync(int sellerId, int page, int pageSize, string? keyword, string? field)
        {
            var comments = await _commentRepository.GetBySellerAsync(sellerId);

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
                        "orderNo" => comments.Where(c =>
                            (c.FoodOrderID.HasValue && c.FoodOrderID.Value.ToString().Contains(normalized))
                        ).ToList(),
                        "user.name" => comments.Where(c => (c.Commenter?.User?.Username ?? "").Contains(keyword)).ToList(),
                        _ => comments
                    };
                }
                else
                {
                    comments = comments.Where(c =>
                        (c.Content ?? "").Contains(keyword) ||
                        c.CommentID.ToString().Contains(keyword) ||
                        (c.Commenter?.User?.Username ?? "").Contains(keyword) ||
                        (c.FoodOrderID.HasValue && c.FoodOrderID.Value.ToString().Contains(normalized)) ||
                        c.StoreID.ToString().Contains(keyword))
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
            var reviewDtos = paginatedComments.Select(c => new MerchantCommentDto
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
                    .Select(item => new DTOs.Dish.OrderDishDto
                    {
                        DishName = item.Dish?.DishName ?? "未知菜品",
                        DishImage = item.Dish?.DishImage ?? "",
                        Quantity = item.Quantity,
                        Price = item.Dish?.Price ?? 0m
                    })
                    .ToList() ?? new List<DTOs.Dish.OrderDishDto>()
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
        public async Task<ApiResponseDto> ReplyToReviewAsync(int id, ReplyCommentDto replyDto)
        {
            var comment = await _commentRepository.GetByIdAsync(id);
            if (comment == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "评论不存在"
                };
            }

            // 更新原评论的回复数
            comment.Replies++;
            await _commentRepository.UpdateAsync(comment);
            await _commentRepository.SaveAsync();

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "回复成功"
            };
        }
    }
}
