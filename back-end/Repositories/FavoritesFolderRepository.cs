using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 收藏夹仓储
    /// </summary>
    public class FavoritesFolderRepository : IFavoritesFolderRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public FavoritesFolderRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有收藏夹
        /// </summary>
        /// <returns>收藏夹列表</returns>
        public async Task<IEnumerable<FavoritesFolder>> GetAllAsync()
        {
            return await _context.FavoritesFolders
                                 .Include(ff => ff.Customer)      // 关联顾客
                                 .Include(ff => ff.FavoriteItems) // 收藏夹中的项目
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取收藏夹
        /// </summary>
        /// <param name="id">收藏夹ID</param>
        /// <returns>收藏夹</returns>
        public async Task<FavoritesFolder?> GetByIdAsync(int id)
        {
            return await _context.FavoritesFolders
                                 .Include(ff => ff.Customer)
                                 .Include(ff => ff.FavoriteItems)
                                 .FirstOrDefaultAsync(ff => ff.FolderID == id);
        }

        /// <summary>
        /// 添加收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        public async Task AddAsync(FavoritesFolder favoritesfolder)
        {
            _context.FavoritesFolders.Add(favoritesfolder);
            await SaveAsync();
        }

        /// <summary>
        /// 更新收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(FavoritesFolder favoritesfolder)
        {
            _context.FavoritesFolders.Update(favoritesfolder);
            await SaveAsync();
        }

        /// <summary>
        /// 删除收藏夹
        /// </summary>
        /// <param name="favoritesfolder">收藏夹</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(FavoritesFolder favoritesfolder)
        {
            _context.FavoritesFolders.Remove(favoritesfolder);
            await SaveAsync();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}