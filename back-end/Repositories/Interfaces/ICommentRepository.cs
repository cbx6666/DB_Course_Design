using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 评论仓储接口
    /// </summary>
    public interface ICommentRepository
    {
        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <returns>评论列表</returns>
        Task<IEnumerable<Comment>> GetAllAsync();

        /// <summary>
        /// 根据ID获取评论
        /// </summary>
        /// <param name="id">评论ID</param>
        /// <returns>评论</returns>
        Task<Comment?> GetByIdAsync(int id);

        /// <summary>
        /// 根据商家ID获取评论
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>评论列表</returns>
        Task<IEnumerable<Comment>> GetBySellerAsync(int sellerId);

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        Task AddAsync(Comment comment);

        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Comment comment);

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Comment comment);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}