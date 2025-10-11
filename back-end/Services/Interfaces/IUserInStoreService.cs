using BackEnd.DTOs.User;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 用户店内服务接口
    /// </summary>
    public interface IUserInStoreService
    {
        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="request">店铺请求</param>
        /// <returns>店铺信息</returns>
        Task<StoreResponseDto?> GetStoreInfoAsync(StoreRequestDto request);

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="request">菜单请求</param>
        /// <returns>菜单列表</returns>
        Task<List<MenuResponseDto>> GetMenuAsync(MenuRequestDto request);

        /// <summary>
        /// 获取评论列表
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>评论列表</returns>
        Task<List<CommentResponseDto>> GetCommentListAsync(int storeId);

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

        /// <summary>
        /// 提交店铺举报
        /// </summary>
        /// <param name="dto">举报请求</param>
        /// <returns>提交任务</returns>
        Task SubmitStoreReportAsync(UserStoreReportDto dto);
    }
}