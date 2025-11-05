using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.DishCategory
{
    /// <summary>
    /// 菜品种类响应（用户端简单展示）
    /// </summary>
    public class CategoryResponseDto
    {
        /// <summary>
        /// 菜品种类ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 菜品种类名称
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// 菜品种类数据传输对象（商家端管理）
    /// </summary>
    public class DishCategoryDto
    {
        /// <summary>
        /// 菜品种类ID
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// 菜品种类名称
        /// </summary>
        public string CategoryName { get; set; } = null!;

        /// <summary>
        /// 菜品数量
        /// </summary>
        public int DishCount { get; set; }
    }

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
