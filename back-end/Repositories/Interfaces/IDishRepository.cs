using BackEnd.DTOs.Menu;
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
        /// 获取指定店铺的轻量化菜品信息（分页）
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <param name="categoryId">分类ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns>结果与是否还有更多</returns>
        Task<(List<MenuBasicResponseDto> Items, bool HasMore)> GetMenuBasicByStoreIdAsync(int storeId, int? categoryId, int page, int pageSize);

        /// <summary>
        /// 根据店铺ID获取菜品
        /// </summary>
        /// <param name="storeId">店铺ID</param>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<Dish>> GetDishesByStoreIdAsync(int storeId);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}