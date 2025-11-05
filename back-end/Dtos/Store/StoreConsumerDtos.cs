using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.DTOs.Store
{
    /// <summary>
    /// 店铺响应（用户端展示）
    /// </summary>
    public class StoreResponseDto
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Image { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Address { get; set; } = string.Empty;
        [Required]
        public TimeSpan OpenTime { get; set; } = TimeSpan.FromHours(9);
        [Required]
        public TimeSpan CloseTime { get; set; } = TimeSpan.FromHours(22);
        public string BusinessHours { get; set; } = string.Empty;
        [Column(TypeName = "decimal(10,2)")]
        public decimal Rating { get; set; } = 0.00m;
        [Required]
        public int MonthlySales { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        [Required]
        public DateTime CreateTime { get; set; }
    }

}
