using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 管理员仓储
    /// </summary>
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns>管理员列表</returns>
        public async Task<IEnumerable<Administrator>> GetAllAsync()
        {
            return await _context.Administrators
                                 .Include(a => a.User)
                                 .Include(a => a.ReviewComments)
                                    .ThenInclude(rc => rc.Comment)
                                 .Include(a => a.Supervise_s)
                                      .ThenInclude(s => s.Penalty)
                                 .Include(a => a.EvaluateAfterSales)
                                      .ThenInclude(eas => eas.Application)
                                 .Include(a => a.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Complaint)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取管理员
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>管理员</returns>
        public async Task<Administrator?> GetByIdAsync(int id)
        {
            return await _context.Administrators
                                 .Include(a => a.User)
                                 .Include(a => a.ReviewComments)
                                    .ThenInclude(rc => rc.Comment)
                                 .Include(a => a.Supervise_s)
                                      .ThenInclude(s => s.Penalty)
                                 .Include(a => a.EvaluateAfterSales)
                                      .ThenInclude(eas => eas.Application)
                                 .Include(a => a.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Complaint)
                                 .FirstOrDefaultAsync(a => a.UserID == id);
        }

        /// <summary>
        /// 根据管理实体获取管理员
        /// </summary>
        /// <param name="managedEntity">管理实体</param>
        /// <returns>管理员列表</returns>
        public async Task<IEnumerable<Administrator>> GetAdministratorsByManagedEntityAsync(string managedEntity)
        {
            return await _context.Administrators
                .Include(a => a.User)
                .Where(a => a.ManagedEntities.Contains(managedEntity))
                .ToListAsync();
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateAdministratorAsync(Administrator administrator)
        {
            try
            {
                _context.Administrators.Update(administrator);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Administrator administrator)
        {
            await _context.Administrators.AddAsync(administrator);
            await SaveAsync();
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Administrator administrator)
        {
            _context.Administrators.Update(administrator);
            await SaveAsync();
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Administrator administrator)
        {
            _context.Administrators.Remove(administrator);
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