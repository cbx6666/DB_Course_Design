using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.DishCategory
{
    /// <summary>
    /// 创建菜品种类数据传输对象
    /// </summary>
    public class CreateDishCategoryDto
    {
        /// <summary>
        /// 菜品种类名称
        /// </summary>
        [Required(ErrorMessage = "菜品种类名称不能为空")]
        [StringLength(50, ErrorMessage = "菜品种类名称长度不能超过50个字符")]
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// 菜单ID（用于关联到特定菜单）
        /// </summary>
        [Required(ErrorMessage = "菜单ID不能为空")]
        public int MenuId { get; set; }
    }
}
