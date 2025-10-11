using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 售后申请评估控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/after-sales")]
    [Authorize]
    public class Evaluate_AfterSaleController : ControllerBase
    {
        private readonly IEvaluate_AfterSaleService _evaluateAfterSaleService;

        /// <summary>
        /// 初始化售后申请评估控制器
        /// </summary>
        /// <param name="evaluateAfterSaleService">售后申请评估服务</param>
        public Evaluate_AfterSaleController(IEvaluate_AfterSaleService evaluateAfterSaleService)
        {
            _evaluateAfterSaleService = evaluateAfterSaleService;
        }

        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        /// <returns>售后申请列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetAfterSaleApplicationsForAdmin()
        {
            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var applicationDtos = await _evaluateAfterSaleService.GetApplicationsForAdminAsync(adminId.Value);
            return applicationDtos == null ? NotFound() : Ok(applicationDtos);
        }

        /// <summary>
        /// 更新售后申请信息
        /// </summary>
        /// <param name="request">售后申请更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAfterSaleApplication([FromBody] SetAfterSaleApplicationInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var result = await _evaluateAfterSaleService.UpdateAfterSaleApplicationAsync(request);
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
