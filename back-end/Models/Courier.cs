using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 配送员信息模型
    /// </summary>
    public class Courier
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
        /// 配送员注册时间
        /// </summary>
        [Required]
        public DateTime CourierRegistrationTime { get; set; }

        /// <summary>
        /// 交通工具类型
        /// </summary>
        [Required]
        [StringLength(20)]
        public string VehicleType { get; set; } = null!;

        /// <summary>
        /// 信誉积分
        /// </summary>
        public int ReputationPoints { get; set; } = 0;

        /// <summary>
        /// 总配送次数
        /// </summary>
        public int TotalDeliveries { get; set; } = 0;

        /// <summary>
        /// 平均配送时间（分钟）
        /// </summary>
        public int AvgDeliveryTime { get; set; } = 0;

        /// <summary>
        /// 平均评分
        /// </summary>
        [Column(TypeName = "decimal(3,2)")]
        public decimal AverageRating { get; set; } = 0.00m;

        /// <summary>
        /// 月薪
        /// </summary>
        public int MonthlySalary { get; set; } = 0;

        /// <summary>
        /// 在线状态
        /// </summary>
        [Required]
        public CourierIsOnline IsOnline { get; set; } = CourierIsOnline.Offline;

        /// <summary>
        /// 经度
        /// </summary>
        [Column(TypeName = "decimal(10,6)")]
        public decimal? CourierLongitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [Column(TypeName = "decimal(10,6)")]
        public decimal? CourierLatitude { get; set; }

        /// <summary>
        /// 最后在线时间
        /// </summary>
        public DateTime? LastOnlineTime { get; set; }

        /// <summary>
        /// 本月佣金
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal CommissionThisMonth { get; set; } = 0.00m;

        /// <summary>
        /// 配送任务列表
        /// </summary>
        public ICollection<DeliveryTask>? DeliveryTasks { get; set; }
    }
}
