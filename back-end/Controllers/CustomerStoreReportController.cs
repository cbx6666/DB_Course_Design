using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 店铺举报管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/store-reports")]
    [Authorize]
    public class CustomerStoreReportController : BaseController
    {
        private readonly ICustomerStoreReportService _customerStoreReportService;

        /// <summary>
        /// 初始化店铺举报管理控制器
        /// </summary>
        /// <param name="customerStoreReportService">店铺举报服务（消费者侧）</param>
        public CustomerStoreReportController(ICustomerStoreReportService customerStoreReportService)
        {
            _customerStoreReportService = customerStoreReportService;
        }

        /// <summary>
        /// 用户投诉店铺
        /// </summary>
        [HttpPost("{storeId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportStore([FromRoute] int storeId, [FromBody] ReportStoreDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("无效的Token");
                }

                dto.StoreId = storeId;
                dto.UserId = userId.Value;

                await _customerStoreReportService.SubmitStoreReportAsync(dto);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "投诉已提交，等待管理员审核" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "提交投诉时发生错误" });
            }
        }

        /// <summary>
        /// 获取用户的店铺举报列表
        /// </summary>
        /// <returns>店铺举报列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetMyReports()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _customerStoreReportService.GetMyReportsAsync(userId.Value);
            return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = result });
        }
    }
}

