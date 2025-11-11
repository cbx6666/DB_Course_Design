using BackEnd.DTOs.Penalty;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺举报惩罚服务实现（管理员侧）
    /// </summary>
    public class AdminPenaltyService : IAdminPenaltyService
    {
        private readonly IStoreViolationPenaltyRepository _storeViolationPenaltyRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminPenaltyService(IStoreViolationPenaltyRepository storeViolationPenaltyRepository)
        {
            _storeViolationPenaltyRepository = storeViolationPenaltyRepository;
        }

        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        public async Task<IEnumerable<AdminPenaltyDetailDto>> GetViolationPenaltiesForAdminAsync(int adminId)
        {
            var penaltiesFromDb = await _storeViolationPenaltyRepository.GetByAdminIdAsync(adminId);

            if (penaltiesFromDb == null || !penaltiesFromDb.Any())
            {
                return Enumerable.Empty<AdminPenaltyDetailDto>();
            }

            var penaltyDtos = penaltiesFromDb.Select(penalty => new AdminPenaltyDetailDto
            {
                PunishmentId = penalty.PenaltyID.ToString(),
                StoreName = penalty.Store.StoreName,
                Reason = penalty.ReportReason ?? "-", // 显示消费者的举报内容
                MerchantPunishment = penalty.SellerPenalty ?? "-",
                StorePunishment = penalty.StorePenalty ?? "-",
                PunishmentTime = penalty.PenaltyTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                Status = penalty.ViolationPenaltyState == ViolationPenaltyState.Pending ? "待处理" : "已完成"
            });

            return penaltyDtos;
        }

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        public async Task<UpdatePenaltyResponseDto> UpdateViolationPenaltyAsync(UpdatePenaltyDto request)
        {
            try
            {
                if (request == null)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "请求数据不能为空"
                    };
                }

                if (!int.TryParse(request.PunishmentId, out int punishmentId))
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "无效的处罚编号格式"
                    };
                }

                var existingPenalty = await _storeViolationPenaltyRepository.GetByIdAsync(punishmentId);
                if (existingPenalty == null)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "未找到指定的违规处罚"
                    };
                }

                ViolationPenaltyState? newState = request.Status switch
                {
                    "待处理" => ViolationPenaltyState.Pending,
                    "已完成" => ViolationPenaltyState.Completed,
                    _ => null
                };

                if (newState == null)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "无效的状态值，只能是：待处理、已完成"
                    };
                }

                // 检查是否已经处理完成
                if (existingPenalty.ViolationPenaltyState == ViolationPenaltyState.Completed)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "该违规处罚已经处理完成，无法重复处理"
                    };
                }

                existingPenalty.ViolationPenaltyState = newState.Value;
                // ReportReason 是消费者的举报内容，不应该被覆盖
                // PenaltyReason 是管理员填写的处理原因
                existingPenalty.PenaltyReason = request.Reason;
                existingPenalty.SellerPenalty = request.MerchantPunishment == "-" ? null : request.MerchantPunishment;
                existingPenalty.StorePenalty = request.StorePunishment == "-" ? null : request.StorePunishment;

                // 当状态变为已完成时，更新 PenaltyTime 为处罚时间
                // 如果管理员指定了处罚时间，则使用指定的时间；否则使用当前时间
                if (newState.Value == ViolationPenaltyState.Completed)
                {
                    if (DateTime.TryParse(request.PunishmentTime, out DateTime newPenaltyTime))
                    {
                        existingPenalty.PenaltyTime = newPenaltyTime;
                    }
                    else
                    {
                        // 如果没有指定处罚时间，使用当前时间作为处罚时间
                        existingPenalty.PenaltyTime = DateTime.UtcNow;
                    }
                }

                await _storeViolationPenaltyRepository.UpdateAsync(existingPenalty);

                var updatedPenaltyDto = new AdminPenaltyDetailDto
                {
                    PunishmentId = existingPenalty.PenaltyID.ToString(),
                    StoreName = existingPenalty.Store.StoreName,
                    Reason = existingPenalty.PenaltyReason ?? "-", // 显示管理员填写的处理原因
                    MerchantPunishment = existingPenalty.SellerPenalty ?? "-",
                    StorePunishment = existingPenalty.StorePenalty ?? "-",
                    PunishmentTime = existingPenalty.PenaltyTime?.ToString("yyyy-MM-dd HH:mm") ?? "",
                    Status = request.Status
                };

                return new UpdatePenaltyResponseDto
                {
                    Success = true,
                    Message = "违规处罚更新成功",
                    Data = updatedPenaltyDto
                };
            }
            catch (Exception ex)
            {
                return new UpdatePenaltyResponseDto
                {
                    Success = false,
                    Message = $"处理违规处罚时发生错误：{ex.Message}"
                };
            }
        }
    }
}
