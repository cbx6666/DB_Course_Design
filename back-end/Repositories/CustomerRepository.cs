using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 客户仓储
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有客户
        /// </summary>
        /// <returns>客户列表</returns>
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                                 .Include(c => c.DeliveryTasks)
                                 .Include(c => c.FoodOrders)
                                 .Include(c => c.Coupons)
                                 .Include(c => c.FavoritesFolders)
                                 .Include(c => c.Comments)
                                 .Include(c => c.ShoppingCarts)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取客户
        /// </summary>
        /// <param name="id">客户ID</param>
        /// <returns>客户</returns>
        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers
                                 .Include(c => c.DeliveryTasks)
                                 .Include(c => c.FoodOrders)
                                 .Include(c => c.Coupons)
                                 .Include(c => c.FavoritesFolders)
                                 .Include(c => c.Comments)
                                 .Include(c => c.ShoppingCarts)
                                 .FirstOrDefaultAsync(c => c.UserID == id);
        }

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await SaveAsync();
        }

        /// <summary>
        /// 更新客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await SaveAsync();
        }

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Customer customer)
        {
            _context.Customers.Remove(customer);
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