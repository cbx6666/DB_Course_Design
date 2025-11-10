using BackEnd.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Penalty
{
    // ========== 用户举报 ==========

    /// <summary>
    /// 用户举报店铺请求DTO（消费者端）
    /// </summary>
    public class ReportStoreDto
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

        /// <summary>
        /// 举报内容
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 举报图片URL列表（多个图片用逗号分隔）
        /// </summary>
        public string? Images { get; set; }
    }

    /// <summary>
    /// 用户店铺举报列表项DTO（消费者端查看自己的举报）
    /// </summary>
    public class CustomerStoreReportListItemDto
    {
        /// <summary>
        /// 处罚ID（举报记录ID）
        /// </summary>
        public int PenaltyId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 举报内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 举报图片URL列表
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 举报时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 商家处罚（处理结果的一部分）
        /// </summary>
        public string? MerchantPunishment { get; set; }

        /// <summary>
        /// 店铺处罚（处理结果的一部分）
        /// </summary>
        public string? StorePunishment { get; set; }

        /// <summary>
        /// 处理原因（管理员填写的处罚原因，当状态为已完成时，PenaltyReason 就是处理原因）
        /// </summary>
        public string? ProcessingReason { get; set; }
    }

    // ========== 商家查看 ==========

    /// <summary>
    /// 处罚记录DTO（商家端查看）
    /// </summary>
    public class MerchantPenaltyRecordDto
    {
        /// <summary>
        /// 处罚记录ID
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 处罚时间
        /// </summary>
        public string Time { get; set; } = null!;

        /// <summary>
        /// 商家处理措施
        /// </summary>
        public string? MerchantAction { get; set; }

        /// <summary>
        /// 平台处理措施
        /// </summary>
        public string? PlatformAction { get; set; }
    }

    /// <summary>
    /// 申诉处罚请求DTO（商家端）
    /// </summary>
    public class AppealPenaltyDto
    {
        /// <summary>
        /// 申诉原因
        /// </summary>
        public string? Reason { get; set; }
    }

    // ========== 管理员处理 ==========

    /// <summary>
    /// 违规处罚详情DTO（管理员端展示）
    /// </summary>
    public class AdminPenaltyDetailDto
    {
        /// <summary>
        /// 处罚编号
        /// </summary>
        public string PunishmentId { get; set; } = null!;

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; } = null!;

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 商家方处理措施
        /// </summary>
        public string MerchantPunishment { get; set; } = null!;

        /// <summary>
        /// 平台方处理措施
        /// </summary>
        public string StorePunishment { get; set; } = null!;

        /// <summary>
        /// 处罚时间
        /// </summary>
        public string PunishmentTime { get; set; } = null!;

        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 更新违规处罚请求DTO（管理员端）
    /// </summary>
    public class UpdatePenaltyDto
    {
        /// <summary>
        /// 处罚编号
        /// </summary>
        public string PunishmentId { get; set; } = null!;

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 商家方处理措施
        /// </summary>
        public string MerchantPunishment { get; set; } = null!;

        /// <summary>
        /// 平台方处理措施
        /// </summary>
        public string StorePunishment { get; set; } = null!;

        /// <summary>
        /// 处罚时间
        /// </summary>
        public string PunishmentTime { get; set; } = null!;

        /// <summary>
        /// 当前状态
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 更新违规处罚响应DTO（管理员端）
    /// </summary>
    public class UpdatePenaltyResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 更新后的处罚详情
        /// </summary>
        public AdminPenaltyDetailDto? Data { get; set; }
    }
}
