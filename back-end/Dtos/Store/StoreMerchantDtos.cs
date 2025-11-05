using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Store
{
    /// <summary>
    /// 店铺详细信息（商家端“店铺信息”）
    /// </summary>
    public class ShopInfoResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CreateTime { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string StartTime { get; set; } = string.Empty;
        public string EndTime { get; set; } = string.Empty;
        public string Feature { get; set; } = string.Empty;
        public int? CreditScore { get; set; }
        public string? StoreImage { get; set; }
        public string Category { get; set; } = string.Empty;
    }

    /// <summary>
    /// 店铺概况（商家端仪表盘/概览）
    /// </summary>
    public class ShopOverviewResponseDto
    {
        public decimal Rating { get; set; }
        public int MonthlySales { get; set; }
        public bool IsOpen { get; set; }
        public int CreditScore { get; set; }
    }

    /// <summary>
    /// 切换营业状态请求（开/关店）
    /// </summary>
    public class ToggleBusinessStatusRequestDto
    {
        [Required]
        public bool IsOpen { get; set; }
    }

    /// <summary>
    /// 更新店铺字段请求（地址/营业时间/特色）
    /// </summary>
    public class UpdateShopFieldRequestDto
    {
        [Required]
        public string Field { get; set; } = string.Empty;
        [Required]
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新店铺种类请求（类目）
    /// </summary>
    public class UpdateStoreCategoryDto
    {
        [Required(ErrorMessage = "店铺种类不能为空")]
        public string Category { get; set; } = string.Empty;
    }
}


