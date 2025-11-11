using BackEnd.Data;
using BackEnd.DTOs.Penalty;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺举报服务实现（消费者侧）
    /// </summary>
    public class CustomerStoreReportService : ICustomerStoreReportService
    {
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;
        private readonly IAdministratorRepository _adminRepository;
        private readonly ISupervise_Repository _superviseRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerStoreReportService(
            IStoreViolationPenaltyRepository penaltyRepository,
            IAdministratorRepository adminRepository,
            ISupervise_Repository superviseRepository)
        {
            _penaltyRepository = penaltyRepository;
            _adminRepository = adminRepository;
            _superviseRepository = superviseRepository;
        }

        /// <summary>
        /// 举报店铺
        /// </summary>
        public async Task SubmitStoreReportAsync(ReportStoreDto dto)
        {
            // 检查该用户对该店铺是否已有未完成的举报
            var existingReports = await _penaltyRepository.GetPendingByCustomerIdAndStoreIdAsync(dto.UserId, dto.StoreId);
            if (existingReports.Any())
            {
                throw new InvalidOperationException("该店铺已有未完成的举报，请等待处理完成后再提交");
            }

            // 先检查是否有可用的管理员，避免创建了举报记录但没有管理员的情况
            var admin = await PickStoreAdminAsync();
            if (admin == null)
            {
                throw new InvalidOperationException("没有可用的管理员，无法提交举报");
            }

            var penalty = new StoreViolationPenalty
            {
                ViolationPenaltyState = ViolationPenaltyState.Pending,
                ReportReason = dto.Content,
                ReportImages = dto.Images,
                ReportTime = DateTime.UtcNow,
                StoreID = dto.StoreId,
                CustomerID = dto.UserId
            };

            await _penaltyRepository.AddAsync(penalty);
            await _penaltyRepository.SaveAsync();

            var supervise = new Supervise_
            {
                AdminID = admin.UserID,
                PenaltyID = penalty.PenaltyID
            };
            await _superviseRepository.AddAsync(supervise);
            await _superviseRepository.SaveAsync();
        }

        /// <summary>
        /// 获取用户的店铺举报列表
        /// </summary>
        public async Task<List<StoreReportListItemDto>> GetMyReportsAsync(int userId)
        {
            var penalties = await _penaltyRepository.GetByCustomerIdAsync(userId);

            var result = penalties.Select(penalty => new StoreReportListItemDto
            {
                PenaltyId = penalty.PenaltyID,
                StoreId = penalty.StoreID,
                StoreName = penalty.Store?.StoreName ?? "未知店铺",
                Content = penalty.ReportReason,
                Images = string.IsNullOrWhiteSpace(penalty.ReportImages)
                    ? Array.Empty<string>()
                    : penalty.ReportImages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                ReportTime = penalty.ReportTime,
                Status = penalty.ViolationPenaltyState.ToString(),
                MerchantPunishment = string.IsNullOrWhiteSpace(penalty.SellerPenalty) ? null : penalty.SellerPenalty,
                StorePunishment = string.IsNullOrWhiteSpace(penalty.StorePenalty) ? null : penalty.StorePenalty,
                // PenaltyReason 是管理员填写的处理原因
                ProcessingReason = string.IsNullOrWhiteSpace(penalty.PenaltyReason) ? null : penalty.PenaltyReason,
                PenaltyTime = penalty.PenaltyTime
            }).ToList();

            return result;
        }

        /// <summary>
        /// 选择一个店铺管理员
        /// </summary>
        private async Task<Administrator?> PickStoreAdminAsync()
        {
            var admins = await _adminRepository.GetAdministratorsByManagedEntityAsync("商家举报");
            if (admins == null || !admins.Any())
                return null;

            var random = new Random();
            return admins.ElementAt(random.Next(admins.Count()));
        }
    }
}

