using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    // ����Ա���ۺ�����֮��������ۺ��ϵ
    // ���룺AdminID��Application

    public class Evaluate_AfterSale
    {
        [Key, Column(Order = 0)]
        public int AdminID { get; set; }
        [ForeignKey("AdminID")]
        public Administrator Admin { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int ApplicationID { get; set; }
        [ForeignKey("ApplicationID")]
        public AfterSaleApplication Application { get; set; } = null!;
    }
}
