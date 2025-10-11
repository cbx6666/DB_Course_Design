namespace BackEnd.DTOs.AfterSale
{
    /// <summary>
    /// 售后用户信息
    /// </summary>
    public class AUserInfoDto
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