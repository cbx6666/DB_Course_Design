namespace BackEnd.DTOs.Dish
{
    /// <summary>
    /// 图片上传结果DTO
    /// </summary>
    public class UploadImageResultDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}
