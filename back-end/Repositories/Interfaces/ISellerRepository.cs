using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 商家仓储接口
    /// </summary>
    public interface ISellerRepository
    {
        /// <summary>
        /// 获取所有商家
        /// </summary>
        /// <returns>商家列表</returns>
        Task<IEnumerable<Seller>> GetAllAsync();

        /// <summary>
        /// 根据ID获取商家
        /// </summary>
        /// <param name="id">商家用户ID</param>
        /// <returns>商家</returns>
        Task<Seller?> GetByIdAsync(int id);

        /// <summary>
        /// 添加商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        Task AddAsync(Seller seller);

        /// <summary>
        /// 更新商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Seller seller);

        /// <summary>
        /// 删除商家
        /// </summary>
        /// <param name="seller">商家</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Seller seller);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}