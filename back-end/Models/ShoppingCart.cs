using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class ShoppingCart
    {
        // ���ﳵ��
        // ���룺CartID
        // ���룺OrderID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }

        [Required]
        public DateTime LastUpdatedTime { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        public int? OrderID { get; set; }
        [ForeignKey("OrderID")]
        public FoodOrder? Order { get; set; }

        // һ�Զർ������
        // ���ﳵ��
        public ICollection<ShoppingCartItem>? ShoppingCartItems { get; set; }
    }
}
