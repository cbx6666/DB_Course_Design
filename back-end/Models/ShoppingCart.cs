using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 购物车模型
    /// </summary>
    public class ShoppingCart
    {
        /// <summary>
        /// 购物车ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        [Required]
        public DateTime LastUpdatedTime { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        /// <summary>
        /// 购物车状态
        /// </summary>
        public ShoppingCartState? ShoppingCartState { get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        public FoodOrder? Order { get; set; }

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        public int? StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store? Store { get; set; }

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
        /// 购物车项集合
        /// </summary>
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
