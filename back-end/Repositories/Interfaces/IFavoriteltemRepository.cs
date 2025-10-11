using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 收藏项仓储接口
    /// </summary>
    public interface IFavoriteItemRepository
    {
        /// <summary>
        /// 获取所有收藏项
        /// </summary>
        /// <returns>收藏项列表</returns>
        Task<IEnumerable<FavoriteItem>> GetAllAsync();

        /// <summary>
        /// 根据ID获取收藏项
        /// </summary>
        /// <param name="id">收藏项ID</param>
        /// <returns>收藏项</returns>
        Task<FavoriteItem?> GetByIdAsync(int id);

        /// <summary>
        /// 添加收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        Task AddAsync(FavoriteItem item);

        /// <summary>
        /// 更新收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        Task UpdateAsync(FavoriteItem item);

        /// <summary>
        /// 删除收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        Task DeleteAsync(FavoriteItem item);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}