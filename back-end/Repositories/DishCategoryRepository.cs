using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 菜品种类仓储
    /// </summary>
    public class DishCategoryRepository : IDishCategoryRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public DishCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有菜品种类
        /// </summary>
        /// <returns>菜品种类列表</returns>
        public async Task<IEnumerable<DishCategory>> GetAllAsync()
        {
            return await _context.DishCategories
                .Include(dc => dc.Dishes)
                .Include(dc => dc.MenuDishCategories)
                    .ThenInclude(mdc => mdc.Menu)
                .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取菜品种类
        /// </summary>
        /// <param name="id">菜品种类ID</param>
        /// <returns>菜品种类</returns>
        public async Task<DishCategory?> GetByIdAsync(int id)
        {
            return await _context.DishCategories
                .Include(dc => dc.Dishes)
                .Include(dc => dc.MenuDishCategories)
                    .ThenInclude(mdc => mdc.Menu)
                .FirstOrDefaultAsync(dc => dc.CategoryID == id);
        }

        /// <summary>
        /// 根据菜单ID获取菜品种类列表
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜品种类列表</returns>
        public async Task<IEnumerable<DishCategory>> GetByMenuIdAsync(int menuId)
        {
            return await _context.Menu_DishCategories
                .Where(mdc => mdc.MenuID == menuId)
                .Include(mdc => mdc.DishCategory)
                    .ThenInclude(dc => dc.Dishes)
                .Select(mdc => mdc.DishCategory)
                .ToListAsync();
        }

        /// <summary>
        /// 添加菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        public async Task AddAsync(DishCategory dishCategory)
        {
            await _context.DishCategories.AddAsync(dishCategory);
            await SaveAsync();
        }

        /// <summary>
        /// 更新菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(DishCategory dishCategory)
        {
            _context.DishCategories.Update(dishCategory);
            await SaveAsync();
        }

        /// <summary>
        /// 删除菜品种类
        /// </summary>
        /// <param name="dishCategory">菜品种类</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(DishCategory dishCategory)
        {
            _context.DishCategories.Remove(dishCategory);
            await SaveAsync();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 添加菜品种类到菜单的关联
        /// </summary>
        /// <param name="menuDishCategory">菜单菜品种类关联</param>
        /// <returns>任务</returns>
        public async Task AddMenuDishCategoryAsync(Menu_DishCategory menuDishCategory)
        {
            await _context.Menu_DishCategories.AddAsync(menuDishCategory);
            await SaveAsync();
        }

        /// <summary>
        /// 移除菜品种类从菜单的关联
        /// </summary>
        /// <param name="menuDishCategory">菜单菜品种类关联</param>
        /// <returns>任务</returns>
        public async Task RemoveMenuDishCategoryAsync(Menu_DishCategory menuDishCategory)
        {
            _context.Menu_DishCategories.Remove(menuDishCategory);
            await SaveAsync();
        }

        /// <summary>
        /// 获取菜单菜品种类关联
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜单菜品种类关联</returns>
        public async Task<Menu_DishCategory?> GetMenuDishCategoryAsync(int menuId, int categoryId)
        {
            return await _context.Menu_DishCategories
                .FirstOrDefaultAsync(mdc => mdc.MenuID == menuId && mdc.CategoryID == categoryId);
        }

        /// <summary>
        /// 获取菜单的所有菜品种类关联
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜单菜品种类关联列表</returns>
        public async Task<IEnumerable<Menu_DishCategory>> GetMenuDishCategoriesByMenuIdAsync(int menuId)
        {
            return await _context.Menu_DishCategories
                .Where(mdc => mdc.MenuID == menuId)
                .Include(mdc => mdc.DishCategory)
                    .ThenInclude(dc => dc.Dishes)
                .ToListAsync();
        }
    }
}
