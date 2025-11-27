using BackEnd.DTOs.Administrator;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 管理员信息服务实现
    /// </summary>
    public class AdminInfoService : IAdminInfoService
    {
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="administratorRepository">管理员仓储</param>
        public AdminInfoService(IAdministratorRepository administratorRepository)
        {
            _administratorRepository = administratorRepository;
        }

        /// <summary>
        /// 获取管理员信息
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <returns>管理员信息</returns>
        public async Task<GetAdminInfo?> GetAdministratorInfoAsync(int adminId)
        {
            try
            {
                var administrator = await _administratorRepository.GetByIdAsync(adminId);
                if (administrator?.User == null)
                {
                    return null;
                }

                var user = administrator.User;

                // 计算问题处理评分：售后评分与配送投诉评分的均值（0-5）
                decimal computedAverage = 0m;
                var afterRatings = administrator.EvaluateAfterSales
                    .Select(e => e.Application?.ConsumerRating)
                    .Where(r => r.HasValue)
                    .Select(r => (decimal)r!.Value)
                    .ToList();
                var complaintRatings = administrator.EvaluateComplaints
                    .Select(e => e.Complaint?.ConsumerRating)
                    .Where(r => r.HasValue)
                    .Select(r => (decimal)r!.Value)
                    .ToList();

                decimal? afterAvg = afterRatings.Count > 0 ? afterRatings.Average() : null;
                decimal? complaintAvg = complaintRatings.Count > 0 ? complaintRatings.Average() : null;

                if (afterAvg.HasValue && complaintAvg.HasValue)
                {
                    computedAverage = Math.Round(((afterAvg.Value + complaintAvg.Value) / 2m), 1, MidpointRounding.AwayFromZero);
                }
                else if (afterAvg.HasValue)
                {
                    computedAverage = Math.Round(afterAvg.Value, 1, MidpointRounding.AwayFromZero);
                }
                else if (complaintAvg.HasValue)
                {
                    computedAverage = Math.Round(complaintAvg.Value, 1, MidpointRounding.AwayFromZero);
                }
                else
                {
                    computedAverage = 0m;
                }

                return new GetAdminInfo
                {
                    Id = user.UserID.ToString(),
                    Username = user.Username,
                    RealName = user.FullName ?? user.Username,
                    RegistrationDate = administrator.AdminRegistrationTime.ToString("yyyy-MM-dd"),
                    AvatarUrl = user.Avatar ?? string.Empty,
                    Phone = user.PhoneNumber.ToString(),
                    Email = user.Email,
                    Gender = user.Gender ?? string.Empty,
                    ManagementScope = administrator.ManagedEntities,
                    AverageRating = computedAverage,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 更新管理员信息
        /// </summary>
        /// <param name="adminId">管理员ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        public async Task<SetAdminInfoResponse> UpdateAdministratorInfoAsync(int adminId, SetAdminInfo request)
        {
            try
            {
                var existingAdmin = await _administratorRepository.GetByIdAsync(adminId);
                if (existingAdmin?.User == null)
                {
                    return new SetAdminInfoResponse
                    {
                        Success = false,
                        Message = "管理员不存在"
                    };
                }

                // 更新允许修改的字段
                existingAdmin.User.Username = request.Username;
                existingAdmin.ManagedEntities = request.ManagementScope;

                // 处理性别（将 '男'/'女' 转换为 'M'/'F'）
                if (!string.IsNullOrWhiteSpace(request.Gender))
                {
                    existingAdmin.User.Gender = request.Gender == "男" ? "M" : (request.Gender == "女" ? "F" : request.Gender);
                }

                bool success = await _administratorRepository.UpdateAdministratorAsync(existingAdmin);

                if (success)
                {
                    // 返回完整的更新后信息
                    var updatedInfo = await GetAdministratorInfoAsync(adminId);
                    return new SetAdminInfoResponse
                    {
                        Success = true,
                        Message = "管理员信息更新成功",
                        Data = updatedInfo
                    };
                }
                else
                {
                    return new SetAdminInfoResponse
                    {
                        Success = false,
                        Message = "更新失败，请稍后重试"
                    };
                }
            }
            catch (Exception ex)
            {
                return new SetAdminInfoResponse
                {
                    Success = false,
                    Message = $"更新管理员信息时发生错误：{ex.Message}"
                };
            }
        }
    }
}
