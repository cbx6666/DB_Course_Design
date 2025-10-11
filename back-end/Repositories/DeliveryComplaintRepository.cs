using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 配送投诉仓储
    /// </summary>
    public class DeliveryComplaintRepository : IDeliveryComplaintRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public DeliveryComplaintRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有配送投诉
        /// </summary>
        /// <returns>配送投诉列表</returns>
        public async Task<IEnumerable<DeliveryComplaint>> GetAllAsync()
        {
            // 预加载所有关联的实体数据
            return await _context.DeliveryComplaints
                                 .Include(dc => dc.Courier)
                                 .Include(dc => dc.Customer)
                                 .Include(dc => dc.DeliveryTask)
                                 .Include(dc => dc.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Admin)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取配送投诉
        /// </summary>
        /// <param name="id">投诉ID</param>
        /// <returns>配送投诉</returns>
        public async Task<DeliveryComplaint?> GetByIdAsync(int id)
        {
            // 对于单个查询，同样建议预加载关联数据
            return await _context.DeliveryComplaints
                                 .Include(dc => dc.Courier)
                                 .Include(dc => dc.Customer)
                                 .Include(dc => dc.DeliveryTask)
                                 .Include(dc => dc.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Admin)
                                 .FirstOrDefaultAsync(dc => dc.ComplaintID == id);
        }

        /// <summary>
        /// 根据配送任务ID获取配送投诉
        /// </summary>
        /// <param name="deliveryTaskId">配送任务ID</param>
        /// <returns>配送投诉列表</returns>
        public async Task<IEnumerable<DeliveryComplaint>> GetByDeliveryTaskIdAsync(int deliveryTaskId)
        {
            return await _context.DeliveryComplaints
                .Where(c => c.DeliveryTaskID == deliveryTaskId)
                .Include(c => c.Customer)
                .Include(c => c.Courier)
                .Include(c => c.DeliveryTask)
                .OrderByDescending(c => c.ComplaintTime)
                .ToListAsync();
        }

        /// <summary>
        /// 添加配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        public async Task AddAsync(DeliveryComplaint complaint)
        {
            await _context.DeliveryComplaints.AddAsync(complaint);
            await SaveAsync();
        }

        /// <summary>
        /// 更新配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(DeliveryComplaint complaint)
        {
            _context.DeliveryComplaints.Update(complaint);
            await SaveAsync();
        }

        /// <summary>
        /// 删除配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(DeliveryComplaint complaint)
        {
            _context.DeliveryComplaints.Remove(complaint);
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