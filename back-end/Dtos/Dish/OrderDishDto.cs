using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Dish
{
    /// <summary>
    /// 订单菜品详情数据传输对象（可复用）
    /// </summary>
    public class OrderDishDto
    {
        /// <summary>
        /// 菜品名称
        /// </summary>
        [Required]
        public string DishName { get; set; } = string.Empty;

        /// <summary>
        /// 菜品图片
        /// </summary>
        [Required]
        public string DishImage { get; set; } = string.Empty;

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public int Quantity { get; set; }
    }
}
