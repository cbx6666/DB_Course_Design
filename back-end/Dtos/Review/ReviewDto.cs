namespace BackEnd.DTOs.Review
{
    /// <summary>
    /// 评价信息
    /// </summary>
    public class ReviewDto
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; } = null!;
        /// <summary>
        /// 用户信息
        /// </summary>
        public RUserInfoDto User { get; set; } = null!;
        /// <summary>
        /// 评价内容
        /// </summary>
        public string Content { get; set; } = null!;
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;
    }
}