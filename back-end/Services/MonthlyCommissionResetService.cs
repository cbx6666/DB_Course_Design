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
    /// 月度提成重置后台服务
    /// </summary>
    public class MonthlyCommissionResetService : IHostedService, IDisposable
    {
        private readonly ILogger<MonthlyCommissionResetService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="logger">日志记录器</param>
        /// <param name="scopeFactory">服务作用域工厂</param>
        public MonthlyCommissionResetService(ILogger<MonthlyCommissionResetService> logger, IServiceScopeFactory scopeFactory)
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
            _logger.LogInformation("月度提成重置后台服务已启动。");
            // 启动时，立即调度第一次任务
            ScheduleNextRun();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 调度下次运行
        /// </summary>
        private void ScheduleNextRun()
        {
            var now = DateTime.UtcNow;
            // 计算下一个月1号的凌晨0点5分 (UTC时间)
            var firstDayOfNextMonth = new DateTime(now.Year, now.Month, 1, 0, 5, 0, DateTimeKind.Utc).AddMonths(1);
            var initialDelay = firstDayOfNextMonth - now;

            if (initialDelay.TotalMilliseconds <= 0)
            {
                // 如果计算出的时间已经过去（例如，在月初启动服务），则计算再下一个月的
                firstDayOfNextMonth = firstDayOfNextMonth.AddMonths(1);
                initialDelay = firstDayOfNextMonth - now;
            }

            _logger.LogInformation("下一次月度提成重置任务将在 {resetTime} (UTC) 执行。", firstDayOfNextMonth);

            // 设置定时器：在指定的延迟后执行一次 DoWork
            _timer = new Timer(DoWork, null, initialDelay, Timeout.InfiniteTimeSpan);
        }

        /// <summary>
        /// 执行重置工作
        /// </summary>
        /// <param name="state">状态对象</param>
        private async void DoWork(object? state)
        {
            _logger.LogInformation("正在执行月度提成重置任务...");
            try
            {
                // IHostedService 是单例的，但 DbContext 是 Scoped 的。
                // 必须创建一个新的作用域来安全地获取 DbContext 实例。
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // 使用 EF Core 7+ 的 ExecuteUpdateAsync 进行高效的批量更新
                    // 这会生成一条单一的 SQL UPDATE 语句，无需查询数据到内存。
                    var rowsAffected = await dbContext.Couriers
                                 .ExecuteUpdateAsync(s => s.SetProperty(c => c.CommissionThisMonth, 0.00m));

                    _logger.LogInformation("所有骑手的本月提成已成功重置为 0。受影响行数: {rows}", rowsAffected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行月度提成重置任务时发生严重错误。");
            }
            finally
            {
                // 无论成功还是失败，都重新调度下一次（再下一个月）的任务
                ScheduleNextRun();
            }
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("月度提成重置后台服务正在停止。");
            _timer?.Change(Timeout.Infinite, 0); // 停止定时器
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