using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 骑手仓储接口
    /// </summary>
    public interface ICourierRepository
    {
        /// <summary>
        /// 获取所有骑手
        /// </summary>
        /// <returns>骑手列表</returns>
        Task<IEnumerable<Courier>> GetAllAsync();

        /// <summary>
        /// 根据ID获取骑手
        /// </summary>
        /// <param name="id">骑手用户ID</param>
        /// <returns>骑手</returns>
        Task<Courier?> GetByIdAsync(int id);

        /// <summary>
        /// 添加骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        Task AddAsync(Courier courier);

        /// <summary>
        /// 更新骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Courier courier);

        /// <summary>
        /// 删除骑手
        /// </summary>
        /// <param name="courier">骑手</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Courier courier);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}