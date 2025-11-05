using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Comment
{
    // ========== 消费者侧 ==========

    /// <summary>
    /// 评论响应DTO（消费者端展示）
    /// </summary>
    public class CustomerCommentDto
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 评分（1-5）
        /// </summary>
        [Range(1, 5)]
        public int? Rating { get; set; }

        /// <summary>
        /// 评论日期
        /// </summary>
        [Required]
        public DateTime Date { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 评论图片
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();
    }

    /// <summary>
    /// 评论状态统计DTO
    /// </summary>
    public class CommentStateDto
    {
        /// <summary>
        /// 评分统计：[5分数量, 4分数量, 3分数量, 2分数量, 1分数量]
        /// </summary>
        public IEnumerable<int> Status { get; set; } = new List<int>();
    }

    /// <summary>
    /// 创建评论请求DTO（消费者端）
    /// </summary>
    public class CreateCommentDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int StoreId { get; set; }

        /// <summary>
        /// 评分（1-5）
        /// </summary>
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Content { get; set; } = string.Empty;
    }

    // ========== 商家侧 ==========

    /// <summary>
    /// 评论信息DTO（商家端展示）
    /// </summary>
    public class MerchantCommentDto
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; } = null!;

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserProfileDto User { get; set; } = null!;

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;
    }

    /// <summary>
    /// 回复评论请求DTO（商家端）
    /// </summary>
    public class ReplyCommentDto
    {
        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; } = null!;
    }

    // ========== 管理员侧 ==========

    /// <summary>
    /// 评论审核详情DTO（管理员端展示）
    /// </summary>
    public class AdminCommentDetailDto
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public string ReviewId { get; set; } = null!;

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;

        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = null!;

        /// <summary>
        /// 评论图片
        /// </summary>
        public string? Image { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        public string Type { get; set; } = null!;

        /// <summary>
        /// 评分
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        public string SubmitTime { get; set; } = null!;

        /// <summary>
        /// 审核状态
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 更新评论审核状态请求DTO（管理员端）
    /// </summary>
    public class UpdateCommentReviewDto
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public string ReviewId { get; set; } = null!;

        /// <summary>
        /// 审核状态：待处理/已完成/违规
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 更新评论审核状态响应DTO（管理员端）
    /// </summary>
    public class UpdateCommentReviewResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 更新后的评论详情
        /// </summary>
        public AdminCommentDetailDto? Data { get; set; }
    }
}
