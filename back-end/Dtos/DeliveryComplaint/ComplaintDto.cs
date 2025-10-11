namespace BackEnd.DTOs.DeliveryComplaint
{
    /// <summary>
    /// 投诉信息（展示）
    /// </summary>
    public class GetComplaintInfo
    {
        /// <summary>
        /// 投诉ID
        /// </summary>
        public string ComplaintId { get; set; } = string.Empty;
        /// <summary>
        /// 投诉对象
        /// </summary>
        public string Target { get; set; } = string.Empty;
        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplicationTime { get; set; } = string.Empty;
        /// <summary>
        /// 投诉内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";
        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;
        /// <summary>
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
        /// <summary>
        /// 罚款
        /// </summary>
        public string Fine { get; set; } = string.Empty;
    }

    /// <summary>
    /// 投诉信息（提交）
    /// </summary>
    public class SetComplaintInfo
    {
        /// <summary>
        /// 投诉ID
        /// </summary>
        public string ComplaintId { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";
        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;
        /// <summary>
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
        /// <summary>
        /// 罚款
        /// </summary>
        public string Fine { get; set; } = string.Empty;
    }

    /// <summary>
    /// 投诉处理响应
    /// </summary>
    public class SetComplaintInfoResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 返回数据
        /// </summary>
        public GetComplaintInfo? Data { get; set; }
    }
}
