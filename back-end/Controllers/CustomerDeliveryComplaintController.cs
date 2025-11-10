using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送投诉管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/delivery-complaints")]
    [Authorize]
    public class CustomerDeliveryComplaintController : BaseController
    {
        private readonly ICustomerDeliveryComplaintService _customerDeliveryComplaintService;

        /// <summary>
        /// 初始化配送投诉管理控制器
        /// </summary>
        /// <param name="customerDeliveryComplaintService">配送投诉服务（消费者侧）</param>
        public CustomerDeliveryComplaintController(ICustomerDeliveryComplaintService customerDeliveryComplaintService)
        {
            _customerDeliveryComplaintService = customerDeliveryComplaintService;
        }

        /// <summary>
        /// 创建配送投诉
        /// </summary>
        /// <param name="request">配送投诉请求数据</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateDeliveryComplaintDto request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerDeliveryComplaintService.CreateComplaintAsync(request, userId.Value);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 获取用户的配送投诉列表
        /// </summary>
        /// <returns>配送投诉列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerDeliveryComplaintService.GetMyComplaintsAsync(userId.Value);
            return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = result });
        }

    }
}
