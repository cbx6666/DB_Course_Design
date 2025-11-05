using BackEnd.DTOs.Cart;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace BackEnd.Services
{
    /// <summary>
    /// 购物车服务
    /// </summary>
    public class CartService : ICartService
    {
        private readonly IUserRepository _userRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IShoppingCartItemRepository _shoppingCartItemRepository;
        private readonly IDishRepository _dishRepository;

        public CartService(
            IUserRepository userRepository,
            IShoppingCartRepository shoppingCartRepository,
            ICustomerRepository customerRepository,
            IShoppingCartItemRepository shoppingCartItemRepository,
            IDishRepository dishRepository)
        {
            _userRepository = userRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _dishRepository = dishRepository;
        }

        /// <summary>
        /// 获取购物车
        /// </summary>
        public async Task<CartResponseDto> GetShoppingCartAsync(int userId, int storeId)
        {
            var customer = await _customerRepository.GetByIdAsync(userId);
            if (customer == null)
            {
                throw new ValidationException("用户不存在或不是顾客");
            }

            var shoppingCart = await _shoppingCartRepository
                .GetActiveCartWithStoreFilterAsync(customer.UserID, storeId);

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

            var cartItems = shoppingCart.ShoppingCartItems ?? new List<ShoppingCartItem>();
            var filteredTotalPrice = cartItems.Sum(item => item.TotalPrice);

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
        /// 更新购物车商品
        /// </summary>
        public async Task UpdateCartItemAsync(UpdateCartItemDto dto)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(dto.CartId);
            if (shoppingCart == null)
            {
                throw new ValidationException("购物车不存在");
            }

            var dish = await _dishRepository.GetByIdAsync(dto.DishId);
            if (dish == null)
            {
                throw new ValidationException("菜品不存在");
            }

            var cartItem = shoppingCart.ShoppingCartItems?
                .FirstOrDefault(item => item.DishID == dto.DishId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCartItem
                {
                    DishID = dto.DishId,
                    Quantity = dto.Quantity,
                    TotalPrice = dish.Price * dto.Quantity,
                    CartID = shoppingCart.CartID
                };
                await _shoppingCartItemRepository.AddAsync(cartItem);
            }
            else
            {
                cartItem.Quantity = dto.Quantity;
                cartItem.TotalPrice = dish.Price * dto.Quantity;
                await _shoppingCartItemRepository.UpdateAsync(cartItem);
            }

            await UpdateCartTotalPriceAsync(shoppingCart);
        }

        /// <summary>
        /// 移除购物车商品
        /// </summary>
        public async Task RemoveCartItemAsync(RemoveCartItemDto dto)
        {
            var shoppingCart = await _shoppingCartRepository.GetByIdAsync(dto.CartId);
            if (shoppingCart == null)
            {
                throw new ValidationException("购物车不存在");
            }

            var cartItem = shoppingCart.ShoppingCartItems?
                .FirstOrDefault(item => item.DishID == dto.DishId);

            if (cartItem == null)
            {
                throw new ValidationException("该菜品不在购物车中");
            }

            await _shoppingCartItemRepository.DeleteAsync(cartItem);
            await UpdateCartTotalPriceAsync(shoppingCart);
        }

        /// <summary>
        /// 更新购物车总价
        /// </summary>
        private async Task UpdateCartTotalPriceAsync(ShoppingCart cart)
        {
            var cartItems = await _shoppingCartItemRepository.GetByCartIdAsync(cart.CartID);
            cart.TotalPrice = cartItems.Sum(item => item.TotalPrice);
            cart.LastUpdatedTime = DateTime.UtcNow;
            await _shoppingCartRepository.UpdateAsync(cart);
        }
    }
}
