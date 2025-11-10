using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 管理员仓储接口
    /// </summary>
    public interface IAdministratorRepository
    {
        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns>管理员列表</returns>
        Task<IEnumerable<Administrator>> GetAllAsync();

        /// <summary>
        /// 根据ID获取管理员
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>管理员</returns>
        Task<Administrator?> GetByIdAsync(int id);

        /// <summary>
        /// 根据管理实体获取管理员
        /// </summary>
        /// <param name="managedEntity">管理实体</param>
        /// <returns>管理员列表</returns>
        Task<IEnumerable<Administrator>> GetAdministratorsByManagedEntityAsync(string managedEntity);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAdministratorAsync(Administrator administrator);

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task AddAsync(Administrator administrator);

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Administrator administrator);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Administrator administrator);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}