using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 店铺违规处罚仓储
    /// </summary>
    public class StoreViolationPenaltyRepository : IStoreViolationPenaltyRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public StoreViolationPenaltyRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有违规处罚
        /// </summary>
        /// <returns>违规处罚列表</returns>
        public async Task<IEnumerable<StoreViolationPenalty>> GetAllAsync()
        {
            return await _context.StoreViolationPenalties
                                 .Include(p => p.Store)
                                 .Include(p => p.Supervise_s)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取违规处罚
        /// </summary>
        /// <param name="id">处罚ID</param>
        /// <returns>违规处罚</returns>
        public async Task<StoreViolationPenalty?> GetByIdAsync(int id)
        {
            return await _context.StoreViolationPenalties
                                 .Include(p => p.Store)
                                 .Include(p => p.Supervise_s)
                                 .FirstOrDefaultAsync(p => p.PenaltyID == id);
        }

        /// <summary>
        /// 根据商家ID获取违规处罚
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>违规处罚列表</returns>
        public async Task<IEnumerable<StoreViolationPenalty>> GetBySellerIdAsync(int sellerId)
        {
            return await _context.StoreViolationPenalties
                                 .Include(p => p.Store)
                                     .ThenInclude(s => s.Seller)
                                 .Where(p => p.Store.SellerID == sellerId)
                                 .OrderBy(p => p.PenaltyID)
                                 .ToListAsync();
        }

        /// <summary>
        /// 添加违规处罚
        /// </summary>
        /// <param name="storeViolationPenalty">违规处罚</param>
        /// <returns>任务</returns>
        public async Task AddAsync(StoreViolationPenalty storeViolationPenalty)
        {
            await _context.StoreViolationPenalties.AddAsync(storeViolationPenalty);
            await SaveAsync();
        }

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="storeViolationPenalty">违规处罚</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(StoreViolationPenalty storeViolationPenalty)
        {
            _context.StoreViolationPenalties.Update(storeViolationPenalty);
            await SaveAsync();
        }

        /// <summary>
        /// 删除违规处罚
        /// </summary>
        /// <param name="storeViolationPenalty">违规处罚</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(StoreViolationPenalty storeViolationPenalty)
        {
            _context.StoreViolationPenalties.Remove(storeViolationPenalty);
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