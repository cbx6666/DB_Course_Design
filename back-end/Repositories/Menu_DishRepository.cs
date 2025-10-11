using BackEnd.Models;
using BackEnd.Data;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 菜单-菜品关系仓储
    /// </summary>
    public class Menu_DishRepository : IMenu_DishRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Menu_DishRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有菜单-菜品关系
        /// </summary>
        /// <returns>菜单-菜品关系列表</returns>
        public async Task<IEnumerable<Menu_Dish>> GetAllAsync()
        {
            return await _context.Menu_Dishes
                                 .Include(md => md.Menu)  // 关联菜单
                                 .Include(md => md.Dish)  // 关联菜品
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据菜单ID与菜品ID获取关系
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="dishId">菜品ID</param>
        /// <returns>菜单-菜品关系</returns>
        public async Task<Menu_Dish?> GetByIdAsync(int menuId, int dishId)
        {
            return await _context.Menu_Dishes
                                 .Include(md => md.Menu)
                                 .Include(md => md.Dish)
                                 .FirstOrDefaultAsync(md => md.MenuID == menuId && md.DishID == dishId);
        }

        /// <summary>
        /// 新增菜单-菜品关系
        /// </summary>
        /// <param name="menuDish">关系实体</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Menu_Dish menuDish)
        {
            await _context.Menu_Dishes.AddAsync(menuDish);
            await SaveAsync();
        }

        /// <summary>
        /// 更新菜单-菜品关系
        /// </summary>
        /// <param name="menuDish">关系实体</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Menu_Dish menuDish)
        {
            _context.Menu_Dishes.Update(menuDish);
            await SaveAsync();
        }

        /// <summary>
        /// 删除菜单-菜品关系
        /// </summary>
        /// <param name="menuDish">关系实体</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Menu_Dish menuDish)
        {
            _context.Menu_Dishes.Remove(menuDish);
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