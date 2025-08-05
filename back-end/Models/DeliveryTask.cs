using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class DeliveryTask
    {
        // ����������
        // ���룺TaskID
        // ���룺CustomerID��StoreID��SellerID��CourierID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        [Required]
        public DateTime EstimatedArrivalTime { get; set; }

        [Required]
        public DateTime EstimatedDeliveryTime { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? CourierLongitude { get; set; }

        [Column(TypeName = "decimal(10,6)")]
        public decimal? CourierLatitude { get; set; }

        // ��������ʱ��
        [Required]
        public DateTime PublishTime { get; set; }

        // �ӵ�ʱ��
        [Required]
        public DateTime AcceptTime { get; set; }

        [Required]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        [Required]
        public int StoreID { get; set; }
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        [Required]
        public int SellerID { get; set; }
        [ForeignKey("SellerID")]
        public Seller Seller { get; set; } = null!;

        [Required]
        public int CourierID { get; set; }
        [ForeignKey("CourierID")]
        public Courier Courier { get; set; } = null!;

        // һ�Զർ������
        // ����Ͷ��
        public ICollection<DeliveryComplaint>? DeliveryComplaints { get; set; }
    }
}
