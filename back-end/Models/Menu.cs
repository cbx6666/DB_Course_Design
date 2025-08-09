using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class Menu
    {
        // �˵���
        // ���룺MenuID
        // ���룺StoreID

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuID { get; set; }

        [Required]
        [StringLength(50)]
        public string Version { get; set; } = null!;

        [Required]
        public DateTime ActivePeriod { get; set; }

        [Required]
        public int StoreID { get; set; }
        [ForeignKey("StoreID")]
        public Store Store { get; set; } = null!;

        // ��Զ��ϵ
        // һ���˵����������Ʒ
        public ICollection<Menu_Dish> MenuDishes { get; set; } = new List<Menu_Dish>();

        // ������ԣ�ֱ�ӻ�ȡ��Ʒ�б�
        [NotMapped]
        public IEnumerable<Dish> Dishes => MenuDishes.Select(md => md.Dish);
    }
}
