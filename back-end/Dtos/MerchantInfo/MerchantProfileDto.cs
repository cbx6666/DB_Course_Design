namespace BackEnd.DTOs.MerchantInfo
{
    /// <summary>
    /// 商家资料信息
    /// </summary>
    public class MerchantProfileDto
    {
		/// <summary>
		/// 商家ID（前端可不展示）
		/// </summary>
		public string Id { get; set; } = null!;
		/// <summary>
		/// 用户名（可修改）
		/// </summary>
		public string Username { get; set; } = null!;
		/// <summary>
		/// 真实姓名
		/// </summary>
		public string? FullName { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = null!;
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = null!;
        /// <summary>
        /// 注册时间
        /// </summary>
        public string RegisterTime { get; set; } = null!;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = null!;
		/// <summary>
		/// 头像URL
		/// </summary>
		public string? Avatar { get; set; }
    }
}