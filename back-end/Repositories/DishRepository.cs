using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 菜品仓储
    /// </summary>
    public class DishRepository : IDishRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public DishRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有菜品
        /// </summary>
        /// <returns>菜品列表</returns>
        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _context.Dishes
                                 .Include(d => d.ShoppingCartItems) // 一对多：购物车项
                                 .Include(d => d.DishCategory)      // 多对一：菜品种类
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据菜品种类ID获取菜品列表
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
        public async Task<IEnumerable<Dish>> GetByCategoryIdAsync(int categoryId)
        {
            return await _context.Dishes
                                 .Include(d => d.ShoppingCartItems) // 一对多：购物车项
                                 .Include(d => d.DishCategory)      // 多对一：菜品种类
                                 .Where(d => d.CategoryID == categoryId)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取菜品
        /// </summary>
        /// <param name="id">菜品ID</param>
        /// <returns>菜品</returns>
        public async Task<Dish?> GetByIdAsync(int id)
        {
            return await _context.Dishes
                                 .Include(d => d.ShoppingCartItems)
                                 .Include(d => d.DishCategory)
                                 .FirstOrDefaultAsync(d => d.DishID == id);
        }

        /// <summary>
        /// 添加菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            await SaveAsync();
        }

        /// <summary>
        /// 更新菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            await SaveAsync();
        }

        /// <summary>
        /// 删除菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Dish dish)
        {
            _context.Dishes.Remove(dish);
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