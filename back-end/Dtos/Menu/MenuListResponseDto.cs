namespace BackEnd.DTOs.Menu
{
    /// <summary>
    /// 菜单列表响应DTO
    /// </summary>
    public class MenuListResponseDto
    {
        /// <summary>
        /// 菜单列表
        /// </summary>
        public List<MenuDto> List { get; set; } = new List<MenuDto>();

        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }
    }
}
