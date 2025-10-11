namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 用户基本信息数据传输对象
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// 用户角色
        /// </summary>
        public required string Role { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string? Avatar { get; set; }
    }
}
