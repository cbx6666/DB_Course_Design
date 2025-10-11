using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 创建配送投诉控制器
    /// </summary>
    [ApiController]
    [Route("api/user/complaints")]
    [Authorize]
    public class CreateComplaintController : ControllerBase
    {
        private readonly ICreateComplaintService _createComplaintService;

        /// <summary>
        /// 初始化创建配送投诉控制器
        /// </summary>
        /// <param name="createComplaintService">创建配送投诉服务</param>
        public CreateComplaintController(ICreateComplaintService createComplaintService)
        {
            _createComplaintService = createComplaintService;
        }

        /// <summary>
        /// 创建配送投诉
        /// </summary>
        /// <param name="request">配送投诉请求数据</param>
        /// <returns>创建结果</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintDto request)
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

            var result = await _createComplaintService.CreateComplaintAsync(request, userId.Value);
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

