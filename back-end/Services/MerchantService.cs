using BackEnd.DTOs.Merchant;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 商家服务
    /// </summary>
    public class MerchantService : IMerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly string _storeImageFolder;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="merchantRepository">商家仓储</param>
        public MerchantService(IMerchantRepository merchantRepository, IWebHostEnvironment env)
        {
            _merchantRepository = merchantRepository;
            _storeImageFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "images", "stores");
            Directory.CreateDirectory(_storeImageFolder);
        }

        /// <summary>
        /// 获取店铺概览
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺概览</returns>
        public async Task<ShopOverviewResponseDto> GetShopOverviewAsync(int sellerId)
        {
            // 1. 获取店铺信息
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);

            // 检查店铺是否存在
            if (store == null)
            {
                // 返回默认值或者抛出异常，这里我选择返回默认值
                return new ShopOverviewResponseDto
                {
                    Rating = 0.0m,
                    MonthlySales = 0,
                    IsOpen = false,
                    CreditScore = 0
                };
            }


            // 2. 获取商家信息（用于信誉积分）
            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
            if (seller == null)
            {
                return new ShopOverviewResponseDto
                {
                    Rating = store.AverageRating,
                    MonthlySales = store.MonthlySales,
                    IsOpen = store.StoreState == StoreState.IsOperation,
                    CreditScore = 0
                };
            }


            // 3. 组装数据
            var rating = store.AverageRating;

            var monthlySales = store.MonthlySales;

            // 从数据库的StoreState字段获取营业状态
            var isOpen = store.StoreState == StoreState.IsOperation; // IsOperation=营业中, Closing=休息中

            var creditScore = seller.ReputationPoints;

            var result = new ShopOverviewResponseDto
            {
                Rating = rating,
                MonthlySales = monthlySales,
                IsOpen = isOpen,
                CreditScore = creditScore
            };

            return result;
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺信息</returns>
        public async Task<ShopInfoResponseDto?> GetShopInfoAsync(int sellerId)
        {

            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);


            var result = new ShopInfoResponseDto
            {
                Id = store?.StoreID.ToString() ?? "0",
                Name = store?.StoreName ?? string.Empty,
                CreateTime = store?.StoreCreationTime.ToString("yyyy-MM-dd HH:mm:ss") ?? string.Empty,
                Address = store?.StoreAddress ?? string.Empty,
                StartTime = store?.OpenTime.ToString(@"hh\:mm") ?? string.Empty,
                EndTime = store?.CloseTime.ToString(@"hh\:mm") ?? string.Empty,
                Feature = store?.StoreFeatures ?? string.Empty,
                CreditScore = seller?.ReputationPoints ?? 0,
                StoreImage = store?.StoreImage
            };

            return result;
        }

        /// <summary>
        /// 切换营业状态
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">切换营业状态请求</param>
        /// <returns>响应结果</returns>
        public async Task<CommonResponseDto> ToggleBusinessStatusAsync(int sellerId, ToggleBusinessStatusRequestDto request)
        {
            try
            {

                // 1. 根据商家ID查询店铺信息
                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    return new CommonResponseDto { Success = false };
                }


                // 2. 根据商家ID查询商家信息
                var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
                if (seller?.BanStatus == SellerState.Banned)
                {
                    return new CommonResponseDto { Success = false };
                }


                // 3. 更新营业状态
                var oldStatus = store.StoreState;
                store.StoreState = request.IsOpen ? StoreState.IsOperation : StoreState.Closing;  // IsOperation=营业中, Closing=休息中


                // 4. 保存到数据库
                var success = await _merchantRepository.UpdateStoreAsync(store);

                if (success)
                {
                }
                else
                {
                }

                return new CommonResponseDto { Success = success };
            }
            catch (Exception)
            {
                return new CommonResponseDto { Success = false };
            }
        }

        /// <summary>
        /// 更新店铺字段
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <param name="request">更新店铺字段请求</param>
        /// <returns>响应结果</returns>
        public async Task<CommonResponseDto> UpdateShopFieldAsync(int sellerId, UpdateShopFieldRequestDto request)
        {
            try
            {

                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    return new CommonResponseDto { Success = false };
                }


                var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
                if (seller?.BanStatus == SellerState.Banned)
                {
                    return new CommonResponseDto { Success = false };
                }


                var fieldKey = (request.Field ?? string.Empty).Trim().ToLowerInvariant();
                switch (fieldKey)
                {
                    case "address":
                        store.StoreAddress = request.Value;
                        break;
                    case "opentime":
                    case "starttime":
                        if (TimeSpan.TryParse(request.Value, out var openTime))
                        {
                            store.OpenTime = openTime;
                        }
                        else
                        {
                            return new CommonResponseDto { Success = false };
                        }
                        break;
                    case "closetime":
                    case "endtime":
                        if (TimeSpan.TryParse(request.Value, out var closeTime))
                        {
                            store.CloseTime = closeTime;
                        }
                        else
                        {
                            return new CommonResponseDto { Success = false };
                        }
                        break;
                    case "feature":
                        store.StoreFeatures = request.Value;
                        break;
                    default:
                        return new CommonResponseDto { Success = false };
                }

                var success = await _merchantRepository.UpdateStoreAsync(store);

                return new CommonResponseDto { Success = success };
            }
            catch (Exception)
            {
                return new CommonResponseDto { Success = false };
            }
        }

        /// <summary>
        /// 上传并更新店铺图片
        /// </summary>
        public async Task<(bool Success, string? Message, string? ImageUrl)> UploadStoreImageAsync(int sellerId, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length <= 0)
            {
                return (false, "文件不能为空", null);
            }

            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            if (store == null)
            {
                return (false, "店铺不存在", null);
            }

            var ext = Path.GetExtension(imageFile.FileName);
            var fileName = $"store_{store.StoreID}_{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(_storeImageFolder, fileName);

            Directory.CreateDirectory(_storeImageFolder);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            var url = $"/images/stores/{fileName}";
            store.StoreImage = url;
            var success = await _merchantRepository.UpdateStoreAsync(store);
            return success ? (true, null, url) : (false, "保存失败", null);
        }
    }
}
