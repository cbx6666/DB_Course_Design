using BackEnd.DTOs.Administrator;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 管理员服务实现
    /// </summary>
    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="administratorRepository">管理员仓储</param>
        public AdministratorService(IAdministratorRepository administratorRepository)
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

                return new GetAdminInfo
                {
                    Id = user.UserID.ToString(),
                    Username = user.Username,
                    RealName = user.FullName ?? user.Username,
                    Role = administrator.AdminRole,
                    RegistrationDate = administrator.AdminRegistrationTime.ToString("yyyy-MM-dd"),
                    AvatarUrl = user.Avatar ?? string.Empty,
                    Phone = user.PhoneNumber.ToString(),
                    Email = user.Email,
                    Gender = user.Gender ?? string.Empty,
                    BirthDate = user.Birthday?.ToString("yyyy-MM-dd") ?? string.Empty,
                    ManagementScope = administrator.ManagedEntities,
                    AverageRating = administrator.IssueHandlingScore,
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

                // 处理生日
                if (!string.IsNullOrWhiteSpace(request.BirthDate))
                {
                    if (DateTime.TryParse(request.BirthDate, out DateTime birthDate))
                    {
                        existingAdmin.User.Birthday = birthDate;
                    }
                    else
                    {
                        return new SetAdminInfoResponse
                        {
                            Success = false,
                            Message = "日期格式错误"
                        };
                    }
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
