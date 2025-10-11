using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 管理员仓储接口
    /// </summary>
    public interface IAdministratorRepository
    {
        /// <summary>
        /// 获取所有管理员
        /// </summary>
        /// <returns>管理员列表</returns>
        Task<IEnumerable<Administrator>> GetAllAsync();

        /// <summary>
        /// 根据ID获取管理员
        /// </summary>
        /// <param name="id">管理员ID</param>
        /// <returns>管理员</returns>
        Task<Administrator?> GetByIdAsync(int id);

        /// <summary>
        /// 根据管理实体获取管理员
        /// </summary>
        /// <param name="managedEntity">管理实体</param>
        /// <returns>管理员列表</returns>
        Task<IEnumerable<Administrator>> GetAdministratorsByManagedEntityAsync(string managedEntity);

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAdministratorAsync(Administrator administrator);

        /// <summary>
        /// 根据管理员ID获取售后申请
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<AfterSaleApplication>> GetAfterSaleApplicationsByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="applicationId">申请ID</param>
        /// <returns>售后申请</returns>
        Task<AfterSaleApplication?> GetAfterSaleApplicationByIdAsync(int applicationId);

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAfterSaleApplicationAsync(AfterSaleApplication application);

        /// <summary>
        /// 根据管理员ID获取配送投诉
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>配送投诉列表</returns>
        Task<IEnumerable<DeliveryComplaint>> GetDeliveryComplaintsByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据ID获取配送投诉
        /// </summary>
        /// <param name="complaintId">投诉ID</param>
        /// <returns>配送投诉</returns>
        Task<DeliveryComplaint?> GetDeliveryComplaintByIdAsync(int complaintId);

        /// <summary>
        /// 更新配送投诉
        /// </summary>
        /// <param name="complaint">配送投诉</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateDeliveryComplaintAsync(DeliveryComplaint complaint);

        /// <summary>
        /// 根据管理员ID获取违规处罚
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>违规处罚列表</returns>
        Task<IEnumerable<StoreViolationPenalty>> GetViolationPenaltiesByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据ID获取违规处罚
        /// </summary>
        /// <param name="penaltyId">处罚ID</param>
        /// <returns>违规处罚</returns>
        Task<StoreViolationPenalty?> GetViolationPenaltyByIdAsync(int penaltyId);

        /// <summary>
        /// 更新违规处罚
        /// </summary>
        /// <param name="penalty">违规处罚</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateViolationPenaltyAsync(StoreViolationPenalty penalty);

        /// <summary>
        /// 根据管理员ID获取评论审核
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>评论列表</returns>
        Task<IEnumerable<Comment>> GetReviewCommentsByAdminIdAsync(int adminId);

        /// <summary>
        /// 根据ID获取评论审核
        /// </summary>
        /// <param name="commentId">评论ID</param>
        /// <returns>评论</returns>
        Task<Comment?> GetReviewCommentByIdAsync(int commentId);

        /// <summary>
        /// 更新评论审核
        /// </summary>
        /// <param name="comment">评论</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateReviewCommentAsync(Comment comment);

        /// <summary>
        /// 添加管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task AddAsync(Administrator administrator);

        /// <summary>
        /// 更新管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Administrator administrator);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="administrator">管理员</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Administrator administrator);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}