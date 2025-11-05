using BackEnd.DTOs.Comment;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 评论服务接口（消费者侧）
    /// </summary>
    public interface ICustomerCommentService
    {
        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论列表</returns>
        Task<List<CustomerCommentDto>> GetCommentListAsync(int storeId);

        /// <summary>
        /// 获取评论状态
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论状态</returns>
        Task<CommentStateDto> GetCommentStateAsync(int storeId);

        /// <summary>
        /// 提交评论
        /// </summary>
        /// <param name="dto">评论请求</param>
        /// <returns>提交任务</returns>
        Task SubmitCommentAsync(CreateCommentDto dto);
    }
}
