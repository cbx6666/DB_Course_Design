using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 优惠券管理模型
    /// </summary>
    public class CouponManager
    {
        /// <summary>
        /// 优惠券管理ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponManagerID { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CouponName { get; set; } = string.Empty;

        /// <summary>
        /// 优惠券类型
        /// </summary>
        [Required]
        public CouponType CouponType { get; set; }

        /// <summary>
        /// 最低消费金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        /// <summary>
        /// 优惠值（满减券为优惠金额，折扣券为折扣比例0-1之间的小数）
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [Required]
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 已使用数量
        /// </summary>
        public int UsedQuantity { get; set; } = 0;

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        [Required]
        public DateTime ValidFrom { get; set; }

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        [Required]
        public DateTime ValidTo { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 优惠券集合
        /// </summary>
        public ICollection<Coupon>? Coupons { get; set; }

        /// <summary>
        /// 优惠券状态（计算属性）
        /// </summary>
        [NotMapped]
        public string Status
        {
            get
            {
                var now = DateTime.Now;
                if (now < ValidFrom) return "upcoming";
                if (now > ValidTo) return "expired";
                return "active";
            }
        }

        /// <summary>
        /// 是否已过期（计算属性）
        /// </summary>
        [NotMapped]
        public bool IsExpired => DateTime.Now > ValidTo;

        /// <summary>
        /// 是否未开始（计算属性）
        /// </summary>
        [NotMapped]
        public bool IsUpcoming => DateTime.Now < ValidFrom;
    }
}
