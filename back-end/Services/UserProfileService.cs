using BackEnd.Data;
using BackEnd.Models;
using BackEnd.DTOs.User;
using Microsoft.EntityFrameworkCore;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户个人资料服务
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        public UserProfileService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// 获取用户个人资料
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户个人资料</returns>
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Name = user.Username,
                PhoneNumber = user.PhoneNumber,
                Image = user.Avatar
            };
        }

        /// <summary>
        /// 获取用户地址信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户地址信息</returns>
        public async Task<UserAddressDto?> GetUserAddressAsync(int userId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return null;
            }

            var customer = userWithCustomer.Customer;
            var recipientName = !string.IsNullOrEmpty(userWithCustomer.FullName) 
                ? userWithCustomer.FullName 
                : userWithCustomer.Username;

            return new UserAddressDto
            {
                Name = recipientName,
                PhoneNumber = userWithCustomer.PhoneNumber,
                Address = customer.DefaultAddress ?? "xx市xx区xx街道xx号"
            };
        }
    }
}