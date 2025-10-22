using BackEnd.DTOs.Menu;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 菜单服务实现
    /// </summary>
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IStoreRepository _storeRepository;

        public MenuService(IMenuRepository menuRepository, IStoreRepository storeRepository)
        {
            _menuRepository = menuRepository;
            _storeRepository = storeRepository;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>菜单列表</returns>
        public async Task<MenuListResponseDto> GetMenusBySellerIdAsync(int sellerId)
        {
            var menus = await _menuRepository.GetBySellerIdAsync(sellerId);
            var menuDtos = menus.Select(MapToMenuDto).ToList();

            return new MenuListResponseDto
            {
                List = menuDtos,
                Total = menuDtos.Count
            };
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">创建菜单DTO</param>
        /// <returns>创建的菜单</returns>
        public async Task<MenuDto> CreateMenuAsync(CreateMenuDto dto)
        {
            // 首先通过sellerId找到对应的StoreID
            var store = await _storeRepository.GetBySellerIdAsync(dto.SellerId);

            if (store == null)
            {
                throw new InvalidOperationException($"找不到商家ID {dto.SellerId} 对应的店铺");
            }

            var menu = new Menu
            {
                Name = dto.Name,
                Description = dto.Description,
                StoreID = store.StoreID,
                IsActive = false,
                CreatedAt = DateTime.Now
            };

            await _menuRepository.AddAsync(menu);
            
            // 重新加载菜单以获取关联数据
            var reloadedMenu = await _menuRepository.GetByIdAsync(menu.MenuID);
            return MapToMenuDto(reloadedMenu!);
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="dto">更新菜单DTO</param>
        /// <returns>更新后的菜单</returns>
        public async Task<MenuDto?> UpdateMenuAsync(int menuId, CreateMenuDto dto)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);

            if (menu == null)
                return null;

            menu.Name = dto.Name;
            menu.Description = dto.Description;

            await _menuRepository.UpdateAsync(menu);
            return MapToMenuDto(menu);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            var menu = await _menuRepository.GetByIdAsync(menuId);
            if (menu == null)
                return false;

            await _menuRepository.DeleteAsync(menu);
            return true;
        }

        /// <summary>
        /// 设置菜单为活跃状态
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>是否设置成功</returns>
        public async Task<bool> SetActiveMenuAsync(int menuId, int sellerId)
        {
            try
            {
                // 首先通过sellerId找到对应的StoreID
                var store = await _storeRepository.GetBySellerIdAsync(sellerId);

                if (store == null)
                    return false;

                // 先将该商家的所有菜单设为非活跃状态
                await _menuRepository.SetAllInactiveByStoreIdAsync(store.StoreID);

                // 将指定菜单设为活跃状态
                await _menuRepository.SetActiveAsync(menuId, true);

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 将Menu实体映射为MenuDto
        /// </summary>
        /// <param name="menu">Menu实体</param>
        /// <returns>MenuDto</returns>
        private static MenuDto MapToMenuDto(Menu menu)
        {
            return new MenuDto
            {
                Id = menu.MenuID,
                Name = menu.Name,
                Description = menu.Description,
                IsActive = menu.IsActive,
                CreatedAt = menu.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                DishCount = menu.DishCount
            };
        }
    }
}