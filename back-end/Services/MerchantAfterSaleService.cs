using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
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
        public async Task<PageResultDto<AfterSaleApplicationListItemDto>> GetAfterSalesAsync(int sellerId, int page, int pageSize, string? keyword)
        {
            var applications = await _afterSaleRepository.GetBySellerIdAsync(sellerId);

            // 应用搜索过滤
            if (!string.IsNullOrEmpty(keyword))
            {
                applications = applications
                    .Where(a => a != null)
                    .Where(a =>
                        a.OrderID.ToString().Contains(keyword) ||
                        (a.Description ?? "").Contains(keyword) ||
                        (a.Order?.Customer?.User?.PhoneNumber.ToString() ?? "").Contains(keyword))
                    .ToList();
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
                User = new UserProfileDto
                {
                    Name = c.Order?.Customer?.User?.Username ?? "未知用户",
                    PhoneNumber = c.Order?.Customer?.User?.PhoneNumber ?? 0,
                    Avatar = c.Order?.Customer?.User?.Avatar
                },
                Reason = c.Description ?? "无售后原因描述",
                CreatedAt = c.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss")
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
                User = new UserProfileDto
                {
                    Name = app.Order?.Customer?.User?.Username ?? "未知用户",
                    PhoneNumber = app.Order?.Customer?.User?.PhoneNumber ?? 0,
                    Avatar = app.Order?.Customer?.User?.Avatar
                },
                Reason = app.Description ?? "无售后原因描述",
                CreatedAt = app.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss")
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
