namespace BackEnd.DTOs.DishCategory
{
    /// <summary>
    /// 菜品种类数据传输对象
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
}
