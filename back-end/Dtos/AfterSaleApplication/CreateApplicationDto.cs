namespace BackEnd.DTOs.AfterSaleApplication
{
    /// <summary>
    /// 创建售后申请请求
    /// </summary>
    public class CreateApplicationDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }
        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// 创建售后申请结果
    /// </summary>
    public class CreateApplicationResult
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
        /// 申请ID
        /// </summary>
        public int? ApplicationId { get; set; }
    }
}
