using BackEnd.DTOs.Courier;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 配送员信息管理控制器（骑手侧）
    /// </summary>
    [ApiController]
    [Route("api/courier/info")]
    [Authorize]
    public class CourierInfoController : BaseController
    {
        private readonly ICourierInfoService _courierService;

        public CourierInfoController(ICourierInfoService courierService)
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
                var courierId = GetCurrentUserId();
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
                var courierId = GetCurrentUserId();
                var statusDto = await _courierService.GetWorkStatusAsync(courierId);
                return statusDto == null ? NotFound("骑手资料未找到，无法获取状态") : Ok(new ApiResponseDto<bool> { Success = true, Code = 200, Message = "获取状态成功", Data = statusDto.IsOnline });
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
                var courierId = GetCurrentUserId();
                var area = await _courierService.GetCurrentLocationAsync(courierId);
                return Ok(new ApiResponseDto<string> { Success = true, Code = 200, Message = "获取位置成功", Data = area });
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
        /// 切换配送员工作状态
        /// </summary>
        /// <param name="request">状态切换请求</param>
        /// <returns>操作结果</returns>
        [HttpPost("status/toggle")]
        public async Task<IActionResult> ToggleStatus([FromBody] ToggleStatusRequestDto request)
        {
            try
            {
                var courierId = GetCurrentUserId();
                var success = await _courierService.ToggleWorkStatusAsync(courierId, request.IsOnline);
                return !success ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "骑手不存在，无法更新状态" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "操作成功" });
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
        /// 获取月度收入
        /// </summary>
        /// <returns>月度收入</returns>
        [HttpGet("income/monthly")]
        public async Task<IActionResult> GetMonthlyIncome()
        {
            try
            {
                var courierId = GetCurrentUserId();
                var monthlyIncome = await _courierService.GetMonthlyIncomeAsync(courierId);
                return Content(monthlyIncome.ToString("F2"), "text/plain");
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
        /// 更新位置信息
        /// </summary>
        /// <param name="locationDto">位置信息</param>
        /// <returns>操作结果</returns>
        [HttpPost("location/update")]
        public async Task<IActionResult> UpdateLocation([FromBody] UpdateLocationDto locationDto)
        {
            try
            {
                var courierId = GetCurrentUserId();
                var success = await _courierService.UpdateCourierLocationAsync(courierId, locationDto.Latitude, locationDto.Longitude);
                return !success ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "骑手未找到，无法更新位置。" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "位置更新成功。" });
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
                var courierId = GetCurrentUserId();
                var success = await _courierService.UpdateProfileAsync(courierId, profileDto);
                return !success ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "用户未找到，更新失败。" }) : Ok(new ApiResponseDto { Success = true, Code = 200, Message = "用户信息更新成功" });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "数据库更新失败，请检查提交的数据是否符合约束。" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
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
                var courierId = GetCurrentUserId();
                var profileData = await _courierService.GetProfileForEditAsync(courierId);
                return profileData == null ? NotFound("无法获取用于编辑的用户资料。") : Ok(profileData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"服务器内部错误: {ex.Message}");
            }
        }
    }
}
