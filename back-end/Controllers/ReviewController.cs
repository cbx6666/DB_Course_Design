using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using BackEnd.DTOs.Review;
using BackEnd.Services.Interfaces;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 评论管理控制器
    /// </summary>
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        /// <summary>
        /// 初始化评论管理控制器
        /// </summary>
        /// <param name="reviewService">评论服务</param>
        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>评论列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetReviews([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? keyword)
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

            var result = await _reviewService.GetReviewsAsync(sellerId.Value, page, pageSize, keyword);
            return Ok(result);
        }

        /// <summary>
        /// 回复评论
        /// </summary>
        /// <param name="id">评论ID</param>
        /// <param name="replyDto">回复请求</param>
        /// <returns>回复结果</returns>
        [HttpPost("{id}/reply")]
        public async Task<IActionResult> ReplyToReview(int id, [FromBody] ReplyDto replyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, message = "请求参数错误" });
            }

            var result = await _reviewService.ReplyToReviewAsync(id, replyDto);
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