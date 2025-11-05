using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送投诉管理控制器（骑手侧）
    /// </summary>
    [ApiController]
    [Route("api/courier/delivery-complaints")]
    [Authorize]
    public class CourierDeliveryComplaintController : BaseController
    {
        private readonly ICourierDeliveryComplaintService _complaintService;

        public CourierDeliveryComplaintController(ICourierDeliveryComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        /// <summary>
        /// 获取投诉列表
        /// </summary>
        /// <returns>投诉列表</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourierComplaintDto>>> GetComplaints()
        {
            try
            {
                var courierId = GetCurrentUserId();
                var complaints = await _complaintService.GetComplaintsAsync(courierId);
                return Ok(complaints);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
            }
        }
    }
}

