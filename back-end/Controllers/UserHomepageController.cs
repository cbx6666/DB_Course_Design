using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackEnd.Services.Interfaces;
using BackEnd.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户首页管理控制器
    /// </summary>
    [ApiController]
    [Route("api/user/home")]
    [Authorize]
    public class UserHomepageController : ControllerBase
    {
        private readonly IUserHomepageService _userHomepageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserHomepageController> _logger;

        public UserHomepageController(IUserHomepageService userHomepageService, IHttpContextAccessor httpContextAccessor, ILogger<UserHomepageController> logger)
        {
            _userHomepageService = userHomepageService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        /// <summary>
        /// 获取推荐商家
        /// </summary>
        /// <returns>推荐商家列表</returns>
        [HttpGet("recommend")]
        public async Task<IActionResult> GetRecommendedStores()
        {
            var result = await _userHomepageService.GetRecommendedStoresAsync();
            return result == null ? NotFound(new { code = 404, message = "There's No Recommend Store For User." }) : Ok(result);
        }

        /// <summary>
        /// 搜索商家和菜品
        /// </summary>
        /// <param name="searchDto">搜索条件</param>
        /// <returns>搜索结果</returns>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] HomeSearchDto searchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, message = "Invalid request" });
            }

            var (stores, dishes) = await _userHomepageService.SearchAsync(searchDto);

            if (stores == null && dishes == null)
            {
                return NotFound(new { code = 404, message = "There's No Search results." });
            }

            var searchStores = new List<object>();
            if (stores?.Any() == true) searchStores.AddRange(stores);
            if (dishes?.Any() == true) searchStores.AddRange(dishes);

            return Ok(new { searchStores });
        }

        /// <summary>
        /// 获取用户历史订单
        /// </summary>
        /// <returns>历史订单列表</returns>
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var orderHistory = await _userHomepageService.GetOrderHistoryAsync(userId.Value);
            return orderHistory == null ? NotFound(new { code = 404, message = "There's No OrderHistory For User." }) : Ok(orderHistory);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns>用户信息</returns>
        [HttpGet("userInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var userInfo = await _userHomepageService.GetUserInfoAsync(userId.Value);
            return userInfo == null ? NotFound(new { code = 404, message = "User not found" }) : Ok(userInfo);
        }

        /// <summary>
        /// 获取用户优惠券信息
        /// </summary>
        /// <returns>优惠券列表</returns>
        [HttpGet("couponInfo")]
        public async Task<IActionResult> GetUserCoupons()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return Unauthorized("无效的Token");
            }

            var userIdDto = new UserIdDto { UserId = userId.Value };
            var coupons = await _userHomepageService.GetUserCouponsAsync(userIdDto);

            return coupons?.Any() != true ? NotFound(new { code = 404, message = "There's No Coupon For User." }) : Ok(coupons);
        }

        /// <summary>
        /// 获取所有商家
        /// </summary>
        /// <returns>商家列表</returns>
        [HttpGet("stores")]
        public async Task<ActionResult<StoresResponseDto>> GetAllStores()
        {
            try
            {
                var stores = await _userHomepageService.GetAllStoresAsync();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商店信息时发生错误");
                return StatusCode(500, "获取商店信息时发生错误");
            }
        }

        /// <summary>
        /// 从Token中获取用户ID
        /// </summary>
        /// <returns>用户ID，如果无效则返回null</returns>
        private int? GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out int userId) ? userId : null;
        }
    }
}
