using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 用户信息响应
    /// </summary>
    public class UserInfoResponseDto
    {
        [Required]
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// 手机号
        /// </summary>
        public long PhoneNumber { get; set; }

        [Required]
        /// <summary>
        /// 头像
        /// </summary>
        public string Image { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// 默认地址
        /// </summary>
        public string DefaultAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// 提交订单请求
    /// </summary>
    public class SubmitOrderRequestDto
    {
        [Required]
        /// <summary>
        /// 支付时间
        /// </summary>
        public string PaymentTime { get; set; } = null!;

        [Required]
        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerId { get; set; }

        [Required]
        /// <summary>
        /// 购物车编号
        /// </summary>
        public int CartId { get; set; }

        [Required]
        /// <summary>
        /// 店铺编号
        /// </summary>
        public int StoreId { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        /// <summary>
        /// 配送费
        /// </summary>
        public decimal DeliveryFee { get; set; } = 0.00m;
    }

    /// <summary>
    /// 使用的优惠券
    /// </summary>
    public class UsedCouponDto
    {
        [Required]
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public int CouponId { get; set; }
    }

    /// <summary>
    /// 获取用户ID请求
    /// </summary>
    public class GetUserIdRequestDto
    {
        /// <summary>
        /// 用户手机号或邮箱
        /// </summary>
        public string Account { get; set; } = string.Empty;
    }

    /// <summary>
    /// 获取用户ID响应
    /// </summary>
    public class GetUserIdResponseDto
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int Id { get; set; }
    }
}