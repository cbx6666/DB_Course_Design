using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 购物车项模型
    /// </summary>
    public class ShoppingCartItem
    {
        /// <summary>
        /// 购物车项ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        /// <summary>
        /// 菜品ID（外键）
        /// </summary>
        [Required]
        public int DishID { get; set; }

        /// <summary>
        /// 关联的菜品
        /// </summary>
        [ForeignKey("DishID")]
        public Dish Dish { get; set; } = null!;

        /// <summary>
        /// 购物车ID（外键）
        /// </summary>
        [Required]
        public int CartID { get; set; }

        /// <summary>
        /// 关联的购物车
        /// </summary>
        [ForeignKey("CartID")]
        public ShoppingCart Cart { get; set; } = null!;
    }
}
