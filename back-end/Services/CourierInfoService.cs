using BackEnd.Data;
using BackEnd.DTOs.Courier;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private readonly IDeliveryTaskRepository _deliveryTaskRepository;
        private readonly IImageUploadService _imageUploadService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="courierRepository">配送员仓储</param>
        /// <param name="deliveryTaskRepository">配送任务仓储</param>
        /// <param name="imageUploadService">图片上传服务</param>
        public CourierInfoService(
            IUserRepository userRepository,
            ICourierRepository courierRepository,
            IDeliveryTaskRepository deliveryTaskRepository,
            IImageUploadService imageUploadService)
        {
            _userRepository = userRepository;
            _courierRepository = courierRepository;
            _deliveryTaskRepository = deliveryTaskRepository;
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// 获取配送员档案
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>配送员档案</returns>
        public async Task<CourierProfileDto?> GetProfileAsync(int courierId)
        {
            var user = await _userRepository.GetByIdAsync(courierId);

            if (user == null)
            {
                return null;
            }

            return new CourierProfileDto
            {
                Id = user.UserID.ToString(),
                Name = user.Username,
                FullName = user.FullName,
                RegisterDate = user.AccountCreationTime.ToString("yyyy-MM-dd"),
                Rating = user.Courier?.AverageRating ?? 0,
                CreditScore = user.Courier?.ReputationPoints ?? 0,
                Avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "/images/default-avatar.jpg" : user.Avatar,
                Gender = user.Gender
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
        /// 获取今日收入（已完成订单的配送费总和）
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>今日收入</returns>
        public async Task<decimal> GetTodayIncomeAsync(int courierId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            // 查询今日已完成的配送任务
            var todayCompletedTasks = await _deliveryTaskRepository.GetCompletedTasksByCourierIdAndDateRangeAsync(courierId, today, tomorrow);

            // 计算今日收入：每单配送费 + 5元
            decimal todayIncome = todayCompletedTasks.Sum(task => task.DeliveryFee + 5);
            return todayIncome;
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
            var courier = await _courierRepository.GetByIdAsync(courierId);

            if (courier == null)
            {
                return false;
            }

            courier.CourierLatitude = latitude;
            courier.CourierLongitude = longitude;

            await _courierRepository.UpdateAsync(courier);
            await _courierRepository.SaveAsync();
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
            var userToUpdate = await _userRepository.GetByIdAsync(courierId);
            if (userToUpdate == null)
            {
                return false;
            }

            userToUpdate.Username = profileDto.Name;
            userToUpdate.Gender = profileDto.Gender;
            userToUpdate.Birthday = profileDto.Birthday;
            userToUpdate.Avatar = profileDto.Avatar;

            await _userRepository.UpdateAsync(userToUpdate);
            await _userRepository.SaveAsync();

            var courierToUpdate = await _courierRepository.GetByIdAsync(courierId);
            if (courierToUpdate == null)
            {
                return false;
            }

            courierToUpdate.VehicleType = profileDto.VehicleType;
            await _courierRepository.UpdateAsync(courierToUpdate);
            await _courierRepository.SaveAsync();
            return true;
        }

        /// <summary>
        /// 获取编辑用档案信息
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>编辑用档案信息</returns>
        public async Task<UpdateProfileDto?> GetProfileForEditAsync(int courierId)
        {
            var user = await _userRepository.GetByIdAsync(courierId);

            if (user == null || user.Courier == null)
            {
                return null;
            }

            return new UpdateProfileDto
            {
                Name = user.Username,
                FullName = user.FullName,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Avatar = user.Avatar,
                VehicleType = user.Courier.VehicleType
            };
        }

        /// <summary>
        /// 更新配送员头像（表单上传）
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="avatarFile">头像文件</param>
        /// <returns>头像URL</returns>
        public async Task<(bool Success, string? Message, string? AvatarUrl)> UpdateCourierAvatarAsync(int courierId, IFormFile avatarFile)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(courierId);
                if (user == null)
                    return (false, "用户不存在", null);

                // 使用统一的图片上传服务，上传到avatars目录
                var avatarUrl = await _imageUploadService.UploadImageAsync(avatarFile, null, "avatars");
                
                user.Avatar = avatarUrl;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();

                return (true, null, avatarUrl);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                return (false, $"上传失败: {ex.Message}", null);
            }
        }
    }
}
