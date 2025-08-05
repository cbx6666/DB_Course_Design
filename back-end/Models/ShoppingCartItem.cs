using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class ShoppingCartItem
    {
        // ���ﳵ����
        // ���룺ItemID
        // ���룺DishID��CartID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; } = 0.00m;

        [Required]
        public int DishID { get; set; }
        [ForeignKey("DishID")]
        public Dish Dish { get; set; } = null!;

        [Required]
        public int CartID { get; set; }
        [ForeignKey("CartID")]
        public ShoppingCart Cart { get; set; } = null!;
    }
}
