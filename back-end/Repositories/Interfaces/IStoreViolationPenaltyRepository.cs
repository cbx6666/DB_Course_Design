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

        /// <summary>
        /// 根据用户ID获取店铺举报列表（包含店铺信息）
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <returns>店铺举报列表</returns>
        Task<List<StoreViolationPenalty>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// 根据骑手ID获取店铺举报列表（包含店铺信息）
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <returns>店铺举报列表</returns>
        Task<List<StoreViolationPenalty>> GetByCourierIdAsync(int courierId);

        /// <summary>
        /// 根据管理员ID获取违规处罚列表（包含店铺信息）
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        Task<List<StoreViolationPenalty>> GetByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据用户ID和店铺ID获取未完成的举报列表
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>未完成的举报列表</returns>
        Task<List<StoreViolationPenalty>> GetPendingByCustomerIdAndStoreIdAsync(int customerId, int storeId);

        /// <summary>
        /// 根据骑手ID和店铺ID获取未完成的举报列表
        /// </summary>
        /// <param name="courierId">骑手ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>未完成的举报列表</returns>
        Task<List<StoreViolationPenalty>> GetPendingByCourierIdAndStoreIdAsync(int courierId, int storeId);
    }
}