using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 用户优惠券数据传输对象
    /// </summary>
    public class UserCouponDto
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        [Required]
        public int CouponID { get; set; }

        /// <summary>
        /// 优惠券状态
        /// </summary>
        [Required]
        public CouponState CouponState { get; set; }

        /// <summary>
        /// 关联订单ID
        /// </summary>
        [Required]
        public int? OrderID { get; set; }

        /// <summary>
        /// 优惠券管理ID
        /// </summary>
        [Required]
        public int CouponManagerID { get; set; }

        /// <summary>
        /// 最低消费金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        /// <summary>
        /// 优惠值（满减券为优惠金额，折扣券为折扣比例）
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }

        /// <summary>
        /// 有效期至
        /// </summary>
        [Required]
        public string ValidTo { get; set; } = string.Empty;
    }

    /// <summary>
    /// 购物车请求数据传输对象
    /// </summary>
    public class CartRequestDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 购物车响应数据传输对象
    /// </summary>
    public class CartResponseDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        /// <summary>
        /// 购物车项目列表
        /// </summary>
        [Required]
        public List<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
    }

    /// <summary>
    /// 购物车项目数据传输对象
    /// </summary>
    public class ShoppingCartItemDto
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        [Required]
        public int ItemId { get; set; }

        /// <summary>
        /// 菜品ID
        /// </summary>
        [Required]
        public int DishId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public int Quantity { get; set; }

        /// <summary>
        /// 总价格
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;
    }

    /// <summary>
    /// 更新购物车项目数据传输对象
    /// </summary>
    public class UpdateCartItemDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 菜品ID
        /// </summary>
        [Required]
        public int DishId { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [Required]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// 移除购物车项目数据传输对象
    /// </summary>
    public class RemoveCartItemDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 菜品ID
        /// </summary>
        [Required]
        public int DishId { get; set; }
    }
}