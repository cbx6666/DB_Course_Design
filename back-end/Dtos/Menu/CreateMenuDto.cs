using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Menu
{
    /// <summary>
    /// 创建菜单请求DTO
    /// </summary>
    public class CreateMenuDto
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required(ErrorMessage = "菜单名称不能为空")]
        [StringLength(100, ErrorMessage = "菜单名称长度不能超过100个字符")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 菜单描述
        /// </summary>
        [Required(ErrorMessage = "菜单描述不能为空")]
        [StringLength(500, ErrorMessage = "菜单描述长度不能超过500个字符")]
        public string Description { get; set; } = null!;


        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required(ErrorMessage = "店铺ID不能为空")]
        public int SellerId { get; set; }
    }
}
