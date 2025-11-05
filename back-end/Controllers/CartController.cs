using BackEnd.DTOs.Cart;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 购物车控制器
    /// </summary>
    [ApiController]
    [Route("api/cart")]
    [Authorize]
    public class CartController : BaseController
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        [HttpGet("store/{storeId}")]
        [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CartResponseDto>> GetShoppingCart([FromRoute, Required] int storeId)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
                }

                var shoppingCart = await _cartService.GetShoppingCartAsync(userId.Value, storeId);
                return Ok(shoppingCart);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "获取购物车信息时发生错误" });
            }
        }

        /// <summary>
        /// 添加或更新购物车项
        /// </summary>
        [HttpPost("item/update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _cartService.UpdateCartItemAsync(dto);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "购物车更新成功" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "更新购物车项时发生错误" });
            }
        }

        /// <summary>
        /// 删除购物车项
        /// </summary>
        [HttpDelete("item/remove")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> RemoveCartItem([FromBody] RemoveCartItemDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _cartService.RemoveCartItemAsync(dto);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "购物车项删除成功" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "删除购物车项时发生错误" });
            }
        }
    }
}
