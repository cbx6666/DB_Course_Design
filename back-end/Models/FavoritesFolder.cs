using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 收藏夹模型
    /// </summary>
    public class FavoritesFolder
    {
        /// <summary>
        /// 收藏夹ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FolderID { get; set; }

        /// <summary>
        /// 收藏夹名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string FolderName { get; set; } = null!;

        /// <summary>
        /// 消费者ID（外键）
        /// </summary>
        [Required]
        public int CustomerID { get; set; }

        /// <summary>
        /// 关联的消费者
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        /// <summary>
        /// 收藏项集合
        /// </summary>
        public ICollection<FavoriteItem>? FavoriteItems { get; set; }
    }
}
