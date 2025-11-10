using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 售后申请管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/after-sales")]
    [Authorize]
    public class CustomerAfterSaleController : BaseController
    {
        private readonly ICustomerAfterSaleService _afterSaleService;

        /// <summary>
        /// 初始化售后申请管理控制器
        /// </summary>
        /// <param name="afterSaleService">售后申请服务（消费者侧）</param>
        public CustomerAfterSaleController(ICustomerAfterSaleService afterSaleService)
        {
            _afterSaleService = afterSaleService;
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request">售后申请请求数据</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        public async Task<IActionResult> CreateApplication([FromBody] CreateAfterSaleApplicationDto request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _afterSaleService.CreateApplicationAsync(request, userId.Value);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 获取用户的售后申请列表
        /// </summary>
        /// <returns>售后申请列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyAfterSales()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            try
            {
                var result = await _afterSaleService.GetMyAfterSalesAsync(userId.Value);
                return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"查询失败: {ex.Message}" });
            }
        }

    }
}
