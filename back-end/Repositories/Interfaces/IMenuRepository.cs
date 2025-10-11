using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 菜单仓储接口
    /// </summary>
    public interface IMenuRepository
    {
        /// <summary>
        /// 获取所有菜单
        /// </summary>
        /// <returns>菜单列表</returns>
        Task<IEnumerable<Menu>> GetAllAsync();

        /// <summary>
        /// 根据ID获取菜单
        /// </summary>
        /// <param name="id">菜单ID</param>
        /// <returns>菜单</returns>
        Task<Menu?> GetByIdAsync(int id);

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        Task AddAsync(Menu menu);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        Task UpdateAsync(Menu menu);

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menu">菜单</param>
        /// <returns>任务</returns>
        Task DeleteAsync(Menu menu);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();
    }
}