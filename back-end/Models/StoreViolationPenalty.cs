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
        /// 处罚原因
        /// </summary>
        [Required]
        [StringLength(255)]
        public string PenaltyReason { get; set; } = null!;

        /// <summary>
        /// 处罚备注
        /// </summary>
        [StringLength(255)]
        public string? PenaltyNote { get; set; }

        /// <summary>
        /// 处罚时间
        /// </summary>
        [Required]
        public DateTime PenaltyTime { get; set; }

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
        /// 监督记录集合
        /// </summary>
        public ICollection<Supervise_> Supervise_s { get; set; } = new List<Supervise_>();
    }
}
