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
            Console.WriteLine($"=== Service层: 获取店铺概览，商家ID: {sellerId} ===");

            // 1. 获取店铺信息
            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);

            // 检查店铺是否存在
            if (store == null)
            {
                Console.WriteLine($"未找到商家ID为 {sellerId} 的店铺信息");
                // 返回默认值或者抛出异常，这里我选择返回默认值
                return new ShopOverviewResponseDto
                {
                    Rating = 0.0m,
                    MonthlySales = 0,
                    IsOpen = false,
                    CreditScore = 0
                };
            }

            Console.WriteLine($"店铺信息: StoreID={store.StoreID}, Name={store.StoreName}, Rating={store.AverageRating}, Sales={store.MonthlySales}");

            // 2. 获取商家信息（用于信誉积分）
            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
            if (seller == null)
            {
                Console.WriteLine($"未找到商家ID为 {sellerId} 的商家信息");
                return new ShopOverviewResponseDto
                {
                    Rating = store.AverageRating,
                    MonthlySales = store.MonthlySales,
                    IsOpen = store.StoreState == StoreState.IsOperation,
                    CreditScore = 0
                };
            }

            Console.WriteLine($"商家信息: UserID={seller.UserID}, ReputationPoints={seller.ReputationPoints}");

            // 3. 组装数据
            var rating = store.AverageRating;
            Console.WriteLine($"店铺评分(从数据库): {rating}");

            var monthlySales = store.MonthlySales;
            Console.WriteLine($"月销量(从数据库): {monthlySales}");

            // 从数据库的StoreState字段获取营业状态
            var isOpen = store.StoreState == StoreState.IsOperation; // IsOperation=营业中, Closing=休息中
            Console.WriteLine($"营业状态(从数据库): {isOpen} (StoreState={store.StoreState})");

            var creditScore = seller.ReputationPoints;
            Console.WriteLine($"信誉积分(从数据库): {creditScore}");

            var result = new ShopOverviewResponseDto
            {
                Rating = rating,
                MonthlySales = monthlySales,
                IsOpen = isOpen,
                CreditScore = creditScore
            };

            Console.WriteLine($"返回结果: {System.Text.Json.JsonSerializer.Serialize(result)}");
            return result;
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>店铺信息</returns>
        public async Task<ShopInfoResponseDto?> GetShopInfoAsync(int sellerId)
        {
            Console.WriteLine($"=== Service层: 获取店铺信息，商家ID: {sellerId} ===");

            var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
            var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);

            Console.WriteLine($"店铺信息: StoreID={store?.StoreID}, Name={store?.StoreName}, Address={store?.StoreAddress}");
            Console.WriteLine($"商家信息: ReputationPoints={seller?.ReputationPoints}");

            Console.WriteLine($"OpenTime原始值: {store?.OpenTime}");
            Console.WriteLine($"CloseTime原始值: {store?.CloseTime}");

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

            Console.WriteLine($"返回结果: {System.Text.Json.JsonSerializer.Serialize(result)}");
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
                Console.WriteLine($"=== Service层: 切换营业状态，商家ID: {sellerId}, 新状态: {request.IsOpen} ===");

                // 1. 根据商家ID查询店铺信息
                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    Console.WriteLine("店铺不存在，切换营业状态失败");
                    return new CommonResponseDto { Success = false };
                }

                Console.WriteLine($"找到店铺: StoreID={store.StoreID}, Name={store.StoreName}, 当前状态: {store.StoreState}");

                // 2. 根据商家ID查询商家信息
                var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
                if (seller?.BanStatus == SellerState.Banned)
                {
                    Console.WriteLine($"商家被禁用，BanStatus={seller.BanStatus}，切换营业状态失败");
                    return new CommonResponseDto { Success = false };
                }

                Console.WriteLine($"商家状态正常，BanStatus={seller?.BanStatus}");

                // 3. 更新营业状态
                var oldStatus = store.StoreState;
                store.StoreState = request.IsOpen ? StoreState.IsOperation : StoreState.Closing;  // IsOperation=营业中, Closing=休息中

                Console.WriteLine($"营业状态从 '{oldStatus}' 更新为 '{store.StoreState}' ({(request.IsOpen ? "营业中" : "休息中")})");

                // 4. 保存到数据库
                var success = await _merchantRepository.UpdateStoreAsync(store);
                Console.WriteLine($"数据库更新结果: {success}");

                if (success)
                {
                    Console.WriteLine("=== Service层: 营业状态切换成功 ===");
                }
                else
                {
                    Console.WriteLine("=== Service层: 营业状态切换失败 ===");
                }

                return new CommonResponseDto { Success = success };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"切换营业状态异常: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
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
                Console.WriteLine($"=== Service层: 更新店铺字段，商家ID: {sellerId}, 字段: {request.Field}, 值: {request.Value} ===");

                var store = await _merchantRepository.GetStoreBySellerIdAsync(sellerId);
                if (store == null)
                {
                    Console.WriteLine("店铺不存在，更新失败");
                    return new CommonResponseDto { Success = false };
                }

                Console.WriteLine($"找到店铺: StoreID={store.StoreID}, Name={store.StoreName}");

                var seller = await _merchantRepository.GetSellerByIdAsync(sellerId);
                if (seller?.BanStatus == SellerState.Banned)
                {
                    Console.WriteLine($"商家被禁用，BanStatus={seller.BanStatus}，更新失败");
                    return new CommonResponseDto { Success = false };
                }

                Console.WriteLine($"商家状态正常，BanStatus={seller?.BanStatus}");

                var fieldKey = (request.Field ?? string.Empty).Trim().ToLowerInvariant();
                switch (fieldKey)
                {
                    case "address":
                        store.StoreAddress = request.Value;
                        Console.WriteLine($"更新地址为: {request.Value}");
                        break;
                    case "opentime":
                    case "starttime":
                        if (TimeSpan.TryParse(request.Value, out var openTime))
                        {
                            store.OpenTime = openTime;
                            Console.WriteLine($"更新营业开始时间为: {openTime}");
                        }
                        else
                        {
                            Console.WriteLine($"时间格式解析失败: {request.Value}");
                            return new CommonResponseDto { Success = false };
                        }
                        break;
                    case "closetime":
                    case "endtime":
                        if (TimeSpan.TryParse(request.Value, out var closeTime))
                        {
                            store.CloseTime = closeTime;
                            Console.WriteLine($"更新营业结束时间为: {closeTime}");
                        }
                        else
                        {
                            Console.WriteLine($"时间格式解析失败: {request.Value}");
                            return new CommonResponseDto { Success = false };
                        }
                        break;
                    case "feature":
                        store.StoreFeatures = request.Value;
                        Console.WriteLine($"更新特色为: {request.Value}");
                        break;
                    default:
                        Console.WriteLine($"不支持的字段: {request.Field}");
                        return new CommonResponseDto { Success = false };
                }

                var success = await _merchantRepository.UpdateStoreAsync(store);
                Console.WriteLine($"数据库更新结果: {success}");

                return new CommonResponseDto { Success = success };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新店铺字段异常: {ex.Message}");
                Console.WriteLine($"异常堆栈: {ex.StackTrace}");
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
