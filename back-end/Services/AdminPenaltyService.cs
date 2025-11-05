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
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminPenaltyService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        public async Task<IEnumerable<AdminPenaltyDetailDto>> GetViolationPenaltiesForAdminAsync(int adminId)
        {
            var penaltiesFromDb = await _administratorRepository.GetViolationPenaltiesByAdminIdAsync(adminId);

            if (penaltiesFromDb == null || !penaltiesFromDb.Any())
            {
                return Enumerable.Empty<AdminPenaltyDetailDto>();
            }

            var penaltyDtos = penaltiesFromDb.Select(penalty => new AdminPenaltyDetailDto
            {
                PunishmentId = penalty.PenaltyID.ToString(),
                StoreName = penalty.Store.StoreName,
                Reason = penalty.PenaltyReason ?? "-",
                MerchantPunishment = penalty.SellerPenalty ?? "-",
                StorePunishment = penalty.StorePenalty ?? "-",
                PunishmentTime = penalty.PenaltyTime.ToString("yyyy-MM-dd HH:mm"),
                Status = penalty.ViolationPenaltyState == ViolationPenaltyState.Pending ? "待处理" : "已完成",
                ProcessingNote = penalty.PenaltyNote ?? "-"
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

                var existingPenalty = await _administratorRepository.GetViolationPenaltyByIdAsync(punishmentId);
                if (existingPenalty == null)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "未找到指定的违规处罚"
                    };
                }

                var newState = request.Status switch
                {
                    "待处理" => ViolationPenaltyState.Pending,
                    "执行中" => ViolationPenaltyState.Processing,
                    "已完成" => ViolationPenaltyState.Completed,
                    _ => (ViolationPenaltyState?)null
                };

                if (newState == null)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "无效的状态值，只能是：待处理、执行中、已完成"
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
                existingPenalty.PenaltyNote = request.ProcessingNote;
                existingPenalty.PenaltyReason = request.Reason;
                existingPenalty.SellerPenalty = request.MerchantPunishment == "-" ? null : request.MerchantPunishment;
                existingPenalty.StorePenalty = request.StorePunishment == "-" ? null : request.StorePunishment;

                if (DateTime.TryParse(request.PunishmentTime, out DateTime newPenaltyTime))
                {
                    existingPenalty.PenaltyTime = newPenaltyTime;
                }

                bool updateSuccess = await _administratorRepository.UpdateViolationPenaltyAsync(existingPenalty);
                if (!updateSuccess)
                {
                    return new UpdatePenaltyResponseDto
                    {
                        Success = false,
                        Message = "更新违规处罚失败，请稍后重试"
                    };
                }

                var updatedPenaltyDto = new AdminPenaltyDetailDto
                {
                    PunishmentId = existingPenalty.PenaltyID.ToString(),
                    StoreName = existingPenalty.Store.StoreName,
                    Reason = existingPenalty.PenaltyReason ?? "-",
                    MerchantPunishment = existingPenalty.SellerPenalty ?? "-",
                    StorePunishment = existingPenalty.StorePenalty ?? "-",
                    PunishmentTime = existingPenalty.PenaltyTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = request.Status,
                    ProcessingNote = existingPenalty.PenaltyNote ?? ""
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
