namespace BackEnd.DTOs.Cart
{
    /// <summary>
    /// 购物车商品项
    /// </summary>
    public class ShoppingCartItemDto
    {
        /// <summary>
        /// 商品项ID
        /// </summary>
        public int ItemId { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// 菜品ID
        /// </summary>
        public int DishId { get; set; }
        /// <summary>
        /// 购物车ID
        /// </summary>
        public int CartId { get; set; }
        /// <summary>
        /// 菜品信息
        /// </summary>
        public CartItemDishRefDto? Dish { get; set; }
    }

    /// <summary>
    /// 购物车商品引用信息
    /// </summary>
    public class CartItemDishRefDto
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
}