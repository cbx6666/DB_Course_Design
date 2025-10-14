using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.MerchantInfo;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 商家信息管理控制器
    /// </summary>
    [ApiController]
    [Route("api/merchant")]
    [Authorize]
    public class MerchantInformationController : ControllerBase
    {
        private readonly IMerchantInformationService _merchantInformationService;

        /// <summary>
        /// 初始化商家信息管理控制器
        /// </summary>
        /// <param name="merchantInformationService">商家信息服务</param>
        public MerchantInformationController(IMerchantInformationService merchantInformationService)
        {
            _merchantInformationService = merchantInformationService;
        }

        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <returns>商家信息</returns>
        [HttpGet("profile")]
        public async Task<IActionResult> FetchMerchantInfo()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _merchantInformationService.GetMerchantInfoAsync(userId.Value);
            return result.Success 
                ? Ok(new { code = 200, success = true, data = result.Data })
                : BadRequest(new { code = 400, success = false, message = result.Message });
        }

        /// <summary>
        /// 更新商家信息
        /// </summary>
        /// <param name="dto">更新商家信息请求</param>
        /// <returns>更新结果</returns>
		[HttpPut("profile")]
		public async Task<IActionResult> SaveShopInfo([FromBody] UpdateMerchantProfileDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _merchantInformationService.UpdateMerchantInfoAsync(userId.Value, dto);
            return result.Success
                ? Ok(new
                {
                    code = 200,
                    success = true,
                    message = result.Message,
                    data = result.Data
                })
                : BadRequest(new { code = 400, success = false, message = result.Message });
        }

		/// <summary>
		/// 更新商家头像（multipart/form-data）
		/// </summary>
		[HttpPut("profile/avatar")]
		public async Task<IActionResult> UpdateAvatar([FromForm] UpdateMerchantAvatarDto dto)
		{
			var userId = GetUserIdFromToken();
			if (userId == null)
			{
				return Unauthorized("无效的Token");
			}

			var result = await _merchantInformationService.UpdateMerchantAvatarAsync(userId.Value, dto.AvatarFile);
			return result.Success
				? Ok(new { code = 200, success = true, avatar = result.AvatarUrl })
				: BadRequest(new { code = 400, success = false, message = result.Message });
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
