using BackEnd.Data;
using BackEnd.DTOs.Courier;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 配送员服务实现
    /// </summary>
    public class CourierService : ICourierService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly IDeliveryTaskRepository _deliveryTaskRepository;
        private readonly AppDbContext _context;
        private readonly IGeoHelper _geoHelper;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="courierRepository">配送员仓储</param>
        /// <param name="deliveryTaskRepository">配送任务仓储</param>
        /// <param name="context">数据库上下文</param>
        /// <param name="geoHelper">地理位置辅助服务</param>
        public CourierService(
            IUserRepository userRepository,
            ICourierRepository courierRepository,
            IDeliveryTaskRepository deliveryTaskRepository,
            AppDbContext context,
            IGeoHelper geoHelper)
        {
            _userRepository = userRepository;
            _courierRepository = courierRepository;
            _deliveryTaskRepository = deliveryTaskRepository;
            _context = context;
            _geoHelper = geoHelper;
        }

        /// <summary>
        /// 获取配送员档案
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>配送员档案</returns>
        public async Task<CourierProfileDto?> GetProfileAsync(int courierId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Courier)
                .FirstOrDefaultAsync(u => u.UserID == courierId);

            if (user == null)
            {
                return null;
            }

            return new CourierProfileDto
            {
                Id = user.UserID.ToString(),
                Name = user.Username,
                RegisterDate = user.AccountCreationTime.ToString("yyyy-MM-dd"),
                Rating = user.Courier?.AverageRating ?? 0,
                CreditScore = user.Courier?.ReputationPoints ?? 0,
                Avatar = user.Avatar
            };
        }

        /// <summary>
        /// 获取工作状态
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>工作状态</returns>
        public async Task<WorkStatusDto?> GetWorkStatusAsync(int courierId)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null) return null;

            var statusDto = new WorkStatusDto
            {
                IsOnline = courier.IsOnline == CourierIsOnline.Online
            };
            return statusDto;
        }

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>当前位置</returns>
        public async Task<string> GetCurrentLocationAsync(int courierId)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);

            if (courier == null || !courier.CourierLongitude.HasValue || !courier.CourierLatitude.HasValue)
            {
                return "位置信息未提供";
            }

            var simulatedArea = $"(经度: {courier.CourierLongitude.Value:F6}, 纬度: {courier.CourierLatitude.Value:F6})";
            return await Task.FromResult(simulatedArea);
        }

        /// <summary>
        /// 切换工作状态
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="isOnline">是否在线</param>
        /// <returns>切换结果</returns>
        public async Task<bool> ToggleWorkStatusAsync(int courierId, bool isOnline)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null) return false;

            courier.IsOnline = isOnline ? CourierIsOnline.Online : CourierIsOnline.Offline;
            await _courierRepository.UpdateAsync(courier);
            await _courierRepository.SaveAsync();
            return true;
        }

        /// <summary>
        /// 获取订单列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="status">订单状态</param>
        /// <returns>订单列表</returns>
        public async Task<IEnumerable<OrderListItemDto>> GetOrdersAsync(int courierId, string status)
        {
            if (!Enum.TryParse<DeliveryStatus>(status, true, out var targetStatus))
            {
                return new List<OrderListItemDto>();
            }

            var tasksQuery = _deliveryTaskRepository.GetQueryable()
                .Where(t => t.CourierID == courierId && t.Status == targetStatus)
                .Include(t => t.Store)
                .Include(t => t.Order)
                .Include(t => t.Customer);

            var tasks = await tasksQuery
                .OrderByDescending(t => t.PublishTime)
                .ToListAsync();

            var orderDtos = tasks.Select(task => new OrderListItemDto
            {
                Id = task.TaskID.ToString(),
                Status = task.Status.ToString().ToLower(),
                Restaurant = task.Store?.StoreName ?? "未知商家",
                Address = task.Customer?.DefaultAddress ?? "未知地址",
                Fee = task.DeliveryFee.ToString("F2"),
                StatusText = GetStatusText(task.Status),
                IsReadyForPickup = task.Order != null && task.Order.FoodOrderState == FoodOrderState.Completed
            }).ToList();

            return orderDtos;
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
                DeliveryStatus.Cancelled => "已取消",
                _ => "未知状态"
            };
        }

        /// <summary>
        /// 获取新订单详情
        /// </summary>
        /// <param name="notificationId">通知ID</param>
        /// <returns>新订单详情</returns>
        public async Task<NewOrderDetailsDto?> GetNewOrderDetailsAsync(int notificationId)
        {
            var taskId = notificationId;
            var task = await _deliveryTaskRepository.GetQueryable()
                .Include(t => t.Store)
                .Include(t => t.Customer)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(t => t.TaskID == taskId);

            if (task == null)
            {
                return null;
            }

            var orderDetailsDto = new NewOrderDetailsDto
            {
                Id = task.TaskID.ToString(),
                RestaurantName = task.Store?.StoreName ?? "未知商家",
                RestaurantAddress = task.Store?.StoreAddress ?? "未知商家地址",
                CustomerName = task.Customer?.User?.FullName ?? task.Customer?.User?.Username ?? "未知顾客",
                CustomerAddress = task.Customer?.DefaultAddress ?? "未知顾客地址",
                Fee = task.DeliveryFee,
                Distance = "约 2.5 公里",
                MapImageUrl = "https://example.com/static-map.png"
            };
            return orderDetailsDto;
        }

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>接受结果</returns>
        public async Task<bool> AcceptOrderAsync(int courierId, int orderId)
        {
            var taskToAccept = await _context.DeliveryTasks.FindAsync(orderId);

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
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>拒绝结果</returns>
        public async Task<bool> RejectOrderAsync(int orderId)
        {
            var task = await _deliveryTaskRepository.GetByIdAsync(orderId);
            if (task == null || task.Status != DeliveryStatus.Pending)
            {
                return false;
            }

            task.Status = DeliveryStatus.Cancelled;
            await _deliveryTaskRepository.UpdateAsync(task);
            await _deliveryTaskRepository.SaveAsync();
            return true;
        }

        /// <summary>
        /// 获取月收入
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>月收入</returns>
        public async Task<decimal> GetMonthlyIncomeAsync(int courierId)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null)
            {
                return 0.00m;
            }

            decimal totalMonthlyIncome = courier.MonthlySalary + courier.CommissionThisMonth;
            return totalMonthlyIncome;
        }

        /// <summary>
        /// 标记任务为已完成
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>标记任务</returns>
        public async Task MarkTaskAsCompletedAsync(int taskId, int courierId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _deliveryTaskRepository.GetByIdAsync(taskId);
                if (task == null || task.CourierID != courierId || task.Status == DeliveryStatus.Completed)
                {
                    await transaction.RollbackAsync();
                    return;
                }

                task.Status = DeliveryStatus.Completed;
                task.CompletionTime = DateTime.UtcNow;
                await _deliveryTaskRepository.UpdateAsync(task);

                var courier = await _courierRepository.GetByIdAsync(task.CourierID!.Value);
                if (courier != null)
                {
                    courier.CommissionThisMonth += task.DeliveryFee;
                    await _courierRepository.UpdateAsync(courier);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 取餐
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>取餐结果</returns>
        public async Task<bool> PickupOrderAsync(int orderId, int courierId)
        {
            var task = await _deliveryTaskRepository.GetByIdAsync(orderId);

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
        /// 配送订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>配送结果</returns>
        public async Task<bool> DeliverOrderAsync(int orderId, int courierId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var task = await _deliveryTaskRepository.GetByIdAsync(orderId);

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
        /// 获取可用订单
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="maxDistance">最大距离</param>
        /// <returns>可用订单列表</returns>
        public async Task<IEnumerable<AvailableOrderDto>> GetAvailableOrdersAsync(int courierId, decimal latitude, decimal longitude, decimal maxDistance)
        {
            var tasksQuery = _context.DeliveryTasks
                .Where(task => task.Status == DeliveryStatus.To_Be_Taken)
                .Include(task => task.Store)
                .Include(task => task.Order)
                    .ThenInclude(order => order.Customer)
                        .ThenInclude(customer => customer.User);

            var allTasks = await tasksQuery.ToListAsync();

            var nearbyTasks = new List<DeliveryTask>();
            foreach (var task in allTasks)
            {
                if (task.Store?.Latitude.HasValue == true && task.Store?.Longitude.HasValue == true)
                {
                    var distanceToStore = _geoHelper.CalculateDistance(
                        latitude, longitude,
                        task.Store.Latitude.Value, task.Store.Longitude.Value
                    );

                    if (distanceToStore <= (double)maxDistance)
                    {
                        nearbyTasks.Add(task);
                    }
                }
            }

            var resultDtos = nearbyTasks.Select(task => new AvailableOrderDto
            {
                Id = task.TaskID.ToString(),
                Status = "to_be_taken",
                Restaurant = task.Store.StoreName,
                PickupAddress = task.Store.StoreAddress,
                Customer = task.Order.Customer.User.Username,
                Fee = task.DeliveryFee.ToString("F2"),
                DeliveryAddress = "接单后可见详细地址",
                Distance = "2.5",
                Time = "15"
            }).ToList();

            return resultDtos;
        }

        /// <summary>
        /// 获取投诉列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>投诉列表</returns>
        public async Task<IEnumerable<ComplaintDto>> GetComplaintsAsync(int courierId)
        {
            var complaints = await _context.DeliveryComplaints
                .Where(c => c.CourierID == courierId)
                .OrderByDescending(c => c.ComplaintTime)
                .ToListAsync();

            var complaintDtos = complaints.Select(complaint =>
            {
                PunishmentDto? punishmentDto = null;

                if (!string.IsNullOrEmpty(complaint.ProcessingResult) && complaint.ProcessingResult != "-")
                {
                    punishmentDto = new PunishmentDto
                    {
                        Description = complaint.ProcessingResult,
                        Type = "官方处理结果",
                        Duration = null
                    };
                }

                return new ComplaintDto
                {
                    ComplaintID = complaint.ComplaintID.ToString(),
                    DeliveryTaskID = complaint.DeliveryTaskID.ToString(),
                    ComplaintTime = complaint.ComplaintTime.ToString("yyyy-MM-dd HH:mm"),
                    ComplaintReason = complaint.ComplaintReason,
                    Punishment = punishmentDto
                };
            }).ToList();

            return complaintDtos;
        }

        /// <summary>
        /// 更新配送员位置
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>更新结果</returns>
        public async Task<bool> UpdateCourierLocationAsync(int courierId, decimal latitude, decimal longitude)
        {
            var courier = await _context.Couriers.FirstOrDefaultAsync(c => c.UserID == courierId);

            if (courier == null)
            {
                return false;
            }

            courier.CourierLatitude = latitude;
            courier.CourierLongitude = longitude;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 更新档案
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="profileDto">档案更新请求</param>
        /// <returns>更新结果</returns>
        public async Task<bool> UpdateProfileAsync(int courierId, UpdateProfileDto profileDto)
        {
            var userToUpdate = await _context.Users.FindAsync(courierId);
            if (userToUpdate == null)
            {
                return false;
            }

            userToUpdate.Username = profileDto.Name;
            userToUpdate.Gender = profileDto.Gender;
            userToUpdate.Birthday = profileDto.Birthday;
            userToUpdate.Avatar = profileDto.Avatar;

            _context.Users.Update(userToUpdate);

            var courierToUpdate = await _context.Couriers.FindAsync(courierId);
            if (courierToUpdate == null)
            {
                return false;
            }

            courierToUpdate.VehicleType = profileDto.VehicleType;
            _context.Couriers.Update(courierToUpdate);

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 获取编辑用档案信息
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>编辑用档案信息</returns>
        public async Task<UpdateProfileDto?> GetProfileForEditAsync(int courierId)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Courier)
                .FirstOrDefaultAsync(u => u.UserID == courierId);

            if (user == null || user.Courier == null)
            {
                return null;
            }

            return new UpdateProfileDto
            {
                Name = user.Username,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Avatar = user.Avatar,
                VehicleType = user.Courier.VehicleType
            };
        }
    }
}