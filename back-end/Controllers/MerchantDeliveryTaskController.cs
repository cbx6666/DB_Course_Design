using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送任务管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/delivery-tasks")]
    [Authorize]
    public class MerchantDeliveryTaskController : BaseController
    {
        private readonly IMerchantDeliveryTaskService _deliveryService;

        public MerchantDeliveryTaskController(IMerchantDeliveryTaskService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        /// <summary>
        /// 发布配送任务
        /// </summary>
        /// <param name="dto">配送任务信息</param>
        /// <returns>发布结果</returns>
        [HttpPost("publish")]
        public async Task<IActionResult> PublishDeliveryTask([FromBody] CreateDeliveryTaskDto dto)
        {
            try
            {
                var sellerId = GetUserIdFromToken();
                if (sellerId == null)
                {
                    return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token，无法获取商家ID" });
                }

                var success = await _deliveryService.PublishDeliveryTaskAsync(dto, sellerId.Value);
                return success ? Ok(new ApiResponseDto { Success = true, Code = 200, Message = "配送任务发布成功" }) 
                    : BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "配送任务发布失败" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ApiResponseDto { Success = false, Code = 403, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送信息</returns>
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetOrderDeliveryInfo(int orderId)
        {
            try
            {
                var info = await _deliveryService.GetOrderDeliveryInfoAsync(orderId);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

    }
}
