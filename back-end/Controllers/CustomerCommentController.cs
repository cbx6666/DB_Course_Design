using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 评论管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/comments")]
    public class CustomerCommentController : BaseController
    {
        private readonly ICustomerCommentService _commentService;

        public CustomerCommentController(ICustomerCommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// 获取商家评论列表
        /// </summary>
        [HttpGet("store/{storeId}/comments")]
        public async Task<ActionResult> GetCommentList([FromRoute] int storeId)
        {
            if (storeId <= 0)
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "店铺编号无效" });

            var result = await _commentService.GetCommentListAsync(storeId);
            return Ok(new ApiResponseDto<object> { Success = true, Code = 200, Message = "获取成功", Data = new { comments = result } });
        }

        /// <summary>
        /// 获取商家评价状态 [好评数, 中评数, 差评数]
        /// </summary>
        [HttpGet("store/{storeId}/commentStatus")]
        public async Task<ActionResult<CommentStateDto>> GetCommentState([FromRoute] int storeId)
        {
            if (storeId <= 0)
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "店铺编号无效" });

            var result = await _commentService.GetCommentStateAsync(storeId);
            return Ok(result);
        }

        /// <summary>
        /// 用户评价店铺
        /// </summary>
        [HttpPost("comment")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("无效的Token");
                }

                dto.UserId = userId.Value;

                await _commentService.SubmitCommentAsync(dto);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "评论已提交，等待审核" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "提交评论时发生错误" });
            }
        }
    }
}
