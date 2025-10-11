namespace BackEnd.DTOs.AfterSale
{
    /// <summary>
    /// 售后申请（展示）
    /// </summary>
    public class AfterSaleApplicationDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; } = null!;
        /// <summary>
        /// 用户信息
        /// </summary>
        public AUserInfoDto User { get; set; } = null!;
        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; } = null!;
        /// <summary>
        /// 申请时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;
    }
}