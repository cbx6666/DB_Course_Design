namespace BackEnd.DTOs.Review
{
    /// <summary>
    /// 评价用户信息
    /// </summary>
    public class RUserInfoDto
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; } = null!;
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = null!;
        /// <summary>
        /// 头像URL
        /// </summary>
        public string? Avatar { get; set; }
    }
}