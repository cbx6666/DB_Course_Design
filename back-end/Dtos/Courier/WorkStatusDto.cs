namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 配送员工作状态
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
}