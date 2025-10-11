using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 配送任务仓储接口
    /// </summary>
    public interface IDeliveryTaskRepository
    {
        /// <summary>
        /// 获取所有配送任务
        /// </summary>
        /// <returns>配送任务列表</returns>
        Task<IEnumerable<DeliveryTask>> GetAllAsync();

        /// <summary>
        /// 根据ID获取配送任务
        /// </summary>
        /// <param name="id">任务ID</param>
        /// <returns>配送任务</returns>
        Task<DeliveryTask?> GetByIdAsync(int id);

        /// <summary>
        /// 根据订单ID获取配送任务
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送任务</returns>
        Task<DeliveryTask?> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// 添加配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        Task AddAsync(DeliveryTask task);

        /// <summary>
        /// 更新配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        Task UpdateAsync(DeliveryTask task);

        /// <summary>
        /// 删除配送任务
        /// </summary>
        /// <param name="task">配送任务</param>
        /// <returns>任务</returns>
        Task DeleteAsync(DeliveryTask task);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();

        /// <summary>
        /// 获取可查询的任务集
        /// </summary>
        /// <returns>IQueryable任务集</returns>
        IQueryable<DeliveryTask> GetQueryable();
    }
}