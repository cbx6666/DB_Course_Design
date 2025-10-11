using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 售后申请评估模型
    /// </summary>
    public class Evaluate_AfterSale
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
        /// 售后申请ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int ApplicationID { get; set; }

        /// <summary>
        /// 关联的售后申请
        /// </summary>
        [ForeignKey("ApplicationID")]
        public AfterSaleApplication Application { get; set; } = null!;
    }
}
