using BackEnd.Data;
using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 配送投诉服务实现（管理员侧）
    /// </summary>
    public class AdminDeliveryComplaintService : IAdminDeliveryComplaintService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AdminDeliveryComplaintService(
            IAdministratorRepository administratorRepository,
            ICourierRepository courierRepository,
            AppDbContext context)
        {
            _administratorRepository = administratorRepository;
            _courierRepository = courierRepository;
            _context = context;
        }

        /// <summary>
        /// 获取管理员的配送投诉列表
        /// </summary>
        public async Task<IEnumerable<AdminComplaintDetailDto>> GetComplaintsForAdminAsync(int adminId)
        {
            var complaintsFromDb = await _administratorRepository.GetDeliveryComplaintsByAdminIdAsync(adminId);

            if (complaintsFromDb == null || !complaintsFromDb.Any())
            {
                return Enumerable.Empty<AdminComplaintDetailDto>();
            }

            var complaintDtos = complaintsFromDb.Select(complaint =>
            {
                string targetName = "未知目标";
                if (complaint.Courier?.User != null)
                {
                    targetName = complaint.Courier.User.FullName ?? complaint.Courier.User.Username;
                }

                return new AdminComplaintDetailDto
                {
                    ComplaintId = complaint.ComplaintID.ToString(),
                    Target = targetName,
                    ApplicationTime = complaint.ComplaintTime.ToString("yyyy-MM-dd HH:mm"),
                    Content = complaint.ComplaintReason,
                    Status = complaint.ComplaintState == ComplaintState.Pending ? "待处理" : "已完成",
                    Punishment = complaint.ProcessingResult ?? "-",
                    PunishmentReason = complaint.ProcessingReason ?? "",
                    ProcessingNote = complaint.ProcessingRemark ?? "",
                    Fine = complaint.FineAmount?.ToString("F2") ?? "0.00"
                };
            }).ToList();

            return complaintDtos;
        }

        /// <summary>
        /// 更新配送投诉处理结果
        /// </summary>
        public async Task<UpdateDeliveryComplaintResponseDto> UpdateComplaintAsync(UpdateDeliveryComplaintDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 验证输入参数
                if (!int.TryParse(request.ComplaintId, out int complaintId))
                {
                    return new UpdateDeliveryComplaintResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "无效的投诉编号格式"
                    };
                }

                if (request.Status != "已完成")
                {
                    return new UpdateDeliveryComplaintResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "状态只能更新为'已完成'"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Punishment) ||
                    string.IsNullOrWhiteSpace(request.PunishmentReason))
                {
                    return new UpdateDeliveryComplaintResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "处罚措施和处罚原因都是必填项"
                    };
                }

                if (!decimal.TryParse(request.Fine, out decimal fineAmount) || fineAmount < 0)
                {
                    return new UpdateDeliveryComplaintResponseDto { Success = false, Code = 400, Message = "罚款金额格式无效或为负数" };
                }

                // 获取现有的配送投诉
                var existingComplaint = await _administratorRepository.GetDeliveryComplaintByIdAsync(complaintId);
                if (existingComplaint == null)
                {
                    return new UpdateDeliveryComplaintResponseDto
                    {
                        Success = false,
                        Code = 404,
                        Message = "未找到指定的配送投诉"
                    };
                }

                // 检查投诉是否已经处理
                if (existingComplaint.ComplaintState == ComplaintState.Completed)
                {
                    return new UpdateDeliveryComplaintResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "该投诉已经处理完成，无法重复处理"
                    };
                }

                // 更新配送投诉信息
                existingComplaint.ComplaintState = ComplaintState.Completed;
                existingComplaint.ProcessingResult = request.Punishment;
                existingComplaint.ProcessingReason = request.PunishmentReason;
                existingComplaint.ProcessingRemark = request.ProcessingNote;
                existingComplaint.FineAmount = fineAmount;

                // 获取骑手信息并扣除薪资
                if (fineAmount > 0)
                {
                    var courier = await _courierRepository.GetByIdAsync(existingComplaint.CourierID);
                    if (courier == null)
                    {
                        await transaction.RollbackAsync();
                        return new UpdateDeliveryComplaintResponseDto { Success = false, Code = 404, Message = "数据异常：未找到关联的骑手" };
                    }
                    if (courier.MonthlySalary >= (int)fineAmount)
                        courier.MonthlySalary -= (int)fineAmount;
                    _context.Couriers.Update(courier);
                }

                // 保存更改
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // 返回更新后的完整信息
                string targetName = existingComplaint.Courier.User.FullName ?? existingComplaint.Courier.User.Username;

                var updatedComplaintDto = new AdminComplaintDetailDto
                {
                    ComplaintId = existingComplaint.ComplaintID.ToString(),
                    Target = targetName,
                    ApplicationTime = existingComplaint.ComplaintTime.ToString("yyyy-MM-dd HH:mm"),
                    Content = existingComplaint.ComplaintReason,
                    Status = "已完成",
                    Punishment = existingComplaint.ProcessingResult ?? "-",
                    PunishmentReason = existingComplaint.ProcessingReason ?? "",
                    ProcessingNote = existingComplaint.ProcessingRemark ?? "",
                    Fine = existingComplaint.FineAmount?.ToString("F2") ?? "0.00"
                };

                return new UpdateDeliveryComplaintResponseDto
                {
                    Success = true,
                    Code = 200,
                    Message = "配送投诉处理成功",
                    Data = updatedComplaintDto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                return new UpdateDeliveryComplaintResponseDto
                {
                    Success = false,
                    Code = 500,
                    Message = $"处理配送投诉时发生错误：{ex.Message}"
                };
            }
        }
    }
}
