namespace BackEnd.DTOs.DeliveryTask
{
    // ========== 商家侧 ==========

    /// <summary>
    /// 创建配送任务请求DTO（商家端发布配送任务）
    /// </summary>
    public class CreateDeliveryTaskDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 预计到达时间
        /// </summary>
        public string EstimatedArrivalTime { get; set; } = null!;

        /// <summary>
        /// 预计配送时间
        /// </summary>
        public string EstimatedDeliveryTime { get; set; } = null!;
    }

    /// <summary>
    /// 订单配送信息DTO（商家侧查看）
    /// 包含前端需要的所有配送相关信息
    /// </summary>
    public class OrderDeliveryInfoDto
    {
        /// <summary>
        /// 配送状态（前端显示使用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 配送员信息（前端显示使用）
        /// </summary>
        public BackEnd.DTOs.Courier.CourierSummaryDto? Courier { get; set; }
    }
}
