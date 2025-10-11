using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 评论仓储
    /// </summary>
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有评论
        /// </summary>
        /// <returns>评论列表</returns>
        public async Task<IEnumerable<Comment>> GetAllAsync()
        {
            // 预加载所有关联的实体数据
            return await _context.Comments
                                 .Include(c => c.ReplyToComment) // 加载被回复的评论
                                 .Include(c => c.Store)          // 加载评论所属的店铺
                                 .Include(c => c.FoodOrder)      // 加载评论所属的订单
                                 .Include(c => c.Commenter)      // 加载发表评论的顾客
                                     .ThenInclude(cu => cu.User)
                                 .Include(c => c.CommentReplies) // 加载评论的回复
                                 .Include(c => c.ReviewComments) // 加载审核评论的管理员
                                     .ThenInclude(rc => rc.Admin)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取评论
        /// </summary>
        /// <param name="id">评论ID</param>
        /// <returns>评论</returns>
        public async Task<Comment?> GetByIdAsync(int id)
        {
            // 对于单个查询，同样建议预加载关联数据
            return await _context.Comments
                                 .Include(c => c.ReplyToComment)
                                 .Include(c => c.Store)
                                 .Include(c => c.FoodOrder)
                                 .Include(c => c.Commenter)
                                 .Include(c => c.ReviewComments)
                                     .ThenInclude(rc => rc.Admin)
                                 .FirstOrDefaultAsync(c => c.CommentID == id);
        }

        /// <summary>
        /// 根据商家ID获取评论
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>评论列表</returns>
        public async Task<IEnumerable<Comment>> GetBySellerAsync(int sellerId)
        {
            return await _context.Comments
                                .Include(c => c.Store)          // 加载评论所属的店铺
                                    .ThenInclude(s => s!.Seller)
                                .Include(c => c.FoodOrder)      // 加载评论所属的订单
                                .Include(c => c.Commenter)      // 加载发表评论的顾客
                                .Where(c => c.Store!.SellerID == sellerId
                                    && c.CommentState == CommentState.Completed)
                                .OrderBy(c => c.CommentID)
                                .ToListAsync();
        }

        /// <summary>
        /// 添加评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            await SaveAsync();
        }

        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await SaveAsync();
        }

        /// <summary>
        /// 删除评论
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
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