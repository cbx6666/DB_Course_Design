using BackEnd.Data;
using BackEnd.DTOs.AfterSaleApplication;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 创建售后申请服务实现
    /// </summary>
    public class CreateApplicationService : ICreateApplicationService
    {
        private readonly IAfterSaleApplicationRepository _applicationRepository;
        private readonly IFoodOrderRepository _orderRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="applicationRepository">售后申请仓储</param>
        /// <param name="orderRepository">订单仓储</param>
        /// <param name="administratorRepository">管理员仓储</param>
        /// <param name="context">数据库上下文</param>
        public CreateApplicationService(
            IAfterSaleApplicationRepository applicationRepository,
            IFoodOrderRepository orderRepository,
            IAdministratorRepository administratorRepository,
            AppDbContext context)
        {
            _applicationRepository = applicationRepository;
            _orderRepository = orderRepository;
            _administratorRepository = administratorRepository;
            _context = context;
        }

        /// <summary>
        /// 创建售后申请
        /// </summary>
        /// <param name="request">创建申请请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        public async Task<CreateApplicationResult> CreateApplicationAsync(CreateApplicationDto request, int userId)
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

                // 创建售后申请
                var application = new AfterSaleApplication
                {
                    OrderID = request.OrderId,
                    Description = request.Description,
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

                await _context.Evaluate_AfterSales.AddAsync(evaluateAfterSale);
                await _context.SaveChangesAsync();

                // 提交事务
                await transaction.CommitAsync();

                return new CreateApplicationResult
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
        /// 创建失败结果
        /// </summary>
        /// <param name="message">失败消息</param>
        /// <returns>失败结果</returns>
        private CreateApplicationResult Fail(string message)
        {
            return new CreateApplicationResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
