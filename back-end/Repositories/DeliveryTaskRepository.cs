using BackEnd.Data;
using BackEnd.Models;
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
                                 .Include(dt => dt.Courier)
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
    }
}