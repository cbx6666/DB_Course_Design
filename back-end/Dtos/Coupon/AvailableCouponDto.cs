using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.DTOs.Coupon
{
    /// <summary>
    /// 可领取优惠券数据传输对象（用户在店铺/首页看到可领取的券池）
    /// - 面向用户端展示“可领取”列表
    /// - 由 UserHomepageService.GetAvailableCouponsAsync 返回
    /// </summary>
    public class AvailableCouponDto
    {
        [Required]
        public int CouponManagerID { get; set; }

        [Required]
        public string CouponName { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = string.Empty; // fixed | discount

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Value { get; set; }

        [Required]
        public string ValidFrom { get; set; } = string.Empty;

        [Required]
        public string ValidTo { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public int StoreID { get; set; }

        [Required]
        public string StoreName { get; set; } = string.Empty;

        public string? StoreImage { get; set; }

        [Required]
        public int RemainingQuantity { get; set; }

        [Required]
        public bool IsClaimed { get; set; }
    }
}
