using Microsoft.AspNetCore.Mvc;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 购物车管理控制器
    /// </summary>
    [ApiController]
    [Route("api/carts")]
    public class CartsController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public CartsController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// 获取购物车商品列表
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车商品列表</returns>
        [HttpGet("{cartId}/items")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            try
            {
                var items = await _orderService.GetCartItemsAsync(cartId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }
    }
}