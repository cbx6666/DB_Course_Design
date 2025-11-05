using BackEnd.DTOs.DeliveryTask;
using BackEnd.DTOs.Courier;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 配送任务服务实现（商家侧）
    /// </summary>
    public class MerchantDeliveryTaskService : IMerchantDeliveryTaskService
    {
        private readonly IDeliveryTaskRepository _deliveryRepo;
        private readonly IFoodOrderRepository _orderRepo;
        private readonly IStoreRepository _storeRepo;
        private readonly ICourierRepository _courierRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="deliveryRepo">配送任务仓储</param>
        /// <param name="orderRepo">订单仓储</param>
        /// <param name="storeRepo">店铺仓储</param>
        /// <param name="courierRepo">配送员仓储</param>
        public MerchantDeliveryTaskService(IDeliveryTaskRepository deliveryRepo,
                                          IFoodOrderRepository orderRepo,
                                          IStoreRepository storeRepo,
                                          ICourierRepository courierRepo)
        {
            _deliveryRepo = deliveryRepo;
            _orderRepo = orderRepo;
            _storeRepo = storeRepo;
            _courierRepo = courierRepo;
        }

        /// <summary>
        /// 发布配送任务
        /// </summary>
        /// <param name="dto">发布任务请求</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>发布结果</returns>
        public async Task<bool> PublishDeliveryTaskAsync(
            CreateDeliveryTaskDto dto, int sellerId)
        {
            // 验证订单存在且属于当前商家
            var order = await _orderRepo.GetByIdAsync(dto.OrderId)
                ?? throw new KeyNotFoundException("订单不存在");

            var store = await _storeRepo.GetStoreInfoForUserAsync(order.StoreID);
            if (store?.SellerID != sellerId)
                throw new UnauthorizedAccessException("无权操作此订单");

            // 创建配送任务
            var task = new DeliveryTask
            {
                OrderID = dto.OrderId,
                EstimatedArrivalTime = DateTime.Parse(dto.EstimatedArrivalTime),
                EstimatedDeliveryTime = DateTime.Parse(dto.EstimatedDeliveryTime),
                PublishTime = DateTime.Now,
                Status = DeliveryStatus.To_Be_Taken,
                DeliveryFee = order.DeliveryFee
            };

            await _deliveryRepo.AddAsync(task);
            return true;
        }

        /// <summary>
        /// 获取订单配送信息
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>订单配送信息</returns>
        public async Task<OrderDeliveryInfoDto> GetOrderDeliveryInfoAsync(int orderId)
        {
            var task = await _deliveryRepo.GetByOrderIdAsync(orderId);
            if (task == null)
            {
                return new OrderDeliveryInfoDto();
            }

            var courier = task.CourierID.HasValue
                              ? await _courierRepo.GetByIdAsync(task.CourierID.Value)
                              : null;

            // 构建返回数据（只包含前端需要的字段）
            var result = new OrderDeliveryInfoDto
            {
                Status = (int)task.Status,
                Courier = courier == null ? null : new CourierSummaryDto
                {
                    UserId = courier.UserID,
                    CourierRegistrationTime = courier.CourierRegistrationTime.ToString("o"),
                    VehicleType = courier.VehicleType,
                    ReputationPoints = courier.ReputationPoints,
                    TotalDeliveries = courier.TotalDeliveries,
                    AvgDeliveryTime = courier.AvgDeliveryTime,
                    AverageRating = courier.AverageRating,
                    MonthlySalary = courier.MonthlySalary,
                    FullName = courier.User?.FullName,
                    PhoneNumber = courier.User?.PhoneNumber,
                    Longitude = courier.CourierLongitude,
                    Latitude = courier.CourierLatitude
                }
            };

            return result;
        }
    }
}
