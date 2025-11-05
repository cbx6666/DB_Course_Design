using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Menu
{
    /// <summary>
    /// 菜单响应数据传输对象（菜品列表）
    /// </summary>
    public class MenuResponseDto
    {
        /// <summary>
        /// 菜品ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 菜品描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品图片
        /// </summary>
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// 是否售罄
        /// </summary>
        public int IsSoldOut { get; set; }

        /// <summary>
        /// 菜品种类ID
        /// </summary>
        public int? CategoryId { get; set; }
    }
    
    /// <summary>
    /// 菜单数据传输对象
    /// </summary>
    public class MenuDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// 是否为当前活跃菜单
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;

        /// <summary>
        /// 菜品数量
        /// </summary>
        public int? DishCount { get; set; }
    }
}
