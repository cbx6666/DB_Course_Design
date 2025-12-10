using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 店铺举报惩罚服务接口（商家侧）
    /// </summary>
    public interface IMerchantPenaltyService
    {
        /// <summary>
        /// 获取处罚记录列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="keyword">搜索关键词</param>
        /// <param name="field">筛选字段（id | reason）</param>
        /// <returns>处罚记录列表</returns>
        Task<List<MerchantPenaltyRecordDto>> GetPenaltiesAsync(int sellerId, string? keyword, string? field);

        /// <summary>
        /// 根据ID获取处罚记录
        /// </summary>
        /// <param name="id">处罚记录ID</param>
        /// <returns>处罚记录详情</returns>
        Task<MerchantPenaltyRecordDto?> GetPenaltyByIdAsync(string id);
    }
}
