using BackEnd.DTOs.Merchant;
using BackEnd.DTOs.Common;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 商家信息服务（商家侧个人信息管理）
    /// </summary>
    public class MerchantInfoService : IMerchantInfoService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageUploadService _imageUploadService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sellerRepository">商家信息仓储</param>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="env">Web主机环境</param>
        public MerchantInfoService(
            ISellerRepository sellerRepository,
            IUserRepository userRepository,
            IImageUploadService imageUploadService)
        {
            _sellerRepository = sellerRepository;
            _userRepository = userRepository;
            _imageUploadService = imageUploadService;
        }

        // ========== 商家信息管理 ==========

        /// <summary>
        /// 获取商家信息
        /// </summary>
        public async Task<(bool Success, string? Message, MerchantProfileDto? Data)> GetMerchantInfoAsync(int merchantUserId)
        {
            var seller = await _sellerRepository.GetByIdAsync(merchantUserId);
            if (seller == null)
                return (false, "商家不存在", null);

            var user = seller.User;
            if (user == null)
                return (false, "用户信息不存在", null);

            var statusText = seller.BanStatus == SellerState.Normal ? "正常营业" : "封禁中";

            var result = new MerchantProfileDto
            {
                Id = seller.UserID.ToString(),
                Username = user.Username,
                FullName = user.FullName,
                Phone = user.PhoneNumber.ToString(),
                Email = user.Email,
                RegisterTime = seller.SellerRegistrationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Status = statusText,
                Avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "/images/default-avatar.jpg" : user.Avatar
            };

            return (true, null, result);
        }

        /// <summary>
        /// 更新商家信息
        /// </summary>
        public async Task<(bool Success, string? Message, MerchantUpdateResultDto? Data)> UpdateMerchantInfoAsync(int merchantUserId, UpdateMerchantProfileDto dto)
        {
            var user = await _userRepository.GetByIdAsync(merchantUserId);
            if (user == null)
                return (false, "用户不存在", null);

            var updatedFields = new List<string>();

            if (!string.Equals(user.Username, dto.Name, StringComparison.Ordinal))
            {
                user.Username = dto.Name;
                updatedFields.Add("username");
            }

            if (long.TryParse(dto.Phone, out long newPhone) && user.PhoneNumber != newPhone)
            {
                user.PhoneNumber = newPhone;
                updatedFields.Add("phone");
            }

            if (user.Email != dto.Email)
            {
                user.Email = dto.Email;
                updatedFields.Add("email");
            }

            if (updatedFields.Count > 0)
            {
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();

                return (true, null, new MerchantUpdateResultDto
                {
                    UpdatedFields = updatedFields.ToArray(),
                    UpdateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }

            return (true, "没有需要更新的信息", null);
        }

        /// <summary>
        /// 更新商家头像（表单上传）
        /// </summary>
        public async Task<(bool Success, string? Message, string? AvatarUrl)> UpdateMerchantAvatarAsync(int merchantUserId, IFormFile avatarFile)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(merchantUserId);
                if (user == null)
                    return (false, "用户不存在", null);

                // 使用统一的图片上传服务，上传到avatars目录
                var avatarUrl = await _imageUploadService.UploadImageAsync(avatarFile, null, "avatars");
                
                user.Avatar = avatarUrl;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();

                return (true, null, avatarUrl);
            }
            catch (ArgumentException ex)
            {
                return (false, ex.Message, null);
            }
            catch (Exception ex)
            {
                return (false, $"上传失败: {ex.Message}", null);
            }
        }
    }
}
