using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 优惠券管理仓储
    /// </summary>
    public class CouponManagerRepository : ICouponManagerRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public CouponManagerRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有优惠券管理
        /// </summary>
        /// <returns>优惠券管理列表</returns>
        public async Task<IEnumerable<CouponManager>> GetAllAsync()
        {
            return await _context.CouponManagers
                .Include(cm => cm.Store)     // 加载关联的店铺
                .Include(cm => cm.Coupons)   // 加载关联的优惠券集合
                .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取优惠券管理
        /// </summary>
        /// <param name="id">优惠券管理ID</param>
        /// <returns>优惠券管理</returns>
        public async Task<CouponManager?> GetByIdAsync(int id)
        {
            return await _context.CouponManagers
                .Include(cm => cm.Store)
                .Include(cm => cm.Coupons)
                .FirstOrDefaultAsync(cm => cm.CouponManagerID == id);
        }

        /// <summary>
        /// 添加优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        public async Task AddAsync(CouponManager couponManager)
        {
            try
            {
                await _context.CouponManagers.AddAsync(couponManager);
                await SaveAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(CouponManager couponManager)
        {
            _context.CouponManagers.Update(couponManager);
            await SaveAsync();
        }

        /// <summary>
        /// 删除优惠券管理
        /// </summary>
        /// <param name="couponManager">优惠券管理</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(CouponManager couponManager)
        {
            _context.CouponManagers.Remove(couponManager);
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
        /// 根据商家ID获取优惠券列表（分页）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>优惠券列表和总数</returns>
        public async Task<(IEnumerable<CouponManager> coupons, int total)> GetByStoreIdAsync(int storeId, int page, int pageSize)
        {
            var query = _context.CouponManagers
                .Where(cm => cm.StoreID == storeId)
                .OrderByDescending(cm => cm.CouponManagerID);

            var total = await query.CountAsync();
            var coupons = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (coupons, total);
        }

        /// <summary>
        /// 根据商家ID获取优惠券统计信息
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>统计信息</returns>
        public async Task<(int total, int active, int expired, int upcoming, int totalUsed, decimal totalValue)> GetStatsByStoreIdAsync(int storeId)
        {
            var coupons = await _context.CouponManagers
                .Where(cm => cm.StoreID == storeId)
                .ToListAsync();

            var total = coupons.Count;
            var now = DateTime.Now;

            var active = coupons.Count(c => c.ValidFrom <= now && c.ValidTo >= now);
            var expired = coupons.Count(c => c.ValidTo < now);
            var upcoming = coupons.Count(c => c.ValidFrom > now);
            var totalUsed = coupons.Sum(c => c.UsedQuantity);
            var totalValue = coupons.Sum(c => c.Value * c.UsedQuantity);

            return (total, active, expired, upcoming, totalUsed, totalValue);
        }

        /// <summary>
        /// 根据商家ID和优惠券ID获取优惠券
        /// </summary>
        /// <param name="id">优惠券ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>优惠券管理</returns>
        public async Task<CouponManager?> GetByIdAndStoreIdAsync(int id, int storeId)
        {
            return await _context.CouponManagers
                .FirstOrDefaultAsync(cm => cm.CouponManagerID == id && cm.StoreID == storeId);
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="ids">优惠券ID列表</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>删除数量</returns>
        public async Task<int> BatchDeleteAsync(IEnumerable<int> ids, int storeId)
        {
            var coupons = await _context.CouponManagers
                .Where(cm => ids.Contains(cm.CouponManagerID) && cm.StoreID == storeId)
                .ToListAsync();

            _context.CouponManagers.RemoveRange(coupons);
            await SaveAsync();

            return coupons.Count;
        }
    }
}