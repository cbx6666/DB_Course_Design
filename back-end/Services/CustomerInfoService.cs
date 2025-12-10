using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Dish;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Store;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Courier;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace BackEnd.Services
{
    /// <summary>
    /// 客户信息服务（整合用户首页和用户档案功能）
    /// </summary>
    public class CustomerInfoService : ICustomerInfoService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IImageUploadService _imageUploadService;
        private readonly IWebHostEnvironment _env;
        private readonly IFavoritesFolderRepository _favoritesFolderRepository;
        private readonly IFavoriteItemRepository _favoriteItemRepository;
        private readonly IStoreRepository _storeRepository;

        public CustomerInfoService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IImageUploadService imageUploadService,
            IWebHostEnvironment env,
            IFavoritesFolderRepository favoritesFolderRepository,
            IFavoriteItemRepository favoriteItemRepository,
            IStoreRepository storeRepository)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _imageUploadService = imageUploadService;
            _env = env;
            _favoritesFolderRepository = favoritesFolderRepository;
            _favoriteItemRepository = favoriteItemRepository;
            _storeRepository = storeRepository;
        }


        /// <summary>
        /// 获取用户档案
        /// </summary>
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            // 验证头像文件是否存在
            string avatarPath = "/images/default-avatar.jpg"; // 默认头像
            
            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                // 检查头像文件是否存在
                string avatarUrl = user.Avatar;
                
                // 如果路径是相对路径，构建完整文件路径
                string filePath;
                if (avatarUrl.StartsWith("/avatars/"))
                {
                    // 提取文件名
                    string fileName = avatarUrl.Replace("/avatars/", "");
                    filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "avatars", fileName);
                }
                else if (avatarUrl.StartsWith("/"))
                {
                    // 其他相对路径
                    filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), avatarUrl.TrimStart('/'));
                }
                else
                {
                    // 可能是文件名格式（如 "21_xxx.jpg"），尝试在 avatars 目录查找
                    filePath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), "avatars", avatarUrl);
                }
                
                // 检查文件是否存在
                if (File.Exists(filePath))
                {
                    avatarPath = avatarUrl.StartsWith("/") ? avatarUrl : $"/avatars/{avatarUrl}";
                }
            }

            return new UserProfileDto
            {
                Name = user.Username ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Avatar = avatarPath
            };
        }

        /// <summary>
        /// 获取用户全部收货地址列表
        /// </summary>
        public async Task<List<UserDeliveryInfoDto>> GetUserAddressesAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.Customer == null)
            {
                return new List<UserDeliveryInfoDto>();
            }

            return user.Customer.DeliveryInfos
                .Select(di => new UserDeliveryInfoDto
                {
                    DeliveryInfoID = di.DeliveryInfoID,
                    Address = di.Address,
                    PhoneNumber = di.PhoneNumber,
                    Name = di.Name,
                    Gender = di.Gender,
                    IsDefault = di.IsDefault == 1
                })
                .OrderByDescending(x => x.IsDefault)
                .ToList();
        }

        /// <summary>
        /// 更新账户信息（姓名、头像）
        /// </summary>
        public async Task<ApiResponseDto> UpdateAccountAsync(UpdateAccountDto dto)
        {
            if (dto == null)
                return new ApiResponseDto { Success = false, Code = 400, Message = "参数不能为空" };

            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在" };

            string avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "/images/default-avatar.jpg" : user.Avatar;

            // 检查是否有上传头像文件
            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                try
                {
                    // 上传新头像
                    avatar = await UpdateUserAvatarAsync(dto.Id, dto.AvatarFile);
                }
                catch (ArgumentException ex)
                {
                    // 文件验证失败（类型、大小等）
                    return new ApiResponseDto { Success = false, Code = 400, Message = ex.Message };
                }
                catch (Exception ex)
                {
                    // 其他错误（文件保存失败等）
                    return new ApiResponseDto { Success = false, Code = 500, Message = $"头像上传失败: {ex.Message}" };
                }
            }

            // 更新用户信息
            await _userRepository.UpdatePartialAsync(dto.Id, dto.Name, avatar);
            await _userRepository.SaveAsync();
            
            return new ApiResponseDto { Success = true, Code = 200, Message = "账户信息更新成功" };
        }

        /// <summary>
        /// 更新用户头像（供内部复用）
        /// </summary>
        private async Task<string> UpdateUserAvatarAsync(int userId, IFormFile file)
        {
            // 使用统一的图片上传服务，上传到avatars目录
            return await _imageUploadService.UploadImageAsync(file, null, "avatars");
        }

        /// <summary>
        /// 新建收货地址
        /// </summary>
        public async Task<ApiResponseDto> CreateAddressAsync(int userId, CreateAddressDto dto)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var newDeliveryInfo = new Models.DeliveryInfo
            {
                Address = dto.Address,
                PhoneNumber = dto.PhoneNumber.ToString(),
                Name = dto.Name,
                Gender = dto.Gender,
                IsDefault = 0,
                CustomerID = userWithCustomer.Customer.UserID
            };

            userWithCustomer.Customer.DeliveryInfos.Add(newDeliveryInfo);
            await _userRepository.SaveAsync();

            return new ApiResponseDto { Success = true, Code = 200, Message = "收货地址创建成功" };
        }

        /// <summary>
        /// 更新收货地址
        /// </summary>
        public async Task<ApiResponseDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收货地址不存在" };
            }

            deliveryInfo.Address = dto.Address;
            deliveryInfo.PhoneNumber = dto.PhoneNumber.ToString();
            deliveryInfo.Name = dto.Name;
            deliveryInfo.Gender = dto.Gender;

            await _userRepository.SaveAsync();
            return new ApiResponseDto { Success = true, Code = 200, Message = "收货地址更新成功" };
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        public async Task<ApiResponseDto> DeleteAddressAsync(int userId, int addressId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收货地址不存在" };
            }

            userWithCustomer.Customer.DeliveryInfos.Remove(deliveryInfo);
            await _userRepository.SaveAsync();
            return new ApiResponseDto { Success = true, Code = 200, Message = "收货地址删除成功" };
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        public async Task<ApiResponseDto> SetDefaultAddressAsync(int userId, int addressId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收货地址不存在" };
            }

            foreach (var info in userWithCustomer.Customer.DeliveryInfos)
            {
                info.IsDefault = 0;
            }

            deliveryInfo.IsDefault = 1;

            await _userRepository.SaveAsync();
            return new ApiResponseDto { Success = true, Code = 200, Message = "默认收货地址设置成功" };
        }

        /// <summary>
        /// 获取用户的收藏夹列表
        /// </summary>
        public async Task<List<FavoritesFolderDto>> GetFavoritesFoldersAsync(int userId)
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new List<FavoritesFolderDto>();
            }

            // 获取所有收藏夹（已包含 FavoriteItems 和 Store 导航属性）
            var folders = await _favoritesFolderRepository.GetByCustomerIdAsync(customer.UserID);
            
            // 如果用户没有收藏夹（旧数据），创建默认收藏夹
            if (folders.Count == 0)
            {
                var hasDefaultFolder = await _favoritesFolderRepository.HasDefaultFolderAsync(customer.UserID);
                if (!hasDefaultFolder)
                {
                    var newDefaultFolder = new FavoritesFolder
                    {
                        FolderName = "默认收藏夹",
                        CustomerID = customer.UserID
                    };

                    await _favoritesFolderRepository.AddAsync(newDefaultFolder);
                    
                    // 重新查询以确保包含所有导航属性
                    folders = await _favoritesFolderRepository.GetByCustomerIdAsync(customer.UserID);
                }
            }
            
            return folders.Select(folder => new FavoritesFolderDto
            {
                FolderID = folder.FolderID,
                FolderName = folder.FolderName,
                FavoriteItems = folder.FavoriteItems?.Select(item => new FavoriteItemDto
                {
                    ItemID = item.ItemID,
                    StoreID = item.StoreID,
                    StoreName = item.Store?.StoreName ?? "未知店铺",
                    StoreImage = item.Store?.StoreImage,
                    FavoritedAt = item.FavoritedAt,
                    FavoriteReason = item.FavoriteReason
                }).ToList() ?? new List<FavoriteItemDto>()
            }).ToList();
        }

        /// <summary>
        /// 创建收藏夹
        /// </summary>
        public async Task<ApiResponseDto> CreateFavoritesFolderAsync(int userId, string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "收藏夹名称不能为空" };
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            // 重名校验（同一用户下）
            var exists = await _favoritesFolderRepository.ExistsByNameAsync(customer.UserID, folderName.Trim());
            if (exists)
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "已存在同名收藏夹" };
            }

            var folder = new FavoritesFolder
            {
                CustomerID = customer.UserID,
                FolderName = folderName.Trim()
            };

            await _favoritesFolderRepository.AddAsync(folder);
            return new ApiResponseDto { Success = true, Code = 200, Message = "创建成功" };
        }

        /// <summary>
        /// 删除收藏夹（不可删除默认收藏夹）
        /// </summary>
        public async Task<ApiResponseDto> DeleteFavoritesFolderAsync(int userId, int folderId)
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var folder = await _favoritesFolderRepository.GetByIdAsync(folderId);
            if (folder == null || folder.CustomerID != customer.UserID)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收藏夹不存在" };
            }

            if (folder.FolderName == "默认收藏夹")
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "默认收藏夹不可删除" };
            }

            await _favoritesFolderRepository.DeleteAsync(folder);
            return new ApiResponseDto { Success = true, Code = 200, Message = "删除成功" };
        }

        /// <summary>
        /// 向收藏夹添加店铺
        /// </summary>
        public async Task<ApiResponseDto> AddFavoriteItemAsync(int userId, int folderId, AddFavoriteItemDto request)
        {
            if (request.StoreId <= 0)
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "无效的店铺ID" };
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var folder = await _favoritesFolderRepository.GetByIdAsync(folderId);
            if (folder == null || folder.CustomerID != customer.UserID)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收藏夹不存在" };
            }

            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "店铺不存在" };
            }

            // 去重：同一收藏夹不可重复
            var exists = await _favoriteItemRepository.ExistsAsync(folderId, request.StoreId);
            if (exists)
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "该店铺已在该收藏夹中" };
            }

            var item = new FavoriteItem
            {
                FolderID = folderId,
                StoreID = request.StoreId,
                FavoriteReason = string.IsNullOrWhiteSpace(request.FavoriteReason) ? "收藏这个店铺" : request.FavoriteReason,
                FavoritedAt = DateTime.Now
            };

            await _favoriteItemRepository.AddAsync(item);
            return new ApiResponseDto { Success = true, Code = 200, Message = "收藏成功" };
        }

        /// <summary>
        /// 从收藏夹删除店铺
        /// </summary>
        public async Task<ApiResponseDto> RemoveFavoriteItemAsync(int userId, int folderId, int storeId)
        {
            if (storeId <= 0)
            {
                return new ApiResponseDto { Success = false, Code = 400, Message = "无效的店铺ID" };
            }

            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "用户不存在或不是顾客" };
            }

            var folder = await _favoritesFolderRepository.GetByIdAsync(folderId);
            if (folder == null || folder.CustomerID != customer.UserID)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收藏夹不存在" };
            }

            var favItem = await _favoriteItemRepository.GetByFolderAndStoreAsync(folderId, storeId);
            if (favItem == null)
            {
                return new ApiResponseDto { Success = false, Code = 404, Message = "收藏项不存在" };
            }

            await _favoriteItemRepository.DeleteAsync(favItem);
            return new ApiResponseDto { Success = true, Code = 200, Message = "删除成功" };
        }
    }
}
