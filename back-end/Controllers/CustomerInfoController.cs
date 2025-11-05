using Microsoft.AspNetCore.Mvc;
using BackEnd.Services.Interfaces;
using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Coupon;
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
        private readonly ICustomerCouponService _customerCouponService;

        public CustomerInfoController(
            ICustomerInfoService customerService,
            ICustomerCouponService customerCouponService)
        {
            _customerService = customerService;
            _customerCouponService = customerCouponService;
        }

        /// <summary>
        /// 获取推荐商家
        /// </summary>
        [HttpGet("home/recommend")]
        public async Task<IActionResult> GetRecommendedStores()
        {
            var result = await _customerService.GetRecommendedStoresAsync();
            return result == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No Recommend Store For User." }) : Ok(result);
        }

        /// <summary>
        /// 搜索商家和菜品
        /// </summary>
        [HttpGet("home/search")]
        public async Task<IActionResult> Search([FromQuery] HomeSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "Invalid request" });
            }

            var (stores, dishes) = await _customerService.SearchAsync(searchDto);

            if (stores == null && dishes == null)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No Search results." });
            }

            var searchStores = new List<object>();
            if (stores?.Any() == true) searchStores.AddRange(stores);
            if (dishes?.Any() == true) searchStores.AddRange(dishes);

            return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "搜索成功", Data = new { searchStores } });
        }

        /// <summary>
        /// 获取用户历史订单
        /// </summary>
        [HttpGet("home/orders")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var orderHistory = await _customerService.GetOrderHistoryAsync(userId.Value);
            return orderHistory == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No OrderHistory For User." }) : Ok(orderHistory);
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
                return Unauthorized("无效的Token");
            }

            var userInfo = await _customerService.GetUserProfileAsync(userId.Value);
            return userInfo == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "User not found" }) : Ok(userInfo);
        }

        /// <summary>
        /// 获取所有商家
        /// </summary>
        [HttpGet("home/stores")]
        public async Task<ActionResult<StoresResponseDto>> GetAllStores()
        {
            try
            {
                var stores = await _customerService.GetAllStoresAsync();
                return Ok(stores);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "获取商店信息时发生错误" });
            }
        }

        /// <summary>
        /// 获取用户优惠券信息（我的优惠券页面）
        /// </summary>
        [HttpGet("home/couponInfo")]
        public async Task<IActionResult> GetUserCoupons()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var coupons = await _customerCouponService.GetUserCouponListAsync(userId.Value);
            return coupons?.Any() != true ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No Coupon For User." }) : Ok(coupons);
        }

        /// <summary>
        /// 获取所有可领取的优惠券
        /// </summary>
        [HttpGet("home/available-coupons")]
        public async Task<IActionResult> GetAvailableCoupons()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            try
            {
                var coupons = await _customerCouponService.GetAvailableCouponsAsync(userId.Value);
                return Ok(coupons);
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "获取可领取优惠券失败" });
            }
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        [HttpPost("home/claim-coupon/{couponManagerId}")]
        public async Task<IActionResult> ClaimCoupon(int couponManagerId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            try
            {
                var success = await _customerCouponService.ClaimCouponAsync(userId.Value, couponManagerId);
                if (success)
                {
                    return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "优惠券领取成功" });
                }
                else
                {
                    return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "优惠券领取失败，可能已领取过或已领完" });
                }
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "领取优惠券失败" });
            }
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
