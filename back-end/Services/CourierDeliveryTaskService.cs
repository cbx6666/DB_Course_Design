using BackEnd.Data;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 配送任务服务实现（骑手侧）
    /// </summary>
    public class CourierDeliveryTaskService : ICourierDeliveryTaskService
    {
        private readonly IDeliveryTaskRepository _deliveryTaskRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly AppDbContext _context;
        private readonly IGeoHelper _geoHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CourierDeliveryTaskService(
            IDeliveryTaskRepository deliveryTaskRepository,
            ICourierRepository courierRepository,
            AppDbContext context,
            IGeoHelper geoHelper)
        {
            _deliveryTaskRepository = deliveryTaskRepository;
            _courierRepository = courierRepository;
            _context = context;
            _geoHelper = geoHelper;
        }

        /// <summary>
        /// 获取配送任务列表（骑手端）
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="status">配送状态</param>
        /// <returns>配送任务列表</returns>
        public async Task<IEnumerable<CourierTaskListItemDto>> GetTasksAsync(int courierId, string? status)
        {
            if (string.IsNullOrEmpty(status) || !Enum.TryParse<DeliveryStatus>(status, true, out var targetStatus))
            {
                return new List<CourierTaskListItemDto>();
            }

            var tasksQuery = _deliveryTaskRepository.GetQueryable()
                .Where(t => t.CourierID == courierId && t.Status == targetStatus)
                .Include(t => t.Order)
                .ThenInclude(o => o.Store)
                .ThenInclude(s => s.Seller)
                .ThenInclude(s => s.User)
                .Include(t => t.Order)
                .ThenInclude(o => o.Customer)
                .Include(t => t.Order)
                .ThenInclude(o => o.DeliveryInfo)
                .Include(t => t.Order)
                .ThenInclude(o => o.Cart)
                .ThenInclude(c => c!.ShoppingCartItems!)
                .ThenInclude(sci => sci.Dish);

            var tasks = await tasksQuery
                .OrderByDescending(t => t.PublishTime)
                .ToListAsync();

            var taskDtos = tasks.Select(task =>
            {
                // 构建客户显示名称：姓氏 + 性别（如：张先生）
                var deliveryName = task.Order.DeliveryInfo?.Name;
                var gender = task.Order.DeliveryInfo?.Gender;

                string customerDisplayName;
                if (string.IsNullOrEmpty(deliveryName))
                {
                    customerDisplayName = "未知";
                }
                else
                {
                    // 提取姓氏（第一个字符）
                    var surname = deliveryName.Length > 0 ? deliveryName[0].ToString() : "";

                    // Gender 字段存储的是 "先生" 或 "女士"，直接拼接
                    if (!string.IsNullOrEmpty(gender))
                    {
                        customerDisplayName = $"{surname}{gender}";
                    }
                    else
                    {
                        customerDisplayName = surname; // 没有性别只显示姓氏
                    }
                }

                // 获取配送地址
                var deliveryAddress = task.Order.DeliveryInfo?.Address ?? "地址未提供";
                var pickupAddress = task.Order.Store?.StoreAddress ?? "地址未提供";

                // 获取电话号码
                var customerPhone = task.Order.DeliveryInfo?.PhoneNumber;
                var restaurantPhone = task.Order.Store?.Seller?.User?.PhoneNumber.ToString();

                // 获取菜品列表
                var dishDetails = new List<DTOs.Dish.OrderDishDto>();
                if (task.Order.Cart?.ShoppingCartItems != null)
                {
                    dishDetails = task.Order.Cart.ShoppingCartItems
                        .Where(sci => sci.Dish != null)
                        .Select(sci => new DTOs.Dish.OrderDishDto
                        {
                            DishName = sci.Dish.DishName,
                            DishImage = sci.Dish.DishImage ?? "",
                            Quantity = sci.Quantity
                        })
                        .ToList();
                }

                return new CourierTaskListItemDto
                {
                    Id = task.TaskID.ToString(),
                    Status = task.Status.ToString().ToLower(),
                    Restaurant = task.Order.Store?.StoreName ?? "未知商家",
                    Address = deliveryAddress,
                    PickupAddress = pickupAddress,
                    DeliveryAddress = deliveryAddress,
                    Customer = customerDisplayName,
                    CustomerPhone = customerPhone,
                    RestaurantPhone = restaurantPhone,
                    Fee = task.DeliveryFee.ToString("F2"),
                    StatusText = GetStatusText(task.Status),
                    IsReadyForPickup = task.Order != null && task.Order.FoodOrderState == FoodOrderState.Completed,
                    Time = task.PublishTime.ToString("yyyy-MM-dd HH:mm"),
                    CompletionTime = task.CompletionTime.HasValue 
                        ? task.CompletionTime.Value.ToString("yyyy-MM-dd HH:mm") 
                        : null,
                    Remarks = task.Order?.Remarks,
                    DishDetails = dishDetails
                };
            }).ToList();

            return taskDtos;
        }

        /// <summary>
        /// 获取可接配送任务列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度（可选）</param>
        /// <param name="longitude">经度（可选）</param>
        /// <param name="maxDistance">最大距离（默认10公里）</param>
        /// <returns>可接配送任务列表</returns>
        public async Task<IEnumerable<CourierAvailableTaskDto>> GetAvailableTasksAsync(int courierId, decimal? latitude = null, decimal? longitude = null, decimal maxDistance = 10)
        {
            var tasksQuery = _context.DeliveryTasks
                .Where(task => task.Status == DeliveryStatus.To_Be_Taken)
                .Include(task => task.Order)
                .ThenInclude(order => order.Store)
                .Include(task => task.Order)
                    .ThenInclude(order => order.Customer)
                        .ThenInclude(customer => customer.User)
                .Include(task => task.Order)
                    .ThenInclude(order => order.DeliveryInfo);

            var allTasks = await tasksQuery.ToListAsync();

            var random = new Random();
            var resultDtos = allTasks.Select(task => 
            {
                // 构建客户显示名称：姓氏 + 性别（如：张先生）
                var deliveryName = task.Order.DeliveryInfo?.Name;
                var gender = task.Order.DeliveryInfo?.Gender;
                
                string customerDisplayName;
                if (string.IsNullOrEmpty(deliveryName))
                {
                    customerDisplayName = "未知";
                }
                else
                {
                    // 提取姓氏（第一个字符）
                    var surname = deliveryName.Length > 0 ? deliveryName[0].ToString() : "";
                    
                    // Gender 字段存储的是 "先生" 或 "女士"，直接拼接
                    if (!string.IsNullOrEmpty(gender))
                    {
                        customerDisplayName = $"{surname}{gender}";
                    }
                    else
                    {
                        customerDisplayName = surname; // 没有性别只显示姓氏
                    }
                }
                
                // 获取配送地址
                var deliveryAddress = task.Order.DeliveryInfo?.Address ?? "地址未提供";
                
                return new CourierAvailableTaskDto
                {
                    Id = task.TaskID.ToString(),
                    Status = "to_be_taken",
                    Restaurant = task.Order.Store?.StoreName ?? "未知商家",
                    PickupAddress = task.Order.Store?.StoreAddress ?? "地址未提供",
                    Customer = customerDisplayName,
                    Fee = task.DeliveryFee.ToString("F2"),
                    DeliveryAddress = deliveryAddress,
                    Distance = (random.NextDouble() * 9 + 1).ToString("F1"), // 1-10km 随机数
                    Time = random.Next(10, 31).ToString(), // 10-30分钟 随机数
                    PublishTime = task.PublishTime.ToString("yyyy-MM-dd HH:mm") // 发布时间
                };
            }).ToList();

            return resultDtos;
        }

        /// <summary>
        /// 接受配送任务
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="taskId">配送任务ID</param>
        /// <returns>接受结果</returns>
        public async Task<bool> AcceptTaskAsync(int courierId, int taskId)
        {
            var taskToAccept = await _context.DeliveryTasks.FindAsync(taskId);

            if (taskToAccept == null || taskToAccept.Status != DeliveryStatus.To_Be_Taken)
            {
                return false;
            }

            taskToAccept.CourierID = courierId;
            taskToAccept.Status = DeliveryStatus.Pending;
            taskToAccept.AcceptTime = DateTime.UtcNow;
            taskToAccept.Courier = await _courierRepository.GetByIdAsync(courierId);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 确认取餐（将状态从Pending改为Delivering）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> PickupTaskAsync(int taskId, int courierId)
        {
            var task = await _deliveryTaskRepository.GetByIdAsync(taskId);

            if (task == null || task.CourierID != courierId || task.Status != DeliveryStatus.Pending)
            {
                return false;
            }

            task.Status = DeliveryStatus.Delivering;
            task.PickupTime = DateTime.UtcNow; // 记录实际到店时间
            await _deliveryTaskRepository.UpdateAsync(task);
            await _deliveryTaskRepository.SaveAsync();
            return true;
        }

        /// <summary>
        /// 确认送达（将状态从Delivering改为Completed）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>操作结果</returns>
        public async Task<bool> DeliverTaskAsync(int taskId, int courierId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _deliveryTaskRepository.GetByIdAsync(taskId);

                if (task == null || task.CourierID != courierId || task.Status != DeliveryStatus.Delivering)
                {
                    return false;
                }

                task.Status = DeliveryStatus.Completed;
                task.CompletionTime = DateTime.UtcNow;
                await _deliveryTaskRepository.UpdateAsync(task);

                var courier = await _courierRepository.GetByIdAsync(courierId);
                if (courier != null)
                {
                    // 骑手每单收入 = 配送费 + 5元
                    courier.CommissionThisMonth += task.DeliveryFee + 5;
                    await _courierRepository.UpdateAsync(courier);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 获取状态文本
        /// </summary>
        /// <param name="status">配送状态</param>
        /// <returns>状态文本</returns>
        private string GetStatusText(DeliveryStatus status)
        {
            return status switch
            {
                DeliveryStatus.To_Be_Taken => "待处理",
                DeliveryStatus.Pending => "待取餐",
                DeliveryStatus.Delivering => "配送中",
                DeliveryStatus.Completed => "已完成",
                _ => "未知状态"
            };
        }
    }
}

