using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 商家仓储
    /// </summary>
    public class SellerRepository : ISellerRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public SellerRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有商家
        /// </summary>
        /// <returns>商家列表</returns>
        public async Task<IEnumerable<Seller>> GetAllAsync()
        {
            return await _context.Sellers
                                 .Include(s => s.User)
                                 .Include(s => s.Store)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取商家
        /// </summary>
        /// <param name="id">商家用户ID</param>
        /// <returns>商家</returns>
        public async Task<Seller?> GetByIdAsync(int id)
        {
            return await _context.Sellers
                                 .Include(s => s.User)
                                 .Include(s => s.Store)
                                 .FirstOrDefaultAsync(s => s.UserID == id);
        }

        /// <summary>
        /// 添加商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Seller seller)
        {
            await _context.Sellers.AddAsync(seller);
            await SaveAsync();
        }

        /// <summary>
        /// 更新商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Seller seller)
        {
            _context.Sellers.Update(seller);
            await SaveAsync();
        }

        /// <summary>
        /// 删除商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Seller seller)
        {
            _context.Sellers.Remove(seller);
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