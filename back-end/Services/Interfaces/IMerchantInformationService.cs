using BackEnd.DTOs.MerchantInfo;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 商家信息服务接口
    /// </summary>
    public interface IMerchantInformationService
    {
        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <param name="merchantUserId">商家用户ID</param>
        /// <returns>商家信息</returns>
        Task<(bool Success, string? Message, MerchantProfileDto? Data)> GetMerchantInfoAsync(int merchantUserId);

        /// <summary>
        /// 更新商家信息
        /// </summary>
        /// <param name="merchantUserId">商家用户ID</param>
        /// <param name="dto">更新请求</param>
        /// <returns>更新结果</returns>
        Task<(bool Success, string? Message, MerchantUpdateResultDto? Data)> UpdateMerchantInfoAsync(int merchantUserId, UpdateMerchantProfileDto dto);

		/// <summary>
		/// 更新商家头像（表单上传）
		/// </summary>
		/// <param name="merchantUserId">商家用户ID</param>
		/// <param name="avatarFile">头像文件</param>
		/// <returns>头像URL</returns>
		Task<(bool Success, string? Message, string? AvatarUrl)> UpdateMerchantAvatarAsync(int merchantUserId, IFormFile avatarFile);
    }
}