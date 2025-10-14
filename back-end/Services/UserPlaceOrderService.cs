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
        private readonly IFoodOrderRepository _foodOrderRepository;

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
            // 账户与地址逻辑已迁移到 UserProfileService
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

        // 账户与地址相关方法已迁移至 UserProfileService
    }
}