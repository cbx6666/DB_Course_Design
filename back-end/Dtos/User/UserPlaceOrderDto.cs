using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.User
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
    }

    /// <summary>
    /// 更新账户信息数据传输对象
    /// </summary>
    public class UpdateAccountDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 头像文件
        /// </summary>
        public IFormFile? AvatarFile { get; set; }
    }

    /// <summary>
    /// 保存地址数据传输对象
    /// </summary>
    public class SaveAddressDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Required]
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        public string? Gender { get; set; }
    }

    /// <summary>
    /// 响应数据传输对象
    /// </summary>
    public class ResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}