using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class AfterSaleApplication
    {
        // �ۺ�������
        // ���룺ApplicationID
        // ���룺CustomerID��OrderID��SellerID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationID { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        [Required]
        public DateTime ApplicationTime { get; set; }

        [Required]
        public int OrderID { get; set; }
        [ForeignKey("OrderID")]
        public FoodOrder Order { get; set; } = null!;

        // ��Զ��ϵ
        // �����ɶ������Ա����
        public ICollection<Evaluate_AfterSale> EvaluateAfterSales { get; set; } = new List<Evaluate_AfterSale>();
    }
}
