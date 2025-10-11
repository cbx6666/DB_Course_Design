using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 购物车仓储
    /// </summary>
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public ShoppingCartRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有购物车
        /// </summary>
        /// <returns>购物车列表</returns>
        public async Task<IEnumerable<ShoppingCart>> GetAllAsync()
        {
            return await _context.ShoppingCarts
                                 .Include(sc => sc.ShoppingCartItems)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart?> GetByIdAsync(int id)
        {
            return await _context.ShoppingCarts
                                 .Include(sc => sc.Order)
                                 .Include(sc => sc.ShoppingCartItems!)
                                     .ThenInclude(sci => sci.Dish)
                                 .Include(sc => sc.Customer)
                                 .Include(sc => sc.Store)
                                 .FirstOrDefaultAsync(sc => sc.CartID == id);
        }

        /// <summary>
        /// 获取指定店铺下用户的活跃购物车
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>购物车</returns>
        public async Task<ShoppingCart?> GetActiveCartWithStoreFilterAsync(int customerId, int storeId)
        {
            return await _context.ShoppingCarts
                .AsNoTracking()
                .Include(c => c.ShoppingCartItems!)
                    .ThenInclude(i => i.Dish)
                .Where(c => c.CustomerID == customerId &&
                        c.ShoppingCartState == ShoppingCartState.Active &&
                        c.StoreID == storeId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>任务</returns>
        public async Task AddAsync(ShoppingCart shoppingCart)
        {
            await _context.ShoppingCarts.AddAsync(shoppingCart);
            await SaveAsync();
        }

        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(ShoppingCart shoppingCart)
        {
            _context.ShoppingCarts.Update(shoppingCart);
            await SaveAsync();
        }

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shoppingCart">购物车</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(ShoppingCart shoppingCart)
        {
            _context.ShoppingCarts.Remove(shoppingCart);
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