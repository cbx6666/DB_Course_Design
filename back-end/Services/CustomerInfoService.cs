using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Dish;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Store;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Courier;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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

        public CustomerInfoService(
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IImageUploadService imageUploadService)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _imageUploadService = imageUploadService;
        }


        /// <summary>
        /// 获取用户档案
        /// </summary>
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Name = user.Username ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                Avatar = string.IsNullOrWhiteSpace(user.Avatar) 
                    ? "/images/default-avatar.jpg" 
                    : user.Avatar
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

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                try
                {
                    avatar = await UpdateUserAvatarAsync(dto.Id, dto.AvatarFile);
                }
                catch (Exception ex)
                {
                    return new ApiResponseDto { Success = false, Code = 500, Message = $"头像上传失败: {ex.Message}" };
                }
            }

            await _userRepository.UpdatePartialAsync(dto.Id, dto.Name, avatar);
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

    }
}
