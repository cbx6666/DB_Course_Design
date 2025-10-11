using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 更新配送员资料请求
    /// </summary>
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "姓名不能为空")]
        [StringLength(15, ErrorMessage = "姓名长度不能超过15个字符")] 
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = null!;

        [StringLength(2, ErrorMessage = "性别代码长度不能超过2个字符")] 
        /// <summary>
        /// 性别（代码形式）
        /// </summary>
        public string? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        [StringLength(1000)]
        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        [Required(ErrorMessage = "车辆类型不能为空")]
        [StringLength(20)] 
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string VehicleType { get; set; } = null!;
    }
}