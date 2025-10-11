using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 更新店铺字段请求
    /// </summary>
    public class UpdateShopFieldRequestDto
    {
        [Required]
        /// <summary>
        /// 字段名（'Address'/'address' | 'OpenTime'/'openTime'/'startTime' | 'CloseTime'/'closeTime'/'endTime' | 'Feature'/'feature'）
        /// </summary>
        public string Field { get; set; } = string.Empty;
        
        [Required]
        /// <summary>
        /// 字段值
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
} 