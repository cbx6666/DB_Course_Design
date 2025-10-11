using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 订单信息模型
    /// </summary>
    public class FoodOrder
    {
        /// <summary>
        /// 订单ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        /// <summary>
        /// 订单时间
        /// </summary>
        [Required]
        public DateTime OrderTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PaymentTime { get; set; }

        /// <summary>
        /// 订单备注
        /// </summary>
        [StringLength(255)]
        public string? Remarks { get; set; }

        /// <summary>
        /// 配送费用
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DeliveryFee { get; set; } = 0.00m;

        /// <summary>
        /// 订单状态
        /// </summary>
        [Required]
        public FoodOrderState FoodOrderState { get; set; } = FoodOrderState.Pending;

        /// <summary>
        /// 配送任务
        /// </summary>
        public DeliveryTask? DeliveryTask { get; set; }

        /// <summary>
        /// 客户ID（外键）
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的客户信息
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 购物车ID（外键）
        /// </summary>
        public int? CartID { get; set; }

        /// <summary>
        /// 关联的购物车信息
        /// </summary>
        [ForeignKey("CartID")]
        public ShoppingCart? Cart { get; set; } = null!;

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺信息
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 优惠券集合
        /// </summary>
        public ICollection<Coupon>? Coupons { get; set; }

        /// <summary>
        /// 售后申请集合
        /// </summary>
        public ICollection<AfterSaleApplication>? AfterSaleApplications { get; set; }

        /// <summary>
        /// 评论集合
        /// </summary>
        public ICollection<Comment>? Comments { get; set; }
    }
}
