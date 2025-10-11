using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.DeliveryComplaint
{
    /// <summary>
    /// 创建配送投诉请求
    /// </summary>
    public class CreateComplaintDto
    {
        /// <summary>
        /// 订单编号（可选）
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// 配送任务编号（可选）
        /// </summary>
        public int? DeliveryTaskId { get; set; }

        [Required(ErrorMessage = "投诉原因不能为空")]
        [StringLength(255, ErrorMessage = "投诉原因不能超过255个字符")]
        public string ComplaintReason { get; set; } = null!;
    }

    /// <summary>
    /// 创建配送投诉响应
    /// </summary>
    public class CreateComplaintResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 提示消息
        /// </summary>
        public string Message { get; set; } = null!;

        /// <summary>
        /// 投诉ID（成功时返回）
        /// </summary>
        public int? ComplaintId { get; set; }
    }
}
