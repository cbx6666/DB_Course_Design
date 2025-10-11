using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 切换营业状态请求
    /// </summary>
    public class ToggleBusinessStatusRequestDto
    {
        [Required]
        /// <summary>
        /// 营业状态
        /// </summary>
        public bool IsOpen { get; set; }
    }
} 