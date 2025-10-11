namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 地理位置辅助服务接口
    /// </summary>
    public interface IGeoHelper
    {
        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        /// <param name="lat1">第一个点的纬度</param>
        /// <param name="lon1">第一个点的经度</param>
        /// <param name="lat2">第二个点的纬度</param>
        /// <param name="lon2">第二个点的经度</param>
        /// <returns>距离（单位：公里）</returns>
        double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2);
    }
}