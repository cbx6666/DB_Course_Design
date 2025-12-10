using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 收藏项仓储
    /// </summary>
    public class FavoriteItemRepository : IFavoriteItemRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public FavoriteItemRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有收藏项
        /// </summary>
        /// <returns>收藏项列表</returns>
        public async Task<IEnumerable<FavoriteItem>> GetAllAsync()
        {
            return await _context.FavoriteItems
                                 .Include(fi => fi.Store)
                                 .Include(fi => fi.Folder)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取收藏项
        /// </summary>
        /// <param name="id">收藏项ID</param>
        /// <returns>收藏项</returns>
        public async Task<FavoriteItem?> GetByIdAsync(int id)
        {
            return await _context.FavoriteItems
                                 .Include(fi => fi.Store)
                                 .Include(fi => fi.Folder)
                                 .FirstOrDefaultAsync(fi => fi.ItemID == id);
        }

        /// <summary>
        /// 添加收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        public async Task AddAsync(FavoriteItem item)
        {
            await _context.FavoriteItems.AddAsync(item);
            await SaveAsync();
        }

        /// <summary>
        /// 更新收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(FavoriteItem item)
        {
            _context.FavoriteItems.Update(item);
            await SaveAsync();
        }

        /// <summary>
        /// 删除收藏项
        /// </summary>
        /// <param name="item">收藏项</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(FavoriteItem item)
        {
            _context.FavoriteItems.Remove(item);
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

        /// <summary>
        /// 检查同一收藏夹是否已存在指定店铺
        /// </summary>
        public async Task<bool> ExistsAsync(int folderId, int storeId)
        {
            // 使用 CountAsync 兼容 Oracle，避免生成 TRUE/FALSE 标识符
            var count = await _context.FavoriteItems
                .CountAsync(fi => fi.FolderID == folderId && fi.StoreID == storeId);
            return count > 0;
        }

        /// <summary>
        /// 根据收藏夹与店铺获取收藏项
        /// </summary>
        public async Task<FavoriteItem?> GetByFolderAndStoreAsync(int folderId, int storeId)
        {
            return await _context.FavoriteItems
                .Include(fi => fi.Store)
                .Include(fi => fi.Folder)
                .FirstOrDefaultAsync(fi => fi.FolderID == folderId && fi.StoreID == storeId);
        }
    }
}