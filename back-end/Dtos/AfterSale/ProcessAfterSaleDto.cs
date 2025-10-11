namespace BackEnd.DTOs.AfterSale
{
    /// <summary>
    /// 处理售后请求数据传输对象
    /// </summary>
    public class ProcessAfterSaleDto
    {
        /// <summary>
        /// 操作：approve/reject/negotiate
        /// </summary>
        public string Action { get; set; } = null!;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = null!;
    }
}