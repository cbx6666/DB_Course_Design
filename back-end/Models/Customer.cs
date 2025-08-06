using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    public class Customer
    {
        // ��������
        // ���룺UserID
        // ���룺USerID

        [Key, ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; } = null!;

        [StringLength(100)]
        public string? DefaultAddress { get; set; }

        public int ReputationPoints { get; set; } = 0;

        public MembershipStatus IsMember { get; set; } = MembershipStatus.NotMember;

        // һ�Զർ������
        // ��������
        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }

        // ���Ͷ���
        public ICollection<FoodOrder>? FoodOrders { get; set; }

        // �Ż�ȯ
        public ICollection<Coupon>? Coupons { get; set; }

        // �ղؼ�
        public ICollection<FavoritesFolder>? FavoritesFolders { get; set; }

        // ����
        public ICollection<Comment>? Comments { get; set; }
    }
}
