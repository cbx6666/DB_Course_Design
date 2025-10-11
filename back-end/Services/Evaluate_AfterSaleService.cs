using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 售后申请评估服务实现
    /// </summary>
    public class Evaluate_AfterSaleService : IEvaluate_AfterSaleService
    {
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="administratorRepository">管理员仓储</param>
        public Evaluate_AfterSaleService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>售后申请列表</returns>
        public async Task<IEnumerable<GetAfterSaleApplicationInfo>> GetApplicationsForAdminAsync(int adminId)
        {
            var applicationsFromDb = await _administratorRepository.GetAfterSaleApplicationsByAdminIdAsync(adminId);

            if (applicationsFromDb == null || !applicationsFromDb.Any())
            {
                return Enumerable.Empty<GetAfterSaleApplicationInfo>();
            }

            var applicationDtos = applicationsFromDb.Select(app => new GetAfterSaleApplicationInfo
            {
                ApplicationId = app.ApplicationID.ToString(),
                OrderId = app.OrderID.ToString(),
                ApplicationTime = app.ApplicationTime.ToString("yyyy-MM-dd HH:mm"),
                Description = app.Description,
                Status = app.AfterSaleState == AfterSaleState.Pending ? "待处理" : "已完成",
                Punishment = app.ProcessingResult ?? "-",
                PunishmentReason = app.ProcessingReason ?? "",
                ProcessingNote = app.ProcessingRemark ?? "-"
            });

            return applicationDtos;
        }

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        public async Task<SetAfterSaleApplicationResponse> UpdateAfterSaleApplicationAsync(SetAfterSaleApplicationInfo request)
        {
            try
            {
                // 验证输入参数
                if (!int.TryParse(request.ApplicationId, out int applicationId))
                {
                    return new SetAfterSaleApplicationResponse
                    {
                        Success = false,
                        Message = "无效的申请编号格式"
                    };
                }

                if (request.Status != "已完成")
                {
                    return new SetAfterSaleApplicationResponse
                    {
                        Success = false,
                        Message = "状态只能更新为'已完成'"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Punishment) ||
                string.IsNullOrWhiteSpace(request.PunishmentReason))
                {
                    return new SetAfterSaleApplicationResponse
                    {
                        Success = false,
                        Message = "处罚措施和处罚原因都是必填项"
                    };
                }

                // 获取现有的售后申请
                var existingApplication = await _administratorRepository.GetAfterSaleApplicationByIdAsync(applicationId);
                if (existingApplication == null)
                {
                    return new SetAfterSaleApplicationResponse
                    {
                        Success = false,
                        Message = "未找到指定的售后申请"
                    };
                }

                // 更新售后申请信息
                existingApplication.AfterSaleState = AfterSaleState.Completed;
                existingApplication.ProcessingResult = request.Punishment;
                existingApplication.ProcessingRemark = request.ProcessingNote;
                existingApplication.ProcessingReason = request.PunishmentReason;

                // 保存更改
                bool updateSuccess = await _administratorRepository.UpdateAfterSaleApplicationAsync(existingApplication);
                if (!updateSuccess)
                {
                    return new SetAfterSaleApplicationResponse
                    {
                        Success = false,
                        Message = "更新售后申请失败，请稍后重试"
                    };
                }

                // 返回更新后的完整信息
                var updatedApplicationDto = new GetAfterSaleApplicationInfo
                {
                    ApplicationId = existingApplication.ApplicationID.ToString(),
                    OrderId = existingApplication.OrderID.ToString(),
                    ApplicationTime = existingApplication.ApplicationTime.ToString("yyyy-MM-dd HH:mm"),
                    Description = existingApplication.Description,
                    Status = "已完成",
                    Punishment = existingApplication.ProcessingResult ?? "-"
                };

                return new SetAfterSaleApplicationResponse
                {
                    Success = true,
                    Message = "售后申请处理成功",
                    Data = updatedApplicationDto
                };
            }
            catch (Exception ex)
            {
                return new SetAfterSaleApplicationResponse
                {
                    Success = false,
                    Message = $"处理售后申请时发生错误：{ex.Message}"
                };
            }
        }
    }
}
