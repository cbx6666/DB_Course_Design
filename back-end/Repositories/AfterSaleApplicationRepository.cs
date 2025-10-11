using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 售后申请仓储
    /// </summary>
    public class AfterSaleApplicationRepository : IAfterSaleApplicationRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public AfterSaleApplicationRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有售后申请
        /// </summary>
        /// <returns>售后申请列表</returns>
        public async Task<IEnumerable<AfterSaleApplication>> GetAllAsync()
        {
            return await _context.AfterSaleApplications
                                 .Include(a => a.Order)
                                 .Include(a => a.EvaluateAfterSales)
                                     .ThenInclude(eas => eas.Admin)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="id">申请ID</param>
        /// <returns>售后申请</returns>
        public async Task<AfterSaleApplication?> GetByIdAsync(int id)
        {
            return await _context.AfterSaleApplications
                                 .Include(a => a.Order)
                                     .ThenInclude(o => o.Customer)
                                         .ThenInclude(c => c.User)
                                 .Include(a => a.EvaluateAfterSales)
                                     .ThenInclude(eas => eas.Admin)
                                 .FirstOrDefaultAsync(a => a.ApplicationID == id);
        }

        /// <summary>
        /// 根据订单ID获取售后申请
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>售后申请列表</returns>
        public async Task<IEnumerable<AfterSaleApplication>> GetByOrderIdAsync(int orderId)
        {
            return await _context.AfterSaleApplications
                                 .Include(a => a.Order)
                                 .Where(a => a.OrderID == orderId)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据商家ID获取售后申请
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>售后申请列表</returns>
        public async Task<IEnumerable<AfterSaleApplication>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.AfterSaleApplications
                .Include(a => a.Order)
                    .ThenInclude(o => o.Customer)
                        .ThenInclude(c => c.User)
                .Include(a => a.Order)
                    .ThenInclude(o => o.Store)
                        .ThenInclude(s => s.Seller)
                .Where(a => a.Order.Store.SellerID == sellerId)
                .ToListAsync();
        }

        /// <summary>
        /// 添加售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        public async Task AddAsync(AfterSaleApplication application)
        {
            await _context.AfterSaleApplications.AddAsync(application);
        }

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(AfterSaleApplication application)
        {
            _context.AfterSaleApplications.Update(application);
            await SaveAsync();
        }

        /// <summary>
        /// 删除售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(AfterSaleApplication application)
        {
            _context.AfterSaleApplications.Remove(application);
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