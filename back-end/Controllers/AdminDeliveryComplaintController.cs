using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送投诉管理控制器（管理员侧）
    /// </summary>
    [ApiController]
    [Route("api/admin/delivery-complaints")]
    [Authorize]
    public class AdminDeliveryComplaintController : BaseController
    {
        private readonly IAdminDeliveryComplaintService _adminDeliveryComplaintService;

        /// <summary>
        /// 初始化配送投诉管理控制器
        /// </summary>
        /// <param name="adminDeliveryComplaintService">配送投诉服务（管理员侧）</param>
        public AdminDeliveryComplaintController(IAdminDeliveryComplaintService adminDeliveryComplaintService)
        {
            _adminDeliveryComplaintService = adminDeliveryComplaintService;
        }

        /// <summary>
        /// 获取管理员的配送投诉列表
        /// </summary>
        /// <returns>配送投诉列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetDeliveryComplaintsForAdmin()
        {
            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var complaintDtos = await _adminDeliveryComplaintService.GetComplaintsForAdminAsync(adminId.Value);
            return complaintDtos == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "未找到配送投诉列表" }) : Ok(complaintDtos);
        }

        /// <summary>
        /// 更新配送投诉信息
        /// </summary>
        /// <param name="request">配送投诉更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDeliveryComplaint([FromBody] UpdateDeliveryComplaintDto request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var result = await _adminDeliveryComplaintService.UpdateComplaintAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
