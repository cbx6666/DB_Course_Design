using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 菜单-菜品关系仓储接口
    /// </summary>
    public interface IMenu_DishRepository
    {
        /// <summary>
        /// 获取所有菜单-菜品关系
        /// </summary>
        /// <returns>关系列表</returns>
        Task<IEnumerable<Menu_Dish>> GetAllAsync();

        /// <summary>
        /// 根据菜单与菜品ID获取关系
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>关系实体</returns>
        Task<Menu_Dish?> GetByIdAsync(int menuId, int dishId);

        /// <summary>
        /// 新增关系
        /// </summary>
        /// <param name="menu_dish">关系实体</param>
        /// <returns>任务</returns>
        Task AddAsync(Menu_Dish menu_dish);

        /// <summary>
        /// 更新关系
        /// </summary>
        /// <param name="menu_dish">关系实体</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Menu_Dish menu_dish);

        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="menu_dish">关系实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Menu_Dish menu_dish);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}