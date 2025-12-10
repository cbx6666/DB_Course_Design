using BackEnd.DTOs.Store;
using BackEnd.DTOs.Common;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Models.Helpers;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺服务实现（商家侧）
    /// </summary>
    public class MerchantStoreService : IMerchantStoreService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IConfiguration _configuration;
        private readonly IImageUploadService _imageUploadService;
        private readonly IStoreViolationPenaltyRepository _storeViolationPenaltyRepository;
        private readonly ISellerRepository _sellerRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantStoreService(
            IMerchantRepository merchantRepository,
            IConfiguration configuration,
            IImageUploadService imageUploadService,
            IStoreViolationPenaltyRepository storeViolationPenaltyRepository,
            ISellerRepository sellerRepository)
        {
            _merchantRepository = merchantRepository;
            _configuration = configuration;
            _imageUploadService = imageUploadService;
            _storeViolationPenaltyRepository = storeViolationPenaltyRepository;
            _sellerRepository = sellerRepository;
        }

        /// <summary>
        /// 获取店铺概况
        /// </summary>
        public async Task<ShopOverviewResponseDto> GetShopOverviewAsync(int sellerId)
        {
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            if (store == null)
            {
                throw new KeyNotFoundException($"商家 {sellerId} 没有对应的店铺");
            }

            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);

            return new ShopOverviewResponseDto
            {
                Rating = store.AverageRating,
                MonthlySales = store.MonthlySales,
                IsOpen = store.StoreState == StoreState.IsOperation,
                CreditScore = seller?.ReputationPoints ?? 0
            };
        }

        /// <summary>
        /// 获取店铺详细信息
        /// </summary>
        public async Task<ShopInfoResponseDto?> GetShopInfoAsync(int sellerId)
        {
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            if (store == null)
            {
                return null;
            }

            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
            if (seller != null)
            {
                seller.ReputationPoints = await CalculateCreditScoreAsync(sellerId);
                await _sellerRepository.UpdateAsync(seller);
            }

            return new ShopInfoResponseDto
            {
                Id = store.StoreID.ToString(),
                Name = store.StoreName,
                CreateTime = store.StoreCreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Address = store.StoreAddress,
                StartTime = store.OpenTime.ToString(@"hh\:mm"),
                EndTime = store.CloseTime.ToString(@"hh\:mm"),
                Feature = store.StoreFeatures ?? string.Empty,
                CreditScore = seller?.ReputationPoints ?? 0,
                StoreImage = store.StoreImage,
                Category = StoreCategoryHelper.GetDisplayName(store.StoreCategory)
            };
        }

        /// <summary>
        /// 切换营业状态
        /// </summary>
        public async Task<ApiResponseDto> ToggleBusinessStatusAsync(int sellerId, ToggleBusinessStatusRequestDto request)
        {
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            if (store == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "店铺不存在"
                };
            }

            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
            if (seller?.BanStatus == SellerState.Banned)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 403,
                    Message = "商家已被封禁，无法切换营业状态"
                };
            }

            store.StoreState = request.IsOpen ? StoreState.IsOperation : StoreState.Closing;
            var success = await _merchantRepository.UpdateStoreAsync(store);

            return new ApiResponseDto
            {
                Success = success,
                Code = success ? 200 : 500,
                Message = success ? (request.IsOpen ? "店铺已营业" : "店铺已打烊") : "更新失败"
            };
        }

        /// <summary>
        /// 更新店铺字段
        /// </summary>
        public async Task<ApiResponseDto> UpdateShopFieldAsync(int sellerId, UpdateShopFieldRequestDto request)
        {
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            if (store == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "店铺不存在"
                };
            }

            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
            if (seller?.BanStatus == SellerState.Banned)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 403,
                    Message = "商家已被封禁，无法更新店铺信息"
                };
            }

            // 根据字段名更新
            var fieldKey = (request.Field ?? string.Empty).Trim().ToLowerInvariant();
            switch (fieldKey)
            {
                case "address":
                    store.StoreAddress = request.Value;
                    break;
                case "opentime":
                case "starttime":
                    if (TimeSpan.TryParse(request.Value, out var openTime))
                        store.OpenTime = openTime;
                    else
                        return new ApiResponseDto { Success = false, Code = 400, Message = "时间格式不正确" };
                    break;
                case "closetime":
                case "endtime":
                    if (TimeSpan.TryParse(request.Value, out var closeTime))
                        store.CloseTime = closeTime;
                    else
                        return new ApiResponseDto { Success = false, Code = 400, Message = "时间格式不正确" };
                    break;
                case "feature":
                    store.StoreFeatures = request.Value;
                    break;
                default:
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "无效的字段名"
                    };
            }

            var success = await _merchantRepository.UpdateStoreAsync(store);

            return new ApiResponseDto
            {
                Success = success,
                Code = success ? 200 : 500,
                Message = success ? "更新成功" : "更新失败"
            };
        }

        /// <summary>
        /// 上传并更新店铺图片
        /// </summary>
        public async Task<(bool Success, string? Message, string? ImageUrl)> UploadStoreImageAsync(int sellerId, IFormFile imageFile)
        {
            try
            {
                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    return (false, "店铺不存在", null);
                }

                // 使用统一的图片上传服务
                var url = await _imageUploadService.UploadImageAsync(imageFile, "stores", "images");
                
                store.StoreImage = url;
                var success = await _merchantRepository.UpdateStoreAsync(store);
                return success ? (true, null, url) : (false, "保存失败", null);
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

        /// <summary>
        /// 更新店铺种类
        /// </summary>
        public async Task<ApiResponseDto> UpdateStoreCategoryAsync(int sellerId, UpdateStoreCategoryDto request)
        {
            try
            {
                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    return new ApiResponseDto { Success = false, Code = 404, Message = "店铺不存在" };
                }

                // 将显示名称转换为枚举值
                var category = StoreCategoryHelper.FromDisplayName(request.Category);
                store.StoreCategory = category;

                var success = await _merchantRepository.UpdateStoreAsync(store);
                return new ApiResponseDto 
                { 
                    Success = success, 
                    Code = success ? 200 : 500,
                    Message = success ? "店铺种类更新成功" : "更新失败" 
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto { Success = false, Code = 500, Message = $"更新失败: {ex.Message}" };
            }
        }

        /// <summary>
        /// 计算商家的信誉积分
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>信誉积分</returns>
        public async Task<int> CalculateCreditScoreAsync(int sellerId)
        {
            var penalties = await _storeViolationPenaltyRepository.GetRecentPenaltiesAsync(sellerId);
            if (penalties == null || penalties.Count == 0)
            {
                return 100;
            }

            var totalPenalty = penalties.Count;
            return Math.Max(0, 100 - totalPenalty * 2);
        }
    }
}
