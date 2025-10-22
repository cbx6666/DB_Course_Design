using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 菜单模型
    /// </summary>
    public class Menu
    {
        /// <summary>
        /// 菜单ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuID { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 菜单描述
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// 是否为当前活跃菜单
        /// </summary>
        [Required]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// 创建时间
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 菜单菜品种类关联集合
        /// </summary>
        public ICollection<Menu_DishCategory> MenuDishCategories { get; set; } = new List<Menu_DishCategory>();

        /// <summary>
        /// 菜品种类集合（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<DishCategory> DishCategories => MenuDishCategories.Select(mdc => mdc.DishCategory);

        /// <summary>
        /// 菜品集合（计算属性，通过菜品种类获取）
        /// </summary>
        [NotMapped]
        public IEnumerable<Dish> Dishes => MenuDishCategories.SelectMany(mdc => mdc.DishCategory.Dishes);

        /// <summary>
        /// 菜品数量（计算属性）
        /// </summary>
        [NotMapped]
        public int DishCount => Dishes.Count();
    }
}
