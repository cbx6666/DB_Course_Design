namespace BackEnd.DTOs.MerchantInfo
{
    /// <summary>
    /// 商家资料信息
    /// </summary>
    public class MerchantProfileDto
    {
        /// <summary>
        /// 商家ID
        /// </summary>
        public string Id { get; set; } = null!;
        /// <summary>
        /// 商家名称
        /// </summary>
        public string Name { get; set; } = null!;
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
    }
}