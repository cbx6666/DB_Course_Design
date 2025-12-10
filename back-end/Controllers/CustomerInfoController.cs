using Microsoft.AspNetCore.Mvc;
using BackEnd.Services.Interfaces;
using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Common;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 客户信息控制器（整合用户首页和用户档案功能）
    /// </summary>
    [ApiController]
    [Route("api/customer/info")]
    [Authorize]
    public class CustomerInfoController : BaseController
    {
        private readonly ICustomerInfoService _customerService;

        public CustomerInfoController(ICustomerInfoService customerService)
        {
            _customerService = customerService;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [HttpGet("home/userInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var userInfo = await _customerService.GetUserProfileAsync(userId.Value);
            if (userInfo == null)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "User not found" });
            }
            
            return Ok(new ApiResponseDto<UserProfileDto> { Success = true, Code = 200, Message = "获取成功", Data = userInfo });
        }

        /// <summary>
        /// 获取用户的收藏夹列表
        /// </summary>
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavoritesFolders()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var folders = await _customerService.GetFavoritesFoldersAsync(userId.Value);
            return Ok(new ApiResponseDto<List<FavoritesFolderDto>> { Success = true, Code = 200, Message = "获取成功", Data = folders });
        }

        /// <summary>
        /// 新建收藏夹
        /// </summary>
        [HttpPost("favorites")]
        public async Task<IActionResult> CreateFavoritesFolder([FromBody] CreateFavoritesFolderDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerService.CreateFavoritesFolderAsync(userId.Value, dto.FolderName);
            return Ok(result);
        }

        /// <summary>
        /// 删除收藏夹（不可删除默认收藏夹）
        /// </summary>
        [HttpDelete("favorites/{folderId}")]
        public async Task<IActionResult> DeleteFavoritesFolder(int folderId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerService.DeleteFavoritesFolderAsync(userId.Value, folderId);
            return Ok(result);
        }

        /// <summary>
        /// 向收藏夹添加店铺
        /// </summary>
        [HttpPost("favorites/{folderId}/items")]
        public async Task<IActionResult> AddFavoriteItem(int folderId, [FromBody] AddFavoriteItemDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerService.AddFavoriteItemAsync(userId.Value, folderId, dto);
            return Ok(result);
        }

        /// <summary>
        /// 从收藏夹删除店铺
        /// </summary>
        [HttpDelete("favorites/{folderId}/items")]
        public async Task<IActionResult> RemoveFavoriteItem(int folderId, [FromBody] RemoveFavoriteItemDto dto)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerService.RemoveFavoriteItemAsync(userId.Value, folderId, dto.StoreId);
            return Ok(result);
        }

        /// <summary>
        /// 获取用户个人资料
        /// </summary>
        [HttpGet("profile/userProfile")]
        public async Task<ActionResult<UserProfileDto>> GetUserProfile()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var userProfile = await _customerService.GetUserProfileAsync(userId.Value);
            if (userProfile == null)
            {
                return NotFound("用户不存在");
            }

            return Ok(userProfile);
        }

        /// <summary>
        /// 获取用户全部收货地址列表
        /// </summary>
        [HttpGet("profile/addresses")]
        public async Task<ActionResult<IEnumerable<UserDeliveryInfoDto>>> GetUserAddresses()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var list = await _customerService.GetUserAddressesAsync(userId.Value);
            return Ok(list);
        }

        /// <summary>
        /// 更新账户信息（姓名、头像）
        /// </summary>
        [HttpPut("profile/account/update")]
        public async Task<ActionResult<ApiResponseDto>> UpdateAccount([FromForm] UpdateAccountDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // 从Token中获取用户ID，确保安全性
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }
            
            // 使用Token中的用户ID，忽略前端传的ID（防止用户修改他人信息）
            dto.Id = userId.Value;
            
            var result = await _customerService.UpdateAccountAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 新建收货地址
        /// </summary>
        [HttpPost("profile/account/address/create")]
        public async Task<ActionResult<ApiResponseDto>> CreateAddress([FromBody][Required] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _customerService.CreateAddressAsync(userId.Value, dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "服务器内部错误，创建收货地址失败" });
            }
        }

        /// <summary>
        /// 更新收货地址
        /// </summary>
        [HttpPut("profile/account/address/update/{addressId}")]
        public async Task<ActionResult<ApiResponseDto>> UpdateAddress(int addressId, [FromBody][Required] CreateAddressDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _customerService.UpdateAddressAsync(userId.Value, addressId, dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "服务器内部错误，更新收货地址失败" });
            }
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        [HttpDelete("profile/account/address/delete/{addressId}")]
        public async Task<ActionResult<ApiResponseDto>> DeleteAddress(int addressId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _customerService.DeleteAddressAsync(userId.Value, addressId);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "服务器内部错误，删除收货地址失败" });
            }
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        [HttpPut("profile/account/address/set-default/{addressId}")]
        public async Task<ActionResult<ApiResponseDto>> SetDefaultAddress(int addressId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return Unauthorized("无效的Token");

            try
            {
                var response = await _customerService.SetDefaultAddressAsync(userId.Value, addressId);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "服务器内部错误，设置默认收货地址失败" });
            }
        }

    }
}
