using BackEnd.DTOs.Courier;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 配送员服务接口
    /// </summary>
    public interface ICourierService
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
        /// 获取订单列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="status">订单状态</param>
        /// <returns>订单列表</returns>
        Task<IEnumerable<OrderListItemDto>> GetOrdersAsync(int courierId, string status);

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
        /// 获取新订单详情
        /// </summary>
        /// <param name="notificationId">通知ID</param>
        /// <returns>新订单详情</returns>
        Task<NewOrderDetailsDto?> GetNewOrderDetailsAsync(int notificationId);

        /// <summary>
        /// 接受订单
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="orderId">订单ID</param>
        /// <returns>接受结果</returns>
        Task<bool> AcceptOrderAsync(int courierId, int orderId);

        /// <summary>
        /// 拒绝订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>拒绝结果</returns>
        Task<bool> RejectOrderAsync(int orderId);

        /// <summary>
        /// 获取月收入
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>月收入</returns>
        Task<decimal> GetMonthlyIncomeAsync(int courierId);

        /// <summary>
        /// 标记任务为已完成
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>标记任务</returns>
        Task MarkTaskAsCompletedAsync(int taskId, int courierId);

        /// <summary>
        /// 取餐
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>取餐结果</returns>
        Task<bool> PickupOrderAsync(int orderId, int courierId);

        /// <summary>
        /// 配送订单
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <param name="courierId">配送员ID</param>
        /// <returns>配送结果</returns>
        Task<bool> DeliverOrderAsync(int orderId, int courierId);

        /// <summary>
        /// 获取投诉列表
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <returns>投诉列表</returns>
        Task<IEnumerable<ComplaintDto>> GetComplaintsAsync(int courierId);

        /// <summary>
        /// 更新配送员位置
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <returns>更新结果</returns>
        Task<bool> UpdateCourierLocationAsync(int courierId, decimal latitude, decimal longitude);

        /// <summary>
        /// 获取可用订单
        /// </summary>
        /// <param name="courierId">配送员ID</param>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        /// <param name="maxDistance">最大距离</param>
        /// <returns>可用订单列表</returns>
        Task<IEnumerable<AvailableOrderDto>> GetAvailableOrdersAsync(int courierId, decimal latitude, decimal longitude, decimal maxDistance);

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