using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 售后申请服务实现（管理员侧）
    /// </summary>
    public class AdminAfterSaleService : IAdminAfterSaleService
    {
        private readonly IAfterSaleApplicationRepository _afterSaleApplicationRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminAfterSaleService(IAfterSaleApplicationRepository afterSaleApplicationRepository)
        {
            _afterSaleApplicationRepository = afterSaleApplicationRepository;
        }

        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        public async Task<IEnumerable<AfterSaleApplicationDetailDto>> GetApplicationsForAdminAsync(int adminId)
        {
            var applicationsFromDb = await _afterSaleApplicationRepository.GetByAdminIdAsync(adminId);

            if (applicationsFromDb == null || !applicationsFromDb.Any())
            {
                return Enumerable.Empty<AfterSaleApplicationDetailDto>();
            }

            var applicationDtos = applicationsFromDb.Select(app => new AfterSaleApplicationDetailDto
            {
                ApplicationId = app.ApplicationID.ToString(),
                OrderId = app.OrderID.ToString(),
                ApplicationTime = app.ApplicationTime.ToString("yyyy-MM-dd HH:mm"),
                Description = app.Description,
                Status = app.AfterSaleState == AfterSaleState.Pending ? "待处理" : "已完成",
                Punishment = app.ProcessingResult ?? "-",
                PunishmentReason = app.ProcessingReason ?? ""
            });

            return applicationDtos;
        }

        /// <summary>
        /// 更新售后申请
        /// </summary>
        public async Task<UpdateAfterSaleApplicationResponseDto> UpdateAfterSaleApplicationAsync(UpdateAfterSaleApplicationDto request)
        {
            try
            {
                // 验证输入参数
                if (!int.TryParse(request.ApplicationId, out int applicationId))
                {
                    return new UpdateAfterSaleApplicationResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "无效的申请编号格式"
                    };
                }

                if (request.Status != "已完成")
                {
                    return new UpdateAfterSaleApplicationResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "状态只能更新为'已完成'"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Punishment) ||
                string.IsNullOrWhiteSpace(request.PunishmentReason))
                {
                    return new UpdateAfterSaleApplicationResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "处罚措施和处罚原因都是必填项"
                    };
                }

                // 获取现有的售后申请
                var existingApplication = await _afterSaleApplicationRepository.GetByIdAsync(applicationId);
                if (existingApplication == null)
                {
                    return new UpdateAfterSaleApplicationResponseDto
                    {
                        Success = false,
                        Code = 404,
                        Message = "未找到指定的售后申请"
                    };
                }

                // 更新售后申请信息
                existingApplication.AfterSaleState = AfterSaleState.Completed;
                existingApplication.ProcessingResult = request.Punishment;
                existingApplication.ProcessingReason = request.PunishmentReason;

                // 保存更改
                await _afterSaleApplicationRepository.UpdateAsync(existingApplication);

                // 返回更新后的完整信息
                var updatedApplicationDto = new AfterSaleApplicationDetailDto
                {
                    ApplicationId = existingApplication.ApplicationID.ToString(),
                    OrderId = existingApplication.OrderID.ToString(),
                    ApplicationTime = existingApplication.ApplicationTime.ToString("yyyy-MM-dd HH:mm"),
                    Description = existingApplication.Description,
                    Status = "已完成",
                    Punishment = existingApplication.ProcessingResult ?? "-"
                };

                return new UpdateAfterSaleApplicationResponseDto
                {
                    Success = true,
                    Code = 200,
                    Message = "售后申请处理成功",
                    Data = updatedApplicationDto
                };
            }
            catch (Exception ex)
            {
                return new UpdateAfterSaleApplicationResponseDto
                {
                    Success = false,
                    Code = 500,
                    Message = $"处理售后申请时发生错误：{ex.Message}"
                };
            }
        }
    }
}
