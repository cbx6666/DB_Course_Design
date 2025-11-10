using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 配送投诉模型
    /// </summary>
    public class DeliveryComplaint
    {
        /// <summary>
        /// 投诉ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ComplaintID { get; set; }

        /// <summary>
        /// 投诉原因
        /// </summary>
        [Required]
        [StringLength(255)]
        public string ComplaintReason { get; set; } = null!;

        /// <summary>
        /// 投诉图片URL（多个图片用逗号分隔）
        /// </summary>
        [StringLength(1000)]
        public string? ComplaintImages { get; set; }

        /// <summary>
        /// 投诉时间
        /// </summary>
        [Required]
        public DateTime ComplaintTime { get; set; }

        /// <summary>
        /// 投诉状态
        /// </summary>
        [Required]
        public ComplaintState ComplaintState { get; set; } = ComplaintState.Pending;

        /// <summary>
        /// 处理结果
        /// </summary>
        [StringLength(255)]
        public string? ProcessingResult { get; set; } = "-";

        /// <summary>
        /// 处理原因
        /// </summary>
        [StringLength(255)]
        public string? ProcessingReason { get; set; }

        /// <summary>
        /// 罚金金额
        /// </summary>
        public decimal? FineAmount { get; set; }

        /// <summary>
        /// 配送员ID（外键）
        /// </summary>
        [Required]
        public int CourierID { get; set; }

        /// <summary>
        /// 关联的配送员
        /// </summary>
        [ForeignKey("CourierID")]
        public Courier Courier { get; set; } = null!;

        /// <summary>
        /// 消费者ID（外键）
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的消费者
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 配送任务ID（外键）
        /// </summary>
        [Required]
        public int DeliveryTaskID { get; set; }

        /// <summary>
        /// 关联的配送任务
        /// </summary>
        [ForeignKey("DeliveryTaskID")]
        public DeliveryTask DeliveryTask { get; set; } = null!;

        /// <summary>
        /// 投诉评估记录集合
        /// </summary>
        public ICollection<Evaluate_Complaint> EvaluateComplaints { get; set; } = new List<Evaluate_Complaint>();
    }
}
