using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 菜品仓储接口
    /// </summary>
    public interface IDishRepository
    {
        /// <summary>
        /// 获取所有菜品
        /// </summary>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<Dish>> GetAllAsync();

        /// <summary>
        /// 根据菜品种类ID获取菜品列表
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<Dish>> GetByCategoryIdAsync(int categoryId);

        /// <summary>
        /// 根据ID获取菜品
        /// </summary>
        /// <param name="id">菜品ID</param>
        /// <returns>菜品</returns>
        Task<Dish?> GetByIdAsync(int id);

        /// <summary>
        /// 添加菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        Task AddAsync(Dish dish);

        /// <summary>
        /// 更新菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Dish dish);

        /// <summary>
        /// 删除菜品
        /// </summary>
        /// <param name="dish">菜品</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Dish dish);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}