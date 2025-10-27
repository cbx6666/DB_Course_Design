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
        /// 获取店铺月销量（优先使用缓存值，必要时实时计算）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>月销量</returns>
        public async Task<int> GetStoreMonthlySalesAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            if (store == null) return 0;

            var currentDate = DateTime.Now;
            
            // 如果缓存值是最近更新的（本月内），直接返回
            if (store.MonthlySalesLastUpdated.HasValue && 
                store.MonthlySalesLastUpdated.Value.Month == currentDate.Month &&
                store.MonthlySalesLastUpdated.Value.Year == currentDate.Year)
            {
                return store.MonthlySales;
            }

            // 否则实时计算并更新缓存
            return await UpdateStoreMonthlySalesAsync(storeId);
        }

        /// <summary>
        /// 更新店铺月销量并返回新值
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>更新后的月销量</returns>
        public async Task<int> UpdateStoreMonthlySalesAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            if (store == null) return 0;

            var currentDate = DateTime.Now;
            var lastMonth = currentDate.AddMonths(-1);
            var firstDayOfLastMonth = new DateTime(lastMonth.Year, lastMonth.Month, 1);
            var lastDayOfLastMonth = firstDayOfLastMonth.AddMonths(1).AddDays(-1);

            // 计算上个月的完成订单数量
            var lastMonthOrders = await _context.FoodOrders
                .Where(o => o.StoreID == storeId &&
                           o.PaymentTime.HasValue &&
                           o.PaymentTime.Value >= firstDayOfLastMonth &&
                           o.PaymentTime.Value <= lastDayOfLastMonth &&
                           o.FoodOrderState == Models.Enums.FoodOrderState.Completed)
                .CountAsync();

            // 如果上个月没有订单，使用本月到目前为止的完成订单数
            if (lastMonthOrders == 0)
            {
                var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
                lastMonthOrders = await _context.FoodOrders
                    .Where(o => o.StoreID == storeId &&
                               o.PaymentTime.HasValue &&
                               o.PaymentTime.Value >= firstDayOfCurrentMonth &&
                               o.FoodOrderState == Models.Enums.FoodOrderState.Completed)
                    .CountAsync();
            }

            // 更新缓存
            store.MonthlySales = lastMonthOrders;
            store.MonthlySalesLastUpdated = currentDate;
            await _context.SaveChangesAsync();

            return lastMonthOrders;
        }

        /// <summary>
        /// 增加店铺月销量（订单完成时调用）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>任务</returns>
        public async Task IncrementStoreMonthlySalesAsync(int storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            if (store == null) return;

            store.MonthlySales++;
            store.MonthlySalesLastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
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