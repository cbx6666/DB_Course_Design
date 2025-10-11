namespace BackEnd.DTOs.Coupon
{
    /// <summary>
    /// 订单关联优惠券信息
    /// </summary>
    public class OrderCouponInfoDto
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public int CouponId { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string? CouponName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 优惠类型（percentage/fixed）
        /// </summary>
        public string DiscountType { get; set; } = null!;

        /// <summary>
        /// 优惠值
        /// </summary>
        public decimal DiscountValue { get; set; }

        /// <summary>
        /// 生效时间（ISO）
        /// </summary>
        public string ValidFrom { get; set; } = null!;

        /// <summary>
        /// 失效时间（ISO）
        /// </summary>
        public string ValidTo { get; set; } = null!;

        /// <summary>
        /// 是否已使用
        /// </summary>
        public bool IsUsed { get; set; }
    }
}