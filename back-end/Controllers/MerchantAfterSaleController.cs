using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 售后服务管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/after-sales")]
    [Authorize]
    public class MerchantAfterSaleController : BaseController
    {
        private readonly IMerchantAfterSaleService _afterSaleService;

        public MerchantAfterSaleController(IMerchantAfterSaleService afterSaleService)
        {
            _afterSaleService = afterSaleService;
        }

        /// <summary>
        /// 获取售后服务列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">关键词</param>
        /// <param name="field">筛选字段（content | user.name | orderNo）</param>
        /// <returns>售后服务列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetAfterSales([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? keyword, [FromQuery] string? field)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "页码和每页数量必须大于0" });
            }

            var sellerId = GetUserIdFromToken();
            if (sellerId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _afterSaleService.GetAfterSalesAsync(sellerId.Value, page, pageSize, keyword, field);
            return Ok(result);
        }

        /// <summary>
        /// 根据ID获取售后服务详情
        /// </summary>
        /// <param name="id">售后服务ID</param>
        /// <returns>售后服务详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAfterSaleById(int id)
        {
            var afterSale = await _afterSaleService.GetAfterSaleByIdAsync(id);
            return afterSale == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "售后申请不存在" }) : Ok(afterSale);
        }

        /// <summary>
        /// 商家提交回复（仅待处理状态可提交，提交后置为商家反馈）
        /// </summary>
        /// <param name="id">售后服务ID</param>
        /// <param name="replyDto">商家回复</param>
        /// <returns>处理结果</returns>
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> SubmitMerchantReply(int id, [FromBody] MerchantReplyDto replyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数错误" });
            }

            if (string.IsNullOrWhiteSpace(replyDto.Remark))
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "回复内容不能为空" });
            }

            var result = await _afterSaleService.SubmitMerchantReplyAsync(id, replyDto);
            return Ok(result);
        }
    }
}
