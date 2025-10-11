namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 配送员个人资料数据传输对象
    /// </summary>
    public class CourierProfileDto
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 配送员ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; } = string.Empty;

        /// <summary>
        /// 评分
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// 信用分数
        /// </summary>
        public int CreditScore { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }
    }
}