using BackEnd.DTOs.DeliveryTask;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送任务服务接口（骑手侧）
    /// </summary>
    public interface ICourierDeliveryTaskService
    {
        /// <summary>
        /// 获取配送任务列表（骑手端）
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="status">配送状态</param>
        /// <returns>配送任务列表</returns>
        Task<IEnumerable<CourierTaskListItemDto>> GetTasksAsync(int courierId, string? status);

        /// <summary>
        /// 获取可接配送任务列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度（可选）</param>
        /// <param name="longitude">经度（可选）</param>
        /// <param name="maxDistance">最大距离（默认10公里）</param>
        /// <returns>可接配送任务列表</returns>
        Task<IEnumerable<CourierAvailableTaskDto>> GetAvailableTasksAsync(int courierId, decimal? latitude = null, decimal? longitude = null, decimal maxDistance = 10);

        /// <summary>
        /// 接受配送任务
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="taskId">配送任务ID</param>
        /// <returns>接受结果</returns>
        Task<bool> AcceptTaskAsync(int courierId, int taskId);

        /// <summary>
        /// 确认取餐（将状态从Pending改为Delivering）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>操作结果</returns>
        Task<bool> PickupTaskAsync(int taskId, int courierId);

        /// <summary>
        /// 确认送达（将状态从Delivering改为Completed）
        /// </summary>
        /// <param name="taskId">配送任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>操作结果</returns>
        Task<bool> DeliverTaskAsync(int taskId, int courierId);
    }
}
