using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class FavoritesFolder
    {
        // �ղؼ���
        // ���룺FolderID
        // ���룺CustomerID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FolderID { get; set; }

        [Required]
        [StringLength(50)]
        public string FolderName { get; set; } = null!;

        [Required]
        public int CustomerID { get; set; }
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; } = null!;

        // һ�Զർ������
        public ICollection<FavoriteItem>? FavoriteItems { get; set; }
    }
}
