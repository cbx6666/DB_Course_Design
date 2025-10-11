using BackEnd.DTOs.Penalty;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 处罚服务接口
    /// </summary>
    public interface IPenaltyService
    {
        /// <summary>
        /// 获取处罚记录列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>处罚记录列表</returns>
        Task<List<PenaltyRecordDto>> GetPenaltiesAsync(int sellerId, string? keyword);

        /// <summary>
        /// 根据ID获取处罚记录
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <returns>处罚记录详情</returns>
        Task<PenaltyRecordDto?> GetPenaltyByIdAsync(string id);

        /// <summary>
        /// 申诉处罚
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <param name="appealDto">申诉请求</param>
        /// <returns>申诉结果</returns>
        Task<AppealResponseDto?> AppealPenaltyAsync(string id, AppealDto appealDto);
    }
}