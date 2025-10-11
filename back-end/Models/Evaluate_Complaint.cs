using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 配送投诉评估模型
    /// </summary>
    public class Evaluate_Complaint
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
        /// 投诉ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int ComplaintID { get; set; }

        /// <summary>
        /// 关联的配送投诉
        /// </summary>
        [ForeignKey("ComplaintID")]
        public DeliveryComplaint Complaint { get; set; } = null!;
    }
}
