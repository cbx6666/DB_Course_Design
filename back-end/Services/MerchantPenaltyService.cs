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
        public async Task<List<MerchantPenaltyRecordDto>> GetPenaltiesAsync(int sellerId, string? keyword, string? field)
        {
            var penalties = await _penaltyRepository.GetBySellerIdAsync(sellerId);

            // 只返回管理员已处理的举报（状态为 Completed）
            penalties = penalties.Where(p => p.ViolationPenaltyState == Models.Enums.ViolationPenaltyState.Completed).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                var normalized = keyword;
                if (field == "id")
                {
                    normalized = new string(keyword.Where(char.IsDigit).ToArray());
                }

                if (!string.IsNullOrWhiteSpace(field))
                {
                    penalties = field switch
                    {
                        "id" => penalties.Where(p => p.PenaltyID.ToString().Contains(normalized)).ToList(),
                        "reason" => penalties.Where(p => (p.PenaltyReason ?? "").Contains(keyword)).ToList(),
                        _ => penalties
                    };
                }
                else
                {
                    penalties = penalties.Where(p =>
                        p.PenaltyID.ToString().Contains(normalized) ||
                        (p.PenaltyReason ?? "").Contains(keyword))
                        .ToList();
                }
            }

            return penalties.Select(p => new MerchantPenaltyRecordDto
            {
                Id = $"PEN{p.PenaltyID}",
                Reason = p.PenaltyReason ?? "",
                Time = p.PenaltyTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
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

            // 只返回管理员已处理的举报（状态为 Completed）
            if (penalty.ViolationPenaltyState != Models.Enums.ViolationPenaltyState.Completed)
            {
                return null;
            }

            return new MerchantPenaltyRecordDto
            {
                Id = $"PEN{penalty.PenaltyID}",
                Reason = penalty.PenaltyReason ?? "",
                Time = penalty.PenaltyTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "",
                MerchantAction = penalty.SellerPenalty ?? "",
                PlatformAction = penalty.StorePenalty ?? ""
            };
        }
    }
}
