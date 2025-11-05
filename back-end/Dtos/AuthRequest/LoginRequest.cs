using BackEnd.DTOs.User;
using BackEnd.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.AuthRequest
{
    /// <summary>
    /// 登录请求
    /// </summary>
    public class LoginRequest
    {
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "请输入正确的手机号码")]
        /// <summary>
        /// 手机号
        /// </summary>
        public required string PhoneNum { get; set; }

        [Required(ErrorMessage = "密码不能为空")]
        /// <summary>
        /// 密码
        /// </summary>
        public required string Password { get; set; }

        [Required(ErrorMessage = "角色不能为空")]
        /// <summary>
        /// 登录角色
        /// </summary>
        public required string Role { get; set; }
    }

    /// <summary>
    /// 登录结果（继承自统一响应类，添加登录特定的数据字段）
    /// </summary>
    public class LoginResult : ApiResponseDto
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInfo? User { get; set; }
    }
}
