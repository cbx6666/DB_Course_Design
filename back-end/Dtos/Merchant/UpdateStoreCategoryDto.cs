using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 更新店铺种类请求DTO
    /// </summary>
    public class UpdateStoreCategoryDto
    {
        /// <summary>
        /// 店铺种类显示名称
        /// </summary>
        [Required(ErrorMessage = "店铺种类不能为空")]
        public string Category { get; set; } = string.Empty;
    }
}
