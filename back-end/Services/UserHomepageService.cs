using BackEnd.DTOs.User;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using BackEnd.Models.Enums;
using BackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户首页服务
    /// </summary>
    public class UserHomepageService : IUserHomepageService
    {
        private readonly IStoreRepository _storeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IFoodOrderRepository _foodOrderRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICouponManagerRepository _couponManagerRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="storeRepository">店铺仓储</param>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="couponRepository">优惠券仓储</param>
        /// <param name="foodOrderRepository">订单仓储</param>
        /// <param name="shoppingCartRepository">购物车仓储</param>
        public UserHomepageService(
            IStoreRepository storeRepository,
            IUserRepository userRepository,
            ICouponRepository couponRepository,
            IFoodOrderRepository foodOrderRepository,
            IShoppingCartRepository shoppingCartRepository,
            ICouponManagerRepository couponManagerRepository,
            AppDbContext context)
        {
            _storeRepository = storeRepository;
            _userRepository = userRepository;
            _couponRepository = couponRepository;
            _foodOrderRepository = foodOrderRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _couponManagerRepository = couponManagerRepository;
            _context = context;
        }

        /// <summary>
        /// 获取推荐店铺
        /// </summary>
        /// <returns>推荐店铺</returns>
        public async Task<HomeRecmDto> GetRecommendedStoresAsync()
        {
            // 直接从数据库获取已排序和限制数量的结果
            var topStores = await _storeRepository.GetTopRatedStoresForHomepageAsync(10);

            // 在内存中进行随机化是OK的，因为数据量已经很小 (只有10条)
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
        /// <param name="searchDto">搜索请求</param>
        /// <returns>搜索结果</returns>
        public async Task<(IEnumerable<HomeSearchGetDto> Stores, IEnumerable<HomeSearchGetDto> Dishes)>
            SearchAsync(HomeSearchDto searchDto)
        {
            // 让数据库执行搜索
            var storeResults = await _storeRepository.SearchStoresByNameAsync(searchDto.Keyword);
            var dishResults = await _storeRepository.SearchStoresByDishNameAsync(searchDto.Keyword);

            return (storeResults, dishResults);
        }

        /// <summary>
        /// 获取订单历史
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>订单历史</returns>
        public async Task<List<HistoryOrderDto>> GetOrderHistoryAsync(int userId)
        {
            // 获取所有订单（包括优惠券信息），然后按消费者ID筛选
            var allOrders = await _foodOrderRepository.GetAllAsync();
            var orders = allOrders
                .Where(o => o.CustomerID == userId)
                .OrderByDescending(o => o.OrderTime)
                .ToList();

            var result = new List<HistoryOrderDto>();

            foreach (var order in orders)
            {
                // 获取店铺信息
                var store = await _storeRepository.GetStoreInfoForUserAsync(order.StoreID);

                // 获取购物车信息（如果存在）
                List<string> dishImages = new List<string>();
                List<OrderDishDto> dishDetails = new List<OrderDishDto>();
                decimal totalAmount = 0;

                if (order.CartID.HasValue)
                {
                    var cart = await _shoppingCartRepository.GetByIdAsync(order.CartID.Value);

                    if (cart != null && cart.ShoppingCartItems != null)
                    {
                        // 获取所有菜品图片
                        dishImages = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null && !string.IsNullOrEmpty(sci.Dish.DishImage))
                            .Select(sci => sci.Dish.DishImage)
                            .OfType<string>() // 过滤掉 null 值
                            .Distinct()
                            .ToList();

                        // 获取菜品详情
                        dishDetails = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null)
                            .Select(sci => new OrderDishDto
                            {
                                DishName = sci.Dish.DishName,
                                DishImage = sci.Dish.DishImage ?? "",
                                Quantity = sci.Quantity
                            })
                            .ToList();

                        // 计算总金额
                        totalAmount = cart.ShoppingCartItems
                            .Where(sci => sci.Dish != null)
                            .Sum(sci => sci.Quantity * sci.Dish.Price);
                    }
                }

                // 获取订单使用的优惠券信息
                DTOs.Coupon.OrderCouponInfoDto? usedCoupon = null;
                
                if (order.Coupons != null && order.Coupons.Any())
                {
                    var coupon = order.Coupons.FirstOrDefault(); // 一个订单通常只有一个优惠券
                    if (coupon != null && coupon.CouponManager != null)
                    {
                        usedCoupon = new DTOs.Coupon.OrderCouponInfoDto
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
                
                // TotalAmount 返回原始商品总价（不含优惠券折扣，不含配送费）
                // 前端会单独显示商品价格、配送费和优惠券信息，然后计算实付金额

                result.Add(new HistoryOrderDto
                {
                    OrderID = order.OrderID,
                    PaymentTime = order.PaymentTime.HasValue ?
                        order.PaymentTime.Value.ToString("yyyy-MM-dd HH:mm:ss") : "",
                    CartID = order.CartID ?? 0,
                    StoreID = order.StoreID,
                    StoreImage = store?.StoreImage ?? "",
                    StoreName = store?.StoreName ?? "",
                    DishImage = dishImages,
                    DishDetails = dishDetails,
                    TotalAmount = totalAmount, // 原始商品总价（不含优惠券折扣，不含配送费）
                    OrderStatus = order.FoodOrderState,
                    DeliveryStatus = order.DeliveryTask?.Status,
                    UsedCoupon = usedCoupon,
                    DeliveryFee = order.DeliveryFee // 配送费单独返回
                });
            }

            return result;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户信息</returns>
        public async Task<UserInfoResponse?> GetUserInfoAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);

                if (user == null)
                    return null;

                return new UserInfoResponse
                {
                    Name = user.Username ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    Image = string.IsNullOrWhiteSpace(user.Avatar) 
                        ? "/images/default-avatar.jpg" 
                        : user.Avatar
                };
            }
            catch (Exception)
            {
                throw; // 重新抛出异常
            }
        }

        /// <summary>
        /// 查询用户优惠券（带 CouponManager 信息）
        /// </summary>
        /// <param name="userIdDto">用户ID请求</param>
        /// <returns>用户优惠券列表</returns>
        public async Task<IEnumerable<CouponDto>> GetUserCouponsAsync(UserIdDto userIdDto)
        {
            var coupons = await _couponRepository.GetAllAsync();

            var results = coupons
                .Where(c => c.Customer.UserID == userIdDto.UserId)   // 过滤用户
                .Select(c => new CouponDto
                {
                    CouponID = c.CouponID,
                    CouponState = c.CouponState,
                    OrderID = c.OrderID,
                    CouponManagerID = c.CouponManagerID,

                    MinimumSpend = c.CouponManager.MinimumSpend,
                    Value = c.CouponManager.Value,
                    ValidTo = c.CouponManager.ValidTo
                });

            return results;
        }

        /// <summary>
        /// 获取所有店铺
        /// </summary>
        /// <returns>所有店铺</returns>
        public async Task<StoresResponseDto> GetAllStoresAsync()
        {
            // 直接从数据库获取所有运营中的店铺
            var operationalStores = await _storeRepository.GetOperationalStoresAsync();

            return new StoresResponseDto { AllStores = operationalStores.ToList() };
        }

        /// <summary>
        /// 获取所有可领取的优惠券
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>可领取优惠券列表</returns>
        public async Task<List<AvailableCouponDto>> GetAvailableCouponsAsync(int userId)
        {
            var now = DateTime.Now;
            
            // 获取用户信息
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Customer == null)
            {
                return new List<AvailableCouponDto>();
            }

            var customerId = user.Customer.UserID;

            // 获取所有有效的优惠券管理（未过期，包括未开始的）
            var allCouponManagers = await _context.CouponManagers
                .Include(cm => cm.Store)
                .Include(cm => cm.Coupons)
                .Where(cm => cm.ValidTo >= now)
                .ToListAsync();
            
            // 过滤出还有剩余数量的优惠券（通过 Coupons.Count 计算）
            allCouponManagers = allCouponManagers
                .Where(cm => (cm.Coupons?.Count ?? 0) < cm.TotalQuantity)
                .ToList();

            // 获取用户已领取的优惠券管理ID列表
            var claimedCouponManagerIds = await _context.Coupons
                .Where(c => c.CustomerID == customerId)
                .Select(c => c.CouponManagerID)
                .Distinct()
                .ToListAsync();

            var result = new List<AvailableCouponDto>();

            foreach (var cm in allCouponManagers)
            {
                // 计算剩余数量
                var totalClaimedCount = cm.Coupons?.Count ?? 0;
                var remainingQuantity = cm.TotalQuantity - totalClaimedCount;

                // 检查是否已领取（同一用户不能重复领取同一优惠券）
                var isClaimed = claimedCouponManagerIds.Contains(cm.CouponManagerID);

                result.Add(new AvailableCouponDto
                {
                    CouponManagerID = cm.CouponManagerID,
                    CouponName = cm.CouponName,
                    Type = cm.CouponType == CouponType.Fixed ? "fixed" : "discount",
                    MinimumSpend = cm.MinimumSpend,
                    Value = cm.Value,
                    ValidFrom = cm.ValidFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                    ValidTo = cm.ValidTo.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = cm.Description,
                    StoreID = cm.StoreID,
                    StoreName = cm.Store?.StoreName ?? "",
                    StoreImage = cm.Store?.StoreImage,
                    RemainingQuantity = remainingQuantity,
                    IsClaimed = isClaimed
                });
            }

            return result;
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="couponManagerId">优惠券管理ID</param>
        /// <returns>领取结果</returns>
        public async Task<bool> ClaimCouponAsync(int userId, int couponManagerId)
        {
            // 获取用户信息
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Customer == null)
            {
                return false;
            }

            var customerId = user.Customer.UserID;

            // 获取优惠券管理信息
            var couponManager = await _couponManagerRepository.GetByIdAsync(couponManagerId);
            if (couponManager == null)
            {
                return false;
            }

            // 检查优惠券是否有效
            var now = DateTime.Now;
            if (now > couponManager.ValidTo)
            {
                return false;
            }

            // 需要重新加载 Coupons 以确保获取最新数量
            await _context.Entry(couponManager)
                .Collection(cm => cm.Coupons!)
                .LoadAsync();

            // 检查是否还有剩余数量
            var totalClaimed = couponManager.Coupons?.Count ?? 0;
            if (totalClaimed >= couponManager.TotalQuantity)
            {
                return false;
            }

            // 检查用户是否已经领取过这个优惠券
            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.CustomerID == customerId && c.CouponManagerID == couponManagerId);
            if (existingCoupon != null)
            {
                return false; // 已经领取过了
            }

            // 创建新的优惠券
            var newCoupon = new BackEnd.Models.Coupon
            {
                CustomerID = customerId,
                CouponManagerID = couponManagerId,
                CouponState = CouponState.Unused
            };

            await _couponRepository.AddAsync(newCoupon);
            await _couponRepository.SaveAsync();

            return true;
        }
    }
}
