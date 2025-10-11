using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;

namespace BackEnd.Models
{
    /// <summary>
    /// 评论模型
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// 评论ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CommentID { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = null!;

        /// <summary>
        /// 发布时间
        /// </summary>
        [Required]
        public DateTime PostedAt { get; set; }

        /// <summary>
        /// 点赞数
        /// </summary>
        public int Likes { get; set; } = 0;

        /// <summary>
        /// 回复数
        /// </summary>
        public int Replies { get; set; } = 0;

        /// <summary>
        /// 评分（1-5分）
        /// </summary>
        [Range(1, 5)]
        public int? Rating { get; set; }

        /// <summary>
        /// 评论图片URL
        /// </summary>
        [StringLength(1000)]
        public string? CommentImage { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        [Required]
        public CommentType CommentType { get; set; }

        /// <summary>
        /// 评论状态
        /// </summary>
        [Required]
        public CommentState CommentState { get; set; } = CommentState.Pending;

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        public int? StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store? Store { get; set; }

        /// <summary>
        /// 订单ID（外键）
        /// </summary>
        public int? FoodOrderID { get; set; }

        /// <summary>
        /// 关联的订单
        /// </summary>
        [ForeignKey("FoodOrderID")]
        public FoodOrder? FoodOrder { get; set; }

        /// <summary>
        /// 回复的评论ID（外键）
        /// </summary>
        public int? ReplyToCommentID { get; set; }

        /// <summary>
        /// 回复的评论
        /// </summary>
        [ForeignKey("ReplyToCommentID")]
        public Comment? ReplyToComment { get; set; }

        /// <summary>
        /// 评论者ID（外键）
        /// </summary>
        [Required]
        public int CommenterID { get; set; }

        /// <summary>
        /// 评论者
        /// </summary>
        [ForeignKey("CommenterID")]
        public Customer Commenter { get; set; } = null!;

        /// <summary>
        /// 评论回复集合
        /// </summary>
        public ICollection<Comment>? CommentReplies { get; set; }

        /// <summary>
        /// 评论审核记录集合
        /// </summary>
        public ICollection<Review_Comment> ReviewComments { get; set; } = new List<Review_Comment>();
    }
}
