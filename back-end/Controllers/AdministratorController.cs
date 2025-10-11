using BackEnd.DTOs.Administrator;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 管理员信息管理控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/info")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly IAdministratorService _administratorService;

        public AdminController(IAdministratorService administratorService)
        {
            _administratorService = administratorService;
        }

        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <returns>管理员信息</returns>
        [HttpGet]
        public async Task<IActionResult> GetAdministratorInfo()
        {
            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var adminInfo = await _administratorService.GetAdministratorInfoAsync(adminId.Value);
            return adminInfo == null ? NotFound("管理员信息未找到") : Ok(adminInfo);
        }

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateAdministratorInfo([FromBody] SetAdminInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _administratorService.UpdateAdministratorInfoAsync(adminId.Value, request);
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

