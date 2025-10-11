using BackEnd.Models.Enums;

namespace BackEnd.DTOs.Order
{
    /// <summary>
    /// 订单数据传输对象
    /// </summary>
    public class FoodOrderDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        public string PaymentTime { get; set; } = null!;

        /// <summary>
        /// 订单备注
        /// </summary>
        public string? Remarks { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// 购物车ID
        /// </summary>
        public int CartId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 商家ID
        /// </summary>
        public int SellerId { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public FoodOrderState OrderState { get; set; }

        /// <summary>
        /// 配送任务ID
        /// </summary>
        public int? DeliveryTaskId { get; set; }

        /// <summary>
        /// 配送状态
        /// </summary>
        public int? DeliveryStatus { get; set; }
    }
}