namespace BackEnd.DTOs.MerchantInfo
{
    /// <summary>
    /// 商家更新结果
    /// </summary>
    public class MerchantUpdateResultDto
    {
        /// <summary>
        /// 更新的字段
        /// </summary>
        public string[] UpdatedFields { get; set; } = Array.Empty<string>();
        /// <summary>
        /// 更新时间
        /// </summary>
        public string UpdateTime { get; set; } = null!;
    }
}