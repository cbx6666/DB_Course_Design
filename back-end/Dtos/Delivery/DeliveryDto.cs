namespace BackEnd.DTOs.Delivery
{
    /// <summary>
    /// 配送任务
    /// </summary>
    public class DeliveryTaskDto
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int TaskId { get; set; }
        /// <summary>
        /// 预计到达时间
        /// </summary>
        public string EstimatedArrivalTime { get; set; } = null!;
        /// <summary>
        /// 预计配送时间
        /// </summary>
        public string EstimatedDeliveryTime { get; set; } = null!;
        /// <summary>
        /// 客户ID
        /// </summary>
        public int CustomerId { get; set; }
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 发布任务
    /// </summary>
    public class PublishTaskDto
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public int SellerId { get; set; }
        /// <summary>
        /// 配送任务ID
        /// </summary>
        public int DeliveryTaskId { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public string PublishTime { get; set; } = null!;
    }

    /// <summary>
    /// 发布配送任务
    /// </summary>
    public class PublishDeliveryTaskDto
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
    /// 接受任务
    /// </summary>
    public class AcceptTaskDto
    {
        /// <summary>
        /// 配送员ID
        /// </summary>
        public int? CourierId { get; set; }
        /// <summary>
        /// 配送任务ID
        /// </summary>
        public int DeliveryTaskId { get; set; }
        /// <summary>
        /// 接受时间
        /// </summary>
        public string AcceptTime { get; set; } = null!;
    }

    /// <summary>
    /// 配送员摘要信息
    /// </summary>
    public class CourierSummaryDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 配送员注册时间
        /// </summary>
        public string? CourierRegistrationTime { get; set; }
        /// <summary>
        /// 交通工具类型
        /// </summary>
        public string VehicleType { get; set; } = null!;
        /// <summary>
        /// 信誉积分
        /// </summary>
        public decimal ReputationPoints { get; set; }
        /// <summary>
        /// 总配送次数
        /// </summary>
        public int TotalDeliveries { get; set; }
        /// <summary>
        /// 平均配送时间
        /// </summary>
        public int AvgDeliveryTime { get; set; }
        /// <summary>
        /// 平均评分
        /// </summary>
        public decimal AverageRating { get; set; }
        /// <summary>
        /// 月薪
        /// </summary>
        public decimal MonthlySalary { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        public string? FullName { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public long? PhoneNumber { get; set; }
    }

    /// <summary>
    /// 订单配送信息
    /// </summary>
    public class OrderDeliveryInfoDto
    {
        /// <summary>
        /// 配送任务
        /// </summary>
        public DeliveryTaskDto? DeliveryTask { get; set; }
        /// <summary>
        /// 发布信息
        /// </summary>
        public PublishTaskDto? Publish { get; set; }
        /// <summary>
        /// 接受信息
        /// </summary>
        public AcceptTaskDto? Accept { get; set; }
        /// <summary>
        /// 配送员信息
        /// </summary>
        public CourierSummaryDto? Courier { get; set; }
    }
}