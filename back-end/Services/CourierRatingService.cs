using BackEnd.DTOs.Courier;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 骑手评分服务实现
    /// </summary>
    public class CourierRatingService : ICourierRatingService
    {
        private readonly IDeliveryTaskRepository _deliveryTaskRepository;
        private readonly ICourierRepository _courierRepository;

        public CourierRatingService(
            IDeliveryTaskRepository deliveryTaskRepository,
            ICourierRepository courierRepository)
        {
            _deliveryTaskRepository = deliveryTaskRepository;
            _courierRepository = courierRepository;
        }

        /// <summary>
        /// 为骑手评分
        /// </summary>
        public async Task RateCourierAsync(CreateCourierRatingDto dto, int customerId, int courierId)
        {
            // 查找配送任务
            DeliveryTask? task = null;
            
            if (dto.TaskId.HasValue)
            {
                task = await _deliveryTaskRepository.GetByIdAsync(dto.TaskId.Value);
            }
            else if (dto.OrderId.HasValue)
            {
                task = await _deliveryTaskRepository.GetByOrderIdAsync(dto.OrderId.Value);
            }

            if (task == null)
            {
                throw new InvalidOperationException("未找到配送任务");
            }

            // 验证任务是否属于该骑手
            if (task.CourierID != courierId)
            {
                throw new InvalidOperationException("该配送任务不属于该骑手");
            }

            // 验证任务是否已完成
            if (task.Status != Models.Enums.DeliveryStatus.Completed)
            {
                throw new InvalidOperationException("只能对已完成的配送任务进行评分");
            }

            // 检查是否已经评分
            if (task.TaskRating.HasValue)
            {
                throw new InvalidOperationException("该配送任务已经评分过了");
            }

            // 更新配送任务的评分
            task.TaskRating = dto.Rating;
            await _deliveryTaskRepository.UpdateAsync(task);
            await _deliveryTaskRepository.SaveAsync();

            // 重新计算骑手的平均评分
            await UpdateCourierAverageRatingAsync(courierId);
        }

        /// <summary>
        /// 更新骑手的平均评分
        /// </summary>
        private async Task UpdateCourierAverageRatingAsync(int courierId)
        {
            var courier = await _courierRepository.GetByIdAsync(courierId);
            if (courier == null)
            {
                return;
            }

            // 获取该骑手所有已完成的配送任务及其评分
            var tasks = await _deliveryTaskRepository.GetCompletedTasksByCourierIdAsync(courierId);
            var taskRatings = tasks.Where(t => t.TaskRating.HasValue).Select(t => t.TaskRating!.Value).ToList();

            if (taskRatings.Any())
            {
                courier.AverageRating = (decimal)taskRatings.Average();
            }
            else
            {
                courier.AverageRating = 0.00m;
            }

            await _courierRepository.UpdateAsync(courier);
            await _courierRepository.SaveAsync();
        }
    }
}
