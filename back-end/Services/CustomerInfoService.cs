using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Dish;
using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Store;
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
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IFoodOrderRepository _foodOrderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IWebHostEnvironment _env;
        private readonly string _avatarFolder;

        public CustomerInfoService(
            IStoreRepository storeRepository,
            IUserRepository userRepository,
            IFoodOrderRepository foodOrderRepository,
            IShoppingCartRepository shoppingCartRepository,
            ICustomerRepository customerRepository,
            IWebHostEnvironment env)
        {
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _foodOrderRepository = foodOrderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
            _env = env;
            _avatarFolder = Path.Combine(env.WebRootPath ?? env.ContentRootPath, "avatars");
            Directory.CreateDirectory(_avatarFolder);
        }

        /// <summary>
        /// 获取推荐店铺
        /// </summary>
        public async Task<HomeRecmDto> GetRecommendedStoresAsync()
        {
            var topStores = await _storeRepository.GetTopRatedStoresForHomepageAsync(10);
            var random = new Random();
            var recommended = topStores
                .OrderBy(s => random.Next())
                .Take(4);

            return new HomeRecmDto
            {
                RecomStore = recommended
            };
        }

        /// <summary>
        /// 搜索店铺和菜品
        /// </summary>
        public async Task<(IEnumerable<ShowStoreDto> Stores, IEnumerable<ShowStoreDto> Dishes)> SearchAsync(HomeSearchDto searchDto)
        {
            var storeResults = await _storeRepository.SearchStoresByNameAsync(searchDto.Keyword);
            var dishResults = await _storeRepository.SearchStoresByDishNameAsync(searchDto.Keyword);

            return (storeResults, dishResults);
        }

        /// <summary>
        /// 获取订单历史
        /// </summary>
        public async Task<List<CustomerOrderViewDto>> GetOrderHistoryAsync(int userId)
        {
            var allOrders = await _foodOrderRepository.GetAllAsync();
            var orders = allOrders
                .Where(o => o.CustomerID == userId)
                .OrderByDescending(o => o.OrderTime)
                .ToList();

            var result = new List<CustomerOrderViewDto>();

            foreach (var order in orders)
            {
                var store = await _storeRepository.GetStoreInfoForUserAsync(order.StoreID);

                List<string> dishImages = new List<string>();
                List<OrderDishDto> dishDetails = new List<OrderDishDto>();
                decimal totalAmount = 0;

                if (order.CartID.HasValue)
                {
                    var cart = await _shoppingCartRepository.GetByIdAsync(order.CartID.Value);

                    if (cart != null && cart.ShoppingCartItems != null)
                    {
                        dishImages = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null && !string.IsNullOrEmpty(sci.Dish.DishImage))
                            .Select(sci => sci.Dish.DishImage)
                            .OfType<string>()
                            .Distinct()
                            .ToList();

                        dishDetails = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null)
                            .Select(sci => new OrderDishDto
                            {
                                DishName = sci.Dish.DishName,
                                DishImage = sci.Dish.DishImage ?? "",
                                Quantity = sci.Quantity
                            })
                            .ToList();

                        totalAmount = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null)
                            .Sum(sci => sci.Quantity * sci.Dish.Price);
                    }
                }

                OrderCouponInfoDto? usedCoupon = null;
                
                if (order.Coupons != null && order.Coupons.Any())
                {
                    var coupon = order.Coupons.FirstOrDefault();
                    if (coupon != null && coupon.CouponManager != null)
                    {
                        usedCoupon = new OrderCouponInfoDto
                        {
                            CouponId = coupon.CouponID,
                            CouponName = coupon.CouponManager.CouponName,
                            Description = coupon.CouponManager.Description,
                            DiscountType = coupon.CouponManager.CouponType == Models.Enums.CouponType.Fixed ? "fixed" : "discount",
                            DiscountValue = coupon.CouponManager.Value,
                            ValidFrom = coupon.CouponManager.ValidFrom.ToString("o"),
                            ValidTo = coupon.CouponManager.ValidTo.ToString("o"),
                            IsUsed = coupon.CouponState == Models.Enums.CouponState.Used
                        };
                    }
                }
                
                result.Add(new CustomerOrderViewDto
                {
                    OrderId = order.OrderID,
                    PaymentTime = order.PaymentTime.HasValue ?
                        order.PaymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    CartId = order.CartID ?? 0,
                    StoreId = order.StoreID,
                    OrderState = order.FoodOrderState,
                    StoreImage = store?.StoreImage ?? "",
                    StoreName = store?.StoreName ?? "",
                    DishImage = dishImages,
                    DishDetails = dishDetails,
                    TotalAmount = totalAmount,
                    DeliveryStatus = (int?)order.DeliveryTask?.Status,
                    DeliveryFee = order.DeliveryFee,
                    UsedCoupon = usedCoupon
                });
            }

            return result;
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
        /// 获取所有店铺
        /// </summary>
        public async Task<StoresResponseDto> GetAllStoresAsync()
        {
            var operationalStores = await _storeRepository.GetOperationalStoresAsync();
            return new StoresResponseDto { AllStores = operationalStores.ToList() };
        }
    }
}
