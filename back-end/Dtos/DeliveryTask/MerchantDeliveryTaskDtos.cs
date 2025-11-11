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
}
