using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 购物车项仓储
    /// </summary>
    public class ShoppingCartItemRepository : IShoppingCartItemRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public ShoppingCartItemRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有购物车项
        /// </summary>
        /// <returns>购物车项列表</returns>
        public async Task<IEnumerable<ShoppingCartItem>> GetAllAsync()
        {
            return await _context.ShoppingCartItems
                                 .Include(sci => sci.Cart)
                                 .Include(sci => sci.Dish)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取购物车项
        /// </summary>
        /// <param name="id">购物车项ID</param>
        /// <returns>购物车项</returns>
        public async Task<ShoppingCartItem?> GetByIdAsync(int id)
        {
            return await _context.ShoppingCartItems
                                 .Include(sci => sci.Cart)
                                 .Include(sci => sci.Dish)
                                 .FirstOrDefaultAsync(sci => sci.ItemID == id);
        }

        /// <summary>
        /// 根据购物车ID获取购物车项
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车项列表</returns>
        public async Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId)
        {
            return await _context.ShoppingCartItems
                                .Include(sci => sci.Cart)
                                .Include(sci => sci.Dish)
                                .Where(sci => sci.CartID == cartId)
                                .ToListAsync();
        }

        /// <summary>
        /// 添加购物车项
        /// </summary>
        /// <param name="shoppingCartItem">购物车项</param>
        /// <returns>任务</returns>
        public async Task AddAsync(ShoppingCartItem shoppingCartItem)
        {
            await _context.ShoppingCartItems.AddAsync(shoppingCartItem);
            await SaveAsync();
        }

        /// <summary>
        /// 更新购物车项
        /// </summary>
        /// <param name="shoppingCartItem">购物车项</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(ShoppingCartItem shoppingCartItem)
        {
            _context.ShoppingCartItems.Update(shoppingCartItem);
            await SaveAsync();
        }

        /// <summary>
        /// 删除购物车项
        /// </summary>
        /// <param name="shoppingCartItem">购物车项</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(ShoppingCartItem shoppingCartItem)
        {
            _context.ShoppingCartItems.Remove(shoppingCartItem);
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