using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 创建骑手评分请求DTO
    /// </summary>
    public class CreateCourierRatingDto
    {
        /// <summary>
        /// 评分（1-5）
        /// </summary>
        [Required(ErrorMessage = "评分不能为空")]
        [Range(1, 5, ErrorMessage = "评分必须在1-5之间")]
        public int Rating { get; set; }

        /// <summary>
        /// 订单ID（可选）
        /// </summary>
        public int? OrderId { get; set; }

        /// <summary>
        /// 配送任务ID（可选）
        /// </summary>
        public int? TaskId { get; set; }
    }
}
