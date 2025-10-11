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
        /// 获取所有订单
        /// </summary>
        /// <returns>订单列表</returns>
        public async Task<IEnumerable<FoodOrder>> GetAllAsync()
        {
            var orders = await _context.FoodOrders
                                       .Include(fo => fo.Customer)               // 顾客
                                       .Include(fo => fo.Cart)                   // 购物车
                                       .Include(fo => fo.Store)                  // 店铺
                                       .Include(fo => fo.Coupons)                // 优惠券
                                       .Include(fo => fo.AfterSaleApplications)  // 售后申请
                                       .Include(fo => fo.Comments)               // 评论
                                       .OrderByDescending(fo => fo.OrderID)
                                       .ToListAsync();

            // 批量加载 DeliveryTasks
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
                        Status = t.Status,
                        OrderID = order.OrderID
                    };
                }
            }

            return orders;
        }

        /// <summary>
        /// 根据用户ID获取订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单列表</returns>
        public async Task<IEnumerable<FoodOrder>> GetByUserIdAsync(int userId)
        {
            var orders = await _context.FoodOrders
                                       .Where(fo => fo.CustomerID == userId)
                                       .Include(fo => fo.Customer)
                                       .Include(fo => fo.Cart)
                                       .Include(fo => fo.Store)
                                       .Include(fo => fo.Coupons)
                                       .Include(fo => fo.AfterSaleApplications)
                                       .ToListAsync();

            // 批量加载 DeliveryTasks
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
                        Status = t.Status,
                        OrderID = order.OrderID
                    };
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