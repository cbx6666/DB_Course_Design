using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using BackEnd.Models.Enums;


namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 店铺请求
    /// </summary>
    public class StoreRequestDto
    {
        [Required]
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 店铺响应
    /// </summary>
    public class StoreResponseDto
    {
        [Required]
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        /// <summary>
        /// 店铺名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// 店铺图片
        /// </summary>
        public string Image { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        /// <summary>
        /// 店铺地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        [Required]
        [JsonIgnore]
        /// <summary>
        /// 开业时间
        /// </summary>
        public TimeSpan OpenTime { get; set; } = TimeSpan.FromHours(9);

        [Required]
        [JsonIgnore]
        /// <summary>
        /// 闭店时间
        /// </summary>
        public TimeSpan CloseTime { get; set; } = TimeSpan.FromHours(22);

        /// <summary>
        /// 营业时间
        /// </summary>
        public string BusinessHours { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10,2)")]
        /// <summary>
        /// 评分
        /// </summary>
        public decimal Rating { get; set; } = 0.00m;

        [Required]
        /// <summary>
        /// 月销量
        /// </summary>
        public int MonthlySales { get; set; }

        [Required]
        [StringLength(500)]
        /// <summary>
        /// 店铺描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 菜单请求
    /// </summary>
    public class MenuRequestDto
    {
        [Required]
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }
    }

    /// <summary>
    /// 菜单响应
    /// </summary>
    public class MenuResponseDto
    {
        [Required]
        /// <summary>
        /// 菜品ID
        /// </summary>
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        /// <summary>
        /// 菜品描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        [Required]
        /// <summary>
        /// 菜品图片
        /// </summary>
        public string Image { get; set; } = string.Empty;

        [Required]
        /// <summary>
        /// 是否售罄
        /// </summary>
        public DishIsSoldOut IsSoldOut { get; set; } = DishIsSoldOut.IsSoldOut;
    }

    /// <summary>
    /// 评论响应
    /// </summary>
    public class CommentResponseDto
    {
        [Required]
        /// <summary>
        /// 评论ID
        /// </summary>
        public int Id { get; set; }

        [Required]
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        [Range(1, 5)]
        /// <summary>
        /// 评分
        /// </summary>
        public int? Rating { get; set; }

        [Required]
        /// <summary>
        /// 评论日期
        /// </summary>
        public DateTime Date { get; set; }

        [Required]
        [StringLength(500)]
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        [MaxLength(255)]
        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 评论图片
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// 评论状态
    /// </summary>
    public class CommentStateDto
    {
        /// <summary>
        /// 评论状态统计（好评数, 中评数, 差评数）
        /// </summary>
        public IEnumerable<int> Status { get; set; } = new List<int>();
    }

    /// <summary>
    /// 创建评论
    /// </summary>
    public class CreateCommentDto
    {
        [Required]
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        [Required]
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "评分必须在 1 到 5 之间")]
        /// <summary>
        /// 评分
        /// </summary>
        public int Rating { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "评论内容不能超过 500 字")]
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// 用户店铺举报
    /// </summary>
    public class UserStoreReportDto
    {
        [Required]
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        [Required]
        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreId { get; set; }

        [Required]
        [MaxLength(500, ErrorMessage = "投诉内容不能超过 500 字")]
        /// <summary>
        /// 投诉内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
