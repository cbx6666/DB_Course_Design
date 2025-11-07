using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 订单管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/orders")]
    [Authorize]
    public class CustomerOrderController : BaseController
    {
        private readonly ICustomerOrderService _customerOrderService;

        public CustomerOrderController(ICustomerOrderService customerOrderService)
        {
            _customerOrderService = customerOrderService;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto>> CreateOrder([FromBody][Required] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数无效" });

            // 从 Token 中获取用户ID，而不是信任前端传递的 CustomerId
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            // 使用从 Token 中获取的 UserID 覆盖 DTO 中的 CustomerId
            dto.CustomerId = userId.Value;

            try
            {
                var response = await _customerOrderService.CreateOrderAsync(dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"服务器内部错误，创建订单失败: {ex.Message}" });
            }
        }

        /// <summary>
        /// 获取用户历史订单
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var orderHistory = await _customerOrderService.GetOrderHistoryAsync(userId.Value);
            return orderHistory == null 
                ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "There's No OrderHistory For User." }) 
                : Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = orderHistory });
        }

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送信息</returns>
        [HttpGet("{orderId}/delivery-info")]
        [ProducesResponseType(typeof(ApiResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrderDeliveryInfo(int orderId)
        {
            try
            {
                var info = await _customerOrderService.GetOrderDeliveryInfoAsync(orderId);
                return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = info });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }
    }
}
