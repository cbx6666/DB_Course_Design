using System;

namespace BackEnd.DTOs.Penalty
{
    /// <summary>
    /// 处罚记录
    /// </summary>
    public class PenaltyRecordDto
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;
        /// <summary>
        /// 处罚时间
        /// </summary>
        public string Time { get; set; } = null!;
        /// <summary>
        /// 商家处理措施
        /// </summary>
        public string? MerchantAction { get; set; }
        /// <summary>
        /// 平台处理措施
        /// </summary>
        public string? PlatformAction { get; set; }
    }
}