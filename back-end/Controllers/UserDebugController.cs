using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户调试功能控制器
    /// </summary>
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class UserDebugController : ControllerBase
    {
        private readonly IUserDebugService _userDebugService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userDebugService">用户调试服务</param>
        public UserDebugController(IUserDebugService userDebugService)
        {
            _userDebugService = userDebugService;
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户基本信息</returns>
        [HttpGet("debug/userinfo")]
        [ProducesResponseType(typeof(UserInfoResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<UserInfoResponseDto>> GetUserInfo([FromQuery] int userId)
        {
            var result = await _userDebugService.GetUserInfoAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// 提交订单
        /// </summary>
        /// <param name="dto">订单提交信息</param>
        /// <returns>提交结果</returns>
        [HttpPost("store/checkout")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderRequestDto dto)
        {
            await _userDebugService.SubmitOrderAsync(dto);
            return Ok(new { Message = "订单提交成功" });
        }

        /// <summary>
        /// 根据手机号或邮箱获取用户ID
        /// </summary>
        /// <param name="dto">包含手机号或邮箱的请求对象</param>
        /// <returns>用户ID信息</returns>
        [HttpPost("getid")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(GetUserIdResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<GetUserIdResponseDto>> GetUserId([FromBody] GetUserIdRequestDto dto)
        {
            var result = await _userDebugService.GetUserIdAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 使用优惠券
        /// </summary>
        /// <param name="dto">优惠券使用信息</param>
        /// <returns>使用结果</returns>
        [HttpPost("user/checkout/coupon")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UseCoupon([FromBody] UsedCouponDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userDebugService.UseCouponAsync(dto.CouponId);
            return Ok(new { message = "优惠券已使用" });
        }
    }
}

