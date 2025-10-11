using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.MerchantInfo
{
    /// <summary>
    /// 更新商家资料请求
    /// </summary>
    public class UpdateMerchantProfileDto
    {
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