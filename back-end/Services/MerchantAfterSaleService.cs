using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Dish;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 售后申请服务实现（商家侧）
    /// </summary>
    public class MerchantAfterSaleService : IMerchantAfterSaleService
    {
        private readonly IAfterSaleApplicationRepository _afterSaleRepository;
        private readonly IFoodOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantAfterSaleService(
            IAfterSaleApplicationRepository afterSaleRepository,
            IFoodOrderRepository orderRepository,
            ICustomerRepository customerRepository)
        {
            _afterSaleRepository = afterSaleRepository;
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// 获取售后申请列表
        /// </summary>
        public async Task<PageResultDto<AfterSaleApplicationListItemDto>> GetAfterSalesAsync(int sellerId, int page, int pageSize, string? keyword, string? field)
        {
            var applications = await _afterSaleRepository.GetBySellerIdAsync(sellerId);

            // 应用搜索过滤
            if (!string.IsNullOrEmpty(keyword))
            {
                var normalized = keyword;
                if (field == "orderNo")
                {
                    normalized = new string(keyword.Where(char.IsDigit).ToArray());
                }

                if (!string.IsNullOrWhiteSpace(field))
                {
                    applications = field switch
                    {
                        "content" => applications.Where(a => (a.Description ?? "").Contains(keyword)).ToList(),
                        "orderNo" => applications.Where(a => a.OrderID.ToString().Contains(normalized)).ToList(),
                        "user.name" => applications.Where(a => (a.Order?.Customer?.User?.Username ?? "").Contains(keyword)).ToList(),
                        _ => applications
                    };
                }
                else
                {
                    applications = applications
                        .Where(a => a != null)
                        .Where(a =>
                            a.OrderID.ToString().Contains(normalized) ||
                            (a.Description ?? "").Contains(keyword) ||
                            (a.Order?.Customer?.User?.PhoneNumber.ToString() ?? "").Contains(keyword) ||
                            (a.Order?.Customer?.User?.Username ?? "").Contains(keyword))
                        .ToList();
                }
            }

            // 分页处理
            var total = applications.Count();
            var paginatedApplications = applications
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var applicationDtos = paginatedApplications.Select(c => new AfterSaleApplicationListItemDto
            {
                Id = c.ApplicationID,
                OrderNo = $"ORD{c.OrderID}",
                OrderId = c.OrderID,
                User = new UserProfileDto
                {
                    Name = c.Order?.Customer?.User?.Username ?? "未知用户",
                    PhoneNumber = c.Order?.Customer?.User?.PhoneNumber ?? 0,
                    Avatar = c.Order?.Customer?.User?.Avatar
                },
                Reason = c.Description ?? "无售后原因描述",
                Images = string.IsNullOrWhiteSpace(c.ApplicationImages)
                    ? Array.Empty<string>()
                    : c.ApplicationImages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                CreatedAt = c.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                DishDetails = c.Order?.Cart?.ShoppingCartItems?
                    .Select(item => new OrderDishDto
                    {
                        DishName = item.Dish?.DishName ?? "未知菜品",
                        DishImage = item.Dish?.DishImage ?? "",
                        Quantity = item.Quantity,
                        Price = item.Dish?.Price ?? 0m
                    })
                    .ToList() ?? new List<OrderDishDto>()
            }).ToList();

            return new PageResultDto<AfterSaleApplicationListItemDto>
            {
                List = applicationDtos,
                Total = total
            };
        }

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        public async Task<AfterSaleApplicationListItemDto?> GetAfterSaleByIdAsync(int id)
        {
            var app = await _afterSaleRepository.GetByIdAsync(id);
            if (app == null)
            {
                return null;
            }

            return new AfterSaleApplicationListItemDto
            {
                Id = app.ApplicationID,
                OrderNo = $"ORD{app.OrderID}",
                OrderId = app.OrderID,
                User = new UserProfileDto
                {
                    Name = app.Order?.Customer?.User?.Username ?? "未知用户",
                    PhoneNumber = app.Order?.Customer?.User?.PhoneNumber ?? 0,
                    Avatar = app.Order?.Customer?.User?.Avatar
                },
                Reason = app.Description ?? "无售后原因描述",
                Images = string.IsNullOrWhiteSpace(app.ApplicationImages)
                    ? Array.Empty<string>()
                    : app.ApplicationImages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                CreatedAt = app.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                DishDetails = app.Order?.Cart?.ShoppingCartItems?
                    .Select(item => new OrderDishDto
                    {
                        DishName = item.Dish?.DishName ?? "未知菜品",
                        DishImage = item.Dish?.DishImage ?? "",
                        Quantity = item.Quantity,
                        Price = item.Dish?.Price ?? 0m
                    })
                    .ToList() ?? new List<OrderDishDto>()
            };
        }

        /// <summary>
        /// 处理售后申请
        /// </summary>
        public async Task<ApiResponseDto> ProcessAfterSaleAsync(int id, ProcessAfterSaleDto processDto)
        {
            var app = await _afterSaleRepository.GetByIdAsync(id);
            if (app == null)
            {
                return new ApiResponseDto
                {
                    Success = false,
                    Code = 404,
                    Message = "售后申请不存在"
                };
            }

            return new ApiResponseDto
            {
                Success = true,
                Code = 200,
                Message = "处理成功"
            };
        }
    }
}
