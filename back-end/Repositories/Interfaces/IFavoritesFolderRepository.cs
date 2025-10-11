using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 收藏夹仓储接口
    /// </summary>
    public interface IFavoritesFolderRepository
    {
        /// <summary>
        /// 获取所有收藏夹
        /// </summary>
        /// <returns>收藏夹列表</returns>
        Task<IEnumerable<FavoritesFolder>> GetAllAsync();

        /// <summary>
        /// 根据ID获取收藏夹
        /// </summary>
        /// <param name="id">收藏夹ID</param>
        /// <returns>收藏夹</returns>
        Task<FavoritesFolder?> GetByIdAsync(int id);

        /// <summary>
        /// 添加收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        Task AddAsync(FavoritesFolder favoritesfolder);

        /// <summary>
        /// 更新收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        Task UpdateAsync(FavoritesFolder favoritesfolder);

        /// <summary>
        /// 删除收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        Task DeleteAsync(FavoritesFolder favoritesfolder);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}