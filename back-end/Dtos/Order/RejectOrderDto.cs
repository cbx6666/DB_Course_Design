namespace BackEnd.DTOs.Order
{
    /// <summary>
    /// 拒绝订单请求
    /// </summary>
    public class RejectOrderDto
    {
        /// <summary>
        /// 拒绝原因
        /// </summary>
        public string? Reason { get; set; }
    }
}