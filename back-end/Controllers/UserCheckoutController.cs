using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户结账控制器
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize] // 添加认证要求
    public class UserCheckoutController : ControllerBase
    {
        private readonly IUserCheckoutService _userCheckoutService;
        private readonly ILogger<UserCheckoutController> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userCheckoutService">用户结账服务</param>
        /// <param name="logger">日志记录器</param>
        public UserCheckoutController(
            IUserCheckoutService userCheckoutService,
            ILogger<UserCheckoutController> logger)
        {
            _userCheckoutService = userCheckoutService;
            _logger = logger;
        }

        /// <summary>
        /// 获取用户优惠券
        /// </summary>
        /// <returns>用户优惠券列表</returns>
        [HttpGet("user/coupons")]
        [ProducesResponseType(typeof(List<UserCouponDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UserCouponDto>>> GetUserCoupons()
        {
            try
            {
                // 从 Token 中安全地获取用户 ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("无效的Token");
                }

                var coupons = await _userCheckoutService.GetUserCouponsAsync(userId);
                return Ok(coupons);
            }
            catch (ValidationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户优惠券信息时发生错误");
                return StatusCode(500, new { message = "获取优惠券信息时发生错误" });
            }
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>购物车信息</returns>
        [HttpGet("store/shoppingcart")]
        [ProducesResponseType(typeof(ShoppingCartItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ShoppingCartItemDto>> GetShoppingCart(
            [FromQuery, Required] int storeId)
        {
            try
            {
                // 从 Token 中安全地获取用户 ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("无效的Token");
                }

                var shoppingCart = await _userCheckoutService.GetShoppingCartAsync(userId, storeId);
                return Ok(shoppingCart);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取用户在店铺 {StoreId} 的购物车信息时发生错误", storeId);
                return StatusCode(500, new { message = "获取购物车信息时发生错误" });
            }
        }

        /// <summary>
        /// 添加或更新购物车项
        /// </summary>
        /// <param name="dto">更新购物车项请求</param>
        /// <returns>更新结果</returns>
        [HttpPost("store/cart/change")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> UpdateCartItem(
            [FromBody] UpdateCartItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userCheckoutService.UpdateCartItemAsync(dto);
                return Ok(new { message = "购物车更新成功" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "更新购物车项时发生错误 (CartId={CartId}, DishId={DishId})", dto.CartId, dto.DishId);
                return StatusCode(500, new { message = "更新购物车项时发生错误" });
            }
        }

        /// <summary>
        /// 删除购物车项
        /// </summary>
        /// <param name="dto">删除购物车项请求</param>
        /// <returns>删除结果</returns>
        [HttpDelete("store/cart/remove")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> RemoveCartItem(
            [FromBody] RemoveCartItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _userCheckoutService.RemoveCartItemAsync(dto);
                return Ok(new { message = "购物车项删除成功" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "删除购物车项时发生错误 (CartId={CartId}, DishId={DishId})", dto.CartId, dto.DishId);
                return StatusCode(500, new { message = "删除购物车项时发生错误" });
            }
        }
    }
}