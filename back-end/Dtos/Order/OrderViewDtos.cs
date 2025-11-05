using System.ComponentModel.DataAnnotations;
using BackEnd.Models.Enums;
using BackEnd.DTOs.Dish;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Cart;

namespace BackEnd.DTOs.Order
{
    /// <summary>
    /// 商家端订单视图（运营侧展示与管理）
    /// </summary>
    public class MerchantOrderViewDto : OrderBaseDto
    {
        public string? Remarks { get; set; }
        public int CustomerId { get; set; }
        public int SellerId { get; set; }
        public int? DeliveryTaskId { get; set; }

        // 配送信息（通用）
        public int? DeliveryStatus { get; set; }
        public decimal DeliveryFee { get; set; }

        // 配送联系信息（商家/骑手侧）
        public string? DeliveryAddress { get; set; }
        public string? DeliveryName { get; set; }
        public string? DeliveryPhone { get; set; }

        // 订单明细与优惠券
        public IEnumerable<ShoppingCartItemDto>? Items { get; set; }
        public OrderCouponInfoDto? UsedCoupon { get; set; }
    }

    /// <summary>
    /// 客户端订单视图（用户侧展示）
    /// </summary>
    public class CustomerOrderViewDto : OrderBaseDto
    {
        // 配送信息（通用）
        public int? DeliveryStatus { get; set; }
        public decimal DeliveryFee { get; set; }

        // 门店展示信息（通用）
        public string StoreImage { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;

        // 展示字段（菜品与金额）
        public List<string> DishImage { get; set; } = new List<string>();
        public List<OrderDishDto> DishDetails { get; set; } = new List<OrderDishDto>();

        /// <summary>
        /// 原始商品总价（不含优惠券、不含配送费）
        /// </summary>
        public decimal TotalAmount { get; set; }

        public OrderCouponInfoDto? UsedCoupon { get; set; }
    }
}
