using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 商家仓储
    /// </summary>
    public class MerchantRepository : IMerchantRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public MerchantRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根据商家ID获取店铺信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺</returns>
        public async Task<Store?> GetStoreBySellerIdAsync(int sellerId)
        {
            return await _context.Stores.FirstOrDefaultAsync(s => s.SellerID == sellerId);
        }

        /// <summary>
        /// 根据商家ID获取商家信息（包含用户）
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>商家</returns>
        public async Task<Seller?> GetSellerByIdAsync(int sellerId)
        {
            var seller = await _context.Sellers.FirstOrDefaultAsync(s => s.UserID == sellerId);
            if (seller == null) return null;

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserID == sellerId);
            if (user == null) return null;

            seller.User = user;
            return seller;
        }

        /// <summary>
        /// 根据商家ID获取用户信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>用户</returns>
        public async Task<User?> GetUserBySellerIdAsync(int sellerId)
        {
            var seller = await _context.Sellers
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.UserID == sellerId);
            return seller?.User;
        }

        /// <summary>
        /// 更新店铺信息
        /// </summary>
        /// <param name="store">店铺</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateStoreAsync(Store store)
        {
            try
            {
                _context.Stores.Update(store);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 获取店铺评分（预留）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评分</returns>
        public Task<decimal> GetStoreRatingAsync(int storeId)
        {
            return Task.FromResult(0m);
        }

        /// <summary>
        /// 获取店铺月销量
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>月销量</returns>
        public async Task<int> GetStoreMonthlySalesAsync(int storeId)
        {
            var currentDate = DateTime.Now;
            var lastMonth = currentDate.AddMonths(-1);

            var lastMonthOrders = await _context.FoodOrders
                .Where(o => o.StoreID == storeId &&
                           o.PaymentTime.HasValue &&
                           o.PaymentTime.Value.Month == lastMonth.Month &&
                           o.PaymentTime.Value.Year == lastMonth.Year)
                .CountAsync();

            if (lastMonthOrders == 0)
            {
                var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                lastMonthOrders = await _context.FoodOrders
                    .Where(o => o.StoreID == storeId &&
                               o.PaymentTime.HasValue &&
                               o.PaymentTime.Value >= firstDayOfMonth)
                    .CountAsync();
            }

            return lastMonthOrders;
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