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

        /// <summary>
        /// 根据评论者ID获取评论列表（包含店铺信息）
        /// </summary>
        /// <param name="commenterId">评论者ID</param>
        /// <returns>评论列表</returns>
        Task<List<Comment>> GetByCommenterIdAsync(int commenterId);

        /// <summary>
        /// 根据管理员ID获取评论审核列表（包含评论者信息）
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>评论列表</returns>
        Task<List<Comment>> GetByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据订单ID获取评论列表
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>评论列表</returns>
        Task<List<Comment>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// 根据用户ID和店铺ID获取未完成的评论列表
        /// </summary>
        /// <param name="commenterId">评论者ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>未完成的评论列表</returns>
        Task<List<Comment>> GetPendingByCommenterIdAndStoreIdAsync(int commenterId, int storeId);

        /// <summary>
        /// 根据店铺ID获取评论列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论列表</returns>
        Task<List<Comment>> GetByStoreIdAsync(int storeId);

        /// <summary>
        /// 检查是否已存在对指定评论的回复
        /// </summary>
        /// <param name="commentId">原始评论ID</param>
        /// <returns>如果存在回复则返回回复，否则返回null</returns>
        Task<Comment?> GetReplyByCommentIdAsync(int commentId);
    }
}