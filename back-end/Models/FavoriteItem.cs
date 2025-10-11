using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    /// <summary>
    /// 收藏项模型
    /// </summary>
    public class FavoriteItem
    {
        /// <summary>
        /// 收藏项ID（主键）
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemID { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        [Required]
        public DateTime FavoritedAt { get; set; }

        /// <summary>
        /// 收藏原因
        /// </summary>
        [Required]
        [StringLength(500)]
        public string FavoriteReason { get; set; } = null!;

        /// <summary>
        /// 店铺ID（外键）
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 关联的店铺
        /// </summary>
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        /// <summary>
        /// 收藏夹ID（外键）
        /// </summary>
        [Required]
        public int FolderID { get; set; }

        /// <summary>
        /// 关联的收藏夹
        /// </summary>
        [ForeignKey("FolderID")]
        public FavoritesFolder Folder { get; set; } = null!;
    }
}
