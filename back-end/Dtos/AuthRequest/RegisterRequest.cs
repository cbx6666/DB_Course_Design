using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.AuthRequest
{
    /// <summary>
    /// 注册请求
    /// </summary>
    public class RegisterRequest
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public required string Nickname { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public required string Password { get; set; }
        /// <summary>
        /// 确认密码
        /// </summary>
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"^1[3-9]\d{9}$", ErrorMessage = "请输入正确的手机号码")]
        /// <summary>
        /// 手机号
        /// </summary>
        public required string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public required string Gender { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public required string Birthday { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public required string Role { get; set; }
        /// <summary>
        /// 头像URL
        /// </summary>
        public string? AvatarUrl { get; set; }

        /// <summary>
        /// 骑手信息
        /// </summary>
        public RiderInfoDto? RiderInfo { get; set; }
        /// <summary>
        /// 管理员信息
        /// </summary>
        public AdminInfoDto? AdminInfo { get; set; }
        /// <summary>
        /// 商家信息
        /// </summary>
        public StoreInfoDto? StoreInfo { get; set; }
    }

    /// <summary>
    /// 骑手信息
    /// </summary>
    public class RiderInfoDto
    {
        /// <summary>
        /// 交通工具类型
        /// </summary>
        public required string VehicleType { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public required string Name { get; set; }
    }

    /// <summary>
    /// 管理员信息
    /// </summary>
    public class AdminInfoDto
    {
        /// <summary>
        /// 管理对象
        /// </summary>
        public required string ManagementObject { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public required string Name { get; set; }
    }

    /// <summary>
    /// 商家信息
    /// </summary>
    public class StoreInfoDto
    {
        /// <summary>
        /// 商家姓名
        /// </summary>
        public required string SellerName { get; set; }
        /// <summary>
        /// 店铺名称
        /// </summary>
        public required string StoreName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public required string Address { get; set; }
        /// <summary>
        /// 营业开始时间
        /// </summary>
        public required string OpenTime { get; set; }
        /// <summary>
        /// 营业结束时间
        /// </summary>
        public required string CloseTime { get; set; }
        /// <summary>
        /// 成立日期
        /// </summary>
        public required string EstablishmentDate { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public required string Category { get; set; }
    }

    /// <summary>
    /// 注册结果
    /// </summary>
    public class RegisterResult
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 业务码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Message { get; set; } = "";
    }
}
