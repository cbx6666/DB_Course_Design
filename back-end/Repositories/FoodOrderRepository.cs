using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 订单数据访问层
    /// </summary>
    public class FoodOrderRepository : IFoodOrderRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public FoodOrderRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有订单（优化版本：移除不必要的关联数据，使用拆分查询）
        /// </summary>
        /// <returns>订单列表</returns>
        public async Task<IEnumerable<FoodOrder>> GetAllAsync()
        {
            // 使用 AsSplitQuery 拆分查询，避免复杂的 JOIN，提高性能
            // 移除 AfterSaleApplications 和 Comments（商家查询订单时通常不需要）
            var orders = await _context.FoodOrders
                                       .AsSplitQuery()  // 拆分查询，避免笛卡尔积
                                       .Include(fo => fo.Store)                  // 店铺（必需，用于筛选）
                                           .ThenInclude(s => s.Seller)           // 商家信息
                                       .Include(fo => fo.DeliveryInfo)           // 配送信息（必需）
                                       .Include(fo => fo.Cart)                   // 购物车（必需，用于获取订单项）
                                       .OrderByDescending(fo => fo.OrderID)
                                       .ToListAsync();

            // 批量加载 DeliveryTasks 和优惠券（避免N+1查询）
            if (orders.Any())
            {
                var orderIds = orders.Select(o => o.OrderID).ToList();
                
                // 批量加载 DeliveryTasks
                var tasks = await _context.DeliveryTasks
                    .Where(d => orderIds.Contains(d.OrderID))
                    .Select(d => new { d.OrderID, d.TaskID, d.Status })
                    .ToListAsync();

                // 批量加载优惠券及其管理信息
                var coupons = await _context.Coupons
                    .Include(c => c.CouponManager)
                    .Where(c => c.OrderID.HasValue && orderIds.Contains(c.OrderID.Value))
                    .ToListAsync();

                // 将优惠券分组到对应的订单
                var couponsByOrder = coupons
                    .Where(c => c.OrderID.HasValue)
                    .GroupBy(c => c.OrderID!.Value)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var taskDict = tasks.ToDictionary(t => t.OrderID);

                foreach (var order in orders)
                {
                    // 手动分配优惠券到订单
                    if (couponsByOrder.TryGetValue(order.OrderID, out var orderCoupons))
                    {
                        order.Coupons ??= new List<Coupon>();
                        foreach (var coupon in orderCoupons)
                        {
                            order.Coupons.Add(coupon);
                        }
                    }

                    // 手动分配 DeliveryTask
                    if (taskDict.TryGetValue(order.OrderID, out var t))
                    {
                        order.DeliveryTask = new DeliveryTask
                        {
                            TaskID = t.TaskID,
                            Status = t.Status,
                            OrderID = order.OrderID
                        };
                    }
                }
            }

            return orders;
        }

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>订单信息</returns>
        public async Task<FoodOrder?> GetByIdAsync(int id)
        {
            return await _context.FoodOrders
                                 .Include(fo => fo.Customer)
                                 .Include(fo => fo.Cart)
                                 .Include(fo => fo.Store)
                                 .Include(fo => fo.DeliveryInfo)
                                 .Include(fo => fo.Coupons)
                                 .Include(fo => fo.AfterSaleApplications)
                                 .Include(fo => fo.Comments)
                                 .FirstOrDefaultAsync(fo => fo.OrderID == id);
        }

        /// <summary>
        /// 根据客户ID获取按日期排序的订单
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>订单列表</returns>
        public async Task<List<FoodOrder>> GetOrdersByCustomerIdOrderedByDateAsync(int customerId)
        {
            var orders = await _context.FoodOrders
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderTime)
                .ToListAsync();

            // 单独查 DeliveryTasks
            var orderIds = orders.Select(o => o.OrderID).ToList();
            var tasks = await _context.DeliveryTasks
                .Where(d => orderIds.Contains(d.OrderID))
                .Select(d => new { d.OrderID, d.TaskID, d.Status })
                .ToListAsync();

            var taskDict = tasks.ToDictionary(t => t.OrderID);

            foreach (var order in orders)
            {
                if (taskDict.TryGetValue(order.OrderID, out var t))
                {
                    order.DeliveryTask = new DeliveryTask
                    {
                        TaskID = t.TaskID,
                        Status = t.Status
                    };
                }
            }

            return orders;
        }

        /// <summary>
        /// 根据购物车ID获取订单
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>订单信息</returns>
        public async Task<FoodOrder?> GetByCartIdAsync(int cartId)
        {
            return await _context.FoodOrders
                .FirstOrDefaultAsync(o => o.CartID == cartId);
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="foodOrder">订单信息</param>
        public async Task AddAsync(FoodOrder foodOrder)
        {
            await _context.FoodOrders.AddAsync(foodOrder);
            await SaveAsync();
        }

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="foodOrder">订单信息</param>
        public async Task UpdateAsync(FoodOrder foodOrder)
        {
            _context.FoodOrders.Update(foodOrder);
            await SaveAsync();
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="foodOrder">订单信息</param>
        public async Task DeleteAsync(FoodOrder foodOrder)
        {
            _context.FoodOrders.Remove(foodOrder);
            await SaveAsync();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}