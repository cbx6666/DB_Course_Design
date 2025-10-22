using BackEnd.DTOs.Courier;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送员管理控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CourierController : ControllerBase
    {
        private readonly ICourierService _courierService;

        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }

        /// <summary>
        /// 获取配送员个人资料
        /// </summary>
        /// <returns>配送员个人资料</returns>
        [HttpGet("profile")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult<CourierProfileDto>> GetProfile()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var profileDto = await _courierService.GetProfileAsync(courierId);
                return profileDto == null ? NotFound("骑手资料未找到") : Ok(profileDto);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取配送员工作状态
        /// </summary>
        /// <returns>工作状态信息</returns>
        [HttpGet("status")]
        public async Task<IActionResult> GetWorkStatus()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var statusDto = await _courierService.GetWorkStatusAsync(courierId);
                return statusDto == null ? NotFound("骑手资料未找到，无法获取状态") : Ok(new { isOnline = statusDto.IsOnline });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取配送员订单列表
        /// </summary>
        /// <param name="status">订单状态</param>
        /// <returns>订单列表</returns>
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("必须提供 status 查询参数");
            }

            try
            {
                var courierId = GetCurrentCourierId();
                var orders = await _courierService.GetOrdersAsync(courierId, status);
                return Ok(orders);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取配送员当前位置
        /// </summary>
        /// <returns>位置信息</returns>
        [HttpGet("location")]
        public async Task<IActionResult> GetLocation()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var area = await _courierService.GetCurrentLocationAsync(courierId);
                return Ok(new { area });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 切换配送员工作状态
        /// </summary>
        /// <param name="request">状态切换请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("status/toggle")]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequestDto request)
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.ToggleWorkStatusAsync(courierId, request.IsOnline);
                return !success ? NotFound("骑手不存在，无法更新状态") : Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取新订单详情
        /// </summary>
        /// <param name="notificationId">通知ID</param>
        /// <returns>订单详情</returns>
        [HttpGet("orders/new/{notificationId}")]
        public async Task<IActionResult> GetNewOrderDetails(string notificationId)
        {
            if (!int.TryParse(notificationId, out int taskId))
            {
                return BadRequest("无效的订单ID格式，必须是数字。");
            }

            try
            {
                var orderDetails = await _courierService.GetNewOrderDetailsAsync(taskId);
                return orderDetails == null ? NotFound($"找不到ID为 {taskId} 的订单详情。") : Ok(orderDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("orders/{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(string orderId)
        {
            if (!int.TryParse(orderId, out int taskId))
            {
                return BadRequest("无效的订单ID格式。");
            }

            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.AcceptOrderAsync(courierId, taskId);
                return !success ? BadRequest(new { success = false, message = "无法接受该订单，它可能已被处理或不存在。" }) : Ok(new { success = true });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("orders/{orderId}/reject")]
        public async Task<IActionResult> RejectOrder(string orderId)
        {
            if (!int.TryParse(orderId, out int taskId))
            {
                return BadRequest("无效的订单ID格式。");
            }

            try
            {
                var success = await _courierService.RejectOrderAsync(taskId);
                return !success ? BadRequest(new { success = false, message = "无法拒绝该订单，它可能已被处理或不存在。" }) : Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取月度收入
        /// </summary>
        /// <returns>月度收入</returns>
        [HttpGet("income/monthly")]
        public async Task<IActionResult> GetMonthlyIncome()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var monthlyIncome = await _courierService.GetMonthlyIncomeAsync(courierId);
                return Content(monthlyIncome.ToString("F2"), "text/plain");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 完成配送任务
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("tasks/{taskId}/complete")]
        public async Task<IActionResult> CompleteTask(int taskId)
        {
            try
            {
                var courierId = GetCurrentCourierId();
                await _courierService.MarkTaskAsCompletedAsync(taskId, courierId);
                return Ok(new { success = true, message = "订单已成功标记为完成" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"操作失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 确认取货
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("orders/{orderId}/pickup")]
        public async Task<IActionResult> PickupOrder(string orderId)
        {
            if (!int.TryParse(orderId, out int taskId))
            {
                return BadRequest("无效的订单ID格式。");
            }

            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.PickupOrderAsync(taskId, courierId);
                return !success ? BadRequest(new { success = false, message = "操作失败，请检查订单状态或权限。" }) : Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 确认送达
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("orders/{orderId}/deliver")]
        public async Task<IActionResult> DeliverOrder(string orderId)
        {
            if (!int.TryParse(orderId, out int taskId))
            {
                return BadRequest("无效的订单ID格式。");
            }

            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.DeliverOrderAsync(taskId, courierId);
                return !success ? BadRequest(new { success = false, message = "操作失败，请检查订单状态或权限。" }) : Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取可接订单列表
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="maxDistance">最大距离</param>
        /// <returns>可接订单列表</returns>
        [HttpGet("orders/available")]
        public async Task<ActionResult<IEnumerable<AvailableOrderDto>>> GetAvailableOrders(
            [FromQuery] decimal latitude,
            [FromQuery] decimal longitude,
            [FromQuery] decimal maxDistance = 100000.0m)
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var availableOrders = await _courierService.GetAvailableOrdersAsync(courierId, latitude, longitude, maxDistance);
                return Ok(availableOrders);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取投诉列表
        /// </summary>
        /// <returns>投诉列表</returns>
        [HttpGet("complaints")]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetComplaints()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var complaints = await _courierService.GetComplaintsAsync(courierId);
                return Ok(complaints);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新位置信息
        /// </summary>
        /// <param name="locationDto">位置信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("location/update")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto locationDto)
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.UpdateCourierLocationAsync(courierId, locationDto.Latitude, locationDto.Longitude);
                return !success ? NotFound(new { message = "骑手未找到，无法更新位置。" }) : Ok(new { message = "位置更新成功。" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新个人资料
        /// </summary>
        /// <param name="profileDto">个人资料信息</param>
        /// <returns>操作结果</returns>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var courierId = GetCurrentCourierId();
                var success = await _courierService.UpdateProfileAsync(courierId, profileDto);
                return !success ? NotFound(new { success = false, message = "用户未找到，更新失败。" }) : Ok(new { success = true, message = "用户信息更新成功" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { success = false, message = "数据库更新失败，请检查提交的数据是否符合约束。" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"服务器内部错误: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取用于编辑的个人资料
        /// </summary>
        /// <returns>个人资料信息</returns>
        [HttpGet("profile/for-edit")]
        public async Task<ActionResult<UpdateProfileDto>> GetProfileForEdit()
        {
            try
            {
                var courierId = GetCurrentCourierId();
                var profileData = await _courierService.GetProfileForEditAsync(courierId);
                return profileData == null ? NotFound("无法获取用于编辑的用户资料。") : Ok(profileData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取当前配送员ID
        /// </summary>
        /// <returns>配送员ID</returns>
        private int GetCurrentCourierId()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("无法从认证信息中解析有效的用户ID。");
        }
    }
}