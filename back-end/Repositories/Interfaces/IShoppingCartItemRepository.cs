using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 购物车项仓储接口
    /// </summary>
    public interface IShoppingCartItemRepository
    {
        /// <summary>
        /// 获取所有购物车项
        /// </summary>
        /// <returns>购物车项列表</returns>
        Task<IEnumerable<ShoppingCartItem>> GetAllAsync();

        /// <summary>
        /// 根据ID获取购物车项
        /// </summary>
        /// <param name="id">购物车项ID</param>
        /// <returns>购物车项</returns>
        Task<ShoppingCartItem?> GetByIdAsync(int id);

        /// <summary>
        /// 根据购物车ID获取购物车项
        /// </summary>
        /// <param name="cartId">购物车ID</param>
        /// <returns>购物车项列表</returns>
        Task<IEnumerable<ShoppingCartItem>> GetByCartIdAsync(int cartId);

        /// <summary>
        /// 添加购物车项
        /// </summary>
        /// <param name="shoppingcartitem">购物车项</param>
        /// <returns>任务</returns>
        Task AddAsync(ShoppingCartItem shoppingcartitem);

        /// <summary>
        /// 更新购物车项
        /// </summary>
        /// <param name="shoppingcartitem">购物车项</param>
        /// <returns>任务</returns>
        Task UpdateAsync(ShoppingCartItem shoppingcartitem);

        /// <summary>
        /// 删除购物车项
        /// </summary>
        /// <param name="shoppingcartitem">购物车项</param>
        /// <returns>任务</returns>
        Task DeleteAsync(ShoppingCartItem shoppingcartitem);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}