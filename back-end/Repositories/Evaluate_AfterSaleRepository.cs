using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 售后评价仓储
    /// </summary>
    public class Evaluate_AfterSaleRepository : IEvaluate_AfterSaleRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Evaluate_AfterSaleRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有售后评价
        /// </summary>
        /// <returns>售后评价列表</returns>
        public async Task<IEnumerable<Evaluate_AfterSale>> GetAllAsync()
        {
            // 预加载关联的 Admin 和 Application 数据
            return await _context.Evaluate_AfterSales
                                 .Include(eas => eas.Admin)
                                 .Include(eas => eas.Application)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据管理员ID和申请ID获取售后评价
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="applicationId">申请ID</param>
        /// <returns>售后评价</returns>
        public async Task<Evaluate_AfterSale?> GetByIdAsync(int adminId, int applicationId)
        {
            return await _context.Evaluate_AfterSales
                                 .Include(eas => eas.Admin)
                                 .Include(eas => eas.Application)
                                 .FirstOrDefaultAsync(eas => eas.AdminID == adminId && eas.ApplicationID == applicationId);
        }

        /// <summary>
        /// 添加售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Evaluate_AfterSale evaluateAfterSale)
        {
            await _context.Evaluate_AfterSales.AddAsync(evaluateAfterSale);
            await SaveAsync();
        }

        /// <summary>
        /// 更新售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Evaluate_AfterSale evaluateAfterSale)
        {
            _context.Evaluate_AfterSales.Update(evaluateAfterSale);
            await SaveAsync();
        }

        /// <summary>
        /// 删除售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Evaluate_AfterSale evaluateAfterSale)
        {
            _context.Evaluate_AfterSales.Remove(evaluateAfterSale);
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