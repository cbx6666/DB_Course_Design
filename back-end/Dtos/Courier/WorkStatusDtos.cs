using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 配送员工作状态DTO（展示用）
    /// </summary>
    public class WorkStatusDto
    {
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool IsOnline { get; set; }

        /// <summary>
        /// 在线时长（小时）
        /// </summary>
        public int OnlineHours { get; set; }

        /// <summary>
        /// 在线时长（分钟）
        /// </summary>
        public int OnlineMinutes { get; set; }

        /// <summary>
        /// 今日接单数
        /// </summary>
        public int TodayOrders { get; set; }

        /// <summary>
        /// 累计完成订单数
        /// </summary>
        public int CompletedOrders { get; set; }

        /// <summary>
        /// 准时率
        /// </summary>
        public double PunctualityRate { get; set; }
    }

    /// <summary>
    /// 切换骑手在线状态请求DTO
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

