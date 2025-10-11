namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 店铺详细信息数据传输对象
    /// </summary>
    public class ShopInfoResponseDto
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间（yyyy-MM-dd HH:mm:ss）
        /// </summary>
        public string CreateTime { get; set; } = string.Empty;

        /// <summary>
        /// 店铺地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 营业开始时间（HH:mm）
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 营业结束时间（HH:mm）
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// 店铺特色
        /// </summary>
        public string Feature { get; set; } = string.Empty;

        /// <summary>
        /// 信誉积分
        /// </summary>
        public int? CreditScore { get; set; }
    }
}