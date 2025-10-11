using System.Collections.Generic;

namespace BackEnd.DTOs.Review
{
    /// <summary>
    /// 评价分页结果
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class RPageResultDto<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T>? List { get; set; }
        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }
    }
}