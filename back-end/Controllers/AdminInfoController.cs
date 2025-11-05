using BackEnd.DTOs.Administrator;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 管理员信息管理控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/info")]
    [Authorize]
    public class AdminInfoController : BaseController
    {
        private readonly IAdminInfoService _administratorService;

        public AdminInfoController(IAdminInfoService administratorService)
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
            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var adminInfo = await _administratorService.GetAdministratorInfoAsync(adminId.Value);
            return adminInfo == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "管理员信息未找到" }) : Ok(adminInfo);
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
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _administratorService.UpdateAdministratorInfoAsync(adminId.Value, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
