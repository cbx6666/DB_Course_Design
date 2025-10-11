using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 优惠券仓储
    /// </summary>
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有优惠券
        /// </summary>
        /// <returns>优惠券列表</returns>
        public async Task<IEnumerable<Coupon>> GetAllAsync()
        {
            // 预加载所有关联的实体数据
            return await _context.Coupons
                                 .Include(c => c.CouponManager)
                                 .Include(c => c.Order)
                                 .Include(c => c.Customer)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取优惠券
        /// </summary>
        /// <param name="id">优惠券ID</param>
        /// <returns>优惠券</returns>
        public async Task<Coupon?> GetByIdAsync(int id)
        {
            // 对于单个查询，同样建议预加载关联数据
            return await _context.Coupons
                                 .Include(c => c.CouponManager)
                                 .Include(c => c.Order)
                                 .Include(c => c.Customer)
                                 .FirstOrDefaultAsync(c => c.CouponID == id);
        }

        /// <summary>
        /// 根据客户ID获取优惠券
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>优惠券列表</returns>
        public async Task<IEnumerable<Coupon>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Coupons
                                 .Include(c => c.CouponManager)
                                 .Include(c => c.Order)
                                 .Include(c => c.Customer)
                                 .Where(c => c.CustomerID == customerId)
                                 .ToListAsync();
        }

        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Coupon coupon)
        {
            await _context.Coupons.AddAsync(coupon);
        }

        /// <summary>
        /// 更新优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Coupon coupon)
        {
            _context.Coupons.Update(coupon);
            await SaveAsync();
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>任务</returns>
        public Task DeleteAsync(Coupon coupon)
        {
            _context.Coupons.Remove(coupon);
            return Task.CompletedTask;
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