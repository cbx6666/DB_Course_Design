using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 订单管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/orders")]
    [Authorize]
    public class MerchantOrderController : BaseController
    {
        private readonly IMerchantOrderService _merchantOrderService;
        private readonly IDeliveryTaskService _deliveryService;

        public MerchantOrderController(
            IMerchantOrderService merchantOrderService,
            IDeliveryTaskService deliveryService)
        {
            _merchantOrderService = merchantOrderService;
            _deliveryService = deliveryService;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>订单列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders([FromQuery] int? sellerId, [FromQuery] int? storeId)
        {
            try
            {
                var orders = await _merchantOrderService.GetOrdersAsync(sellerId, storeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{orderId}/accept")]
        public async Task<IActionResult> AcceptOrder(int orderId)
        {
            try
            {
                var result = await _merchantOrderService.AcceptOrderAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 标记订单为准备完成
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{orderId}/ready")]
        public async Task<IActionResult> MarkAsReady(int orderId)
        {
            try
            {
                var result = await _merchantOrderService.MarkAsReadyAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取订单优惠券信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>优惠券信息</returns>
        [HttpGet("{orderId}/coupons")]
        public async Task<IActionResult> GetOrderCoupons(int orderId)
        {
            try
            {
                var coupons = await _merchantOrderService.GetOrderCouponsAsync(orderId);
                return Ok(coupons);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送信息</returns>
        [HttpGet("{orderId}/delivery-info")]
        public async Task<IActionResult> GetOrderDeliveryInfo(int orderId)
        {
            try
            {
                var info = await _deliveryService.GetOrderDeliveryInfoAsync(orderId);
                return Ok(info);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取购物车商品列表
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车商品列表</returns>
        [HttpGet("cart/{cartId}/items")]
        public async Task<IActionResult> GetCartItems(int cartId)
        {
            try
            {
                var items = await _merchantOrderService.GetCartItemsAsync(cartId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }
    }
}
