using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.DTOs.Coupon
{
    /// <summary>
    /// 用户侧优惠券数据传输对象（用户已领取/拥有的优惠券，用于"我的优惠券"等场景）
    /// - 面向用户端展示
    /// - 由 UserHomepageService 等返回
    /// </summary>
    public class CouponDto
    {
        [Key]
        public int CouponID { get; set; }

        public CouponState CouponState { get; set; } = CouponState.Unused;

        public int? OrderID { get; set; }

        [Required]
        public int CouponManagerID { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }

        [Required]
        public DateTime ValidTo { get; set; }
    }

    /// <summary>
    /// 客户优惠券数据传输对象（用于结账页面选择优惠券）
    /// </summary>
    public class CustomerCouponDto
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        [Key]
        public int CouponID { get; set; }

        /// <summary>
        /// 优惠券状态
        /// </summary>
        public CouponState CouponState { get; set; } = CouponState.Unused;

        /// <summary>
        /// 优惠券管理ID
        /// </summary>
        [Required]
        public int CouponManagerID { get; set; }

        /// <summary>
        /// 最低消费金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        /// <summary>
        /// 优惠值
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }

        /// <summary>
        /// 优惠券类型：'fixed' | 'discount'
        /// </summary>
        public string CouponType { get; set; } = string.Empty;

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        [Required]
        public string ValidFrom { get; set; } = string.Empty;

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        [Required]
        public string ValidTo { get; set; } = string.Empty;

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string? CouponName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public int? StoreID { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string? StoreName { get; set; }

        /// <summary>
        /// 店铺图片
        /// </summary>
        public string? StoreImage { get; set; }
    }
}
