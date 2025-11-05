using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.DTOs.Store
{
    /// <summary>
    /// 展示店铺数据传输对象（可复用：推荐、搜索、列表）
    /// </summary>
    public class ShowStoreDto
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Image { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageRating { get; set; } = 0.00m;

        [Required]
        public int MonthlySales { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }
    }
}


