using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户个人资料控制器
    /// </summary>
    [Route("api/user/profile")]
    [ApiController]
    [Authorize]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userService;

        public UserProfileController(IUserProfileService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取用户个人资料
        /// </summary>
        /// <returns>用户个人资料信息</returns>
        [HttpGet("userProfile")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var userProfile = await _userService.GetUserProfileAsync(userId.Value);

            if (userProfile == null)
            {
                return NotFound("用户不存在");
            }

            return Ok(userProfile);
        }
        
        /// <summary>
        /// 获取用户地址信息
        /// </summary>
        /// <returns>用户地址信息</returns>
        [HttpGet("address")]
        public async Task<ActionResult<UserAddressDto>> GetUserAddress()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var userAddress = await _userService.GetUserAddressAsync(userId.Value);

            if (userAddress == null)
            {
                return NotFound("用户地址信息不存在");
            }

            return Ok(userAddress);
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