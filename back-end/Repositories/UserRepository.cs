using BackEnd.Data;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories
{
    /// <summary>
    /// 用户数据访问层
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context">数据库上下文</param>
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns>用户列表</returns>
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// 根据ID获取用户
        /// </summary>
        /// <param name="id">用户ID</param>
        /// <returns>用户信息</returns>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Customer)
                .FirstOrDefaultAsync(u => u.UserID == id);
        }

        /// <summary>
        /// 根据手机号获取用户
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>用户信息</returns>
        public async Task<User?> GetByPhoneAsync(long phoneNumber)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
        }

        /// <summary>
        /// 根据邮箱获取用户
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>用户信息</returns>
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="user">用户信息</param>
        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await SaveAsync();
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="user">用户信息</param>
        public async Task UpdateAsync(User user)
        {
            _context.Set<User>().Update(user);
            await SaveAsync();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="user">用户信息</param>
        public async Task DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            await SaveAsync();
        }

        /// <summary>
        /// 保存更改
        /// </summary>
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 检查手机号是否已存在
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <returns>是否存在</returns>
        public async Task<bool> ExistsByPhoneAsync(string phone)
        {
            if (long.TryParse(phone, out long phoneNumber))
            {
                return await _context.Users.CountAsync(u => u.PhoneNumber == phoneNumber) > 0;
            }
            return false;
        }
    }
}