using BackEnd.DTOs.Merchant;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 商家服务接口
    /// </summary>
    public interface IMerchantService
    {
        /// <summary>
        /// 获取店铺概况
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺概况</returns>
        Task<ShopOverviewResponseDto> GetShopOverviewAsync(int sellerId);

        /// <summary>
        /// 获取店铺详细信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺详细信息</returns>
        Task<ShopInfoResponseDto?> GetShopInfoAsync(int sellerId);

        

        /// <summary>
        /// 切换营业状态
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">切换状态请求</param>
        /// <returns>切换结果</returns>
        Task<CommonResponseDto> ToggleBusinessStatusAsync(int sellerId, ToggleBusinessStatusRequestDto request);

        /// <summary>
        /// 更新店铺字段
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">更新字段请求</param>
        /// <returns>更新结果</returns>
        Task<CommonResponseDto> UpdateShopFieldAsync(int sellerId, UpdateShopFieldRequestDto request);

		/// <summary>
		/// 上传并更新店铺图片
		/// </summary>
		/// <param name="sellerId">商家ID</param>
		/// <param name="imageFile">图片文件</param>
		/// <returns>新图片URL</returns>
		Task<(bool Success, string? Message, string? ImageUrl)> UploadStoreImageAsync(int sellerId, IFormFile imageFile);
    }
} 