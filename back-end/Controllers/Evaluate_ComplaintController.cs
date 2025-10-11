using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送投诉评估控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/delivery-complaints")]
    [Authorize]
    public class Evaluate_ComplaintController : ControllerBase
    {
        private readonly IEvaluate_DeliveryComplaintService _evaluateDeliveryComplaintService;

        /// <summary>
        /// 初始化配送投诉评估控制器
        /// </summary>
        /// <param name="evaluateDeliveryComplaintService">配送投诉评估服务</param>
        public Evaluate_ComplaintController(IEvaluate_DeliveryComplaintService evaluateDeliveryComplaintService)
        {
            _evaluateDeliveryComplaintService = evaluateDeliveryComplaintService;
        }

        /// <summary>
        /// 获取管理员的配送投诉列表
        /// </summary>
        /// <returns>配送投诉列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetDeliveryComplaintsForAdmin()
        {
            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var complaintDtos = await _evaluateDeliveryComplaintService.GetComplaintsForAdminAsync(adminId.Value);
            return complaintDtos == null ? NotFound() : Ok(complaintDtos);
        }

        /// <summary>
        /// 更新配送投诉信息
        /// </summary>
        /// <param name="request">配送投诉更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateDeliveryComplaint([FromBody] SetComplaintInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var result = await _evaluateDeliveryComplaintService.UpdateComplaintAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 从Token中获取管理员ID
        /// </summary>
        /// <returns>管理员ID，如果无效则返回null</returns>
        private int? GetAdminIdFromToken()
        {
            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(adminIdString, out int adminId) ? adminId : null;
        }
    }
}
