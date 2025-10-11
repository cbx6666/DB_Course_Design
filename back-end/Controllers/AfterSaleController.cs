using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BackEnd.DTOs.AfterSale;
using BackEnd.Services.Interfaces;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 售后服务管理控制器
    /// </summary>
    [ApiController]
    [Route("api/aftersale")]
    public class AfterSalesController : ControllerBase
    {
        private readonly IAfterSaleService _afterSaleService;

        public AfterSalesController(IAfterSaleService afterSaleService)
        {
            _afterSaleService = afterSaleService;
        }

        /// <summary>
        /// 获取售后服务列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">关键词</param>
        /// <returns>售后服务列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetAfterSales([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? keyword)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new { code = 400, message = "页码和每页数量必须大于0" });
            }

            var sellerId = GetSellerIdFromToken();
            if (sellerId == null)
            {
                return Unauthorized("无效的Token");
            }

            var result = await _afterSaleService.GetAfterSalesAsync(sellerId.Value, page, pageSize, keyword);
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
            return afterSale == null ? NotFound(new { code = 404, message = "售后申请不存在" }) : Ok(afterSale);
        }

        /// <summary>
        /// 处理售后服务申请
        /// </summary>
        /// <param name="id">售后服务ID</param>
        /// <param name="processDto">处理信息</param>
        /// <returns>处理结果</returns>
        [HttpPost("{id}/decide")]
        public async Task<IActionResult> ProcessAfterSale(int id, [FromBody] ProcessAfterSaleDto processDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, message = "请求参数错误" });
            }

            if (processDto.Action != "approve" && processDto.Action != "reject" && processDto.Action != "negotiate")
            {
                return BadRequest(new { code = 400, message = "无效的处理动作" });
            }

            var result = await _afterSaleService.ProcessAfterSaleAsync(id, processDto);
            return Ok(result);
        }

        /// <summary>
        /// 从Token中获取商家ID
        /// </summary>
        /// <returns>商家ID，如果无效则返回null</returns>
        private int? GetSellerIdFromToken()
        {
            var sellerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(sellerIdString, out int sellerId) ? sellerId : null;
        }
    }
}