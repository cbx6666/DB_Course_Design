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
        /// 购物车项集合
        /// </summary>
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }

        /// <summary>
        /// 菜单菜品关联集合
        /// </summary>
        public ICollection<Menu_Dish> MenuDishes { get; set; } = new List<Menu_Dish>();

        /// <summary>
        /// 包含此菜品的菜单集合（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<Menu> Menus => MenuDishes.Select(md => md.Menu);
    }
}
