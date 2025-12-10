using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Enums;
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

        /// <summary>
        /// 根据用户ID获取店铺举报列表（包含店铺信息）
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <returns>店铺举报列表</returns>
        public async Task<List<StoreViolationPenalty>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.StoreViolationPenalties
                .Include(p => p.Store)
                .Where(p => p.CustomerID == customerId)
                .OrderByDescending(p => p.PenaltyTime)
                .ToListAsync();
        }

        /// <summary>
        /// 根据管理员ID获取违规处罚列表（包含店铺信息）
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        public async Task<List<StoreViolationPenalty>> GetByAdminIdAsync(int adminId)
        {
            return await _context.Supervise_s
                .Where(s => s.AdminID == adminId)
                .Include(s => s.Penalty)
                    .ThenInclude(p => p.Store)
                .Select(s => s.Penalty)
                .ToListAsync();
        }

        /// <summary>
        /// 根据用户ID和店铺ID获取未完成的举报列表
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>未完成的举报列表</returns>
        public async Task<List<StoreViolationPenalty>> GetPendingByCustomerIdAndStoreIdAsync(int customerId, int storeId)
        {
            return await _context.StoreViolationPenalties
                .Where(p => p.CustomerID == customerId 
                    && p.StoreID == storeId 
                    && p.ViolationPenaltyState != ViolationPenaltyState.Completed)
                .ToListAsync();
        }

        /// <summary>
        /// 根据商家ID获取近三个月的违规处罚列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>违规处罚列表</returns>
        public async Task<List<StoreViolationPenalty>> GetRecentPenaltiesAsync(int sellerId)
        {
            return await _context.StoreViolationPenalties
                .Include(p => p.Store)
                .Where(p => p.Store.SellerID == sellerId 
                    && p.PenaltyTime >= DateTime.Now.AddMonths(-3) 
                    && p.ViolationPenaltyState == ViolationPenaltyState.Completed)
                .OrderByDescending(p => p.PenaltyTime)
                .ToListAsync();
        }
    }
}