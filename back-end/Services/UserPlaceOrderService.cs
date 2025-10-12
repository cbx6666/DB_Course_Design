using BackEnd.Models;
using BackEnd.DTOs.User;
using BackEnd.Services.Interfaces;
using BackEnd.Repositories.Interfaces;
using BackEnd.Data;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户下单服务
    /// </summary>
    public class UserPlaceOrderService : IUserPlaceOrderService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IFoodOrderRepository _foodOrderRepository;
        private readonly string _avatarFolder;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="cartRepository">购物车仓储</param>
        /// <param name="foodOrderRepository">订单仓储</param>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="customerRepository">客户仓储</param>
        /// <param name="env">Web主机环境</param>
        public UserPlaceOrderService(
            IShoppingCartRepository cartRepository, 
            IFoodOrderRepository foodOrderRepository, 
            IUserRepository userRepository, 
            ICustomerRepository customerRepository,
            IWebHostEnvironment env)
        {
            _cartRepository = cartRepository;
            _foodOrderRepository = foodOrderRepository;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _avatarFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "avatars");
            Directory.CreateDirectory(_avatarFolder);
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="dto">创建订单请求</param>
        /// <returns>响应结果</returns>
        public async Task<ResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            var cart = await _cartRepository.GetByIdAsync(dto.CartId);
            if (cart == null || cart.ShoppingCartItems?.Count == 0)
            {
                return await Task.FromResult(new ResponseDto
                {
                    Success = false,
                    Message = "购物车为空，无法生成订单"
                });
            }

            var foodOrder = new FoodOrder
            {
                CustomerID = dto.CustomerId,
                CartID = dto.CartId,
                StoreID = dto.StoreId,
                DeliveryFee = dto.DeliveryFee,
                OrderTime = DateTime.Now,
                PaymentTime = dto.PaymentTime,   // 下单时传入
                Remarks = dto.Remarks,
                FoodOrderState = Models.Enums.FoodOrderState.Pending
            };

            await _foodOrderRepository.AddAsync(foodOrder);
            await _foodOrderRepository.SaveAsync();

            return await Task.FromResult(new ResponseDto
            {
                Success = true,
                Message = "订单创建成功"
            });
        }

        /// <summary>
        /// 更新账户信息
        /// </summary>
        /// <param name="dto">更新账户请求</param>
        /// <returns>响应结果</returns>
        public async Task<ResponseDto> UpdateAccountAsync(UpdateAccountDto dto)
        {
            if (dto == null)
                return new ResponseDto { Success = false, Message = "参数不能为空" };

            // 检查 _userRepository 是否已注入
            if (_userRepository == null)
            {
                return new ResponseDto { Success = false, Message = "内部错误" };
            }

            // 查找用户
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
            {
                return new ResponseDto { Success = false, Message = "用户不存在" };
            }

            // 调试信息：检查当前用户数据
            Console.WriteLine($"当前用户数据:");
            Console.WriteLine($"- UserID: {user.UserID}");
            Console.WriteLine($"- Username: {user.Username}");
            Console.WriteLine($"- FullName: {user.FullName ?? "null"}");
            Console.WriteLine($"- PhoneNumber: {user.PhoneNumber}");
            Console.WriteLine($"- Avatar: {user.Avatar ?? "null"}");
            Console.WriteLine($"- Role: {user.Role}");

            // 更新用户信息
            user.Username = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;
            user.Avatar = string.IsNullOrWhiteSpace(dto.Image) 
                          ? "/images/default-avatar.jpg" // 本地默认头像
                          : dto.Image;
            
            // 对于消费者用户，确保FullName为null，避免Oracle类型转换错误
            if (user.Role == Models.Enums.UserIdentity.Customer)
            {
                user.FullName = null;
            }
            
            // 使用Repository的更新方法
            await _userRepository.UpdateAsync(user);

            return new ResponseDto { Success = true, Message = "账户信息更新成功" };
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="file">头像文件</param>
        /// <returns>头像URL</returns>
        public async Task<string> UpdateUserAvatarAsync(int userId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("文件不能为空");

            // 获取上传文件的扩展名，比如 .jpg、.png
            var ext = Path.GetExtension(file.FileName);
            // 构建文件名，使用用户ID作为文件名的一部分
            var fileName = $"{userId}{ext}";
            var filePath = Path.Combine(_avatarFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 更新数据库
            var relativeUrl = $"/avatars/{fileName}";
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("用户不存在");

            user.Avatar = relativeUrl;
            await _userRepository.UpdateAsync(user);

            return relativeUrl;
        }

        /// <summary>
        /// 保存或更新地址
        /// </summary>
        /// <param name="dto">保存地址请求</param>
        /// <returns>响应结果</returns>
        public async Task<ResponseDto> SaveOrUpdateAddressAsync(SaveAddressDto dto)
        {
            if (dto == null)
                return new ResponseDto { Success = false, Message = "参数不能为空" };

            // 查找客户
            var customer = await _customerRepository.GetByIdAsync(dto.Id);
            if (customer == null)
            {
                return new ResponseDto { Success = false, Message = "客户不存在" };
            }

            // 构建地址字符串 (详细地址)
            string addressString = dto.Address;

            // 创建或更新收货信息
            var existingDeliveryInfo = customer.DeliveryInfos.FirstOrDefault(di => di.IsDefault == 1);
            if (existingDeliveryInfo != null)
            {
                // 更新现有默认收货信息
                existingDeliveryInfo.Address = addressString;
                existingDeliveryInfo.PhoneNumber = dto.PhoneNumber.ToString();
                existingDeliveryInfo.Name = dto.Name;
                existingDeliveryInfo.Gender = dto.Gender;
            }
            else
            {
                // 创建新的默认收货信息
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

            // EF 已跟踪 customer，直接保存即可
            await _customerRepository.SaveAsync();

            return new ResponseDto { Success = true, Message = "收货地址保存成功" };
        }
    }
}