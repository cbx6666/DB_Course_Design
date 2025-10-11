using BackEnd.DTOs.Penalty;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 处罚服务
    /// </summary>
    public class PenaltyService : IPenaltyService
    {
        private readonly IStoreViolationPenaltyRepository _penaltyRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="penaltyRepository">处罚仓储</param>
        public PenaltyService(IStoreViolationPenaltyRepository penaltyRepository)
        {
            _penaltyRepository = penaltyRepository;
        }

        /// <summary>
        /// 获取处罚记录
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="keyword">关键词</param>
        /// <returns>处罚记录列表</returns>
        public async Task<List<PenaltyRecordDto>> GetPenaltiesAsync(int sellerId, string? keyword)
        {
            var penalties = await _penaltyRepository.GetBySellerIdAsync(sellerId);

            if (!string.IsNullOrEmpty(keyword))
            {
                penalties = penalties.Where(p =>
                    p.PenaltyID.ToString().Contains(keyword) ||
                    p.PenaltyReason.Contains(keyword))
                    .ToList();
            }

            return penalties.Select(p => new PenaltyRecordDto
            {
                Id = $"PEN{p.PenaltyID}",
                Reason = p.PenaltyReason,
                Time = p.PenaltyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MerchantAction = p.SellerPenalty ?? "",
                PlatformAction = p.StorePenalty ?? ""
            }).ToList();
        }

        /// <summary>
        /// 根据ID获取处罚记录
        /// </summary>
        /// <param name="id">处罚ID</param>
        /// <returns>处罚记录</returns>
        public async Task<PenaltyRecordDto?> GetPenaltyByIdAsync(string id)
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

            return new PenaltyRecordDto
            {
                Id = $"PEN{penalty.PenaltyID}",
                Reason = penalty.PenaltyReason,
                Time = penalty.PenaltyTime.ToString("yyyy-MM-dd HH:mm:ss"),
                MerchantAction = penalty.SellerPenalty ?? "",
                PlatformAction = penalty.StorePenalty ?? ""
            };
        }

        /// <summary>
        /// 申诉处罚
        /// </summary>
        /// <param name="id">处罚ID</param>
        /// <param name="appealDto">申诉请求</param>
        /// <returns>申诉响应</returns>
        public async Task<AppealResponseDto?> AppealPenaltyAsync(string id, AppealDto appealDto)
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

            return new AppealResponseDto
            {
                Success = true,
                Message = "申诉提交成功"
            };
        }
    }
}