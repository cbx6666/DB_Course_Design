using BackEnd.DTOs.Order;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 订单管理控制器
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IDeliveryTaskService _deliveryService;

        public OrdersController(IOrderService orderService, IDeliveryTaskService deliveryService)
        {
            _orderService = orderService;
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
                var orders = await _orderService.GetOrdersAsync(sellerId, storeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取订单详情
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单详情</returns>
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(orderId);
                return order == null ? NotFound(new { code = 404, message = "订单不存在" }) : Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var result = await _orderService.AcceptOrderAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = 404, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var result = await _orderService.MarkAsReadyAsync(orderId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = 404, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="dto">拒绝原因</param>
        /// <returns>操作结果</returns>
        [HttpPost("{orderId}/reject")]
        public async Task<IActionResult> RejectOrder(int orderId, [FromBody] RejectOrderDto dto)
        {
            try
            {
                var result = await _orderService.RejectOrderAsync(orderId, dto.Reason);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = 404, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var coupons = await _orderService.GetOrderCouponsAsync(orderId);
                return Ok(coupons);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { code = 404, message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }
    }
}