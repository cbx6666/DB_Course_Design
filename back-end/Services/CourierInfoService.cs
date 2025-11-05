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
    /// 配送员信息服务实现
    /// </summary>
    public class CourierInfoService : ICourierInfoService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICourierRepository _courierRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="courierRepository">配送员仓储</param>
        /// <param name="context">数据库上下文</param>
        public CourierInfoService(
            IUserRepository userRepository,
            ICourierRepository courierRepository,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _courierRepository = courierRepository;
            _context = context;
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
