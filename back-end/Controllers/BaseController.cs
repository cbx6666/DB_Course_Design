using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 控制器基类，提供通用的辅助方法
    /// </summary>
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// 从Token中获取用户ID（通用方法，适用于用户、商家、管理员、骑手等所有角色）
        /// </summary>
        /// <returns>用户ID，如果无效则返回null</returns>
        protected int? GetUserIdFromToken()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out int userId) ? userId : null;
        }

        /// <summary>
        /// 获取当前用户ID（从Token中），如果无效则抛出异常
        /// </summary>
        /// <returns>用户ID</returns>
        /// <exception cref="UnauthorizedAccessException">如果无法从认证信息中解析有效的用户ID</exception>
        protected int GetCurrentUserId()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("无法从认证信息中解析有效的用户ID。");
        }

    }
}
