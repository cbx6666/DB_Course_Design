namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 新订单详情
    /// </summary>
    public class NewOrderDetailsDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 餐厅名称
        /// </summary>
        public string RestaurantName { get; set; } = string.Empty;
        /// <summary>
        /// 餐厅地址
        /// </summary>
        public string RestaurantAddress { get; set; } = string.Empty;
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; } = string.Empty;
        /// <summary>
        /// 客户地址
        /// </summary>
        public string CustomerAddress { get; set; } = string.Empty;
        /// <summary>
        /// 距离
        /// </summary>
        public string Distance { get; set; } = string.Empty;
        /// <summary>
        /// 配送费
        /// </summary>
        public decimal Fee { get; set; }
        /// <summary>
        /// 地图图片URL
        /// </summary>
        public string MapImageUrl { get; set; } = string.Empty;
    }
}