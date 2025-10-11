namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 投诉中的处罚信息
    /// </summary>
    public class PunishmentDto
    {
        /// <summary>
        /// 处罚类型
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// 处罚描述
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 持续时间（可选）
        /// </summary>
        public string? Duration { get; set; }
    }

    /// <summary>
    /// 配送投诉信息
    /// </summary>
    public class ComplaintDto
    {
        /// <summary>
        /// 投诉编号
        /// </summary>
        public string ComplaintID { get; set; } = null!;

        /// <summary>
        /// 配送任务编号
        /// </summary>
        public string DeliveryTaskID { get; set; } = null!;

        /// <summary>
        /// 投诉时间
        /// </summary>
        public string ComplaintTime { get; set; } = null!;

        /// <summary>
        /// 投诉原因
        /// </summary>
        public string ComplaintReason { get; set; } = null!;

        /// <summary>
        /// 处罚信息（可选）
        /// </summary>
        public PunishmentDto? Punishment { get; set; }
    }
}