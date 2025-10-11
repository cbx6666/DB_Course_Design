namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 店铺概况数据传输对象
    /// </summary>
    public class ShopOverviewResponseDto
    {
        /// <summary>
        /// 店铺评分
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// 月销量
        /// </summary>
        public int MonthlySales { get; set; }

        /// <summary>
        /// 是否营业中
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// 信誉积分
        /// </summary>
        public int CreditScore { get; set; }
    }
}