using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 购物车仓储接口
    /// </summary>
    public interface IShoppingCartRepository
    {
        /// <summary>
        /// 获取所有购物车
        /// </summary>
        /// <returns>购物车列表</returns>
        Task<IEnumerable<ShoppingCart>> GetAllAsync();

        /// <summary>
        /// 根据ID获取购物车
        /// </summary>
        /// <param name="id">购物车ID</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart?> GetByIdAsync(int id);

        /// <summary>
        /// 获取指定店铺下用户的活跃购物车
        /// </summary>
        /// <param name="customerId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>购物车</returns>
        Task<ShoppingCart?> GetActiveCartWithStoreFilterAsync(int customerId, int storeId);

        /// <summary>
        /// 添加购物车
        /// </summary>
        /// <param name="shoppingcart">购物车</param>
        /// <returns>任务</returns>
        Task AddAsync(ShoppingCart shoppingcart);

        /// <summary>
        /// 更新购物车
        /// </summary>
        /// <param name="shoppingcart">购物车</param>
        /// <returns>任务</returns>
        Task UpdateAsync(ShoppingCart shoppingcart);

        /// <summary>
        /// 删除购物车
        /// </summary>
        /// <param name="shoppingcart">购物车</param>
        /// <returns>任务</returns>
        Task DeleteAsync(ShoppingCart shoppingcart);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}