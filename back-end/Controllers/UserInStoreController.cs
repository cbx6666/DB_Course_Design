using BackEnd.DTOs.User;
using Microsoft.AspNetCore.Mvc;
using BackEnd.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户店铺控制器
    /// </summary>
    [ApiController]
    [Route("api")]
    [Authorize] // 添加认证要求
    public class UserInStoreController : ControllerBase
    {
        private readonly IUserInStoreService _userInStoreService;
        private readonly ILogger<UserInStoreController> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userInStoreService">用户店铺服务</param>
        /// <param name="logger">日志记录器</param>
        public UserInStoreController(IUserInStoreService userInStoreService, ILogger<UserInStoreController> logger)
        {
            _userInStoreService = userInStoreService;
            _logger = logger;
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="request">店铺请求</param>
        /// <returns>店铺信息</returns>
        [HttpGet("user/store/storeInfo")]
        public async Task<ActionResult<StoreResponseDto>> GetStoreInfo([FromQuery] StoreRequestDto request)
        {
            if (request.StoreId <= 0)
                return BadRequest(new
                {
                    code = 400,
                    message = "店铺编号无效",
                });

            var result = await _userInStoreService.GetStoreInfoAsync(request);

            if (result == null) return NotFound("店铺不存在");

            return Ok(result);
        }

        /// <summary>
        /// 获取菜单（平铺菜品）
        /// </summary>
        /// <param name="request">菜单请求</param>
        /// <returns>菜单列表</returns>
        [HttpGet("store/dish")]
        public async Task<ActionResult<List<MenuResponseDto>>> GetMenu([FromQuery] MenuRequestDto request)
        {
            if (request.StoreId <= 0)
                return BadRequest("参数无效");

            var result = await _userInStoreService.GetMenuAsync(request);

            if (result == null) return NotFound("当前无菜品");

            return Ok(result);
        }

        /// <summary>
        /// 获取商家评论列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论列表</returns>
        [HttpGet("user/store/commentList")]
        public async Task<ActionResult> GetCommentList([FromQuery] int storeId)
        {
            if (storeId <= 0)
                return BadRequest("店铺编号无效");

            var result = await _userInStoreService.GetCommentListAsync(storeId);

            return Ok(new { comments = result });
        }

        /// <summary>
        /// 获取商家评价状态 [好评数, 中评数, 差评数]
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评价状态</returns>
        [HttpGet("user/store/commentStatus")]
        public async Task<ActionResult<CommentStateDto>> GetCommentState([FromQuery] int storeId)
        {
            if (storeId <= 0)
                return BadRequest("店铺编号无效");

            var result = await _userInStoreService.GetCommentStateAsync(storeId);

            return Ok(result);
        }

        /// <summary>
        /// 用户评价店铺
        /// </summary>
        /// <param name="dto">创建评论请求</param>
        /// <returns>提交结果</returns>
        [HttpPost("user/comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 从 Token 中安全地获取用户 ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("无效的Token");
                }

                // 设置用户ID到DTO中
                dto.UserId = userId;

                await _userInStoreService.SubmitCommentAsync(dto);
                return Ok(new { message = "评论已提交，等待审核" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户提交评论时发生错误 (StoreId={StoreId})", dto.StoreId);
                return StatusCode(500, new { message = "提交评论时发生错误" });
            }
        }

        /// <summary>
        /// 用户投诉店铺
        /// </summary>
        /// <param name="dto">用户店铺举报请求</param>
        /// <returns>提交结果</returns>
        [HttpPost("user/store/report")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportStore([FromBody] UserStoreReportDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // 从 Token 中安全地获取用户 ID
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdString, out int userId))
                {
                    return Unauthorized("无效的Token");
                }

                // 设置用户ID到DTO中
                dto.UserId = userId;

                await _userInStoreService.SubmitStoreReportAsync(dto);
                return Ok(new { message = "投诉已提交，等待管理员审核" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "用户投诉店铺时发生错误 (StoreId={StoreId})", dto.StoreId);
                return StatusCode(500, new { message = "提交投诉时发生错误" });
            }
        }
    }
}
