namespace BackEnd.DTOs.Review
{
    /// <summary>
    /// 回复响应
    /// </summary>
    public class ReplyResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = null!;
    }
}