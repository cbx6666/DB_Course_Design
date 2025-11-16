using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 评论管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/comments")]
    [Authorize]
    public class MerchantCommentController : BaseController
    {
        private readonly IMerchantCommentService _merchantCommentService;

        /// <summary>
        /// 初始化评论管理控制器
        /// </summary>
        /// <param name="merchantCommentService">评论服务（商家侧）</param>
        public MerchantCommentController(IMerchantCommentService merchantCommentService)
        {
            _merchantCommentService = merchantCommentService;
        }

        /// <summary>
        /// 获取评价列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="field">筛选字段（content | orderNo | user.name）</param>
        /// <returns>评价列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? keyword, [FromQuery] string? field)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "页码和每页数量必须大于0" });
            }

            var sellerId = GetUserIdFromToken();
            if (sellerId == null)
            {
                return Unauthorized(new ApiResponseDto { Success = false, Code = 401, Message = "无效的Token" });
            }

            var result = await _merchantCommentService.GetReviewsAsync(sellerId.Value, page, pageSize, keyword, field);
            return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = result });
        }

        /// <summary>
        /// 回复评价
        /// </summary>
        /// <param name="id">评价ID</param>
        /// <param name="replyDto">回复请求</param>
        /// <returns>回复结果</returns>
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> ReplyToReview(int id, [FromBody] ReplyCommentDto replyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数错误" });
            }

            var result = await _merchantCommentService.ReplyToReviewAsync(id, replyDto);
            return Ok(result);
        }

    }
}
