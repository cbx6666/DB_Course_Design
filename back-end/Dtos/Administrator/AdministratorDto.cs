namespace BackEnd.DTOs.Administrator
{
    /// <summary>
    /// 管理员信息（展示）
    /// </summary>
    public class GetAdminInfo
    {
        /// <summary>
        /// 管理员ID
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; } = string.Empty;
        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; } = string.Empty;
        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegistrationDate { get; set; } = string.Empty;
        /// <summary>
        /// 头像URL
        /// </summary>
        public string AvatarUrl { get; set; } = string.Empty;
        /// <summary>
        /// 手机
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; } = string.Empty;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthDate { get; set; } = string.Empty;
        /// <summary>
        /// 管理范围
        /// </summary>
        public string ManagementScope { get; set; } = string.Empty;
        /// <summary>
        /// 平均评分
        /// </summary>
        public decimal AverageRating { get; set; }
    }

    /// <summary>
    /// 管理员信息（提交）
    /// </summary>
    public class SetAdminInfo
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// 管理范围
        /// </summary>
        public string ManagementScope { get; set; } = string.Empty;
        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthDate { get; set; } = string.Empty;
    }

    /// <summary>
    /// 管理员信息提交响应
    /// </summary>
    public class SetAdminInfoResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 返回数据
        /// </summary>
        public GetAdminInfo? Data { get; set; }
    }
}
