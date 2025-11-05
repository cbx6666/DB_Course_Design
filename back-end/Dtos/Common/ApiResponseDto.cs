namespace BackEnd.DTOs.Common
{
    /// <summary>
    /// 统一API响应数据传输对象（基类）
    /// 统一格式：{ Success, Code, Message }
    /// </summary>
    public class ApiResponseDto
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 响应代码（HTTP状态码或业务码）
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }

    /// <summary>
    /// 统一API响应数据传输对象（泛型派生类）
    /// 统一格式：{ Success, Code, Message, Data }
    /// 当需要返回数据时，使用此派生类，通过 Data 属性传递数据
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    public class ApiResponseDto<T> : ApiResponseDto
    {
        /// <summary>
        /// 响应数据（派生类属性，需要时赋值）
        /// </summary>
        public T? Data { get; set; }
    }
}
