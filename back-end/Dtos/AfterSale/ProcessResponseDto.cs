namespace BackEnd.DTOs.AfterSale
{
    /// <summary>
    /// 售后处理响应
    /// </summary>
    public class ProcessResponseDto
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