using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 切换骑手在线状态请求
    /// </summary>
    public class ToggleStatusRequestDto
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        [Required(ErrorMessage = "isOnline 字段是必需的。")]
        public required bool IsOnline { get; set; }
    }
}