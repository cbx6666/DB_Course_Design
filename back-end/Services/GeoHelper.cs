using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 地理位置辅助服务
    /// </summary>
    public class GeoHelper : IGeoHelper
    {
        /// <summary>
        /// 地球半径（公里）
        /// </summary>
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// 计算两点之间的距离
        /// </summary>
        /// <param name="lat1">第一个点的纬度</param>
        /// <param name="lon1">第一个点的经度</param>
        /// <param name="lat2">第二个点的纬度</param>
        /// <param name="lon2">第二个点的经度</param>
        /// <returns>两点之间的距离（公里）</returns>
        public double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2)
        {
            var dLat = ToRadians((double)(lat2 - lat1));
            var dLon = ToRadians((double)(lon2 - lon1));

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// 将角度转换为弧度
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns>弧度</returns>
        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}