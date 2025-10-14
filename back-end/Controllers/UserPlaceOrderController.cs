using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.User;
using System.ComponentModel.DataAnnotations;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户下单控制器
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize] // 添加认证要求
    public class UserPlaceOrderController : ControllerBase
    {
        private readonly IUserPlaceOrderService _orderService;
        private readonly ILogger<UserPlaceOrderController> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="orderService">订单服务</param>
        /// <param name="logger">日志记录器</param>
        public UserPlaceOrderController(IUserPlaceOrderService orderService, ILogger<UserPlaceOrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>创建结果</returns>
        [HttpPost("store/order/create")]
        [ProducesResponseType(typeof(ResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto>> CreateOrder([FromBody][Required] CreateOrderDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _orderService.CreateOrderAsync(dto);
                return Ok(response);
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ResponseDto { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "创建订单时发生错误");
                return StatusCode(500, new ResponseDto { Success = false, Message = "服务器内部错误，创建订单失败" });
            }
        }

        // 账户更新与地址保存已迁移到 UserProfileController
    }
}
