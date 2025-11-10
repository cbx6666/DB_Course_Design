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
    /// 店铺举报服务实现（骑手侧）
    /// </summary>
    public class CourierStoreReportService : ICourierStoreReportService
    {
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;
        private readonly IAdministratorRepository _adminRepository;
        private readonly ISupervise_Repository _superviseRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CourierStoreReportService(
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
            // 检查该骑手对该店铺是否已有未完成的举报
            var existingReports = await _penaltyRepository.GetPendingByCourierIdAndStoreIdAsync(dto.UserId, dto.StoreId);
            if (existingReports.Any())
            {
                throw new InvalidOperationException("该店铺已有未完成的举报，请等待处理完成后再提交");
            }

            var penalty = new StoreViolationPenalty
            {
                ViolationPenaltyState = ViolationPenaltyState.Pending,
                PenaltyReason = dto.Content,
                ReportImages = dto.Images,
                PenaltyTime = DateTime.UtcNow,
                StoreID = dto.StoreId,
                CourierID = dto.UserId  // 骑手ID
            };

            await _penaltyRepository.AddAsync(penalty);
            await _penaltyRepository.SaveAsync();

            var admin = await PickStoreAdminAsync();
            if (admin == null)
                throw new InvalidOperationException("没有可用的管理员");

            var supervise = new Supervise_
            {
                AdminID = admin.UserID,
                PenaltyID = penalty.PenaltyID
            };
            await _superviseRepository.AddAsync(supervise);
            await _superviseRepository.SaveAsync();
        }

        /// <summary>
        /// 获取骑手的店铺举报列表
        /// </summary>
        public async Task<List<CustomerStoreReportListItemDto>> GetMyReportsAsync(int courierId)
        {
            var penalties = await _penaltyRepository.GetByCourierIdAsync(courierId);

            var result = penalties.Select(penalty => new CustomerStoreReportListItemDto
            {
                PenaltyId = penalty.PenaltyID,
                StoreId = penalty.StoreID,
                StoreName = penalty.Store?.StoreName ?? "未知店铺",
                Content = penalty.PenaltyReason,
                Images = string.IsNullOrWhiteSpace(penalty.ReportImages)
                    ? Array.Empty<string>()
                    : penalty.ReportImages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                ReportTime = penalty.PenaltyTime,
                Status = penalty.ViolationPenaltyState.ToString(),
                MerchantPunishment = string.IsNullOrWhiteSpace(penalty.SellerPenalty) ? null : penalty.SellerPenalty,
                StorePunishment = string.IsNullOrWhiteSpace(penalty.StorePenalty) ? null : penalty.StorePenalty,
                // 当状态为已完成时，PenaltyReason 是管理员填写的处理原因
                ProcessingReason = penalty.ViolationPenaltyState == ViolationPenaltyState.Completed 
                    ? (string.IsNullOrWhiteSpace(penalty.PenaltyReason) ? null : penalty.PenaltyReason)
                    : null
            }).ToList();

            return result;
        }

        /// <summary>
        /// 选择一个店铺管理员
        /// </summary>
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

