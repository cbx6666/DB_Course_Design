namespace BackEnd.DTOs.Dish
{
    /// <summary>
    /// 菜品信息
    /// </summary>
    public class DishDto
    {
        /// <summary>
        /// 菜品ID
        /// </summary>
        public int DishId { get; set; }
        /// <summary>
        /// 菜品名称
        /// </summary>
        public string DishName { get; set; } = null!;
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 是否售罄
        /// </summary>
        public int IsSoldOut { get; set; }
    }

    /// <summary>
    /// 创建菜品请求
    /// </summary>
    public class CreateDishDto
    {
        /// <summary>
        /// 菜品名称
        /// </summary>
        public string DishName { get; set; } = null!;
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = null!;
        /// <summary>
        /// 是否售罄
        /// </summary>
        public int IsSoldOut { get; set; } = 0;
    }

    /// <summary>
    /// 更新菜品请求
    /// </summary>
    public class UpdateDishDto
    {
        /// <summary>
        /// 菜品名称
        /// </summary>
        public string? DishName { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 是否售罄
        /// </summary>
        public int? IsSoldOut { get; set; }
    }

    /// <summary>
    /// 切换售罄状态请求
    /// </summary>
    public class ToggleSoldOutDto
    {
        /// <summary>
        /// 是否售罄
        /// </summary>
        public int IsSoldOut { get; set; }
    }
}