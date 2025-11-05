using BackEnd.DTOs.AuthRequest;
using BackEnd.DTOs.Common;
using BackEnd.Models.Helpers;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 认证控制器（登录和注册）
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("无效的Token");
                }

                await _authService.LogoutAsync(userId.Value);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "登出成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误: {ex.Message}" });
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 获取店铺种类选项列表
        /// </summary>
        /// <returns>店铺种类选项列表</returns>
        [HttpGet("store-categories")]
        public IActionResult GetStoreCategories()
        {
            var categories = StoreCategoryHelper.GetCategoryOptions()
                .Select(kvp => new { value = (int)kvp.Key, label = kvp.Value })
                .ToList();

            return Ok(categories);
        }

    }
}
