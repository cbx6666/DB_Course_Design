namespace BackEnd.DTOs.DishCategory
{
    /// <summary>
    /// 菜品种类列表响应数据传输对象
    /// </summary>
    public class DishCategoryListResponseDto
    {
        /// <summary>
        /// 菜品种类列表
        /// </summary>
        public List<DishCategoryDto> List { get; set; } = new List<DishCategoryDto>();

        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }
    }
}
