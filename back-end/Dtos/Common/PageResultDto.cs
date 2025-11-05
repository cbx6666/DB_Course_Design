namespace BackEnd.DTOs.Common
{
    /// <summary>
    /// 通用分页结果数据传输对象
    /// 用于统一所有分页列表的响应格式
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class PageResultDto<T>
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<T> List { get; set; } = new List<T>();

        /// <summary>
        /// 总数量
        /// </summary>
        public int Total { get; set; }
    }
}
