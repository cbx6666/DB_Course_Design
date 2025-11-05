using BackEnd.DTOs.Comment;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

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
            var comments = (await _commentRepository.GetAllAsync())
                .Where(c => c.StoreID == storeId && !(c.CommentState == CommentState.Illegal))
                .OrderByDescending(c => c.PostedAt);

            return comments.Select(c => new CustomerCommentDto
            {
                Id = c.CommentID,
                Username = c.Commenter?.User?.Username ?? "匿名用户",
                Rating = c.Rating,
                Date = c.PostedAt,
                Content = c.Content,
                Avatar = c.Commenter?.User?.Avatar ?? "/images/user/default.png",
                Images = string.IsNullOrWhiteSpace(c.CommentImage)
                        ? Array.Empty<string>()
                        : c.CommentImage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            }).ToList();
        }

        /// <summary>
        /// 获取评论状态
        /// </summary>
        public async Task<CommentStateDto> GetCommentStateAsync(int storeId)
        {
            var comments = (await _commentRepository.GetAllAsync())
                .Where(c => c.StoreID == storeId)
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
            var comment = new Comment
            {
                Content = dto.Content,
                PostedAt = DateTime.UtcNow,
                Rating = dto.Rating,
                CommentType = CommentType.Store,
                CommentState = CommentState.Pending,
                StoreID = dto.StoreId,
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
