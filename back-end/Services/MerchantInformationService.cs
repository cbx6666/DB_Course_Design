using BackEnd.DTOs.MerchantInfo;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 商家信息服务
    /// </summary>
    public class MerchantInformationService : IMerchantInformationService
    {
        private readonly ISellerRepository _sellerRepository;
        private readonly IUserRepository _userRepository;
		private readonly string _avatarFolder;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="sellerRepository">商家仓储</param>
        /// <param name="userRepository">用户仓储</param>
		public MerchantInformationService(ISellerRepository sellerRepository, IUserRepository userRepository, IWebHostEnvironment env)
        {
            _sellerRepository = sellerRepository;
            _userRepository = userRepository;
			_avatarFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "avatars");
			Directory.CreateDirectory(_avatarFolder);
        }

        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <param name="merchantUserId">商家用户ID</param>
        /// <returns>商家信息</returns>
        public async Task<(bool Success, string? Message, MerchantProfileDto? Data)> GetMerchantInfoAsync(int merchantUserId)
        {
            // 获取商家信息及关联用户
            var seller = await _sellerRepository.GetByIdAsync(merchantUserId);
            if (seller == null)
                return (false, "商家不存在", null);

            var user = seller.User;
            if (user == null)
                return (false, "用户信息不存在", null);

            // 转换状态显示文本
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
        /// <param name="merchantUserId">商家用户ID</param>
        /// <param name="dto">更新商家信息请求</param>
        /// <returns>更新结果</returns>
		public async Task<(bool Success, string? Message, MerchantUpdateResultDto? Data)> UpdateMerchantInfoAsync(int merchantUserId, UpdateMerchantProfileDto dto)
        {
            // 获取用户信息
            var user = await _userRepository.GetByIdAsync(merchantUserId);
            if (user == null)
                return (false, "用户不存在", null);

            var updatedFields = new List<string>();

            // 处理用户名更新
            if (!string.Equals(user.Username, dto.Name, StringComparison.Ordinal))
            {
                user.Username = dto.Name;
                updatedFields.Add("username");
            }

            // 处理手机号更新
            if (long.TryParse(dto.Phone, out long newPhone) && user.PhoneNumber != newPhone)
            {
                user.PhoneNumber = newPhone;
                updatedFields.Add("phone");
            }

            // 处理邮箱更新
            if (user.Email != dto.Email)
            {
                user.Email = dto.Email;
                updatedFields.Add("email");
            }

			// 保存更新
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
			var user = await _userRepository.GetByIdAsync(merchantUserId);
			if (user == null)
				return (false, "用户不存在", null);

			if (avatarFile == null || avatarFile.Length <= 0)
				return (false, "文件不能为空", null);

			var fileExtension = Path.GetExtension(avatarFile.FileName);
			var fileName = $"{merchantUserId}_{Guid.NewGuid()}{fileExtension}";
			var filePath = Path.Combine(_avatarFolder, fileName);

			Directory.CreateDirectory(_avatarFolder);
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await avatarFile.CopyToAsync(stream);
			}

			var avatarUrl = $"/avatars/{fileName}";
			user.Avatar = avatarUrl;
			await _userRepository.UpdateAsync(user);
			await _userRepository.SaveAsync();

			return (true, null, avatarUrl);
		}
    }
}