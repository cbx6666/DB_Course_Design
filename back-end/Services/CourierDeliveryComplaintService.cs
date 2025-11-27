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
                // 获取配送任务的详细信息
                var task = complaint.DeliveryTask;
                var order = task?.Order;
                var deliveryInfo = order?.DeliveryInfo;
                var store = order?.Store;

                return new CourierComplaintDto
                {
                    ComplaintID = complaint.ComplaintID.ToString(),
                    DeliveryTaskID = complaint.DeliveryTaskID.ToString(),
                    ComplaintTime = complaint.ComplaintTime.ToString("yyyy-MM-dd HH:mm"),
                    ComplaintReason = complaint.ComplaintReason,
                    ProcessingResult = (!string.IsNullOrEmpty(complaint.ProcessingResult) && complaint.ProcessingResult != "-") 
                        ? complaint.ProcessingResult 
                        : null,
                    DeliveryAddress = deliveryInfo?.Address,
                    AcceptTime = task?.AcceptTime.ToString("yyyy-MM-dd HH:mm"),
                    PickupTime = task?.PickupTime?.ToString("yyyy-MM-dd HH:mm"),
                    CompletionTime = task?.CompletionTime?.ToString("yyyy-MM-dd HH:mm"),
                    PickupAddress = store?.StoreAddress
                };
            }).ToList();

            return complaintDtos;
        }
    }
}
