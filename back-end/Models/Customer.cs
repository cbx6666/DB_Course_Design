using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 消费者信息模型
    /// </summary>
    public class Customer
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
        /// 默认地址
        /// </summary>
        [StringLength(100)]
        public string? DefaultAddress { get; set; }

        /// <summary>
        /// 信誉积分
        /// </summary>
        public int ReputationPoints { get; set; } = 0;

        /// <summary>
        /// 会员状态
        /// </summary>
        public MembershipStatus IsMember { get; set; } = MembershipStatus.NotMember;

        /// <summary>
        /// 配送任务集合
        /// </summary>
        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }

        /// <summary>
        /// 订单集合
        /// </summary>
        public ICollection<FoodOrder>? FoodOrders { get; set; }

        /// <summary>
        /// 优惠券集合
        /// </summary>
        public ICollection<Coupon>? Coupons { get; set; }

        /// <summary>
        /// 收藏夹集合
        /// </summary>
        public ICollection<FavoritesFolder>? FavoritesFolders { get; set; }

        /// <summary>
        /// 评论集合
        /// </summary>
        public ICollection<Comment>? Comments { get; set; }

        /// <summary>
        /// 购物车集合
        /// </summary>
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
    }
}
