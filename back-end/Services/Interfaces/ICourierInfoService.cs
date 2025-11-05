using BackEnd.DTOs.Courier;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送员信息服务接口（骑手侧）
    /// </summary>
    public interface ICourierInfoService
    {
        /// <summary>
        /// 获取配送员档案
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>配送员档案</returns>
        Task<CourierProfileDto?> GetProfileAsync(int courierId);

        /// <summary>
        /// 获取工作状态
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>工作状态</returns>
        Task<WorkStatusDto?> GetWorkStatusAsync(int courierId);

        /// <summary>
        /// 获取当前位置
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>当前位置</returns>
        Task<string> GetCurrentLocationAsync(int courierId);

        /// <summary>
        /// 切换工作状态
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="isOnline">是否在线</param>
        /// <returns>切换结果</returns>
        Task<bool> ToggleWorkStatusAsync(int courierId, bool isOnline);

        /// <summary>
        /// 获取月收入
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>月收入</returns>
        Task<decimal> GetMonthlyIncomeAsync(int courierId);

        /// <summary>
        /// 更新配送员位置
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>更新结果</returns>
        Task<bool> UpdateCourierLocationAsync(int courierId, decimal latitude, decimal longitude);

        /// <summary>
        /// 更新档案
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="profileDto">档案更新请求</param>
        /// <returns>更新结果</returns>
        Task<bool> UpdateProfileAsync(int courierId, UpdateProfileDto profileDto);

        /// <summary>
        /// 获取编辑用档案信息
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>编辑用档案信息</returns>
        Task<UpdateProfileDto?> GetProfileForEditAsync(int courierId);
    }
}
