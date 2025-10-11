using BackEnd.DTOs.ViolationPenalty;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 违规处罚管理控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/violation-penalties")]
    [Authorize]
    public class Supervise_Controller : ControllerBase
    {
        private readonly ISupervise_Service _superviseService;

        public Supervise_Controller(ISupervise_Service superviseService)
        {
            _superviseService = superviseService;
        }

        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        /// <returns>违规处罚列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetViolationPenaltiesForAdmin()
        {
            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var penaltyDtos = await _superviseService.GetViolationPenaltiesForAdminAsync(adminId.Value);
            return penaltyDtos == null ? NotFound() : Ok(penaltyDtos);
        }

        /// <summary>
        /// 更新违规处罚信息
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateViolationPenalty([FromBody] SetViolationPenaltyInfo request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                return BadRequest(new
                {
                    success = false,
                    message = "模型验证失败",
                    errors = errors
                });
            }

            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var result = await _superviseService.UpdateViolationPenaltyAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 从Token中获取管理员ID
        /// </summary>
        /// <returns>管理员ID，如果无效则返回null</returns>
        private int? GetAdminIdFromToken()
        {
            var adminIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(adminIdString, out int adminId) ? adminId : null;
        }
    }
}
