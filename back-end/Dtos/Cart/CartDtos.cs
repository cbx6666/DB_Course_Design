using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.DTOs.Cart
{
    /// <summary>
    /// 购物车请求
    /// </summary>
    public class CartRequestDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 购物车响应
    /// </summary>
    public class CartResponseDto
    {
        [Required]
        public int CartId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        [Required]
        public List<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
    }

    /// <summary>
    /// 更新购物车项目
    /// </summary>
    public class UpdateCartItemDto
    {
        [Required]
        public int CartId { get; set; }
        [Required]
        public int DishId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// 移除购物车项目
    /// </summary>
    public class RemoveCartItemDto
    {
        [Required]
        public int CartId { get; set; }
        [Required]
        public int DishId { get; set; }
    }
}
