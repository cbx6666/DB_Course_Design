using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.MerchantInfo
{
	/// <summary>
	/// 更新商家头像请求
	/// </summary>
	public class UpdateMerchantAvatarDto
	{
		/// <summary>
		/// 头像文件
		/// </summary>
		[Required]
		public IFormFile AvatarFile { get; set; } = null!;
	}
}

