using BackEnd.Data;
using BackEnd.Models;
using BackEnd.DTOs.User;
using Microsoft.EntityFrameworkCore;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户个人资料服务
    /// </summary>
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly string _avatarFolder;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        public UserProfileService(IUserRepository userRepository, ICustomerRepository customerRepository, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _avatarFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "avatars");
            Directory.CreateDirectory(_avatarFolder);
        }

        /// <summary>
        /// 获取用户个人资料
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户个人资料</returns>
        public async Task<UserProfileDto?> GetUserProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;

            return new UserProfileDto
            {
                Name = user.Username,
                PhoneNumber = user.PhoneNumber,
                Image = string.IsNullOrWhiteSpace(user.Avatar) ? "/images/default-avatar.jpg" : user.Avatar
            };
        }

        /// <summary>
        /// 获取用户地址信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户地址信息</returns>
        public async Task<UserAddressDto?> GetUserAddressAsync(int userId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return null;
            }

            var customer = userWithCustomer.Customer;
            var recipientName = userWithCustomer.Username;

            // 获取默认收货信息
            var defaultDeliveryInfo = customer.DeliveryInfos.FirstOrDefault(di => di.IsDefault == 1);
            
            return new UserAddressDto
            {
                Name = defaultDeliveryInfo?.Name ?? recipientName,
                PhoneNumber = defaultDeliveryInfo != null && long.TryParse(defaultDeliveryInfo.PhoneNumber, out long phone) 
                    ? phone 
                    : userWithCustomer.PhoneNumber,
                Address = defaultDeliveryInfo?.Address ?? "xx市xx区xx街道xx号"
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
        /// 更新用户头像
        /// </summary>
        public async Task<string> UpdateUserAvatarAsync(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("文件不能为空");
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = $"{userId}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(_avatarFolder, fileName);

            Directory.CreateDirectory(_avatarFolder);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/avatars/{fileName}";
        }

        /// <summary>
        /// 更新账户基本信息（姓名、头像）
        /// </summary>
        public async Task<ResponseDto> UpdateAccountAsync(UpdateAccountDto dto)
        {
            if (dto == null)
                return new ResponseDto { Success = false, Message = "参数不能为空" };

            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
                return new ResponseDto { Success = false, Message = "用户不存在" };

            string avatar = string.IsNullOrWhiteSpace(user.Avatar) ? "/images/default-avatar.jpg" : user.Avatar;

            if (dto.AvatarFile != null && dto.AvatarFile.Length > 0)
            {
                try
                {
                    avatar = await UpdateUserAvatarAsync(dto.Id, dto.AvatarFile);
                }
                catch (Exception ex)
                {
                    return new ResponseDto { Success = false, Message = $"头像上传失败: {ex.Message}" };
                }
            }

            await _userRepository.UpdatePartialAsync(dto.Id, dto.Name, avatar);
            return new ResponseDto { Success = true, Message = "账户信息更新成功" };
        }

        /// <summary>
        /// 保存或更新默认收货地址
        /// </summary>
        public async Task<ResponseDto> SaveOrUpdateAddressAsync(SaveAddressDto dto)
        {
            if (dto == null)
                return new ResponseDto { Success = false, Message = "参数不能为空" };

            var customer = await _customerRepository.GetByIdAsync(dto.Id);
            if (customer == null)
                return new ResponseDto { Success = false, Message = "客户不存在" };

            string addressString = dto.Address;
            var existingDeliveryInfo = customer.DeliveryInfos.FirstOrDefault(di => di.IsDefault == 1);
            if (existingDeliveryInfo != null)
            {
                existingDeliveryInfo.Address = addressString;
                existingDeliveryInfo.PhoneNumber = dto.PhoneNumber.ToString();
                existingDeliveryInfo.Name = dto.Name;
                existingDeliveryInfo.Gender = dto.Gender;
            }
            else
            {
                var newDeliveryInfo = new DeliveryInfo
                {
                    Address = addressString,
                    PhoneNumber = dto.PhoneNumber.ToString(),
                    Name = dto.Name,
                    Gender = dto.Gender,
                    IsDefault = 1,
                    CustomerID = customer.UserID
                };
                customer.DeliveryInfos.Add(newDeliveryInfo);
            }
            await _customerRepository.SaveAsync();
            return new ResponseDto { Success = true, Message = "收货地址保存成功" };
        }

        /// <summary>
        /// 新建收货地址
        /// </summary>
        public async Task<ResponseDto> CreateAddressAsync(int userId, CreateAddressDto dto)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ResponseDto { Success = false, Message = "用户不存在或不是顾客" };
            }

            var newDeliveryInfo = new DeliveryInfo
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

            return new ResponseDto { Success = true, Message = "收货地址创建成功" };
        }

        /// <summary>
        /// 更新收货地址
        /// </summary>
        public async Task<ResponseDto> UpdateAddressAsync(int userId, int addressId, CreateAddressDto dto)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ResponseDto { Success = false, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ResponseDto { Success = false, Message = "收货地址不存在" };
            }

            deliveryInfo.Address = dto.Address;
            deliveryInfo.PhoneNumber = dto.PhoneNumber.ToString();
            deliveryInfo.Name = dto.Name;
            deliveryInfo.Gender = dto.Gender;

            await _userRepository.SaveAsync();
            return new ResponseDto { Success = true, Message = "收货地址更新成功" };
        }

        /// <summary>
        /// 删除收货地址
        /// </summary>
        public async Task<ResponseDto> DeleteAddressAsync(int userId, int addressId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ResponseDto { Success = false, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ResponseDto { Success = false, Message = "收货地址不存在" };
            }

            userWithCustomer.Customer.DeliveryInfos.Remove(deliveryInfo);
            await _userRepository.SaveAsync();
            return new ResponseDto { Success = true, Message = "收货地址删除成功" };
        }

        /// <summary>
        /// 设置默认收货地址
        /// </summary>
        public async Task<ResponseDto> SetDefaultAddressAsync(int userId, int addressId)
        {
            var userWithCustomer = await _userRepository.GetByIdAsync(userId);
            if (userWithCustomer?.Customer == null)
            {
                return new ResponseDto { Success = false, Message = "用户不存在或不是顾客" };
            }

            var deliveryInfo = userWithCustomer.Customer.DeliveryInfos.FirstOrDefault(d => d.DeliveryInfoID == addressId);
            if (deliveryInfo == null)
            {
                return new ResponseDto { Success = false, Message = "收货地址不存在" };
            }

            // 先将所有地址设为非默认
            foreach (var info in userWithCustomer.Customer.DeliveryInfos)
            {
                info.IsDefault = 0;
            }

            // 设置当前地址为默认
            deliveryInfo.IsDefault = 1;

            await _userRepository.SaveAsync();
            return new ResponseDto { Success = true, Message = "默认收货地址设置成功" };
        }
    }
}