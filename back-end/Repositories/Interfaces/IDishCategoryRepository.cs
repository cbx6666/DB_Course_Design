using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 菜品种类仓储接口
    /// </summary>
    public interface IDishCategoryRepository
    {
        /// <summary>
        /// 获取所有菜品种类
        /// </summary>
        /// <returns>菜品种类列表</returns>
        Task<IEnumerable<DishCategory>> GetAllAsync();

        /// <summary>
        /// 根据ID获取菜品种类
        /// </summary>
        /// <param name="id">菜品种类ID</param>
        /// <returns>菜品种类</returns>
        Task<DishCategory?> GetByIdAsync(int id);

        /// <summary>
        /// 根据菜单ID获取菜品种类列表
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜品种类列表</returns>
        Task<IEnumerable<DishCategory>> GetByMenuIdAsync(int menuId);

        /// <summary>
        /// 添加菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        Task AddAsync(DishCategory dishCategory);

        /// <summary>
        /// 更新菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        Task UpdateAsync(DishCategory dishCategory);

        /// <summary>
        /// 删除菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        Task DeleteAsync(DishCategory dishCategory);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();

        /// <summary>
        /// 添加菜品种类到菜单的关联
        /// </summary>
        /// <param name="menuDishCategory">菜单菜品种类关联</param>
        /// <returns>任务</returns>
        Task AddMenuDishCategoryAsync(Menu_DishCategory menuDishCategory);

        /// <summary>
        /// 移除菜品种类从菜单的关联
        /// </summary>
        /// <param name="menuDishCategory">菜单菜品种类关联</param>
        /// <returns>任务</returns>
        Task RemoveMenuDishCategoryAsync(Menu_DishCategory menuDishCategory);

        /// <summary>
        /// 获取菜单菜品种类关联
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜单菜品种类关联</returns>
        Task<Menu_DishCategory?> GetMenuDishCategoryAsync(int menuId, int categoryId);

        /// <summary>
        /// 获取菜单的所有菜品种类关联
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜单菜品种类关联列表</returns>
        Task<IEnumerable<Menu_DishCategory>> GetMenuDishCategoriesByMenuIdAsync(int menuId);
    }
}
