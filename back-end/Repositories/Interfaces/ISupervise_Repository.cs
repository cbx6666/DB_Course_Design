using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 管理员监督-处罚关系仓储接口
    /// </summary>
    public interface ISupervise_Repository
    {
        /// <summary>
        /// 获取所有监督-处罚关系
        /// </summary>
        /// <returns>关系列表</returns>
        Task<IEnumerable<Supervise_>> GetAllAsync();

        /// <summary>
        /// 根据管理员与处罚ID获取关系
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="penaltyId">处罚ID</param>
        /// <returns>关系实体</returns>
        Task<Supervise_?> GetByIdAsync(int adminId, int penaltyId);

        /// <summary>
        /// 新增关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        Task AddAsync(Supervise_ supervise_);

        /// <summary>
        /// 更新关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Supervise_ supervise_);

        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="supervise_">关系实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Supervise_ supervise_);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}