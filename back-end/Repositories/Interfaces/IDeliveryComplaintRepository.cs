using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 配送投诉仓储接口
    /// </summary>
    public interface IDeliveryComplaintRepository
    {
        /// <summary>
        /// 获取所有配送投诉
        /// </summary>
        /// <returns>配送投诉列表</returns>
        Task<IEnumerable<DeliveryComplaint>> GetAllAsync();

        /// <summary>
        /// 根据ID获取配送投诉
        /// </summary>
        /// <param name="id">投诉ID</param>
        /// <returns>配送投诉</returns>
        Task<DeliveryComplaint?> GetByIdAsync(int id);

        /// <summary>
        /// 根据任务ID获取配送投诉
        /// </summary>
        /// <param name="deliveryTaskId">配送任务ID</param>
        /// <returns>配送投诉列表</returns>
        Task<IEnumerable<DeliveryComplaint>> GetByDeliveryTaskIdAsync(int deliveryTaskId);

        /// <summary>
        /// 添加配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        Task AddAsync(DeliveryComplaint complaint);

        /// <summary>
        /// 更新配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        Task UpdateAsync(DeliveryComplaint complaint);

        /// <summary>
        /// 删除配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>任务</returns>
        Task DeleteAsync(DeliveryComplaint complaint);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}