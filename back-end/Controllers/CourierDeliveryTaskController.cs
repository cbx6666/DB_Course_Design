using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送任务管理控制器（骑手侧）
    /// </summary>
    [ApiController]
    [Route("api/courier/delivery-tasks")]
    [Authorize]
    public class CourierDeliveryTaskController : BaseController
    {
        private readonly ICourierDeliveryTaskService _deliveryTaskService;

        public CourierDeliveryTaskController(ICourierDeliveryTaskService deliveryTaskService)
        {
            _deliveryTaskService = deliveryTaskService;
        }

        /// <summary>
        /// 获取配送任务列表（骑手端）
        /// </summary>
        /// <param name="status">配送状态（可选）</param>
        /// <returns>配送任务列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] string? status)
        {
            try
            {
                var courierId = GetCurrentUserId();
                var tasks = await _deliveryTaskService.GetTasksAsync(courierId, status);
                return Ok(tasks);
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

        /// <summary>
        /// 获取可接配送任务列表
        /// </summary>
        /// <param name="latitude">纬度（可选）</param>
        /// <param name="longitude">经度（可选）</param>
        /// <param name="maxDistance">最大距离（可选，默认10公里）</param>
        /// <returns>可接配送任务列表</returns>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<CourierAvailableTaskDto>>> GetAvailableTasks(
            [FromQuery] decimal? latitude = null,
            [FromQuery] decimal? longitude = null,
            [FromQuery] decimal maxDistance = 10)
        {
            try
            {
                var courierId = GetCurrentUserId();
                var availableTasks = await _deliveryTaskService.GetAvailableTasksAsync(courierId, latitude, longitude, maxDistance);
                return Ok(availableTasks);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
            }
        }

        /// <summary>
        /// 接受配送任务
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{taskId}/accept")]
        public async Task<IActionResult> AcceptTask(string taskId)
        {
            if (!int.TryParse(taskId, out int taskIdInt))
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "无效的配送任务ID格式。" });
            }

            try
            {
                var courierId = GetCurrentUserId();
                var success = await _deliveryTaskService.AcceptTaskAsync(courierId, taskIdInt);
                return !success ? BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "无法接受该配送任务，它可能已被处理或不存在。" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "操作成功" });
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

        /// <summary>
        /// 确认取餐（将状态从Pending改为Delivering）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{taskId}/pickup")]
        public async Task<IActionResult> PickupTask(string taskId)
        {
            if (!int.TryParse(taskId, out int taskIdInt))
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "无效的配送任务ID格式。" });
            }

            try
            {
                var courierId = GetCurrentUserId();
                var success = await _deliveryTaskService.PickupTaskAsync(taskIdInt, courierId);
                return !success ? BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "操作失败，请检查配送任务状态或权限。" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
            }
        }

        /// <summary>
        /// 确认送达（将状态从Delivering改为Completed）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{taskId}/deliver")]
        public async Task<IActionResult> DeliverTask(string taskId)
        {
            if (!int.TryParse(taskId, out int taskIdInt))
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "无效的配送任务ID格式。" });
            }

            try
            {
                var courierId = GetCurrentUserId();
                var success = await _deliveryTaskService.DeliverTaskAsync(taskIdInt, courierId);
                return !success ? BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "操作失败，请检查配送任务状态或权限。" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "操作成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
            }
        }
    }
}
