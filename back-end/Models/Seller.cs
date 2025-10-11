using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 商家信息模型
    /// </summary>
    public class Seller
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
        /// 商家注册时间
        /// </summary>
        [Required]
        public DateTime SellerRegistrationTime { get; set; }

        /// <summary>
        /// 信誉积分
        /// </summary>
        public int ReputationPoints { get; set; } = 0;

        /// <summary>
        /// 封禁状态
        /// </summary>
        [Required]
        public SellerState BanStatus { get; set; } = SellerState.Normal;

        /// <summary>
        /// 关联的店铺信息
        /// </summary>
        public Store? Store { get; set; }
    }
}
