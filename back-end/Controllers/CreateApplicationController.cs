using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 创建售后申请控制器
    /// </summary>
    [ApiController]
    [Route("api/user/applications")]
    [Authorize]
    public class CreateApplicationController : ControllerBase
    {
        private readonly ICreateApplicationService _createApplicationService;

        /// <summary>
        /// 初始化创建售后申请控制器
        /// </summary>
        /// <param name="createApplicationService">创建售后申请服务</param>
        public CreateApplicationController(ICreateApplicationService createApplicationService)
        {
            _createApplicationService = createApplicationService;
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request">售后申请请求数据</param>
        /// <returns>创建结果</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateApplication([FromBody] CreateApplicationDto request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _createApplicationService.CreateApplicationAsync(request, userId.Value);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 从Token中获取用户ID
        /// </summary>
        /// <returns>用户ID，如果无效则返回null</returns>
        private int? GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out int userId) ? userId : null;
        }
    }
}
