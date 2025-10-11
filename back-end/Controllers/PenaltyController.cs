using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BackEnd.DTOs.Penalty;
using BackEnd.Services.Interfaces;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 处罚管理控制器
    /// </summary>
    [ApiController]
    [Route("api/penalties")]
    public class PenaltiesController : ControllerBase
    {
        private readonly IPenaltyService _penaltyService;

        /// <summary>
        /// 初始化处罚管理控制器
        /// </summary>
        /// <param name="penaltyService">处罚服务</param>
        public PenaltiesController(IPenaltyService penaltyService)
        {
            _penaltyService = penaltyService;
        }

        /// <summary>
        /// 获取处罚记录列表
        /// </summary>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>处罚记录列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetPenalties([FromQuery] string? keyword)
        {
            var sellerId = GetSellerIdFromToken();
            if (sellerId == null)
            {
                return Unauthorized("无效的Token");
            }

            var penalties = await _penaltyService.GetPenaltiesAsync(sellerId.Value, keyword);
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
            var penalty = await _penaltyService.GetPenaltyByIdAsync(id);
            return penalty == null 
                ? NotFound(new { code = 404, message = "处罚记录不存在" })
                : Ok(penalty);
        }

        /// <summary>
        /// 对处罚记录进行申诉
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <param name="appealDto">申诉请求</param>
        /// <returns>申诉结果</returns>
        [HttpPost("{id}/appeal")]
        public async Task<IActionResult> AppealPenalty(string id, [FromBody] AppealDto appealDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, message = "请求参数错误" });
            }

            var result = await _penaltyService.AppealPenaltyAsync(id, appealDto);
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