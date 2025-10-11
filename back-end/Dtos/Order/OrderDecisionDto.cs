namespace BackEnd.DTOs.Order
{
    /// <summary>
    /// 订单决策
    /// </summary>
    public class OrderDecisionDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 决策结果（"accepted" 或 "rejected"）
        /// </summary>
        public string Decision { get; set; } = null!;
        /// <summary>
        /// 决策时间
        /// </summary>
        public string DecidedAt { get; set; } = null!;
        /// <summary>
        /// 决策原因
        /// </summary>
        public string? Reason { get; set; }
    }
}