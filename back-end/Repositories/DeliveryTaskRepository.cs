using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 配送任务仓储
    /// </summary>
    public class DeliveryTaskRepository : IDeliveryTaskRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public DeliveryTaskRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有配送任务
        /// </summary>
        /// <returns>配送任务列表</returns>
        public async Task<IEnumerable<DeliveryTask>> GetAllAsync()
        {
            // 预加载关联的 Order、Customer 和 Store 数据
            return await _context.DeliveryTasks
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.Customer)
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.Store)
                                 .Include(dt => dt.Courier)
                                 .Include(dt => dt.DeliveryComplaints)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取配送任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>配送任务</returns>
        public async Task<DeliveryTask?> GetByIdAsync(int id)
        {
            // 对于单个查询，同样建议预加载关联数据
            return await _context.DeliveryTasks
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.Customer)
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.Store)
                                 .Include(dt => dt.Courier)
                                 .Include(dt => dt.DeliveryComplaints)
                                 .FirstOrDefaultAsync(dt => dt.TaskID == id);
        }

        /// <summary>
        /// 根据订单ID获取配送任务
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送任务</returns>
        public async Task<DeliveryTask?> GetByOrderIdAsync(int orderId)
        {
            return await _context.DeliveryTasks
                                 .Include(dt => dt.Order)
                                    .ThenInclude(o => o.Customer)
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.Store)
                                 .Include(dt => dt.Order)
                                 .ThenInclude(o => o.DeliveryInfo)
                                 .Include(dt => dt.Courier)
                                    .ThenInclude(c => c!.User)
                                 .Include(dt => dt.DeliveryComplaints)
                                 .FirstOrDefaultAsync(dt => dt.OrderID == orderId);
        }

        /// <summary>
        /// 添加配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        public async Task AddAsync(DeliveryTask task)
        {
            await _context.DeliveryTasks.AddAsync(task);
            await SaveAsync();
        }

        /// <summary>
        /// 更新配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(DeliveryTask task)
        {
            _context.DeliveryTasks.Update(task);
            await SaveAsync();
        }

        /// <summary>
        /// 删除配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(DeliveryTask task)
        {
            _context.DeliveryTasks.Remove(task);
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
        /// 获取可查询的配送任务
        /// </summary>
        /// <returns>可查询的配送任务</returns>
        public IQueryable<DeliveryTask> GetQueryable()
        {
            return _context.DeliveryTasks.AsQueryable();
        }

        /// <summary>
        /// 根据骑手ID和日期范围获取已完成的配送任务
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <returns>配送任务列表</returns>
        public async Task<List<DeliveryTask>> GetCompletedTasksByCourierIdAndDateRangeAsync(int courierId, DateTime startDate, DateTime endDate)
        {
            return await _context.DeliveryTasks
                .Where(dt => dt.CourierID == courierId
                    && dt.Status == DeliveryStatus.Completed
                    && dt.CompletionTime.HasValue
                    && dt.CompletionTime.Value >= startDate
                    && dt.CompletionTime.Value < endDate)
                .ToListAsync();
        }

        /// <summary>
        /// 根据骑手ID获取所有已完成的配送任务
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <returns>配送任务列表</returns>
        public async Task<List<DeliveryTask>> GetCompletedTasksByCourierIdAsync(int courierId)
        {
            return await _context.DeliveryTasks
                .Where(dt => dt.CourierID == courierId
                    && dt.Status == DeliveryStatus.Completed)
                .ToListAsync();
        }
    }
}