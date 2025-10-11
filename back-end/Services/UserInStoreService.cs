using BackEnd.DTOs.User;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户店铺服务
    /// </summary>
    public class UserInStoreService : IUserInStoreService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReview_CommentRepository _reviewCommentRepository;
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;
        private readonly ISupervise_Repository _superviseRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IAdministratorRepository _adminRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storeRepository">店铺仓储</param>
        /// <param name="commentRepository">评论仓储</param>
        /// <param name="reviewCommentRepository">评论审核仓储</param>
        /// <param name="penaltyRepository">违规处罚仓储</param>
        /// <param name="superviseRepository">监督仓储</param>
        /// <param name="couponRepository">优惠券仓储</param>
        /// <param name="adminRepository">管理员仓储</param>
        public UserInStoreService(
            IStoreRepository storeRepository,
            ICommentRepository commentRepository,
            IReview_CommentRepository reviewCommentRepository,
            IStoreViolationPenaltyRepository penaltyRepository,
            ISupervise_Repository superviseRepository,
            ICouponRepository couponRepository,
            IAdministratorRepository adminRepository)
        {
            _storeRepository = storeRepository;
            _commentRepository = commentRepository;
            _reviewCommentRepository = reviewCommentRepository;
            _penaltyRepository = penaltyRepository;
            _superviseRepository = superviseRepository;
            _couponRepository = couponRepository;
            _adminRepository = adminRepository;
        }

        /// <summary>
        /// 获取店铺详情
        /// </summary>
        /// <param name="request">店铺请求</param>
        /// <returns>店铺响应</returns>
        public async Task<StoreResponseDto?> GetStoreInfoAsync(StoreRequestDto request)
        {
            var store = await _storeRepository.GetStoreInfoForUserAsync(request.StoreId);

            if (store == null) return null;

            return new StoreResponseDto
            {
                Id = store.StoreID,
                Name = store.StoreName,
                Image = store.StoreImage ?? string.Empty, // TODO: 店铺图片字段
                Address = store.StoreAddress,
                OpenTime = store.OpenTime,
                CloseTime = store.CloseTime,
                BusinessHours = $"{store.OpenTime:hh\\:mm}-{store.CloseTime:hh\\:mm}",
                Rating = store.AverageRating,
                MonthlySales = store.MonthlySales,
                Description = store.StoreFeatures ?? string.Empty,
                CreateTime = store.StoreCreationTime
            };
        }

        /// <summary>
        /// 获取菜单（平铺所有菜品）
        /// </summary>
        /// <param name="request">菜单请求</param>
        /// <returns>菜单响应列表</returns>
        public async Task<List<MenuResponseDto>> GetMenuAsync(MenuRequestDto request)
        {
            var dishes = await _storeRepository.GetDishesByStoreIdAsync(request.StoreId);

            if (dishes == null || !dishes.Any()) return new List<MenuResponseDto>();

            return dishes.Select(d => new MenuResponseDto
            {
                Id = d.DishID,
                Name = d.DishName,
                Description = d.Description,
                Price = d.Price,
                Image = d.DishImage ?? string.Empty,
                IsSoldOut = d.IsSoldOut
            }).ToList();
        }

        /// <summary>
        /// 获取商家评论列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论响应列表</returns>
        public async Task<List<CommentResponseDto>> GetCommentListAsync(int storeId)
        {
            var comments = (await _commentRepository.GetAllAsync())
                .Where(c => c.StoreID == storeId && !(c.CommentState == CommentState.Illegal))
                .OrderByDescending(c => c.PostedAt);

            return comments.Select(c => new CommentResponseDto
            {
                Id = c.CommentID,
                Username = c.Commenter?.User?.Username ?? "匿名用户",
                Rating = c.Rating,
                Date = c.PostedAt,
                Content = c.Content,
                Avatar = c.Commenter?.User?.Avatar ?? "/images/user/default.png",
                Images = string.IsNullOrWhiteSpace(c.CommentImage)
                        ? Array.Empty<string>()  // 返回一个空数组 []
                        : c.CommentImage.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            }).ToList();
        }

        /// <summary>
        /// 获取商家评价状态 [好评数, 中评数, 差评数]
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评价状态</returns>
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
        /// 用户评价店铺（Pending状态，自动分配管理员）
        /// </summary>
        /// <param name="dto">创建评论请求</param>
        /// <returns>任务</returns>
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

            // 自动分配管理员
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
        }

        /// <summary>
        /// 用户投诉店铺（Pending状态，自动分配管理员）
        /// </summary>
        /// <param name="dto">用户店铺举报请求</param>
        /// <returns>任务</returns>
        public async Task SubmitStoreReportAsync(UserStoreReportDto dto)
        {
            var penalty = new StoreViolationPenalty
            {
                ViolationPenaltyState = ViolationPenaltyState.Pending,
                PenaltyReason = dto.Content,
                PenaltyTime = DateTime.UtcNow,
                StoreID = dto.StoreId
            };

            await _penaltyRepository.AddAsync(penalty);

            // 自动分配管理员
            var admin = await PickStoreAdminAsync();
            if (admin == null)
                throw new InvalidOperationException("没有可用的管理员");

            var supervise = new Supervise_
            {
                AdminID = admin.UserID,
                PenaltyID = penalty.PenaltyID
            };
            await _superviseRepository.AddAsync(supervise);
        }

        /// <summary>
        /// 选择一个评论管理员（这里写了随机，可以换成轮询或负载最小）
        /// </summary>
        /// <returns>管理员</returns>
        private async Task<Administrator?> PickCommentAdminAsync()
        {
            var admins = await _adminRepository.GetAdministratorsByManagedEntityAsync("评论审核");
            if (admins == null || !admins.Any())
                return null;

            var random = new Random();

            return admins.ElementAt(random.Next(admins.Count()));
        }

        /// <summary>
        /// 选择一个店铺管理员（这里写了随机，可以换成轮询或负载最小）
        /// </summary>
        /// <returns>管理员</returns>
        private async Task<Administrator?> PickStoreAdminAsync()
        {
            var admins = await _adminRepository.GetAdministratorsByManagedEntityAsync("店铺举报");
            if (admins == null || !admins.Any())
                return null;

            var random = new Random();

            return admins.ElementAt(random.Next(admins.Count()));
        }
    }
}
