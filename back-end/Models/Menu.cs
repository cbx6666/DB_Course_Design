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
        /// 菜单版本
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Version { get; set; } = null!;

        /// <summary>
        /// 活跃期间
        /// </summary>
        [Required]
        public DateTime ActivePeriod { get; set; }

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
        /// 菜单菜品关联集合
        /// </summary>
        public ICollection<Menu_Dish> MenuDishes { get; set; } = new List<Menu_Dish>();

        /// <summary>
        /// 菜品集合（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<Dish> Dishes => MenuDishes.Select(md => md.Dish);
    }
}
