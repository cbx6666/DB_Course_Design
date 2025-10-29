using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BackEnd.Services
{
    /// <summary>
    /// 月销量更新后台服务
    /// </summary>
    public class MonthlySalesUpdateService : IHostedService, IDisposable
    {
        private readonly ILogger<MonthlySalesUpdateService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="scopeFactory">服务作用域工厂</param>
        public MonthlySalesUpdateService(ILogger<MonthlySalesUpdateService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("月销量更新后台服务已启动。");
            // 启动时立即执行一次更新
            _ = Task.Run(async () => await UpdateAllStoresMonthlySales(), cancellationToken);
            // 然后调度定期任务
            ScheduleNextRun();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 调度下次运行
        /// </summary>
        private void ScheduleNextRun()
        {
            var now = DateTime.UtcNow;
            // 计算下一个月1号的凌晨0点10分 (UTC时间)
            var firstDayOfNextMonth = new DateTime(now.Year, now.Month, 1, 0, 10, 0, DateTimeKind.Utc).AddMonths(1);
            var initialDelay = firstDayOfNextMonth - now;

            if (initialDelay.TotalMilliseconds <= 0)
            {
                // 如果计算出的时间已经过去，则计算再下一个月的
                firstDayOfNextMonth = firstDayOfNextMonth.AddMonths(1);
                initialDelay = firstDayOfNextMonth - now;
            }

            _logger.LogInformation("下一次月销量更新任务将在 {updateTime} (UTC) 执行。", firstDayOfNextMonth);

            // 设置定时器：在指定的延迟后执行一次 DoWork
            _timer = new Timer(DoWork, null, initialDelay, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// 执行更新工作
        /// </summary>
        /// <param name="state">状态对象</param>
        private async void DoWork(object? state)
        {
            await UpdateAllStoresMonthlySales();
            // 重新调度下一次任务
            ScheduleNextRun();
        }

        /// <summary>
        /// 更新所有店铺的月销量
        /// </summary>
        private async Task UpdateAllStoresMonthlySales()
        {
            _logger.LogInformation("正在执行月销量更新任务...");
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var currentDate = DateTime.Now;
                var firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);

                // 获取所有店铺
                var stores = await dbContext.Stores.ToListAsync();
                var updatedCount = 0;

                foreach (var store in stores)
                {
                    // 计算本月到目前为止的已完成订单数
                    var monthlySales = await dbContext.FoodOrders
                        .Where(o => o.StoreID == store.StoreID &&
                                   o.PaymentTime.HasValue &&
                                   o.PaymentTime.Value >= firstDayOfCurrentMonth &&
                                   o.FoodOrderState == Models.Enums.FoodOrderState.Completed)
                        .CountAsync();

                    // 更新店铺月销量
                    if (store.MonthlySales != monthlySales)
                    {
                        store.MonthlySales = monthlySales;
                        store.MonthlySalesLastUpdated = currentDate;
                        updatedCount++;
                    }
                }

                // 批量保存更改
                var rowsAffected = await dbContext.SaveChangesAsync();
                _logger.LogInformation("月销量更新完成。检查了 {totalStores} 个店铺，更新了 {updatedStores} 个店铺，数据库受影响行数: {rowsAffected}", 
                    stores.Count, updatedCount, rowsAffected);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行月销量更新任务时发生错误。");
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("月销量更新后台服务正在停止。");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
