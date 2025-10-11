using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 监督模型
    /// </summary>
    public class Supervise_
    {
        /// <summary>
        /// 管理员ID（复合主键）
        /// </summary>
        [Key, Column(Order = 0)]
        public int AdminID { get; set; }

        /// <summary>
        /// 关联的管理员
        /// </summary>
        [ForeignKey("AdminID")]
        public Administrator Admin { get; set; } = null!;

        /// <summary>
        /// 处罚ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int PenaltyID { get; set; }

        /// <summary>
        /// 关联的店铺违规处罚
        /// </summary>
        [ForeignKey("PenaltyID")]
        public StoreViolationPenalty Penalty { get; set; } = null!;
    }
}
