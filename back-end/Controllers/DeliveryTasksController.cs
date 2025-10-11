using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Delivery;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送任务管理控制器
    /// </summary>
    [ApiController]
    [Route("api/delivery-tasks")]
    [Authorize]
    public class DeliveryTasksController : ControllerBase
    {
        private readonly IDeliveryTaskService _deliveryService;

        public DeliveryTasksController(IDeliveryTaskService deliveryService)
        {
            _deliveryService = deliveryService;
        }

        /// <summary>
        /// 发布配送任务
        /// </summary>
        /// <param name="dto">配送任务信息</param>
        /// <returns>发布结果</returns>
        [HttpPost("publish")]
        public async Task<IActionResult> PublishDeliveryTask([FromBody] PublishDeliveryTaskDto dto)
        {
            try
            {
                var sellerId = GetSellerIdFromToken();
                if (sellerId == null)
                {
                    return Unauthorized(new { code = 401, message = "无效的Token，无法获取商家ID" });
                }

                var result = await _deliveryService.PublishDeliveryTaskAsync(dto, sellerId.Value);
                return Ok(new { deliveryTask = result.DeliveryTask, publish = result.Publish });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = 404, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { code = 403, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 从Token中获取商家ID
        /// </summary>
        /// <returns>商家ID，如果无效则返回null</returns>
        private int? GetSellerIdFromToken()
        {
            var sellerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sellerIdString, out int sellerId) ? sellerId : null;
        }
    }
}