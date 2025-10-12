using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 菜单菜品种类关联模型
    /// </summary>
    public class Menu_DishCategory
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
        /// 菜品种类ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int CategoryID { get; set; }

        /// <summary>
        /// 关联的菜品种类
        /// </summary>
        [ForeignKey("CategoryID")]
        public DishCategory DishCategory { get; set; } = null!;
    }
}
