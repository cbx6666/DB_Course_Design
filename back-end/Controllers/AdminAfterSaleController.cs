using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 售后申请管理控制器（管理员侧）
    /// </summary>
    [ApiController]
    [Route("api/admin/after-sales")]
    [Authorize]
    public class AdminAfterSaleController : BaseController
    {
        private readonly IAdminAfterSaleService _adminAfterSaleService;

        /// <summary>
        /// 初始化售后申请管理控制器
        /// </summary>
        /// <param name="adminAfterSaleService">售后申请服务（管理员侧）</param>
        public AdminAfterSaleController(IAdminAfterSaleService adminAfterSaleService)
        {
            _adminAfterSaleService = adminAfterSaleService;
        }

        /// <summary>
        /// 获取管理员的售后申请列表
        /// </summary>
        /// <returns>售后申请列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetAfterSaleApplicationsForAdmin()
        {
            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var applicationDtos = await _adminAfterSaleService.GetApplicationsForAdminAsync(adminId.Value);
            return applicationDtos == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "未找到售后申请列表" }) : Ok(applicationDtos);
        }

        /// <summary>
        /// 更新售后申请信息
        /// </summary>
        /// <param name="request">售后申请更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAfterSaleApplication([FromBody] UpdateAfterSaleApplicationDto request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var result = await _adminAfterSaleService.UpdateAfterSaleApplicationAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
