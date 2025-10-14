namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 商家信息数据传输对象
    /// </summary>
    public class MerchantInfoResponseDto
    {
        /// <summary>
        /// 商家用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 商家用户ID
        /// </summary>
        public int SellerId { get; set; }

		/// <summary>
		/// 商家头像
		/// </summary>
		public string? Avatar { get; set; }
    }
}