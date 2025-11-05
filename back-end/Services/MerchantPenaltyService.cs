using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺举报惩罚服务实现（商家侧）
    /// </summary>
    public class MerchantPenaltyService : IMerchantPenaltyService
    {
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantPenaltyService(IStoreViolationPenaltyRepository penaltyRepository)
        {
            _penaltyRepository = penaltyRepository;
        }

        /// <summary>
        /// 获取处罚记录列表
        /// </summary>
        public async Task<List<MerchantPenaltyRecordDto>> GetPenaltiesAsync(int sellerId, string? keyword)
        {
            var penalties = await _penaltyRepository.GetBySellerIdAsync(sellerId);

            if (!string.IsNullOrEmpty(keyword))
            {
                penalties = penalties.Where(p =>
                    p.PenaltyID.ToString().Contains(keyword) ||
                    (p.PenaltyReason ?? "").Contains(keyword))
                    .ToList();
            }

            return penalties.Select(p => new MerchantPenaltyRecordDto
            {
                Id = $"PEN{p.PenaltyID}",
                Reason = p.PenaltyReason ?? "",
                Time = p.PenaltyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MerchantAction = p.SellerPenalty ?? "",
                PlatformAction = p.StorePenalty ?? ""
            }).ToList();
        }

        /// <summary>
        /// 根据ID获取处罚记录
        /// </summary>
        public async Task<MerchantPenaltyRecordDto?> GetPenaltyByIdAsync(string id)
        {
            // 从ID中提取数字部分
            if (!int.TryParse(id.Replace("PEN", ""), out int penaltyId))
            {
                return null;
            }

            var penalty = await _penaltyRepository.GetByIdAsync(penaltyId);
            if (penalty == null)
            {
                return null;
            }

            return new MerchantPenaltyRecordDto
            {
                Id = $"PEN{penalty.PenaltyID}",
                Reason = penalty.PenaltyReason ?? "",
                Time = penalty.PenaltyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MerchantAction = penalty.SellerPenalty ?? "",
                PlatformAction = penalty.StorePenalty ?? ""
            };
        }

        /// <summary>
        /// 申诉处罚
        /// </summary>
        public async Task<ApiResponseDto?> AppealPenaltyAsync(string id, AppealPenaltyDto appealDto)
        {
            // 从ID中提取数字部分
            if (!int.TryParse(id.Replace("PEN", ""), out int penaltyId))
            {
                return null;
            }

            var penalty = await _penaltyRepository.GetByIdAsync(penaltyId);
            if (penalty == null)
            {
                return null;
            }

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "申诉提交成功"
            };
        }
    }
}
