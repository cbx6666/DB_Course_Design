using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 菜品种类模型
    /// </summary>
    public class DishCategory
    {
        /// <summary>
        /// 菜品种类ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryID { get; set; }

        /// <summary>
        /// 菜品种类名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// 菜品集合（一对多关系）
        /// </summary>
        public ICollection<Dish> Dishes { get; set; } = new List<Dish>();

        /// <summary>
        /// 菜单菜品种类关联集合（多对多关系）
        /// </summary>
        public ICollection<Menu_DishCategory> MenuDishCategories { get; set; } = new List<Menu_DishCategory>();

        /// <summary>
        /// 菜单集合（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<Menu> Menus => MenuDishCategories.Select(mdc => mdc.Menu);
    }
}
