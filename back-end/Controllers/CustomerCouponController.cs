using Microsoft.AspNetCore.Mvc;
using BackEnd.Services.Interfaces;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Coupon;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 优惠券管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/coupons")]
    [Authorize]
    public class CustomerCouponController : BaseController
    {
        private readonly ICustomerCouponService _customerCouponService;

        public CustomerCouponController(ICustomerCouponService customerCouponService)
        {
            _customerCouponService = customerCouponService;
        }

        /// <summary>
        /// 获取用户优惠券信息（我的优惠券页面，用于结账页面选择优惠券）
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserCoupons()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            try
            {
                // 使用 GetUserCouponsAsync 返回 CustomerCouponDto，包含 CouponType 字段
                var coupons = await _customerCouponService.GetUserCouponsAsync(userId.Value);
                return coupons?.Any() != true 
                    ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No Coupon For User." }) 
                    : Ok(new ApiResponseDto<List<CustomerCouponDto>> { Success = true, Code = 200, Message = "获取成功", Data = coupons });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取所有可领取的优惠券
        /// </summary>
        [HttpGet("available")]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAvailableCoupons()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            try
            {
                var coupons = await _customerCouponService.GetAvailableCouponsAsync(userId.Value);
                return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = coupons });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "获取可领取优惠券失败" });
            }
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="couponManagerId">优惠券管理ID</param>
        [HttpPost("claim/{couponManagerId}")]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ClaimCoupon(int couponManagerId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
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
    }
}

