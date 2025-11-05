using BackEnd.DTOs.AuthRequest;
using BackEnd.DTOs.User;
using BackEnd.DTOs.Common;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using BackEnd.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Services
{
    /// <summary>
    /// 认证服务（整合登录和注册功能）
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(
            IUserRepository userRepository,
            IStoreRepository storeRepository,
            IConfiguration configuration,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public async Task<LoginResult> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByPhoneAsync(long.Parse(request.PhoneNum));
            if (user == null)
                return FailLogin("手机号或密码错误", 401);

            if (!await IsPasswordValid(request.Password, user))
                return FailLogin("手机号或密码错误", 401);

            if (!IsRoleMatch(user.Role, request.Role))
                return FailLogin("角色选择错误，请选择正确的登录身份", 403);

            var token = GenerateJwtToken(user);

            return new LoginResult
            {
                Success = true,
                Code = 200,
                Message = "登录成功",
                Token = token,
                User = new UserInfo
                {
                    UserId = user.UserID,
                    Username = user.Username,
                    Role = user.Role.ToString().ToLower(),
                    Avatar = user.Avatar
                }
            };
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        public async Task LogoutAsync(int userId)
        {
            // 登出逻辑可以根据需要实现，例如记录日志或清理缓存
            await Task.CompletedTask;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        public async Task<ApiResponseDto> RegisterAsync(RegisterRequest req)
        {
            if (req.Password != req.ConfirmPassword)
                return new ApiResponseDto { Success = false, Code = 400, Message = "两次密码不一致" };

            if (await _userRepository.ExistsByPhoneAsync(req.Phone))
                return new ApiResponseDto { Success = false, Code = 409, Message = "手机号已被注册" };

            var user = new User
            {
                Username = req.Nickname,
                Password = HashPassword(req.Password),
                PhoneNumber = long.TryParse(req.Phone, out var phone) ? phone :
                    throw new ArgumentException("手机号格式错误"),
                Email = req.Email,
                Gender = req.Gender,
                Birthday = !string.IsNullOrEmpty(req.Birthday) ? DateTime.Parse(req.Birthday) : null,
                Avatar = string.IsNullOrWhiteSpace(req.AvatarUrl) 
                         ? "/images/default-avatar.jpg" 
                         : req.AvatarUrl,
                AccountCreationTime = DateTime.UtcNow,
                Role = MapStringToRole(req.Role)
            };

            switch (req.Role.ToLower())
            {
                case "rider":
                    if (req.RiderInfo == null)
                        return FailRegister("缺少 RiderInfo");

                    if (req.RiderInfo.Name.Trim().Length > 6)
                        return FailRegister("真实姓名长度不能超过6个字符", 400);

                    user.FullName = req.RiderInfo.Name.Trim();
                    user.Courier = new Courier
                    {
                        VehicleType = req.RiderInfo.VehicleType,
                        CourierRegistrationTime = DateTime.Now
                    };
                    break;

                case "admin":
                    if (req.AdminInfo == null)
                        return FailRegister("缺少 AdminInfo");

                    if (req.AdminInfo.Name.Trim().Length > 6)
                        return FailRegister("真实姓名长度不能超过6个字符", 400);

                    user.FullName = req.AdminInfo.Name.Trim();
                    user.Administrator = new Administrator
                    {
                        ManagedEntities = req.AdminInfo.ManagementObject,
                        AdminRegistrationTime = DateTime.Now
                    };
                    break;

                case "merchant":
                    if (req.StoreInfo == null)
                        return FailRegister("缺少 StoreInfo");

                    if (req.StoreInfo.SellerName.Trim().Length > 6)
                        return FailRegister("真实姓名长度不能超过6个字符", 400);

                    user.FullName = req.StoreInfo.SellerName.Trim();
                    user.Seller = new Seller
                    {
                        SellerRegistrationTime = DateTime.Now
                    };
                    break;

                case "customer":
                    user.Customer = new Customer();
                    break;

                default:
                    return FailRegister("不支持的角色类型");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _userRepository.AddAsync(user);

                if (req.Role.ToLower() == "merchant" && req.StoreInfo != null && user.Seller != null)
                {
                    await CreateStoreAsync(req.StoreInfo, user.UserID);
                }

                await transaction.CommitAsync();

                return new ApiResponseDto
                {
                    Success = true,
                    Code = 201,
                    Message = "注册成功！"
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return FailRegister($"注册失败: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        private Task<bool> IsPasswordValid(string password, Models.User user)
        {
            return Task.FromResult(BCrypt.Net.BCrypt.Verify(password, user.Password));
        }

        /// <summary>
        /// 检查角色是否匹配
        /// </summary>
        private bool IsRoleMatch(UserIdentity userRole, string requestRole)
        {
            var roleMapping = new Dictionary<string, UserIdentity>
            {
                { "customer", UserIdentity.Customer },
                { "rider", UserIdentity.Courier },
                { "merchant", UserIdentity.Merchant },
                { "admin", UserIdentity.Administrator }
            };

            return roleMapping.TryGetValue(requestRole.ToLower(), out var expectedRole)
                   && userRole == expectedRole;
        }

        /// <summary>
        /// 生成JWT Token
        /// </summary>
        private string GenerateJwtToken(Models.User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key未配置");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer未配置");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username ?? ""),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtIssuer,
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 字符串角色映射方法
        /// </summary>
        private UserIdentity MapStringToRole(string roleStr)
        {
            return roleStr.ToLower() switch
            {
                "customer" => UserIdentity.Customer,
                "rider" => UserIdentity.Courier,
                "admin" => UserIdentity.Administrator,
                "merchant" => UserIdentity.Merchant,
                _ => throw new ArgumentException($"不支持的角色类型: {roleStr}")
            };
        }

        /// <summary>
        /// 密码加密
        /// </summary>
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, 12);
        }

        /// <summary>
        /// 注册店铺
        /// </summary>
        private async Task CreateStoreAsync(StoreInfoDto storeInfo, int sellerId)
        {
            if (!TimeSpan.TryParse(storeInfo.OpenTime, out var openTime))
                throw new ArgumentException("营业开始时间格式错误，请使用 HH:mm 格式");

            if (!TimeSpan.TryParse(storeInfo.CloseTime, out var closeTime))
                throw new ArgumentException("营业结束时间格式错误，请使用 HH:mm 格式");

            if (!DateTime.TryParse(storeInfo.EstablishmentDate, out var establishmentDate))
                establishmentDate = DateTime.Now;

            var store = new Store
            {
                StoreName = storeInfo.StoreName.Trim(),
                StoreAddress = storeInfo.Address.Trim(),
                OpenTime = openTime,
                CloseTime = closeTime,
                StoreCreationTime = establishmentDate,
                StoreCategory = storeInfo.Category,
                SellerID = sellerId
            };

            await _storeRepository.AddAsync(store);
        }

        /// <summary>
        /// 表示登录失败
        /// </summary>
        private LoginResult FailLogin(string message, int code = 400)
        {
            return new LoginResult
            {
                Success = false,
                Code = code,
                Message = message,
                Token = null,
                User = null
            };
        }

        /// <summary>
        /// 表示注册失败
        /// </summary>
        private ApiResponseDto FailRegister(string message, int code = 400)
        {
            return new ApiResponseDto
            {
                Success = false,
                Code = code,
                Message = message
            };
        }
    }
}
