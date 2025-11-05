using BackEnd.DTOs.Comment;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 评论服务接口（管理员侧）
    /// </summary>
    public interface IAdminCommentService
    {
        /// <summary>
        /// 获取管理员的评论列表
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>评论列表</returns>
        Task<IEnumerable<AdminCommentDetailDto>> GetCommentsForAdminAsync(int adminId);

        /// <summary>
        /// 更新评论
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        Task<UpdateCommentReviewResponseDto> UpdateCommentAsync(UpdateCommentReviewDto request);
    }
}
