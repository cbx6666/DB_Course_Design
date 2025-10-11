using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 订单仓储接口
    /// </summary>
    public interface IFoodOrderRepository
    {
        /// <summary>
        /// 根据用户ID获取订单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单列表</returns>
        Task<IEnumerable<FoodOrder>> GetByUserIdAsync(int userId);

        /// <summary>
        /// 获取所有订单
        /// </summary>
        /// <returns>订单列表</returns>
        Task<IEnumerable<FoodOrder>> GetAllAsync();

        /// <summary>
        /// 根据ID获取订单
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns>订单</returns>
        Task<FoodOrder?> GetByIdAsync(int id);

        /// <summary>
        /// 根据客户ID获取按时间倒序的订单
        /// </summary>
        /// <param name="customerId">客户ID</param>
        /// <returns>订单列表</returns>
        Task<List<FoodOrder>> GetOrdersByCustomerIdOrderedByDateAsync(int customerId);

        /// <summary>
        /// 根据购物车ID获取订单
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>订单</returns>
        Task<FoodOrder?> GetByCartIdAsync(int cartId);

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="foodorder">订单</param>
        /// <returns>任务</returns>
        Task AddAsync(FoodOrder foodorder);

        /// <summary>
        /// 更新订单
        /// </summary>
        /// <param name="foodorder">订单</param>
        /// <returns>任务</returns>
        Task UpdateAsync(FoodOrder foodorder);

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="foodorder">订单</param>
        /// <returns>任务</returns>
        Task DeleteAsync(FoodOrder foodorder);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}