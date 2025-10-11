using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 评论审核模型
    /// </summary>
    public class Review_Comment
    {
        /// <summary>
        /// 管理员ID（复合主键）
        /// </summary>
        [Key, Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdminID { get; set; }

        /// <summary>
        /// 关联的管理员
        /// </summary>
        [ForeignKey("AdminID")]
        public Administrator Admin { get; set; } = null!;

        /// <summary>
        /// 评论ID（复合主键）
        /// </summary>
        [Key, Column(Order = 1)]
        public int CommentID { get; set; }

        /// <summary>
        /// 关联的评论
        /// </summary>
        [ForeignKey("CommentID")]
        public Comment Comment { get; set; } = null!;

        /// <summary>
        /// 审核时间
        /// </summary>
        [Required]
        public DateTime ReviewTime { get; set; }
    }
}
