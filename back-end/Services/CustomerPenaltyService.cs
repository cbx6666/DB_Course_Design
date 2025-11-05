using BackEnd.DTOs.Penalty;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺举报惩罚服务实现（消费者侧）
    /// </summary>
    public class CustomerPenaltyService : ICustomerPenaltyService
    {
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;
        private readonly IAdministratorRepository _adminRepository;
        private readonly ISupervise_Repository _superviseRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerPenaltyService(
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
            var penalty = new StoreViolationPenalty
            {
                ViolationPenaltyState = ViolationPenaltyState.Pending,
                PenaltyReason = dto.Content,
                PenaltyTime = DateTime.UtcNow,
                StoreID = dto.StoreId
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
