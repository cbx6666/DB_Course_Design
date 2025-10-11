using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 菜单菜品关联模型
    /// </summary>
    public class Menu_Dish
    {
        /// <summary>
        /// 菜单ID（复合主键）
        /// </summary>
        [Key, Column(Order = 0)]
        public int MenuID { get; set; }

        /// <summary>
        /// 关联的菜单
        /// </summary>
        [ForeignKey("MenuID")]
        public Menu Menu { get; set; } = null!;

        /// <summary>
        /// 菜品ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int DishID { get; set; }

        /// <summary>
        /// 关联的菜品
        /// </summary>
        [ForeignKey("DishID")]
        public Dish Dish { get; set; } = null!;
    }
}
