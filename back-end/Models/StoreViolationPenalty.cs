using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 店铺违规处罚模型
    /// </summary>
    public class StoreViolationPenalty
    {
        /// <summary>
        /// 处罚ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PenaltyID { get; set; }

        /// <summary>
        /// 违规处罚状态
        /// </summary>
        [Required]
        public ViolationPenaltyState ViolationPenaltyState { get; set; } = ViolationPenaltyState.Pending;

        /// <summary>
        /// 举报原因（消费者填写的举报内容）
        /// </summary>
        [Required]
        [StringLength(255)]
        public string ReportReason { get; set; } = null!;

        /// <summary>
        /// 举报图片URL（多个图片用逗号分隔）
        /// </summary>
        [StringLength(1000)]
        public string? ReportImages { get; set; }

        /// <summary>
        /// 举报时间（消费者提交举报的时间）
        /// </summary>
        [Required]
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 处罚时间（管理员处理完成时设置）
        /// </summary>
        public DateTime? PenaltyTime { get; set; }

        /// <summary>
        /// 商家处罚
        /// </summary>
        [StringLength(50)]
        public string? SellerPenalty { get; set; }

        /// <summary>
        /// 店铺处罚
        /// </summary>
        [StringLength(50)]
        public string? StorePenalty { get; set; }

        /// <summary>
        /// 处罚原因（管理员填写的处理原因）
        /// </summary>
        [StringLength(255)]
        public string? PenaltyReason { get; set; }

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 举报消费者ID（外键，可选）
        /// </summary>
        public int? CustomerID { get; set; }

        /// <summary>
        /// 关联的举报消费者
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer? Customer { get; set; }

        /// <summary>
        /// 举报骑手ID（外键，可选）
        /// </summary>
        public int? CourierID { get; set; }

        /// <summary>
        /// 关联的举报骑手
        /// </summary>
        [ForeignKey("CourierID")]
        public Courier? Courier { get; set; }

        /// <summary>
        /// 监督记录集合
        /// </summary>
        public ICollection<Supervise_> Supervise_s { get; set; } = new List<Supervise_>();
    }
}
