using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 售后申请模型
    /// </summary>
    public class AfterSaleApplication
    {
        /// <summary>
        /// 售后申请ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationID { get; set; }

        /// <summary>
        /// 申请描述
        /// </summary>
        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        /// <summary>
        /// 申请时间
        /// </summary>
        [Required]
        public DateTime ApplicationTime { get; set; }

        /// <summary>
        /// 售后申请状态
        /// </summary>
        [Required]
        public AfterSaleState AfterSaleState { get; set; } = AfterSaleState.Pending;

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
        /// 处理备注
        /// </summary>
        [StringLength(255)]
        public string? ProcessingRemark { get; set; }

        /// <summary>
        /// 订单ID（外键）
        /// </summary>
        [Required]
        public int OrderID { get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("OrderID")]
        public FoodOrder Order { get; set; } = null!;

        /// <summary>
        /// 售后申请评估记录集合
        /// </summary>
        public ICollection<Evaluate_AfterSale> EvaluateAfterSales { get; set; } = new List<Evaluate_AfterSale>();
    }
}
