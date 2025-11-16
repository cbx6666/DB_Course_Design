using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 店铺举报惩罚管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/penalties")]
    [Authorize]
    public class MerchantPenaltyController : BaseController
    {
        private readonly IMerchantPenaltyService _merchantPenaltyService;

        /// <summary>
        /// 初始化店铺举报惩罚管理控制器
        /// </summary>
        /// <param name="merchantPenaltyService">店铺举报惩罚服务（商家侧）</param>
        public MerchantPenaltyController(IMerchantPenaltyService merchantPenaltyService)
        {
            _merchantPenaltyService = merchantPenaltyService;
        }

        /// <summary>
        /// 获取处罚记录列表
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="field">筛选字段（id | reason）</param>
        /// <returns>处罚记录列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetPenalties([FromQuery] string? keyword, [FromQuery] string? field)
        {
            var sellerId = GetUserIdFromToken();
            if (sellerId == null)
            {
                return Unauthorized("无效的Token");
            }

            var penalties = await _merchantPenaltyService.GetPenaltiesAsync(sellerId.Value, keyword, field);
            return Ok(penalties);
        }

        /// <summary>
        /// 根据ID获取处罚记录
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <returns>处罚记录详情</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPenaltyById(string id)
        {
            var penalty = await _merchantPenaltyService.GetPenaltyByIdAsync(id);
            return penalty == null 
                ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "处罚记录不存在" })
                : Ok(penalty);
        }

        /// <summary>
        /// 对处罚记录进行申诉
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <param name="appealDto">申诉请求</param>
        /// <returns>申诉结果</returns>
        [HttpPost("{id}/appeal")]
        public async Task<IActionResult> AppealPenalty(string id, [FromBody] AppealPenaltyDto appealDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数错误" });
            }

            var result = await _merchantPenaltyService.AppealPenaltyAsync(id, appealDto);
            return Ok(result);
        }

    }
}
