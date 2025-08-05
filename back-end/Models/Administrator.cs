using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class Administrator
    {
        // ����Ա��
        // ���룺UserID
        // ���룺UserID

        [Key, ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public DateTime AdminRegistrationTime { get; set; }

        [Required]
        [StringLength(50)]
        public string ManagedEntities { get; set; } = null!;

        [Column(TypeName = "decimal(3,2)")]
        public decimal IssueHandlingScore { get; set; } = 0.00m;

        // ��Զ��ϵ
        // �������
        public ICollection<Review_Comment> ReviewComments { get; set; } = new List<Review_Comment>();

        // �ලΥ�����
        public ICollection<Supervise> Supervises { get; set; } = new List<Supervise>();

        // �����ۺ�����
        public ICollection<Evaluate_AfterSale> EvaluateAfterSales { get; set; } = new List<Evaluate_AfterSale>();

        // ��������Ͷ��
        public ICollection<Evaluate_Complaint> EvaluateComplaints { get; set; } = new List<Evaluate_Complaint>();

        // �������
        [NotMapped]
        public IEnumerable<Comment> Comments => ReviewComments.Select(rc => rc.Comment);

        [NotMapped]
        public IEnumerable<StoreViolationPenalty> Penalties => Supervises.Select(s => s.Penalty);

        [NotMapped]
        public IEnumerable<AfterSaleApplication> Applications => EvaluateAfterSales.Select(eas => eas.Application);

        [NotMapped]
        public IEnumerable<DeliveryComplaint> DeliveryComplaints => EvaluateComplaints.Select(ec => ec.Complaint);
    }
}
