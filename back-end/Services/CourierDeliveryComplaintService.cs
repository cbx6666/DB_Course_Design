using BackEnd.Data;
using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 配送投诉服务实现（骑手侧）
    /// </summary>
    public class CourierDeliveryComplaintService : ICourierDeliveryComplaintService
    {
        private readonly IDeliveryComplaintRepository _complaintRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CourierDeliveryComplaintService(IDeliveryComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        /// <summary>
        /// 获取骑手的投诉列表
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <returns>投诉列表</returns>
        public async Task<IEnumerable<CourierComplaintDto>> GetComplaintsAsync(int courierId)
        {
            var complaints = await _complaintRepository.GetByCourierIdAsync(courierId);

            var complaintDtos = complaints.Select(complaint =>
            {
                ComplaintPunishmentDto? punishmentDto = null;

                if (!string.IsNullOrEmpty(complaint.ProcessingResult) && complaint.ProcessingResult != "-")
                {
                    punishmentDto = new ComplaintPunishmentDto
                    {
                        Description = complaint.ProcessingResult,
                        Type = "官方处理结果",
                        Duration = null
                    };
                }

                return new CourierComplaintDto
                {
                    ComplaintID = complaint.ComplaintID.ToString(),
                    DeliveryTaskID = complaint.DeliveryTaskID.ToString(),
                    ComplaintTime = complaint.ComplaintTime.ToString("yyyy-MM-dd HH:mm"),
                    ComplaintReason = complaint.ComplaintReason,
                    Punishment = punishmentDto
                };
            }).ToList();

            return complaintDtos;
        }
    }
}
