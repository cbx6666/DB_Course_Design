using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 客户仓储接口
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// 获取所有客户
        /// </summary>
        /// <returns>客户列表</returns>
        Task<IEnumerable<Customer>> GetAllAsync();

        /// <summary>
        /// 根据ID获取客户
        /// </summary>
        /// <param name="id">客户用户ID</param>
        /// <returns>客户</returns>
        Task<Customer?> GetByIdAsync(int id);

        /// <summary>
        /// 添加客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        Task AddAsync(Customer customer);

        /// <summary>
        /// 更新客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Customer customer);

        /// <summary>
        /// 删除客户
        /// </summary>
        /// <param name="customer">客户</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Customer customer);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}