namespace BackEnd.DTOs.ViolationPenalty
{
    /// <summary>
    /// 违规处罚信息（用于展示）
    /// </summary>
    public class GetViolationPenaltyInfo
    {
        /// <summary>
        /// 处罚编号
        /// </summary>
        public string PunishmentId { get; set; } = null!;

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; } = null!;

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 商家方处理措施
        /// </summary>
        public string MerchantPunishment { get; set; } = null!;

        /// <summary>
        /// 平台方处理措施
        /// </summary>
        public string StorePunishment { get; set; } = null!;

        /// <summary>
        /// 处罚时间
        /// </summary>
        public string PunishmentTime { get; set; } = null!;

        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
    }

    /// <summary>
    /// 违规处罚信息（用于提交）
    /// </summary>
    public class SetViolationPenaltyInfo
    {
        /// <summary>
        /// 处罚编号
        /// </summary>
        public string PunishmentId { get; set; } = null!;

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 商家方处理措施
        /// </summary>
        public string MerchantPunishment { get; set; } = null!;

        /// <summary>
        /// 平台方处理措施
        /// </summary>
        public string StorePunishment { get; set; } = null!;

        /// <summary>
        /// 处罚时间
        /// </summary>
        public string PunishmentTime { get; set; } = null!;

        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
    }

    /// <summary>
    /// 违规处罚提交响应
    /// </summary>
    public class SetViolationPenaltyInfoResponse
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
        /// 数据（可选）
        /// </summary>
        public GetViolationPenaltyInfo? Data { get; set; }
    }
}
