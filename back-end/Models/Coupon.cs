using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 优惠券模型
    /// </summary>
    public class Coupon
    {
        /// <summary>
        /// 优惠券ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CouponID { get; set; }

        /// <summary>
        /// 优惠券状态
        /// </summary>
        public CouponState CouponState { get; set; } = CouponState.Unused;

        /// <summary>
        /// 优惠券管理ID（外键）
        /// </summary>
        [Required]
        public int CouponManagerID { get; set; }

        /// <summary>
        /// 关联的优惠券管理
        /// </summary>
        [ForeignKey("CouponManagerID")]
        public CouponManager CouponManager { get; set; } = null!;

        /// <summary>
        /// 消费者ID（外键）
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的消费者
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 订单ID（外键）
        /// </summary>
        public int? OrderID { get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("OrderID")]
        public FoodOrder? Order { get; set; }

        /// <summary>
        /// 是否已过期（计算属性）
        /// </summary>
        [NotMapped]
        public bool IsExpired => CouponManager.ValidTo < DateTime.Now;
    }
}
