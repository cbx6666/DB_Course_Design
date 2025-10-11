namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 更新位置请求
    /// </summary>
    public class UpdateLocationDto
    {
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }
    }
}