using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public class Seller
    {
        // �̼���
        // ���룺UserID
        // ���룺UserID

        [Key, ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public DateTime SellerRegistrationTime { get; set; }

        public int ReputationPoints { get; set; } = 0;

        [Required]
        public SellerState BanStatus { get; set; } = SellerState.Normal;

        // �̼Һ͵���һ��һ
        public Store? Store { get; set; }
    }
}
