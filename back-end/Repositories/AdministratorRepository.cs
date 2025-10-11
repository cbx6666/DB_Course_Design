using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 管理员仓储
    /// </summary>
    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns>管理员列表</returns>
        public async Task<IEnumerable<Administrator>> GetAllAsync()
        {
            return await _context.Administrators
                                 .Include(a => a.User)
                                 .Include(a => a.ReviewComments)
                                    .ThenInclude(rc => rc.Comment)
                                 .Include(a => a.Supervise_s)
                                      .ThenInclude(s => s.Penalty)
                                 .Include(a => a.EvaluateAfterSales)
                                      .ThenInclude(eas => eas.Application)
                                 .Include(a => a.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Complaint)
                                 .ToListAsync();
        }

        /// <summary>
        /// 根据ID获取管理员
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>管理员</returns>
        public async Task<Administrator?> GetByIdAsync(int id)
        {
            return await _context.Administrators
                                 .Include(a => a.User)
                                 .Include(a => a.ReviewComments)
                                    .ThenInclude(rc => rc.Comment)
                                 .Include(a => a.Supervise_s)
                                      .ThenInclude(s => s.Penalty)
                                 .Include(a => a.EvaluateAfterSales)
                                      .ThenInclude(eas => eas.Application)
                                 .Include(a => a.EvaluateComplaints)
                                     .ThenInclude(ec => ec.Complaint)
                                 .FirstOrDefaultAsync(a => a.UserID == id);
        }

        /// <summary>
        /// 根据管理实体获取管理员
        /// </summary>
        /// <param name="managedEntity">管理实体</param>
        /// <returns>管理员列表</returns>
        public async Task<IEnumerable<Administrator>> GetAdministratorsByManagedEntityAsync(string managedEntity)
        {
            return await _context.Administrators
                .Include(a => a.User)
                .Where(a => a.ManagedEntities.Contains(managedEntity))
                .ToListAsync();
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateAdministratorAsync(Administrator administrator)
        {
            try
            {
                _context.Administrators.Update(administrator);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据管理员ID获取售后申请
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>售后申请列表</returns>
        public async Task<IEnumerable<AfterSaleApplication>> GetAfterSaleApplicationsByAdminIdAsync(int adminId)
        {
            var applications = await _context.Evaluate_AfterSales
                                             .Where(eas => eas.AdminID == adminId)
                                             .Select(eas => eas.Application)
                                             .ToListAsync();

            return applications;
        }

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="applicationId">申请ID</param>
        /// <returns>售后申请</returns>
        public async Task<AfterSaleApplication?> GetAfterSaleApplicationByIdAsync(int applicationId)
        {
            return await _context.AfterSaleApplications
                .FirstOrDefaultAsync(app => app.ApplicationID == applicationId);
        }

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateAfterSaleApplicationAsync(AfterSaleApplication application)
        {
            try
            {
                _context.AfterSaleApplications.Update(application);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据管理员ID获取配送投诉
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>配送投诉列表</returns>
        public async Task<IEnumerable<DeliveryComplaint>> GetDeliveryComplaintsByAdminIdAsync(int adminId)
        {
            var complaints = await _context.Evaluate_Complaints
                                           .Where(ec => ec.AdminID == adminId)
                                           .Include(ec => ec.Complaint)
                                               .ThenInclude(c => c.Courier)
                                                   .ThenInclude(courier => courier.User)
                                           .Select(ec => ec.Complaint)
                                           .ToListAsync();

            return complaints;
        }

        /// <summary>
        /// 根据ID获取配送投诉
        /// </summary>
        /// <param name="complaintId">投诉ID</param>
        /// <returns>配送投诉</returns>
        public async Task<DeliveryComplaint?> GetDeliveryComplaintByIdAsync(int complaintId)
        {
            return await _context.DeliveryComplaints
               .Include(dc => dc.Courier)
                   .ThenInclude(c => c.User)
               .FirstOrDefaultAsync(dc => dc.ComplaintID == complaintId);
        }

        /// <summary>
        /// 更新配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateDeliveryComplaintAsync(DeliveryComplaint complaint)
        {
            try
            {
                _context.DeliveryComplaints.Update(complaint);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据管理员ID获取违规处罚
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        public async Task<IEnumerable<StoreViolationPenalty>> GetViolationPenaltiesByAdminIdAsync(int adminId)
        {
            var penalties = await _context.Supervise_s
                                          .Where(s => s.AdminID == adminId)
                                          .Include(s => s.Penalty)
                                              .ThenInclude(p => p.Store)
                                          .Select(s => s.Penalty)
                                          .ToListAsync();

            return penalties;
        }

        /// <summary>
        /// 根据ID获取违规处罚
        /// </summary>
        /// <param name="penaltyId">处罚ID</param>
        /// <returns>违规处罚</returns>
        public async Task<StoreViolationPenalty?> GetViolationPenaltyByIdAsync(int penaltyId)
        {
            return await _context.StoreViolationPenalties
                .Include(p => p.Store)
                .Include(p => p.Store.Seller)
                .Include(p => p.Store.Seller.User)
                .FirstOrDefaultAsync(p => p.PenaltyID == penaltyId);
        }

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="penalty">违规处罚</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateViolationPenaltyAsync(StoreViolationPenalty penalty)
        {
            try
            {
                _context.StoreViolationPenalties.Update(penalty);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据管理员ID获取评论审核
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>评论列表</returns>
        public async Task<IEnumerable<Comment>> GetReviewCommentsByAdminIdAsync(int adminId)
        {
            var comments = await _context.Review_Comments
                                        .Where(rc => rc.AdminID == adminId)
                                        .Include(rc => rc.Comment)
                                            .ThenInclude(c => c.Commenter)
                                                .ThenInclude(customer => customer.User)
                                        .Select(rc => rc.Comment)
                                        .ToListAsync();

            return comments;
        }

        /// <summary>
        /// 根据ID获取评论审核
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>评论</returns>
        public async Task<Comment?> GetReviewCommentByIdAsync(int commentId)
        {
            return await _context.Comments
                .Include(c => c.Commenter)
                .Include(c => c.Commenter.User)
                .Include(c => c.Store)
                .Include(c => c.FoodOrder)
                .FirstOrDefaultAsync(c => c.CommentID == commentId);
        }

        /// <summary>
        /// 更新评论审核
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>是否成功</returns>
        public async Task<bool> UpdateReviewCommentAsync(Comment comment)
        {
            try
            {
                _context.Comments.Update(comment);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task AddAsync(Administrator administrator)
        {
            await _context.Administrators.AddAsync(administrator);
            await SaveAsync();
        }

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task UpdateAsync(Administrator administrator)
        {
            _context.Administrators.Update(administrator);
            await SaveAsync();
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        public async Task DeleteAsync(Administrator administrator)
        {
            _context.Administrators.Remove(administrator);
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