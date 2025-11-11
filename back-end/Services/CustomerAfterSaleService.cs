using BackEnd.Data;
using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.DTOs.Dish;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 售后申请服务实现（消费者侧）
    /// </summary>
    public class CustomerAfterSaleService : ICustomerAfterSaleService
    {
        private readonly IAfterSaleApplicationRepository _applicationRepository;
        private readonly IFoodOrderRepository _orderRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IEvaluate_AfterSaleRepository _evaluateAfterSaleRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerAfterSaleService(
            IAfterSaleApplicationRepository applicationRepository,
            IFoodOrderRepository orderRepository,
            IAdministratorRepository administratorRepository,
            IEvaluate_AfterSaleRepository evaluateAfterSaleRepository,
            AppDbContext context)
        {
            _applicationRepository = applicationRepository;
            _orderRepository = orderRepository;
            _administratorRepository = administratorRepository;
            _evaluateAfterSaleRepository = evaluateAfterSaleRepository;
            _context = context;
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        public async Task<CreateAfterSaleApplicationResponseDto> CreateApplicationAsync(CreateAfterSaleApplicationDto request, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 验证订单是否存在
                var order = await _orderRepository.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return Fail("订单不存在");
                }

                // 验证订单是否属于当前用户
                if (order.CustomerID != userId)
                {
                    return Fail("无权对此订单申请售后");
                }

                // 检查该订单是否已有售后申请（一个订单只能发起一次）
                var existingApplications = await _applicationRepository.GetByOrderIdAsync(request.OrderId);
                if (existingApplications.Any())
                {
                    return Fail("该订单已有售后申请，一个订单只能发起一次售后申请");
                }

                // 创建售后申请
                var application = new AfterSaleApplication
                {
                    OrderID = request.OrderId,
                    Description = request.Description,
                    ApplicationImages = request.Images,
                    ApplicationTime = DateTime.Now,
                    AfterSaleState = AfterSaleState.Pending
                };

                await _applicationRepository.AddAsync(application);
                await _applicationRepository.SaveAsync();

                // 分配给有"售后处理"权限的管理员
                var availableAdmins = await _administratorRepository.GetAdministratorsByManagedEntityAsync("售后处理");

                if (!availableAdmins.Any())
                {
                    return Fail("当前没有可用的售后处理管理员");
                }

                // 随机选择一名管理员
                var random = new Random();
                var adminList = availableAdmins.ToList();
                var selectedAdmin = adminList[random.Next(adminList.Count)];

                // 创建分配关系
                var evaluateAfterSale = new Evaluate_AfterSale
                {
                    AdminID = selectedAdmin.UserID,
                    ApplicationID = application.ApplicationID,
                };

                await _evaluateAfterSaleRepository.AddAsync(evaluateAfterSale);

                // 提交事务
                await transaction.CommitAsync();

                return new CreateAfterSaleApplicationResponseDto
                {
                    Success = true,
                    Message = "售后申请创建成功，已分配给相关管理员处理",
                    ApplicationId = application.ApplicationID
                };
            }
            catch (Exception ex)
            {
                // 回滚事务
                await transaction.RollbackAsync();
                return Fail($"创建售后申请失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取用户的售后申请列表
        /// </summary>
        public async Task<List<CustomerAfterSaleListItemDto>> GetMyAfterSalesAsync(int userId)
        {
            var applications = await _applicationRepository.GetByCustomerIdAsync(userId);

            var result = applications.Select(app => new CustomerAfterSaleListItemDto
            {
                ApplicationId = app.ApplicationID,
                OrderId = app.OrderID,
                StoreName = app.Order?.Store?.StoreName ?? "未知店铺",
                Description = app.Description,
                Images = string.IsNullOrWhiteSpace(app.ApplicationImages)
                    ? Array.Empty<string>()
                    : app.ApplicationImages.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                ApplicationTime = app.ApplicationTime,
                Status = app.AfterSaleState.ToString(),
                ProcessingResult = app.ProcessingResult ?? "-",
                ProcessingReason = string.IsNullOrWhiteSpace(app.ProcessingReason) ? null : app.ProcessingReason,
                DishDetails = app.Order?.Cart?.ShoppingCartItems?
                    .Select(item => new OrderDishDto
                    {
                        DishName = item.Dish?.DishName ?? "未知菜品",
                        DishImage = item.Dish?.DishImage ?? "",
                        Quantity = item.Quantity,
                        Price = item.Dish?.Price ?? 0m
                    })
                    .ToList() ?? new List<OrderDishDto>()
            }).ToList();

            return result;
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        private CreateAfterSaleApplicationResponseDto Fail(string message)
        {
            return new CreateAfterSaleApplicationResponseDto
            {
                Success = false,
                Message = message
            };
        }
    }
}
