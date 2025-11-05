using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 评论管理控制器（管理员侧）
    /// </summary>
    [ApiController]
    [Route("api/admin/comments")]
    [Authorize]
    public class AdminCommentController : BaseController
    {
        private readonly IAdminCommentService _adminCommentService;

        /// <summary>
        /// 初始化评论管理控制器
        /// </summary>
        /// <param name="adminCommentService">评论服务（管理员侧）</param>
        public AdminCommentController(IAdminCommentService adminCommentService)
        {
            _adminCommentService = adminCommentService;
        }

        /// <summary>
        /// 获取管理员的评论审核列表
        /// </summary>
        /// <returns>评论审核列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetReviewCommentsForAdmin()
        {
            var adminId = GetUserIdFromToken();
            if (adminId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var commentDtos = await _adminCommentService.GetCommentsForAdminAsync(adminId.Value);
            return commentDtos == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "未找到评论列表" }) : Ok(commentDtos);
        }

        /// <summary>
        /// 更新评论审核信息
        /// </summary>
        /// <param name="request">评论审核更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReviewComment([FromBody] UpdateCommentReviewDto request)
        {
            if (request == null)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求数据不能为空" });
            }

            var result = await _adminCommentService.UpdateCommentAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
