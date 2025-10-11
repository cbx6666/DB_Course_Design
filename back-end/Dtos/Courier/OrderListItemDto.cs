// BackEnd/DTOs/Courier/OrderListItemDto.cs
namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 订单列表项
    /// </summary>
    public class OrderListItemDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 餐厅名称
        /// </summary>
        public string Restaurant { get; set; } = string.Empty;
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = string.Empty;
        /// <summary>
        /// 配送费
        /// </summary>
        public string Fee { get; set; } = string.Empty;
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;
        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusText { get; set; } = string.Empty;
        /// <summary>
        /// 是否准备取餐
        /// </summary>
        public bool IsReadyForPickup { get; set; }
    }
}