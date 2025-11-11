using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.DTOs.Courier;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
        private readonly ICourierRatingService _courierRatingService;

        /// <summary>
        /// 初始化配送投诉管理控制器
        /// </summary>
        /// <param name="customerDeliveryComplaintService">配送投诉服务（消费者侧）</param>
        /// <param name="courierRatingService">骑手评分服务</param>
        public CustomerDeliveryComplaintController(
            ICustomerDeliveryComplaintService customerDeliveryComplaintService,
            ICourierRatingService courierRatingService)
        {
            _customerDeliveryComplaintService = customerDeliveryComplaintService;
            _courierRatingService = courierRatingService;
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

        /// <summary>
        /// 为骑手评分
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <param name="dto">评分请求</param>
        /// <returns>评分结果</returns>
        [HttpPost("rate/{courierId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RateCourier([FromRoute] int courierId, [FromBody] CreateCourierRatingDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "数据验证失败" });
                }

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
                }

                await _courierRatingService.RateCourierAsync(dto, userId.Value, courierId);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "评分提交成功" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"评分时发生错误: {ex.Message}" });
            }
        }
    }
}
