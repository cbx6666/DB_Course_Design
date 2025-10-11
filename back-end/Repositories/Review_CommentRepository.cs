using BackEnd.Models;
using BackEnd.Data;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 评论审核关系仓储
    /// </summary>
    public class Review_CommentRepository : IReview_CommentRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public Review_CommentRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有评论审核关系
        /// </summary>
        /// <returns>评论审核关系列表</returns>
        public async Task<IEnumerable<Review_Comment>> GetAllAsync()
        {
            return await _context.Review_Comments
                                 .Include(rc => rc.Admin)   // 关联管理员
                                 .Include(rc => rc.Comment) // 关联评论
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据管理员ID与评论ID获取关系
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="commentId">评论ID</param>
        /// <returns>评论审核关系</returns>
        public async Task<Review_Comment?> GetByIdAsync(int adminId, int commentId)
        {
            return await _context.Review_Comments
                                 .Include(rc => rc.Admin)
                                 .Include(rc => rc.Comment)
                                 .FirstOrDefaultAsync(rc => rc.AdminID == adminId && rc.CommentID == commentId);
        }

        /// <summary>
        /// 新增评论审核关系
        /// </summary>
        /// <param name="reviewComment">关系实体</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Review_Comment reviewComment)
        {
            await _context.Review_Comments.AddAsync(reviewComment);
            await SaveAsync();
        }

        /// <summary>
        /// 更新评论审核关系
        /// </summary>
        /// <param name="reviewComment">关系实体</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Review_Comment reviewComment)
        {
            _context.Review_Comments.Update(reviewComment);
            await SaveAsync();
        }

        /// <summary>
        /// 删除评论审核关系
        /// </summary>
        /// <param name="reviewComment">关系实体</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Review_Comment reviewComment)
        {
            _context.Review_Comments.Remove(reviewComment);
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