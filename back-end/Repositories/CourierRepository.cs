using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 骑手仓储
    /// </summary>
    public class CourierRepository : ICourierRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public CourierRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有骑手
        /// </summary>
        /// <returns>骑手列表</returns>
        public async Task<IEnumerable<Courier>> GetAllAsync()
        {
            return await _context.Couriers
                                 .Include(c => c.User)
                                 .Include(c => c.DeliveryTasks)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取骑手
        /// </summary>
        /// <param name="id">骑手ID</param>
        /// <returns>骑手</returns>
        public async Task<Courier?> GetByIdAsync(int id)
        {
            return await _context.Couriers
                                 .Include(c => c.User)
                                 .Include(c => c.DeliveryTasks)
                                 .FirstOrDefaultAsync(c => c.UserID == id);
        }

        /// <summary>
        /// 添加骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Courier courier)
        {
            await _context.Couriers.AddAsync(courier);
            await SaveAsync();
        }

        /// <summary>
        /// 更新骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Courier courier)
        {
            _context.Couriers.Update(courier);
            await SaveAsync();
        }

        /// <summary>
        /// 删除骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Courier courier)
        {
            _context.Couriers.Remove(courier);
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