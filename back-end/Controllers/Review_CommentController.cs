using BackEnd.DTOs.Comment;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 评论审核管理控制器
    /// </summary>
    [ApiController]
    [Route("api/admin/review-comments")]
    [Authorize]
    public class Review_CommentController : ControllerBase
    {
        private readonly IReview_CommentService _reviewCommentService;

        /// <summary>
        /// 初始化评论审核管理控制器
        /// </summary>
        /// <param name="reviewCommentService">评论审核服务</param>
        public Review_CommentController(IReview_CommentService reviewCommentService)
        {
            _reviewCommentService = reviewCommentService;
        }

        /// <summary>
        /// 获取管理员的评论审核列表
        /// </summary>
        /// <returns>评论审核列表</returns>
        [HttpGet("mine")]
        public async Task<IActionResult> GetReviewCommentsForAdmin()
        {
            var adminId = GetAdminIdFromToken();
            if (adminId == null)
            {
                return Unauthorized("无效的Token");
            }

            var commentDtos = await _reviewCommentService.GetCommentsForAdminAsync(adminId.Value);
            return commentDtos == null ? NotFound() : Ok(commentDtos);
        }

        /// <summary>
        /// 更新评论审核信息
        /// </summary>
        /// <param name="request">评论审核更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("update")]
        public async Task<IActionResult> UpdateReviewComment([FromBody] SetCommentInfo request)
        {
            if (request == null)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "请求数据不能为空"
                });
            }

            var result = await _reviewCommentService.UpdateCommentAsync(request);
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

