using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 售后申请仓储接口
    /// </summary>
    public interface IAfterSaleApplicationRepository
    {
        /// <summary>
        /// 获取所有售后申请
        /// </summary>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<AfterSaleApplication>> GetAllAsync();

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="id">申请ID</param>
        /// <returns>售后申请</returns>
        Task<AfterSaleApplication?> GetByIdAsync(int id);

        /// <summary>
        /// 根据订单ID获取售后申请
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<AfterSaleApplication>> GetByOrderIdAsync(int orderId);

        /// <summary>
        /// 根据商家ID获取售后申请
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>售后申请列表</returns>
        Task<IEnumerable<AfterSaleApplication>> GetBySellerIdAsync(int sellerId);

        /// <summary>
        /// 添加售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        Task AddAsync(AfterSaleApplication application);

        /// <summary>
        /// 更新售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        Task UpdateAsync(AfterSaleApplication application);

        /// <summary>
        /// 删除售后申请
        /// </summary>
        /// <param name="application">售后申请</param>
        /// <returns>任务</returns>
        Task DeleteAsync(AfterSaleApplication application);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}