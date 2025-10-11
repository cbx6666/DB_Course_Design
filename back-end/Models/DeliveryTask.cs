using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 配送任务模型
    /// </summary>
    public class DeliveryTask
    {
        /// <summary>
        /// 任务ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TaskID { get; set; }

        /// <summary>
        /// 预计到达时间
        /// </summary>
        [Required]
        public DateTime EstimatedArrivalTime { get; set; }

        /// <summary>
        /// 预计配送时间
        /// </summary>
        [Required]
        public DateTime EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// 发布任务时间
        /// </summary>
        [Required]
        public DateTime PublishTime { get; set; }

        /// <summary>
        /// 接单时间
        /// </summary>
        [Required]
        public DateTime AcceptTime { get; set; }

        /// <summary>
        /// 配送状态
        /// </summary>
        [Required]
        public DeliveryStatus Status { get; set; } = DeliveryStatus.To_Be_Taken;

        /// <summary>
        /// 任务完成时间
        /// </summary>
        public DateTime? CompletionTime { get; set; }

        /// <summary>
        /// 配送费
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal DeliveryFee { get; set; } = 0.00m;

        /// <summary>
        /// 消费者ID（外键）
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的消费者
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 配送员ID（外键）
        /// </summary>
        public int? CourierID { get; set; }

        /// <summary>
        /// 关联的配送员
        /// </summary>
        [ForeignKey("CourierID")]
        public Courier? Courier { get; set; }

        /// <summary>
        /// 订单ID（外键）
        /// </summary>
        [Required]
        public int OrderID { get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("OrderID")]
        public FoodOrder Order { get; set; } = null!;

        /// <summary>
        /// 配送投诉集合
        /// </summary>
        public ICollection<DeliveryComplaint>? DeliveryComplaints { get; set; }
    }
}
