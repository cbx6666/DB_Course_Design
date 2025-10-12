using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 菜单仓储
    /// </summary>
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns>菜单列表</returns>
        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            return await _context.Menus
                                 .Include(m => m.Store)                  // 关联商店
                                 .Include(m => m.MenuDishCategories)     // 菜品种类关联
                                     .ThenInclude(mdc => mdc.DishCategory) // 菜品种类
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单</returns>
        public async Task<Menu?> GetByIdAsync(int id)
        {
            return await _context.Menus
                                 .Include(m => m.Store)
                                 .Include(m => m.MenuDishCategories)
                                     .ThenInclude(mdc => mdc.DishCategory)
                                 .FirstOrDefaultAsync(m => m.MenuID == id);
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Menu menu)
        {
            await _context.Menus.AddAsync(menu);
            await SaveAsync();
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Menu menu)
        {
            _context.Menus.Update(menu);
            await SaveAsync();
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Menu menu)
        {
            _context.Menus.Remove(menu);
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
    }
}