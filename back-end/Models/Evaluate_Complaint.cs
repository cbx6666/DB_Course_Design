using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class Evaluate_Complaint
    {
        // ����Ա������Ͷ��֮�������Ͷ�߹�ϵ
        // ���룺AdminID��ComplaintID

        [Key, Column(Order = 0)]
        public int AdminID { get; set; }
        [ForeignKey("AdminID")]
        public Administrator Admin { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int ComplaintID { get; set; }
        [ForeignKey("ComplaintID")]
        public DeliveryComplaint Complaint { get; set; } = null!;
    }
}
