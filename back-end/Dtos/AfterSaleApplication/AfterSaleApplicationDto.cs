namespace BackEnd.DTOs.AfterSaleApplication
{
    /// <summary>
    /// 售后申请信息（展示）
    /// </summary>
    public class GetAfterSaleApplicationInfo
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;
        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; } = string.Empty;
        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplicationTime { get; set; } = string.Empty;
        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
        /// <summary>
        /// 处理状态
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
    }

    /// <summary>
    /// 售后申请信息（提交）
    /// </summary>
    public class SetAfterSaleApplicationInfo
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;
        /// <summary>
        /// 处理状态
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
    }

    /// <summary>
    /// 售后申请处理响应
    /// </summary>
    public class SetAfterSaleApplicationResponse
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
        public GetAfterSaleApplicationInfo? Data { get; set; }
    }
}
