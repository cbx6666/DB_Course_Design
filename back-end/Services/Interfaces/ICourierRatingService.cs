using BackEnd.DTOs.Courier;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 骑手评分服务接口
    /// </summary>
    public interface ICourierRatingService
    {
        /// <summary>
        /// 为骑手评分
        /// </summary>
        /// <param name="dto">评分请求</param>
        /// <param name="customerId">消费者ID</param>
        /// <param name="courierId">骑手ID</param>
        /// <returns>任务</returns>
        Task RateCourierAsync(CreateCourierRatingDto dto, int customerId, int courierId);
    }
}
