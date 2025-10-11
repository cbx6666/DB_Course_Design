using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 售后评价仓储接口
    /// </summary>
    public interface IEvaluate_AfterSaleRepository
    {
        /// <summary>
        /// 获取所有售后评价
        /// </summary>
        /// <returns>售后评价列表</returns>
        Task<IEnumerable<Evaluate_AfterSale>> GetAllAsync();

        /// <summary>
        /// 根据管理员与申请ID获取售后评价
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="applicationId">申请ID</param>
        /// <returns>售后评价</returns>
        Task<Evaluate_AfterSale?> GetByIdAsync(int adminId, int applicationId);

        /// <summary>
        /// 添加售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        Task AddAsync(Evaluate_AfterSale evaluateAfterSale);

        /// <summary>
        /// 更新售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Evaluate_AfterSale evaluateAfterSale);

        /// <summary>
        /// 删除售后评价
        /// </summary>
        /// <param name="evaluateAfterSale">售后评价</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Evaluate_AfterSale evaluateAfterSale);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}