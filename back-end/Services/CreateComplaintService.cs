using BackEnd.Data;
using BackEnd.DTOs.DeliveryComplaint;
using BackEnd.Models.Enums;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 创建配送投诉服务实现
    /// </summary>
    public class CreateComplaintService : ICreateComplaintService
    {
        private readonly IDeliveryComplaintRepository _complaintRepository;
        private readonly IDeliveryTaskRepository _deliveryTaskRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="complaintRepository">投诉仓储</param>
        /// <param name="deliveryTaskRepository">配送任务仓储</param>
        /// <param name="administratorRepository">管理员仓储</param>
        /// <param name="context">数据库上下文</param>
        public CreateComplaintService(
            IDeliveryComplaintRepository complaintRepository,
            IDeliveryTaskRepository deliveryTaskRepository,
            IAdministratorRepository administratorRepository,
            AppDbContext context)
        {
            _complaintRepository = complaintRepository;
            _deliveryTaskRepository = deliveryTaskRepository;
            _administratorRepository = administratorRepository;
            _context = context;
        }

        /// <summary>
        /// 创建配送投诉
        /// </summary>
        /// <param name="request">创建投诉请求</param>
        /// <param name="userId">用户ID</param>
        /// <returns>创建结果</returns>
        public async Task<CreateComplaintResult> CreateComplaintAsync(CreateComplaintDto request, int userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 验证配送任务是否存在
                int deliveryTaskId;

                if (request.DeliveryTaskId.HasValue)
                {
                    deliveryTaskId = request.DeliveryTaskId.Value;
                }
                else if (request.OrderId.HasValue)
                {
                    var task = await _deliveryTaskRepository.GetByOrderIdAsync(request.OrderId.Value);
                    if (task == null)
                        return Fail("该订单没有对应的配送任务");
                    deliveryTaskId = task.TaskID;
                }
                else
                {
                    return Fail("必须提供订单ID或配送任务ID");
                }

                var deliveryTask = await _deliveryTaskRepository.GetByIdAsync(deliveryTaskId);
                if (deliveryTask == null)
                {
                    return Fail("配送任务不存在");
                }

                // 验证配送任务是否属于当前用户的订单
                if (deliveryTask.CustomerID != userId)
                {
                    return Fail("无权对此配送任务发起投诉");
                }

                // 检查配送任务状态
                if (deliveryTask.Status != DeliveryStatus.Delivering &&
                    deliveryTask.Status != DeliveryStatus.Completed)
                {
                    return Fail("该配送任务当前状态不支持发起投诉");
                }

                // 分配给有"投诉处理"权限的管理员
                var availableAdmins = await _administratorRepository.GetAdministratorsByManagedEntityAsync("配送投诉");
                if (!availableAdmins.Any())
                {
                    return Fail("当前没有可用的投诉处理管理员");
                }

                // 创建配送投诉
                var complaint = new DeliveryComplaint
                {
                    DeliveryTaskID = deliveryTaskId,
                    CourierID = deliveryTask.CourierID!.Value,
                    CustomerID = userId,
                    ComplaintReason = request.ComplaintReason,
                    ComplaintTime = DateTime.Now,
                    ComplaintState = ComplaintState.Pending
                };

                await _complaintRepository.AddAsync(complaint);
                await _complaintRepository.SaveAsync();

                // 随机选择一名管理员并创建分配关系
                var random = new Random();
                var adminList = availableAdmins.ToList();
                var selectedAdmin = adminList[random.Next(adminList.Count)];

                var evaluateComplaint = new Evaluate_Complaint
                {
                    AdminID = selectedAdmin.UserID,
                    ComplaintID = complaint.ComplaintID,
                };

                await _context.Evaluate_Complaints.AddAsync(evaluateComplaint);
                await _context.SaveChangesAsync();

                // 提交事务
                await transaction.CommitAsync();

                return new CreateComplaintResult
                {
                    Success = true,
                    Message = "配送投诉提交成功，已分配给相关管理员处理",
                    ComplaintId = complaint.ComplaintID
                };
            }
            catch (Exception ex)
            {
                // 回滚事务
                await transaction.RollbackAsync();
                return Fail($"提交配送投诉失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 创建失败结果
        /// </summary>
        /// <param name="message">失败消息</param>
        /// <returns>失败结果</returns>
        private CreateComplaintResult Fail(string message)
        {
            Console.WriteLine($"[CreateComplaintService] {message}");
            return new CreateComplaintResult
            {
                Success = false,
                Message = message
            };
        }
    }
}
