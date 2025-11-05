using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Merchant;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 商家信息管理控制器
    /// </summary>
    [ApiController]
    [Route("api/merchant/info")]
    [Authorize]
    public class MerchantInfoController : BaseController
    {
        private readonly IMerchantInfoService _merchantService;

        public MerchantInfoController(IMerchantInfoService merchantService)
        {
            _merchantService = merchantService;
        }

        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <returns>商家信息</returns>
        [HttpGet("info")]
        public async Task<ActionResult<MerchantProfileDto>> GetMerchantInfo()
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var profile = await _merchantService.GetMerchantInfoAsync(sellerId);
                if (!profile.Success || profile.Data == null)
                {
                    return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = profile.Message ?? "获取商家信息失败" });
                }

                return Ok(new ApiResponseDto<MerchantProfileDto> { Success = true, Code = 200, Message = "获取成功", Data = profile.Data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 更新商家信息
        /// </summary>
        [HttpPut("info")]
        public async Task<IActionResult> UpdateMerchantInfo([FromBody] UpdateMerchantProfileDto dto)
        {
            var sellerId = GetCurrentUserId();
            var result = await _merchantService.UpdateMerchantInfoAsync(sellerId, dto);
            return result.Success
                ? Ok(new ApiResponseDto<MerchantUpdateResultDto> { Success = true, Code = 200, Message = result.Message ?? "更新成功", Data = result.Data })
                : BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = result.Message ?? "更新失败" });
        }

        /// <summary>
        /// 更新商家头像
        /// </summary>
        [HttpPut("info/avatar")]
        public async Task<IActionResult> UpdateMerchantAvatar([FromForm] UpdateMerchantAvatarDto dto)
        {
            var sellerId = GetCurrentUserId();
            var result = await _merchantService.UpdateMerchantAvatarAsync(sellerId, dto.AvatarFile);
            return result.Success
                ? Ok(new ApiResponseDto<string> { Success = true, Code = 200, Message = "头像更新成功", Data = result.AvatarUrl ?? string.Empty })
                : BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = result.Message ?? "更新失败" });
        }

    }
}
