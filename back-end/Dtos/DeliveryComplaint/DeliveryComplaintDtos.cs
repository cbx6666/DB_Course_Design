using BackEnd.DTOs.Common;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.DeliveryComplaint
{
    // ========== 消费者侧 ==========

    /// <summary>
    /// 创建配送投诉请求DTO（消费者端）
    /// </summary>
    public class CreateDeliveryComplaintDto
    {
        /// <summary>
        /// 订单编号（可选）
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// 配送任务编号（可选）
        /// </summary>
        public int? DeliveryTaskId { get; set; }

        /// <summary>
        /// 投诉原因
        /// </summary>
        [Required(ErrorMessage = "投诉原因不能为空")]
        [StringLength(255, ErrorMessage = "投诉原因不能超过255个字符")]
        public string ComplaintReason { get; set; } = null!;

        /// <summary>
        /// 投诉图片URL列表（多个图片用逗号分隔）
        /// </summary>
        public string? Images { get; set; }
    }

    /// <summary>
    /// 创建配送投诉响应DTO（消费者端）
    /// </summary>
    public class CreateDeliveryComplaintResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 投诉ID（成功时返回）
        /// </summary>
        public int? ComplaintId { get; set; }
    }

    /// <summary>
    /// 用户配送投诉列表项DTO（消费者端查看自己的投诉）
    /// </summary>
    public class CustomerDeliveryComplaintListItemDto
    {
        /// <summary>
        /// 投诉ID
        /// </summary>
        public int ComplaintId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 配送任务ID
        /// </summary>
        public int DeliveryTaskId { get; set; }

        /// <summary>
        /// 投诉原因
        /// </summary>
        public string ComplaintReason { get; set; } = string.Empty;

        /// <summary>
        /// 投诉图片URL列表
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 投诉时间
        /// </summary>
        public DateTime ComplaintTime { get; set; }

        /// <summary>
        /// 投诉状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处理结果
        /// </summary>
        public string? ProcessingResult { get; set; }

        /// <summary>
        /// 处理原因
        /// </summary>
        public string? ProcessingReason { get; set; }
    }

    // ========== 骑手侧 ==========

    /// <summary>
    /// 投诉中的处罚信息DTO（骑手端）
    /// </summary>
    public class ComplaintPunishmentDto
    {
        /// <summary>
        /// 处罚类型
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// 处罚描述
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 持续时间（可选）
        /// </summary>
        public string? Duration { get; set; }
    }

    /// <summary>
    /// 配送投诉信息DTO（骑手端查看）
    /// </summary>
    public class CourierComplaintDto
    {
        /// <summary>
        /// 投诉编号
        /// </summary>
        public string ComplaintID { get; set; } = null!;

        /// <summary>
        /// 配送任务编号
        /// </summary>
        public string DeliveryTaskID { get; set; } = null!;

        /// <summary>
        /// 投诉时间
        /// </summary>
        public string ComplaintTime { get; set; } = null!;

        /// <summary>
        /// 投诉原因
        /// </summary>
        public string ComplaintReason { get; set; } = null!;

        /// <summary>
        /// 处罚信息（可选）
        /// </summary>
        public ComplaintPunishmentDto? Punishment { get; set; }
    }

    // ========== 管理员侧 ==========

    /// <summary>
    /// 配送投诉详情DTO（管理员端展示）
    /// </summary>
    public class AdminComplaintDetailDto
    {
        /// <summary>
        /// 投诉ID
        /// </summary>
        public string ComplaintId { get; set; } = string.Empty;

        /// <summary>
        /// 投诉对象
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplicationTime { get; set; } = string.Empty;

        /// <summary>
        /// 投诉内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;

        /// <summary>
        /// 罚款
        /// </summary>
        public string Fine { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新配送投诉请求DTO（管理员端）
    /// </summary>
    public class UpdateDeliveryComplaintDto
    {
        /// <summary>
        /// 投诉ID
        /// </summary>
        public string ComplaintId { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;

        /// <summary>
        /// 罚款
        /// </summary>
        public string Fine { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新配送投诉响应DTO（管理员端）
    /// </summary>
    public class UpdateDeliveryComplaintResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 更新后的投诉详情
        /// </summary>
        public AdminComplaintDetailDto? Data { get; set; }
    }
}
