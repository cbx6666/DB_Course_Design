using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 投诉评价仓储
    /// </summary>
    public class Evaluate_ComplaintRepository : IEvaluate_ComplaintRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Evaluate_ComplaintRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有投诉评价
        /// </summary>
        /// <returns>投诉评价列表</returns>
        public async Task<IEnumerable<Evaluate_Complaint>> GetAllAsync()
        {
            // 预加载关联的 Admin 和 Complaint 数据
            return await _context.Evaluate_Complaints
                                 .Include(ec => ec.Admin)
                                 .Include(ec => ec.Complaint)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据管理员ID和投诉ID获取投诉评价
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="complaintId">投诉ID</param>
        /// <returns>投诉评价</returns>
        public async Task<Evaluate_Complaint?> GetByIdAsync(int adminId, int complaintId)
        {
            return await _context.Evaluate_Complaints
                                 .Include(ec => ec.Admin)
                                 .Include(ec => ec.Complaint)
                                 .FirstOrDefaultAsync(ec => ec.AdminID == adminId && ec.ComplaintID == complaintId);
        }

        /// <summary>
        /// 添加投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Evaluate_Complaint evaluateComplaint)
        {
            await _context.Evaluate_Complaints.AddAsync(evaluateComplaint);
            await SaveAsync();
        }

        /// <summary>
        /// 更新投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Evaluate_Complaint evaluateComplaint)
        {
            _context.Evaluate_Complaints.Update(evaluateComplaint);
            await SaveAsync();
        }

        /// <summary>
        /// 删除投诉评价
        /// </summary>
        /// <param name="evaluateComplaint">投诉评价</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Evaluate_Complaint evaluateComplaint)
        {
            _context.Evaluate_Complaints.Remove(evaluateComplaint);
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