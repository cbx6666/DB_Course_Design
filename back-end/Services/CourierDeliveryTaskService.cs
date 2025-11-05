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
                .Include(t => t.Order)
                .ThenInclude(o => o.Customer);

            var tasks = await tasksQuery
                .OrderByDescending(t => t.PublishTime)
                .ToListAsync();

            var taskDtos = tasks.Select(task => new CourierTaskListItemDto
            {
                Id = task.TaskID.ToString(),
                Status = task.Status.ToString().ToLower(),
                Restaurant = task.Order.Store?.StoreName ?? "未知商家",
                Address = task.Order.Customer?.DeliveryInfos.FirstOrDefault(di => di.IsDefault == 1)?.Address ?? "未知地址",
                Fee = task.DeliveryFee.ToString("F2"),
                StatusText = GetStatusText(task.Status),
                IsReadyForPickup = task.Order != null && task.Order.FoodOrderState == FoodOrderState.Completed,
                Time = task.PublishTime.ToString("yyyy-MM-dd HH:mm")
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
                        .ThenInclude(customer => customer.User);

            var allTasks = await tasksQuery.ToListAsync();

            // 获取骑手位置（优先使用传入的参数，否则从数据库获取）
            decimal courierLat, courierLng;
            if (latitude.HasValue && longitude.HasValue)
            {
                courierLat = latitude.Value;
                courierLng = longitude.Value;
            }
            else
            {
                var courier = await _context.Couriers
                    .FirstOrDefaultAsync(c => c.UserID == courierId);

                if (courier == null || !courier.CourierLatitude.HasValue || !courier.CourierLongitude.HasValue)
                {
                    return Enumerable.Empty<CourierAvailableTaskDto>();
                }

                courierLat = courier.CourierLatitude.Value;
                courierLng = courier.CourierLongitude.Value;
            }

            var nearbyTasks = new List<DeliveryTask>();
            foreach (var task in allTasks)
            {
                if (task.Order.Store?.Latitude.HasValue == true && task.Order.Store?.Longitude.HasValue == true)
                {
                    var distanceToStore = _geoHelper.CalculateDistance(
                        courierLat, courierLng,
                        task.Order.Store.Latitude.Value, task.Order.Store.Longitude.Value
                    );

                    if (distanceToStore <= (double)maxDistance)
                    {
                        nearbyTasks.Add(task);
                    }
                }
            }

            var resultDtos = nearbyTasks.Select(task => new CourierAvailableTaskDto
            {
                Id = task.TaskID.ToString(),
                Status = "to_be_taken",
                Restaurant = task.Order.Store.StoreName,
                PickupAddress = task.Order.Store.StoreAddress,
                Customer = task.Order.Customer.User.Username,
                Fee = task.DeliveryFee.ToString("F2"),
                DeliveryAddress = "接单后可见详细地址",
                Distance = "2.5",
                Time = "15"
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
                    courier.CommissionThisMonth += task.DeliveryFee;
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

