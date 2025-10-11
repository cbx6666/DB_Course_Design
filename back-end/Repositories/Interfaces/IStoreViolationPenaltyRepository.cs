using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 店铺违规处罚仓储接口
    /// </summary>
    public interface IStoreViolationPenaltyRepository
    {
        /// <summary>
        /// 获取所有违规处罚
        /// </summary>
        /// <returns>违规处罚列表</returns>
        Task<IEnumerable<StoreViolationPenalty>> GetAllAsync();

        /// <summary>
        /// 根据ID获取违规处罚
        /// </summary>
        /// <param name="id">处罚ID</param>
        /// <returns>违规处罚</returns>
        Task<StoreViolationPenalty?> GetByIdAsync(int id);

        /// <summary>
        /// 根据商家ID获取违规处罚
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>违规处罚列表</returns>
        Task<IEnumerable<StoreViolationPenalty>> GetBySellerIdAsync(int sellerId);

        /// <summary>
        /// 添加违规处罚
        /// </summary>
        /// <param name="storeviolationpenalty">违规处罚</param>
        /// <returns>任务</returns>
        Task AddAsync(StoreViolationPenalty storeviolationpenalty);

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="storeviolationpenalty">违规处罚</param>
        /// <returns>任务</returns>
        Task UpdateAsync(StoreViolationPenalty storeviolationpenalty);

        /// <summary>
        /// 删除违规处罚
        /// </summary>
        /// <param name="storeviolationpenalty">违规处罚</param>
        /// <returns>任务</returns>
        Task DeleteAsync(StoreViolationPenalty storeviolationpenalty);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}