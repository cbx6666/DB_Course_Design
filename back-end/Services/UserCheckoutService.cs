using BackEnd.DTOs.User;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// 用户结账服务
    /// </summary>
    public class UserCheckoutService : IUserCheckoutService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IDishRepository _dishRepository; // 用于获取菜品单价

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="userRepository">用户仓储</param>
        /// <param name="couponRepository">优惠券仓储</param>
        /// <param name="shoppingCartRepository">购物车仓储</param>
        /// <param name="customerRepository">客户仓储</param>
        /// <param name="shoppingCartItemRepository">购物车项仓储</param>
        /// <param name="dishRepository">菜品仓储</param>
        public UserCheckoutService(
            IUserRepository userRepository, 
            ICouponRepository couponRepository, 
            IShoppingCartRepository shoppingCartRepository, 
            ICustomerRepository customerRepository, 
            IShoppingCartItemRepository shoppingCartItemRepository, 
            IDishRepository dishRepository)
        {
            _userRepository = userRepository;
            _couponRepository = couponRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _dishRepository = dishRepository;
        }

        /// <summary>
        /// 获取用户优惠券
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>用户优惠券列表</returns>
        public async Task<List<UserCouponDto>> GetUserCouponsAsync(int userId)
        {
            // 验证用户是否存在
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ValidationException("用户不存在");
            }

            // 检查用户是否有顾客记录
            if (user.Customer == null)
            {
                return new List<UserCouponDto>();
            }

            // 查询用户的优惠券信息
            var coupons = await _couponRepository.GetByCustomerIdAsync(user.Customer.UserID);

            // 转换为 DTO
            return coupons.Select(c => new UserCouponDto
            {
                CouponID = c.CouponID,
                CouponState = GetActualCouponState(c),
                OrderID = c.OrderID,
                CouponManagerID = c.CouponManagerID,
                MinimumSpend = c.CouponManager.MinimumSpend,
                Value = c.CouponManager.Value,
                ValidTo = c.CouponManager.ValidTo.ToString("yyyy-MM-ddTHH:mm:ss")
            }).ToList();
        }

        /// <summary>
        /// 获取实际优惠券状态
        /// </summary>
        /// <param name="coupon">优惠券</param>
        /// <returns>优惠券状态</returns>
        private CouponState GetActualCouponState(Coupon coupon)
        {
            // 如果优惠券已过期且未使用，返回过期状态
            if (coupon.IsExpired && coupon.CouponState == CouponState.Unused)
            {
                return CouponState.Expired;
            }

            return coupon.CouponState;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>购物车响应</returns>
        public async Task<CartResponseDto> GetShoppingCartAsync(int userId, int storeId)
        {
            // 验证用户是否存在
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                throw new ValidationException("用户不存在或不是顾客");
            }

            // 查找该用户未锁定的购物车
            var shoppingCart = await _shoppingCartRepository
                .GetActiveCartWithStoreFilterAsync(customer.UserID, storeId);

            // 如果没有未锁定的购物车，就新建一个
            if (shoppingCart == null)
            {
                shoppingCart = new ShoppingCart
                {
                    CustomerID = customer.UserID,
                    ShoppingCartItems = new List<ShoppingCartItem>(),
                    LastUpdatedTime = DateTime.UtcNow,
                    ShoppingCartState = ShoppingCartState.Active,
                    StoreID = storeId,
                    TotalPrice = 0
                };

                await _shoppingCartRepository.AddAsync(shoppingCart);

                return new CartResponseDto
                {
                    CartId = shoppingCart.CartID,
                    TotalPrice = 0,
                    Items = new List<ShoppingCartItemDto>()
                };
            }

            // 获取购物车项（只取该店铺的）
            var cartItems = shoppingCart.ShoppingCartItems ?? new List<ShoppingCartItem>();

            // 计算总价
            var filteredTotalPrice = cartItems.Sum(item => item.TotalPrice);

            // 转换为 DTO
            return new CartResponseDto
            {
                CartId = shoppingCart.CartID,
                TotalPrice = filteredTotalPrice,
                Items = cartItems.Select(item => new ShoppingCartItemDto
                {
                    ItemId = item.ItemID,
                    DishId = item.DishID,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }).ToList()
            };
        }

        /// <summary>
        /// 更新购物车项
        /// </summary>
        /// <param name="dto">更新购物车项请求</param>
        /// <returns>任务</returns>
        public async Task UpdateCartItemAsync(UpdateCartItemDto dto)
        {
            // 1. 获取购物车
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(dto.CartId);
            if (shoppingCart == null)
            {
                throw new ValidationException("购物车不存在");
            }

            // 2. 获取菜品
            var dish = await _dishRepository.GetByIdAsync(dto.DishId);

            // 3. 查找购物车项
            var cartItem = shoppingCart.ShoppingCartItems?
            .FirstOrDefault(item => item.DishID == dto.DishId);

            if (cartItem == null)
            {
                // 新增购物车项
                cartItem = new ShoppingCartItem
                {
                    DishID = dto.DishId,
                    Quantity = dto.Quantity,
                    TotalPrice = dish!.Price * dto.Quantity,
                    CartID = shoppingCart.CartID
                };
                await _shoppingCartItemRepository.AddAsync(cartItem);
            }
            else
            {
                // 更新购物车项
                cartItem.Quantity = dto.Quantity;
                cartItem.TotalPrice = dish!.Price * dto.Quantity;
                await _shoppingCartItemRepository.UpdateAsync(cartItem);
            }

            // 4. 更新购物车总价（只算该店铺的商品）
            await UpdateCartTotalPriceAsync(shoppingCart);
        }

        /// <summary>
        /// 移除购物车项
        /// </summary>
        /// <param name="dto">移除购物车项请求</param>
        /// <returns>任务</returns>
        public async Task RemoveCartItemAsync(RemoveCartItemDto dto)
        {
            // 1. 获取购物车
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(dto.CartId);
            if (shoppingCart == null)
            {
                throw new ValidationException("购物车不存在");
            }

            // 2. 查找购物车项
            var cartItem = shoppingCart.ShoppingCartItems?
                .FirstOrDefault(item => item.DishID == dto.DishId);

            if (cartItem == null)
            {
                throw new ValidationException("该菜品不在购物车中");
            }

            // 3. 删除购物车项
            await _shoppingCartItemRepository.DeleteAsync(cartItem);

            // 4. 更新购物车总价（只算该店铺的商品）
            await UpdateCartTotalPriceAsync(shoppingCart);
        }

        /// <summary>
        /// 更新购物车总价
        /// </summary>
        /// <param name="cart">购物车</param>
        /// <returns>任务</returns>
        private async Task UpdateCartTotalPriceAsync(ShoppingCart cart)
        {
            var cartItems = await _shoppingCartItemRepository.GetByCartIdAsync(cart.CartID);

            cart.TotalPrice = cartItems.Sum(item => item.TotalPrice);
            cart.LastUpdatedTime = DateTime.UtcNow;
            await _shoppingCartRepository.UpdateAsync(cart);
        }
    }
}
