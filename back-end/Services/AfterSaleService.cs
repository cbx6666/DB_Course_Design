using BackEnd.DTOs.AfterSale;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 售后服务实现
    /// </summary>
    public class AfterSaleService : IAfterSaleService
    {
        private readonly IAfterSaleApplicationRepository _afterSaleRepository;
        private readonly IFoodOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="afterSaleRepository">售后申请仓储</param>
        /// <param name="orderRepository">订单仓储</param>
        /// <param name="customerRepository">客户仓储</param>
        public AfterSaleService(
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
        /// <param name="sellerId">商家ID</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="keyword">搜索关键词</param>
        /// <returns>售后申请列表</returns>
        public async Task<APageResultDto<AfterSaleApplicationDto>> GetAfterSalesAsync(int sellerId, int page, int pageSize, string? keyword)
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

            var applicationDtos = paginatedApplications.Select(c => new AfterSaleApplicationDto
            {
                Id = c.ApplicationID,
                OrderNo = $"ORD{c.OrderID}",
                User = new AUserInfoDto
                {
                    Name = c.Order?.Customer?.User?.Username ?? "未知用户",
                    Phone = c.Order?.Customer?.User?.PhoneNumber.ToString() ?? "未知电话",
                    Avatar = c.Order?.Customer?.User?.Avatar ?? ""
                },
                Reason = c.Description ?? "无售后原因描述",
                CreatedAt = c.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss")
            }).ToList();

            return new APageResultDto<AfterSaleApplicationDto>
            {
                List = applicationDtos,
                Total = total
            };
        }

        /// <summary>
        /// 根据ID获取售后申请
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <returns>售后申请详情</returns>
        public async Task<AfterSaleApplicationDto?> GetAfterSaleByIdAsync(int id)
        {
            var app = await _afterSaleRepository.GetByIdAsync(id);
            if (app == null)
            {
                return null;
            }

            return new AfterSaleApplicationDto
            {
                Id = app.ApplicationID,
                OrderNo = $"ORD{app.OrderID}",
                User = new AUserInfoDto
                {
                    Name = app.Order?.Customer?.User?.Username ?? "未知用户",
                    Phone = app.Order?.Customer?.User?.PhoneNumber.ToString() ?? "未知电话",
                    Avatar = app.Order?.Customer?.User?.Avatar ?? ""
                },
                Reason = app.Description ?? "无售后原因描述",
                CreatedAt = app.ApplicationTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        /// <summary>
        /// 处理售后申请
        /// </summary>
        /// <param name="id">售后申请ID</param>
        /// <param name="processDto">处理请求</param>
        /// <returns>处理结果</returns>
        public async Task<ProcessResponseDto> ProcessAfterSaleAsync(int id, ProcessAfterSaleDto processDto)
        {
            var app = await _afterSaleRepository.GetByIdAsync(id);
            if (app == null)
            {
                return new ProcessResponseDto
                {
                    Success = false,
                    Message = "售后申请不存在"
                };
            }

            return new ProcessResponseDto
            {
                Success = true,
                Message = "处理成功"
            };
        }
    }
}