using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Customer
{
    /// <summary>
    /// 用户个人资料
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 性别（可选）
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 全名（可选）
        /// </summary>
        public string? FullName { get; set; }
    }

    /// <summary>
    /// 更新账户信息数据传输对象
    /// </summary>
    public class UpdateAccountDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 头像文件
        /// </summary>
        public IFormFile? AvatarFile { get; set; }
    }
}
