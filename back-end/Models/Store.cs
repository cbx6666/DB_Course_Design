using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 店铺信息模型
    /// </summary>
    public class Store
    {
        /// <summary>
        /// 店铺ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreID { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string StoreName { get; set; } = null!;

        /// <summary>
        /// 店铺地址
        /// </summary>
        [Required]
        [StringLength(100)]
        public string StoreAddress { get; set; } = null!;

        /// <summary>
        /// 开业时间
        /// </summary>
        [Required]
        public TimeSpan OpenTime { get; set; } = TimeSpan.FromHours(9);

        /// <summary>
        /// 关门时间
        /// </summary>
        [Required]
        public TimeSpan CloseTime { get; set; } = TimeSpan.FromHours(22);

        /// <summary>
        /// 纬度
        /// </summary>
        [Column(TypeName = "decimal(10,6)")]
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [Column(TypeName = "decimal(10,6)")]
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 是否营业中（计算属性）
        /// </summary>
        [NotMapped]
        public bool IsOpen
        {
            get
            {
                var now = DateTime.Now.TimeOfDay;
                return OpenTime <= CloseTime 
                    ? now >= OpenTime && now <= CloseTime 
                    : now >= OpenTime || now <= CloseTime;
            }
        }

        /// <summary>
        /// 营业时间显示（计算属性）
        /// </summary>
        [NotMapped]
        public string BusinessHoursDisplay => $"{OpenTime:hh\\:mm} - {CloseTime:hh\\:mm}";

        /// <summary>
        /// 平均评分
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageRating { get; set; } = 0.00m;

        /// <summary>
        /// 月销量
        /// </summary>
        [Required]
        public int MonthlySales { get; set; }

        /// <summary>
        /// 店铺特色
        /// </summary>
        [StringLength(500)]
        public string? StoreFeatures { get; set; }

        /// <summary>
        /// 店铺创建时间
        /// </summary>
        [Required]
        public DateTime StoreCreationTime { get; set; }

        /// <summary>
        /// 店铺状态
        /// </summary>
        [Required]
        public StoreState StoreState { get; set; } = StoreState.IsOperation;

        /// <summary>
        /// 店铺种类
        /// </summary>
        [Required]
        public string StoreCategory { get; set; } = null!;

        /// <summary>
        /// 店铺图片
        /// </summary>
        public string? StoreImage { get; set; }

        /// <summary>
        /// 商家ID（外键）
        /// </summary>
        [Required]
        public int SellerID { get; set; }

        /// <summary>
        /// 关联的商家信息
        /// </summary>
        [ForeignKey("SellerID")]
        public Seller Seller { get; set; } = null!;

        /// <summary>
        /// 订单集合
        /// </summary>
        public ICollection<FoodOrder>? FoodOrders { get; set; }

        /// <summary>
        /// 优惠券管理集合
        /// </summary>
        public ICollection<CouponManager>? CouponManagers { get; set; }

        /// <summary>
        /// 菜单集合
        /// </summary>
        public ICollection<Menu>? Menus { get; set; }

        /// <summary>
        /// 收藏项集合
        /// </summary>
        public ICollection<FavoriteItem>? FavoriteItems { get; set; }

        /// <summary>
        /// 违规处罚集合
        /// </summary>
        public ICollection<StoreViolationPenalty>? StoreViolationPenalties { get; set; }

        /// <summary>
        /// 评论集合
        /// </summary>
        public ICollection<Comment>? Comments { get; set; }

        /// <summary>
        /// 配送任务集合
        /// </summary>
        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }

        /// <summary>
        /// 购物车集合
        /// </summary>
        public ICollection<ShoppingCart>? ShoppingCarts { get; set; }
    }
}
