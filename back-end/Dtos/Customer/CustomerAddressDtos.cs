using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Customer
{
    /// <summary>
    /// 用户收货地址列表项
    /// </summary>
    public class UserDeliveryInfoDto
    {
        public int DeliveryInfoID { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public bool IsDefault { get; set; }
    }

    /// <summary>
    /// 新建/更新 收货地址请求
    /// </summary>
    public class CreateAddressDto
    {
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public long PhoneNumber { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Gender { get; set; }
        public bool IsDefault { get; set; } = false;
    }

    /// <summary>
    /// 保存地址数据传输对象（用于保存或更新默认收货地址）
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
}
