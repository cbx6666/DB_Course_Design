using BackEnd.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 用户基础信息模型
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Username { get; set; } = null!;

        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        [MaxLength(60)]
        public string Password { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        [Required]
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Email { get; set; } = null!;

        /// <summary>
        /// 性别
        /// </summary>
        [MaxLength(2)]
        public string? Gender { get; set; }

        /// <summary>
        /// 全名
        /// </summary>
        [MaxLength(6)]
        public string? FullName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(1000)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 账户创建时间
        /// </summary>
        [Required]
        public DateTime AccountCreationTime { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        [Required]
        public UserIdentity Role { get; set; } = UserIdentity.Customer;

        /// <summary>
        /// 消费者信息
        /// </summary>
        public Customer? Customer { get; set; }

        /// <summary>
        /// 配送员信息
        /// </summary>
        public Courier? Courier { get; set; }

        /// <summary>
        /// 管理员信息
        /// </summary>
        public Administrator? Administrator { get; set; }

        /// <summary>
        /// 商家信息
        /// </summary>
        public Seller? Seller { get; set; }
    }
}
