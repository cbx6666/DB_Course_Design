using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 菜品信息模型
    /// </summary>
    public class Dish
    {
        /// <summary>
        /// 菜品ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DishID { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string DishName { get; set; } = null!;

        /// <summary>
        /// 菜品价格
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品描述
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// 菜品售罄状态
        /// </summary>
        [Required]
        public DishIsSoldOut IsSoldOut { get; set; } = DishIsSoldOut.IsSoldOut;

        /// <summary>
        /// 菜品图片
        /// </summary>
        public string? DishImage { get; set; }

        /// <summary>
        /// 菜品种类ID（外键）
        /// </summary>
        [Required]
        public int CategoryID { get; set; }

        /// <summary>
        /// 关联的菜品种类
        /// </summary>
        [ForeignKey("CategoryID")]
        public DishCategory DishCategory { get; set; } = null!;

        /// <summary>
        /// 购物车项集合
        /// </summary>
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }

        /// <summary>
        /// 包含此菜品的菜单集合（计算属性，通过菜品种类获取）
        /// </summary>
        [NotMapped]
        public IEnumerable<Menu> Menus => DishCategory.MenuDishCategories.Select(mdc => mdc.Menu);
    }
}
