using BackEnd.DTOs.Menu;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 菜单服务接口
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>菜单列表</returns>
        Task<MenuListResponseDto> GetMenusBySellerIdAsync(int sellerId);

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">创建菜单DTO</param>
        /// <returns>创建的菜单</returns>
        Task<MenuDto> CreateMenuAsync(CreateMenuDto dto);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="dto">更新菜单DTO</param>
        /// <returns>更新后的菜单</returns>
        Task<MenuDto?> UpdateMenuAsync(int menuId, CreateMenuDto dto);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteMenuAsync(int menuId);

        /// <summary>
        /// 设置菜单为活跃状态
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>是否设置成功</returns>
        Task<bool> SetActiveMenuAsync(int menuId, int sellerId);
    }
}
