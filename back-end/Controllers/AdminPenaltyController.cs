using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 店铺举报惩罚管理控制器（管理员侧）
    /// </summary>
    [ApiController]
    [Route("api/admin/penalties")]
    [Authorize]
    public class AdminPenaltyController : BaseController
    {
        private readonly IAdminPenaltyService _adminPenaltyService;

        public AdminPenaltyController(IAdminPenaltyService adminPenaltyService)
        {
            _adminPenaltyService = adminPenaltyService;
        }

        /// <summary>
        /// 获取管理员的违规处罚列表
        /// </summary>
        /// <returns>违规处罚列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetViolationPenaltiesForAdmin()
        {
            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var penaltyDtos = await _adminPenaltyService.GetViolationPenaltiesForAdminAsync(adminId.Value);
            return penaltyDtos == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "未找到违规处罚列表" }) : Ok(penaltyDtos);
        }

        /// <summary>
        /// 更新违规处罚信息
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateViolationPenalty([FromBody] UpdatePenaltyDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "模型验证失败" });
            }

            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var result = await _adminPenaltyService.UpdateViolationPenaltyAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
