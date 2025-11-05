using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 商家资料信息（运营端“账户资料”展示）
    /// </summary>
    public class MerchantProfileDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string RegisterTime { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? Avatar { get; set; }
    }

    /// <summary>
    /// 更新商家资料请求（名称/电话/邮箱）
    /// </summary>
    public class UpdateMerchantProfileDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        [Phone]
        public string Phone { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    /// <summary>
    /// 更新商家头像请求
    /// </summary>
    public class UpdateMerchantAvatarDto
    {
        [Required]
        public IFormFile AvatarFile { get; set; } = null!;
    }

    /// <summary>
    /// 商家更新结果（变更字段与时间）
    /// </summary>
    public class MerchantUpdateResultDto
    {
        public string[] UpdatedFields { get; set; } = Array.Empty<string>();
        public string UpdateTime { get; set; } = null!;
    }
}
