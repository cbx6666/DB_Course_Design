using BackEnd.DTOs.Order;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Courier;
using BackEnd.DTOs.Dish;
using BackEnd.DTOs.Coupon;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 订单服务实现（消费者侧）
    /// </summary>
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IFoodOrderRepository _orderRepo;
        private readonly IShoppingCartRepository _cartRepo;
        private readonly ICouponRepository _couponRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly IDeliveryTaskRepository _deliveryTaskRepo;
        private readonly ICourierRepository _courierRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerOrderService(
            IFoodOrderRepository orderRepo,
            IShoppingCartRepository cartRepo,
            ICouponRepository couponRepo,
            ICustomerRepository customerRepo,
            IStoreRepository storeRepo,
            IDeliveryTaskRepository deliveryTaskRepo,
            ICourierRepository courierRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _couponRepo = couponRepo;
            _customerRepo = customerRepo;
            _storeRepo = storeRepo;
            _deliveryTaskRepo = deliveryTaskRepo;
            _courierRepo = courierRepo;
        }

        /// <summary>
        /// 创建订单
        /// </summary>
        public async Task<ApiResponseDto> CreateOrderAsync(CreateOrderDto dto)
        {
            // 验证 CustomerID 是否在 CUSTOMERS 表中存在
            // CustomerID 实际上就是 UserID（Customer 表的主键是 UserID）
            var customer = await _customerRepo.GetByIdAsync(dto.CustomerId);
            if (customer == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "用户不存在或不是顾客，无法创建订单"
                };
            }

            var cart = await _cartRepo.GetByIdAsync(dto.CartId);
            if (cart == null || cart.ShoppingCartItems?.Count == 0)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 400,
                    Message = "购物车为空，无法生成订单"
                };
            }

            var orderTotal = cart.ShoppingCartItems?.Sum(item => item.TotalPrice) ?? 0;
            
            Coupon? usedCoupon = null;
            if (dto.CouponId.HasValue && dto.CouponId.Value > 0)
            {
                usedCoupon = await _couponRepo.GetByIdAsync(dto.CouponId.Value);
                
                if (usedCoupon == null)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 404,
                        Message = "优惠券不存在"
                    };
                }

                if (usedCoupon.CustomerID != dto.CustomerId)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 403,
                        Message = "优惠券不属于当前用户"
                    };
                }

                if (usedCoupon.CouponState != CouponState.Unused)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券已使用或无效"
                    };
                }

                if (usedCoupon.IsExpired)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券已过期"
                    };
                }

                var now = DateTime.Now;
                if (now < usedCoupon.CouponManager.ValidFrom || now > usedCoupon.CouponManager.ValidTo)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = "优惠券不在有效期内"
                    };
                }

                if (usedCoupon.CouponManager.StoreID != dto.StoreId)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 403,
                        Message = "优惠券不属于当前店铺"
                    };
                }

                if (orderTotal < usedCoupon.CouponManager.MinimumSpend)
                {
                    return new ApiResponseDto
                    {
                        Success = false,
                        Code = 400,
                        Message = $"订单金额未达到优惠券使用门槛（需满{usedCoupon.CouponManager.MinimumSpend:F2}元）"
                    };
                }
            }

            var foodOrder = new FoodOrder
            {
                CustomerID = dto.CustomerId,
                CartID = dto.CartId,
                StoreID = dto.StoreId,
                DeliveryInfoID = dto.DeliveryInfoID,
                DeliveryFee = dto.DeliveryFee,
                OrderTime = DateTime.Now,
                PaymentTime = dto.PaymentTime,
                Remarks = dto.Remarks,
                FoodOrderState = FoodOrderState.Pending
            };

            await _orderRepo.AddAsync(foodOrder);
            await _orderRepo.SaveAsync();

            if (usedCoupon != null)
            {
                usedCoupon.OrderID = foodOrder.OrderID;
                usedCoupon.CouponState = CouponState.Used;
                await _couponRepo.UpdateAsync(usedCoupon);
                await _couponRepo.SaveAsync();
            }

            cart.ShoppingCartState = ShoppingCartState.Done;
            cart.LastUpdatedTime = DateTime.UtcNow;
            await _cartRepo.UpdateAsync(cart);

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "订单创建成功"
            };
        }

        /// <summary>
        /// 获取订单历史
        /// </summary>
        public async Task<List<CustomerOrderViewDto>> GetOrderHistoryAsync(int userId)
        {
            var allOrders = await _orderRepo.GetAllAsync();
            var orders = allOrders
                .Where(o => o.CustomerID == userId)
                .OrderByDescending(o => o.OrderTime)
                .ToList();

            var result = new List<CustomerOrderViewDto>();

            foreach (var order in orders)
            {
                var store = await _storeRepo.GetStoreInfoForUserAsync(order.StoreID);

                List<string> dishImages = new List<string>();
                List<OrderDishDto> dishDetails = new List<OrderDishDto>();
                decimal totalAmount = 0;

                if (order.CartID.HasValue)
                {
                    var cart = await _cartRepo.GetByIdAsync(order.CartID.Value);

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
                                Quantity = sci.Quantity,
                                Price = sci.Dish.Price
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
                            DiscountType = coupon.CouponManager.CouponType == CouponType.Fixed ? "fixed" : "discount",
                            DiscountValue = coupon.CouponManager.Value,
                            ValidFrom = coupon.CouponManager.ValidFrom.ToString("o"),
                            ValidTo = coupon.CouponManager.ValidTo.ToString("o"),
                            IsUsed = coupon.CouponState == CouponState.Used
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
        /// 获取订单配送信息（消费者侧）
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>配送信息</returns>
        public async Task<OrderDeliveryInfoDto> GetOrderDeliveryInfoAsync(int orderId)
        {
            var task = await _deliveryTaskRepo.GetByOrderIdAsync(orderId);
            
            // 即使没有配送任务，也要获取订单的收货信息
            FoodOrder? order = null;
            if (task != null)
            {
                order = task.Order;
            }
            else
            {
                // 如果没有配送任务，直接从订单仓储获取订单信息（包含 DeliveryInfo）
                order = await _orderRepo.GetByIdAsync(orderId);
            }

            // 获取收货信息
            OrderDeliveryDetailDto? orderDetail = null;
            if (order?.DeliveryInfo != null)
            {
                orderDetail = new OrderDeliveryDetailDto
                {
                    DeliveryName = order.DeliveryInfo.Name,
                    DeliveryPhone = order.DeliveryInfo.PhoneNumber,
                    DeliveryAddress = order.DeliveryInfo.Address
                };
            }

            // 如果没有配送任务，只返回订单的收货信息
            if (task == null)
            {
                return new OrderDeliveryInfoDto
                {
                    TaskId = 0,
                    Status = 0,
                    Order = orderDetail
                };
            }

            // 有配送任务，返回完整的配送信息
            var courier = task.CourierID.HasValue
                              ? await _courierRepo.GetByIdAsync(task.CourierID.Value)
                              : null;

            // 构建返回数据（只包含前端需要的字段）
            var result = new OrderDeliveryInfoDto
            {
                TaskId = task.TaskID,
                Status = (int)task.Status,
                AcceptTime = task.AcceptTime.ToString("yyyy-MM-dd HH:mm:ss"),
                EstimatedArrivalTime = task.EstimatedArrivalTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ActualPickupTime = task.PickupTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                EstimatedDeliveryTime = task.EstimatedDeliveryTime.ToString("yyyy-MM-dd HH:mm:ss"),
                ActualDeliveryTime = task.CompletionTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                Courier = courier == null ? null : new CourierSummaryDto
                {
                    UserId = courier.UserID,
                    CourierRegistrationTime = courier.CourierRegistrationTime.ToString("o"),
                    VehicleType = courier.VehicleType,
                    ReputationPoints = courier.ReputationPoints,
                    TotalDeliveries = courier.TotalDeliveries,
                    AvgDeliveryTime = courier.AvgDeliveryTime,
                    AverageRating = courier.AverageRating,
                    MonthlySalary = courier.MonthlySalary,
                    FullName = courier.User?.FullName,
                    PhoneNumber = courier.User?.PhoneNumber,
                    Longitude = courier.CourierLongitude,
                    Latitude = courier.CourierLatitude
                },
                Order = orderDetail
            };

            return result;
        }
    }
}
