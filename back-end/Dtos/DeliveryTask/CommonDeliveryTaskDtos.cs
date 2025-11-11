namespace BackEnd.DTOs.DeliveryTask
{
    // ========== 共享 DTO（消费者和商家共用） ==========

    /// <summary>
    /// 订单配送信息DTO（消费者和商家共用）
    /// 包含前端需要的所有配送相关信息
    /// </summary>
    public class OrderDeliveryInfoDto
    {
        /// <summary>
        /// 配送任务ID
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// 配送状态（前端显示使用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 配送员信息（前端显示使用）
        /// </summary>
        public BackEnd.DTOs.Courier.CourierSummaryDto? Courier { get; set; }

        /// <summary>
        /// 接单时间
        /// </summary>
        public string? AcceptTime { get; set; }

        /// <summary>
        /// 预计到店时间
        /// </summary>
        public string? EstimatedArrivalTime { get; set; }

        /// <summary>
        /// 实际到店时间（骑手取餐时间）
        /// </summary>
        public string? ActualPickupTime { get; set; }

        /// <summary>
        /// 预计送达时间
        /// </summary>
        public string? EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// 实际送达时间（任务完成时间）
        /// </summary>
        public string? ActualDeliveryTime { get; set; }

        /// <summary>
        /// 配送任务评分（1-5，可选）- 消费者对该次配送任务的评分
        /// </summary>
        public int? TaskRating { get; set; }

        /// <summary>
        /// 订单配送信息（收货人信息）
        /// </summary>
        public OrderDeliveryDetailDto? Order { get; set; }
    }

    /// <summary>
    /// 订单配送详情DTO（包含收货信息）
    /// </summary>
    public class OrderDeliveryDetailDto
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string? DeliveryName { get; set; }

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string? DeliveryPhone { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string? DeliveryAddress { get; set; }
    }
}
