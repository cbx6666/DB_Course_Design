namespace BackEnd.DTOs.Penalty
{
    /// <summary>
    /// 申诉响应
    /// </summary>
    public class AppealResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string? Message { get; set; }
    }
}