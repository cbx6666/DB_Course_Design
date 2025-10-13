using BackEnd.Models;

namespace BackEnd.Repositories.Interfaces
{
    /// <summary>
    /// 用户仓储接口
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户</returns>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>用户</returns>
        Task<User?> GetByPhoneAsync(long phoneNumber);

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>用户</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>任务</returns>
        Task AddAsync(User user);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>任务</returns>
        Task UpdateAsync(User user);

        /// <summary>
        /// 部分更新用户信息（只更新指定字段）
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="username">用户名</param>
        /// <param name="avatar">头像</param>
        /// <returns>任务</returns>
        Task UpdatePartialAsync(int userId, string username, string? avatar);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">用户</param>
        /// <returns>任务</returns>
        Task DeleteAsync(User user);

        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns>任务</returns>
        Task SaveAsync();

        /// <summary>
        /// 检查手机号是否已存在
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>是否存在</returns>
        Task<bool> ExistsByPhoneAsync(string phone);
    }
}
