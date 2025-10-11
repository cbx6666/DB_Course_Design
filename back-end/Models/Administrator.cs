using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 管理员信息模型
    /// </summary>
    public class Administrator
    {
        /// <summary>
        /// 用户ID（主键，外键）
        /// </summary>
        [Key, ForeignKey("User")]
        public int UserID { get; set; }

        /// <summary>
        /// 关联的用户信息
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// 管理员注册时间
        /// </summary>
        [Required]
        public DateTime AdminRegistrationTime { get; set; }

        /// <summary>
        /// 管理员角色
        /// </summary>
        [Required]
        [StringLength(20)]
        public string AdminRole { get; set; } = "系统管理员";

        /// <summary>
        /// 管理的实体
        /// </summary>
        [Required]
        [StringLength(50)]
        public string ManagedEntities { get; set; } = null!;

        /// <summary>
        /// 问题处理评分
        /// </summary>
        [Column(TypeName = "decimal(3,2)")]
        public decimal IssueHandlingScore { get; set; } = 0.00m;

        /// <summary>
        /// 审核的评论
        /// </summary>
        public ICollection<Review_Comment> ReviewComments { get; set; } = new List<Review_Comment>();

        /// <summary>
        /// 监督的违规店铺
        /// </summary>
        public ICollection<Supervise_> Supervise_s { get; set; } = new List<Supervise_>();

        /// <summary>
        /// 处理的售后请求
        /// </summary>
        public ICollection<Evaluate_AfterSale> EvaluateAfterSales { get; set; } = new List<Evaluate_AfterSale>();

        /// <summary>
        /// 处理的配送投诉
        /// </summary>
        public ICollection<Evaluate_Complaint> EvaluateComplaints { get; set; } = new List<Evaluate_Complaint>();

        /// <summary>
        /// 评论列表（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<Comment> Comments => ReviewComments.Select(rc => rc.Comment);

        /// <summary>
        /// 处罚记录列表（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<StoreViolationPenalty> Penalties => Supervise_s.Select(s => s.Penalty);

        /// <summary>
        /// 售后申请列表（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<AfterSaleApplication> Applications => EvaluateAfterSales.Select(eas => eas.Application);

        /// <summary>
        /// 配送投诉列表（计算属性）
        /// </summary>
        [NotMapped]
        public IEnumerable<DeliveryComplaint> DeliveryComplaints => EvaluateComplaints.Select(ec => ec.Complaint);
    }
}
