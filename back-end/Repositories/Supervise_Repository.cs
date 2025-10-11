using BackEnd.Models;
using BackEnd.Data;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 管理员监督-处罚关系仓储
    /// </summary>
    public class Supervise_Repository : ISupervise_Repository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Supervise_Repository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有监督-处罚关系
        /// </summary>
        /// <returns>监督-处罚关系列表</returns>
        public async Task<IEnumerable<Supervise_>> GetAllAsync()
        {
            return await _context.Supervise_s
                                 .Include(s => s.Admin)
                                 .Include(s => s.Penalty)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据管理员ID与处罚ID获取关系
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="penaltyId">处罚ID</param>
        /// <returns>监督-处罚关系</returns>
        public async Task<Supervise_?> GetByIdAsync(int adminId, int penaltyId)
        {
            return await _context.Supervise_s
                                 .Include(s => s.Admin)
                                 .Include(s => s.Penalty)
                                 .FirstOrDefaultAsync(s => s.AdminID == adminId && s.PenaltyID == penaltyId);
        }

        /// <summary>
        /// 新增监督-处罚关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Supervise_ supervise_)
        {
            await _context.Supervise_s.AddAsync(supervise_);
            await SaveAsync();
        }

        /// <summary>
        /// 更新监督-处罚关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Supervise_ supervise_)
        {
            _context.Supervise_s.Update(supervise_);
            await SaveAsync();
        }

        /// <summary>
        /// 删除监督-处罚关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Supervise_ supervise_)
        {
            _context.Supervise_s.Remove(supervise_);
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