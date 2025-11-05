using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BackEnd.Models.Enums;

namespace BackEnd.DTOs.Order
{
    /// <summary>
    /// 创建订单数据传输对象
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        [Required]
        public int CustomerId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int StoreId { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [Required]
        public DateTime PaymentTime { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [JsonPropertyName("deliveryFee")]
        public decimal DeliveryFee { get; set; } = 0.00m;

        /// <summary>
        /// 订单备注
        /// </summary>
        public string? Remarks { get; set; }

        /// <summary>
        /// 收货地址ID
        /// </summary>
        [Required]
        public int DeliveryInfoID { get; set; }

        /// <summary>
        /// 优惠券ID（可选）
        /// </summary>
        public int? CouponId { get; set; }
    }

    /// <summary>
    /// 订单通用基础信息
    /// </summary>
    public class OrderBaseDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// 支付时间（ISO 字符串）
        /// </summary>
        [Required]
        public string PaymentTime { get; set; } = null!;

        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int StoreId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Required]
        public FoodOrderState OrderState { get; set; }
    }

    /// <summary>
    /// 订单通用配送元信息
    /// </summary>
    public class OrderDeliveryMetaDto
    {
        /// <summary>
        /// 配送状态（通常为 int 枚举值）
        /// </summary>
        public int? DeliveryStatus { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        [Required]
        public decimal DeliveryFee { get; set; }
    }

    /// <summary>
    /// 订单通用门店展示元信息
    /// </summary>
    public class OrderStoreMetaDto
    {
        /// <summary>
        /// 店铺图片
        /// </summary>
        [Required]
        public string StoreImage { get; set; } = string.Empty;

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        public string StoreName { get; set; } = string.Empty;
    }
}
