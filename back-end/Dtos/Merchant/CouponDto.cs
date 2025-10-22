using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 优惠券DTO - 用于前端显示
    /// </summary>
    public class CouponDto
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
        /// 优惠券类型: 'fixed' | 'discount'
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
        /// 开始时间（ISO格式）
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        /// <summary>
        /// 结束时间（ISO格式）
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        /// <summary>
        /// 发放数量
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
        /// 状态: 'active' | 'expired' | 'upcoming'
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 优惠券统计DTO
    /// </summary>
    public class CouponStatsDto
    {
        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 有效数量
        /// </summary>
        public int Active { get; set; }

        /// <summary>
        /// 已过期数量
        /// </summary>
        public int Expired { get; set; }

        /// <summary>
        /// 未开始数量
        /// </summary>
        public int Upcoming { get; set; }

        /// <summary>
        /// 总使用次数
        /// </summary>
        public int TotalUsed { get; set; }

        /// <summary>
        /// 总优惠金额
        /// </summary>
        public decimal TotalDiscountAmount { get; set; }
    }

    /// <summary>
    /// 优惠券列表响应DTO
    /// </summary>
    public class CouponListResponseDto
    {
        /// <summary>
        /// 优惠券列表
        /// </summary>
        public List<CouponDto> List { get; set; } = new List<CouponDto>();

        /// <summary>
        /// 总条数
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// 创建优惠券请求DTO
    /// </summary>
    public class CreateCouponRequestDto
    {
        /// <summary>
        /// 优惠券ID（由后端自动生成）
        /// </summary>
        public int? Id { get; set; }

        [Required(ErrorMessage = "优惠券名称不能为空")]
        [MaxLength(100, ErrorMessage = "优惠券名称长度不能超过100个字符")]
        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "优惠券类型不能为空")]
        /// <summary>
        /// 优惠券类型: 'fixed' | 'discount'
        /// </summary>
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "优惠值不能为空")]
        /// <summary>
        /// 优惠值（满减券为优惠金额，折扣券为折扣比例）
        /// </summary>
        public decimal Value { get; set; }

        /// <summary>
        /// 最低消费金额（可选）
        /// </summary>
        public decimal? MinAmount { get; set; }

        /// <summary>
        /// 店铺ID（由后端自动获取）
        /// </summary>
        public int? StoreId { get; set; }

        [Required(ErrorMessage = "发放数量不能为空")]
        [Range(1, 100000, ErrorMessage = "发放数量必须在1-100000之间")]
        /// <summary>
        /// 发放数量
        /// </summary>
        public int TotalQuantity { get; set; }

        [Required(ErrorMessage = "开始时间不能为空")]
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "结束时间不能为空")]
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "描述长度不能超过500个字符")]
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新优惠券请求DTO
    /// </summary>
    public class UpdateCouponRequestDto : CreateCouponRequestDto
    {
        // 继承CreateCouponRequestDto的所有字段，包括id字段
        // 不需要重新定义id字段
    }

    /// <summary>
    /// 批量删除请求DTO
    /// </summary>
    public class BatchDeleteCouponsRequestDto
    {
        [Required(ErrorMessage = "优惠券ID列表不能为空")]
        /// <summary>
        /// 优惠券ID列表
        /// </summary>
        public List<int> Ids { get; set; } = new List<int>();
    }

    /// <summary>
    /// 批量删除响应DTO
    /// </summary>
    public class BatchDeleteResponseDto
    {
        /// <summary>
        /// 删除数量
        /// </summary>
        public int DeletedCount { get; set; }
    }

    /// <summary>
    /// 创建优惠券响应DTO
    /// </summary>
    public class CreateCouponResponseDto
    {
        /// <summary>
        /// 新创建的优惠券ID
        /// </summary>
        public int Id { get; set; }
    }
}
