using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 评论审核关系仓储接口
    /// </summary>
    public interface IReview_CommentRepository
    {
        /// <summary>
        /// 获取所有评论审核关系
        /// </summary>
        /// <returns>关系列表</returns>
        Task<IEnumerable<Review_Comment>> GetAllAsync();

        /// <summary>
        /// 根据管理员与评论ID获取关系
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="commentId">评论ID</param>
        /// <returns>关系实体</returns>
        Task<Review_Comment?> GetByIdAsync(int adminId, int commentId);

        /// <summary>
        /// 新增关系
        /// </summary>
        /// <param name="review_comment">关系实体</param>
        /// <returns>任务</returns>
        Task AddAsync(Review_Comment review_comment);

        /// <summary>
        /// 更新关系
        /// </summary>
        /// <param name="review_comment">关系实体</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Review_Comment review_comment);

        /// <summary>
        /// 删除关系
        /// </summary>
        /// <param name="review_comment">关系实体</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Review_Comment review_comment);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}