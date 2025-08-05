using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class FoodOrder
    {
        // ���Ͷ�����
        // ���룺OrderID
        // ���룺CustomerID��CartID��StoreID��SellerID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public DateTime PaymentTime { get; set; }

        [StringLength(255)]
        public string? Remarks { get; set; }

        // ������������ֶ�
        [Range(1, 5)]  // �������ַ�ΧΪ1-5
        [Column(TypeName = "decimal(2,1)")] 
        public decimal? Rating { get; set; } 

        [StringLength(500)]
        public string? RatingComment { get; set; }  // ��������

        public DateTime? RatingTime { get; set; }  // ����ʱ��

        [Required]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        [Required]
        public int CartID { get; set; }
        [ForeignKey("CartID")]
        public ShoppingCart Cart { get; set; } = null!;

        [Required]
        public int StoreID { get; set; }
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        [Required]
        public int SellerID { get; set; }
        [ForeignKey("SellerID")]
        public Seller Seller { get; set; } = null!;

        // һ�Զർ������
        // �Ż�ȯ
        public ICollection<Coupon>? Coupons { get; set; }

        // �ۺ�����
        public ICollection<AfterSaleApplication>? AfterSaleApplications { get; set; }
    }
}
