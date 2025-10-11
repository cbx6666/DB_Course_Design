namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 可接订单
    /// </summary>
    public class AvailableOrderDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = null!;
        /// <summary>
        /// 餐厅
        /// </summary>
        public string Restaurant { get; set; } = null!;
        /// <summary>
        /// 取餐地址
        /// </summary>
        public string PickupAddress { get; set; } = null!;
        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress { get; set; } = null!;
        /// <summary>
        /// 客户
        /// </summary>
        public string Customer { get; set; } = null!;
        /// <summary>
        /// 配送费
        /// </summary>
        public string Fee { get; set; } = null!;
        /// <summary>
        /// 距离
        /// </summary>
        public string Distance { get; set; } = null!;
        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = null!;
    }
}