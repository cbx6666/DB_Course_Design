using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

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
        private readonly ILogger<UserProfileController> _logger;

        public UserProfileController(IUserProfileService userService, ILogger<UserProfileController> logger)
        {
            _userService = userService;
            _logger = logger;
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
        /// 获取用户全部收货地址列表
        /// </summary>
        [HttpGet("addresses")]
        public async Task<ActionResult<IEnumerable<UserDeliveryInfoDto>>> GetUserAddresses()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var list = await _userService.GetUserAddressesAsync(userId.Value);
            return Ok(list);
        }

        /// <summary>
        /// 更新账户信息（姓名、头像）
        /// </summary>
        [HttpPut("account/update")]
        public async Task<ActionResult<ResponseDto>> UpdateAccount([FromForm] UpdateAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.UpdateAccountAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 新增或更新默认收货地址
        /// </summary>
        [HttpPut("account/address/save")]
        public async Task<ActionResult<ResponseDto>> SaveOrUpdateAddress([FromBody] SaveAddressDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _userService.SaveOrUpdateAddressAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 新建收货地址
        /// </summary>
        [HttpPost("account/address/create")]
        public async Task<ActionResult<ResponseDto>> CreateAddress([FromBody][Required] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _userService.CreateAddressAsync(userId.Value, dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建收货地址时发生错误");
                return StatusCode(500, new ResponseDto { Success = false, Message = "服务器内部错误，创建收货地址失败" });
            }
        }

        /// <summary>
        /// 更新收货地址
        /// </summary>
        [HttpPut("account/address/update/{addressId}")]
        public async Task<ActionResult<ResponseDto>> UpdateAddress(int addressId, [FromBody][Required] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _userService.UpdateAddressAsync(userId.Value, addressId, dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新收货地址时发生错误");
                return StatusCode(500, new ResponseDto { Success = false, Message = "服务器内部错误，更新收货地址失败" });
            }
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        [HttpDelete("account/address/delete/{addressId}")]
        public async Task<ActionResult<ResponseDto>> DeleteAddress(int addressId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _userService.DeleteAddressAsync(userId.Value, addressId);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除收货地址时发生错误");
                return StatusCode(500, new ResponseDto { Success = false, Message = "服务器内部错误，删除收货地址失败" });
            }
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        [HttpPut("account/address/set-default/{addressId}")]
        public async Task<ActionResult<ResponseDto>> SetDefaultAddress(int addressId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _userService.SetDefaultAddressAsync(userId.Value, addressId);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "设置默认收货地址时发生错误");
                return StatusCode(500, new ResponseDto { Success = false, Message = "服务器内部错误，设置默认收货地址失败" });
            }
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