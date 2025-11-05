using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Coupon
{
    /// <summary>
    /// 商家侧优惠券展示 DTO（商家后台列表/详情展示）
    /// - 面向商家运营端展示
    /// - 由 CouponService.GetCouponsAsync 等返回
    /// </summary>
    public class MerchantCouponDto
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 优惠券类型：'fixed' | 'discount'
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 优惠值（金额或折扣比例）
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 最低消费（仅满减券）
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// 总数量
        /// </summary>
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 已使用数量
        /// </summary>
        public int UsedQuantity { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 状态：'active' | 'expired' | 'upcoming'
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 商家侧优惠券统计 DTO（仪表盘/概览）
    /// - 用于展示统计总览数据
    /// </summary>
    public class CouponStatsDto
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 激活数量
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// 已过期数量
        /// </summary>
        public int Expired { get; set; }

        /// <summary>
        /// 即将开始数量
        /// </summary>
        public int Upcoming { get; set; }

        /// <summary>
        /// 总使用数量
        /// </summary>
        public int TotalUsed { get; set; }

        /// <summary>
        /// 总折扣金额
        /// </summary>
        public decimal TotalDiscountAmount { get; set; }
    }

    /// <summary>
    /// 商家侧创建/更新优惠券请求 DTO
    /// - 被 CouponMapper 映射为 CouponManager 实体
    /// </summary>
    public class CreateCouponRequestDto
    {
        /// <summary>
        /// 优惠券ID（可选，更新时使用）
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        [Required(ErrorMessage = "优惠券名称不能为空")]
        [MaxLength(100, ErrorMessage = "优惠券名称长度不能超过100个字符")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 优惠券类型：'fixed' | 'discount'
        /// </summary>
        [Required(ErrorMessage = "优惠券类型不能为空")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 优惠值（满减券为优惠金额，折扣券为折扣比例）
        /// </summary>
        [Required(ErrorMessage = "优惠值不能为空")]
        public decimal Value { get; set; }

        /// <summary>
        /// 最低消费金额（可选）
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 店铺ID（由后端自动获取）
        /// </summary>
        public int? StoreId { get; set; }

        /// <summary>
        /// 发放数量
        /// </summary>
        [Required(ErrorMessage = "发放数量不能为空")]
        [Range(1, 100000, ErrorMessage = "发放数量必须在1-100000之间")]
        public int TotalQuantity { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required(ErrorMessage = "开始时间不能为空")]
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required(ErrorMessage = "结束时间不能为空")]
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(500, ErrorMessage = "描述长度不能超过500个字符")]
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 商家侧批量删除优惠券请求 DTO
    /// </summary>
    public class BatchDeleteCouponsRequestDto
    {
        /// <summary>
        /// 优惠券ID列表
        /// </summary>
        [Required(ErrorMessage = "优惠券ID列表不能为空")]
        public List<int> Ids { get; set; } = new List<int>();
    }
}
