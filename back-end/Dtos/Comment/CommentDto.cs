namespace BackEnd.DTOs.Comment
{
    /// <summary>
    /// 评论信息（展示）
    /// </summary>
    public class GetCommentInfo
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public string ReviewId { get; set; } = null!;
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = null!;
        /// <summary>
        /// 评论内容
        /// </summary>
        public string Content { get; set; } = null!;
        /// <summary>
        /// 图片
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = null!;
        /// <summary>
        /// 评分
        /// </summary>
        public int Rating { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        public string SubmitTime { get; set; } = null!;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 评论信息（提交）
    /// </summary>
    public class SetCommentInfo
    {
        /// <summary>
        /// 评论ID
        /// </summary>
        public string ReviewId { get; set; } = null!;
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// 评论处理响应
    /// </summary>
    public class SetCommentInfoResponse
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = null!;
        /// <summary>
        /// 返回数据
        /// </summary>
        public GetCommentInfo? Data { get; set; }
    }
}
