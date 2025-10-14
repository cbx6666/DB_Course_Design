using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.MerchantInfo
{
    /// <summary>
    /// 更新商家资料请求
    /// </summary>
    public class UpdateMerchantProfileDto
    {
        /// <summary>
        /// 用户名（可修改）
        /// </summary>
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Phone]
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = null!;
    }
}