using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 收货信息模型
    /// </summary>
    public class DeliveryInfo
    {
        /// <summary>
        /// 收货信息ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeliveryInfoID { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Address { get; set; } = null!;

        /// <summary>
        /// 收货电话
        /// </summary>
        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = null!;

        /// <summary>
        /// 收货人称呼
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 性别
        /// </summary>
        [StringLength(10)]
        public string? Gender { get; set; }

        /// <summary>
        /// 是否默认地址 (0=否, 1=是)
        /// </summary>
        public int IsDefault { get; set; } = 0;

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
    }
}
