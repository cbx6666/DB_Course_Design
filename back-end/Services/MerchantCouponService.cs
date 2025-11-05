using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Common;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BackEnd.Services
{
    /// <summary>
    /// 优惠券服务实现（商家侧）
    /// </summary>
    public class MerchantCouponService : IMerchantCouponService
    {
        private readonly ICouponManagerRepository _couponRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogger<MerchantCouponService> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantCouponService(
            ICouponManagerRepository couponRepository,
            IStoreRepository storeRepository,
            ILogger<MerchantCouponService> logger)
        {
            _couponRepository = couponRepository;
            _storeRepository = storeRepository;
            _logger = logger;
        }

        /// <summary>
        /// 获取优惠券列表（分页）
        /// </summary>
        public async Task<PageResultDto<MerchantCouponDto>> GetCouponsAsync(int sellerId, int page, int pageSize)
        {
            try
            {
                _logger.LogInformation("获取商家 {SellerId} 的优惠券列表，页码: {Page}, 页大小: {PageSize}", sellerId, page, pageSize);

                int? storeIdNullable = await _storeRepository.GetStoreIdBySellerIdAsync(sellerId);
                if (!storeIdNullable.HasValue)
                {
                    throw new ArgumentException($"商家 {sellerId} 没有对应的店铺");
                }
                int storeId = storeIdNullable.Value;

                var (coupons, total) = await _couponRepository.GetByStoreIdAsync(storeId, page, pageSize);
                var couponDtos = coupons.ToDtoList();

                return new PageResultDto<MerchantCouponDto>
                {
                    List = couponDtos,
                    Total = total
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家 {SellerId} 的优惠券列表失败", sellerId);
                throw;
            }
        }

        /// <summary>
        /// 获取优惠券统计信息
        /// </summary>
        public async Task<CouponStatsDto> GetStatsAsync(int sellerId)
        {
            try
            {
                _logger.LogInformation("获取商家 {SellerId} 的优惠券统计信息", sellerId);

                int? storeIdNullable = await _storeRepository.GetStoreIdBySellerIdAsync(sellerId);
                if (!storeIdNullable.HasValue)
                {
                    throw new ArgumentException($"商家 {sellerId} 没有对应的店铺");
                }
                int storeId = storeIdNullable.Value;

                var (total, active, expired, upcoming, totalUsed, totalValue) =
                    await _couponRepository.GetStatsByStoreIdAsync(storeId);

                return new CouponStatsDto
                {
                    Total = total,
                    Active = active,
                    Expired = expired,
                    Upcoming = upcoming,
                    TotalUsed = totalUsed,
                    TotalDiscountAmount = totalValue
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "获取商家 {SellerId} 的优惠券统计信息失败", sellerId);
                throw;
            }
        }

        /// <summary>
        /// 创建优惠券
        /// </summary>
        public async Task<int> CreateCouponAsync(int sellerId, CreateCouponRequestDto request)
        {
            try
            {
                _logger.LogInformation("商家 {SellerId} 创建优惠券: {CouponName}", sellerId, request.Name);

                // 验证请求数据
                ValidateCouponRequest(request);

                // 获取商家默认店铺ID
                var storeId = await GetDefaultStoreIdForSeller(sellerId);
                _logger.LogInformation("商家 {SellerId} 的店铺ID: {StoreId}", sellerId, storeId);

                // 创建优惠券模型
                var coupon = request.ToModel(sellerId, storeId);

                // 保存到数据库
                await _couponRepository.AddAsync(coupon);
                await _couponRepository.SaveAsync();

                // 获取数据库生成的ID
                var generatedId = coupon.CouponManagerID;
                _logger.LogInformation("优惠券创建成功，ID: {CouponId}", generatedId);

                return generatedId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商家 {SellerId} 创建优惠券失败", sellerId);
                throw;
            }
        }

        /// <summary>
        /// 更新优惠券
        /// </summary>
        public async Task UpdateCouponAsync(int sellerId, CreateCouponRequestDto request)
        {
            try
            {
                if (!request.Id.HasValue)
                {
                    throw new ArgumentException("更新优惠券时必须提供优惠券ID");
                }

                _logger.LogInformation("商家 {SellerId} 更新优惠券: {CouponId}", sellerId, request.Id);

                // 验证请求数据
                ValidateCouponRequest(request);

                // 获取商家默认店铺ID
                var storeId = await GetDefaultStoreIdForSeller(sellerId);

                // 获取现有优惠券
                var existingCoupon = await _couponRepository.GetByIdAndStoreIdAsync(request.Id.Value, storeId);
                if (existingCoupon == null)
                {
                    throw new ArgumentException($"优惠券 {request.Id} 不存在或不属于商家 {sellerId}");
                }

                // 更新优惠券信息
                existingCoupon.UpdateModel(request);

                // 保存更改
                await _couponRepository.UpdateAsync(existingCoupon);
                await _couponRepository.SaveAsync();

                _logger.LogInformation("优惠券 {CouponId} 更新成功", request.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商家 {SellerId} 更新优惠券 {CouponId} 失败", sellerId, request.Id);
                throw;
            }
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        public async Task DeleteCouponAsync(int sellerId, int couponId)
        {
            try
            {
                _logger.LogInformation("商家 {SellerId} 删除优惠券: {CouponId}", sellerId, couponId);

                int? storeIdNullable = await _storeRepository.GetStoreIdBySellerIdAsync(sellerId);
                if (!storeIdNullable.HasValue)
                {
                    throw new ArgumentException($"商家 {sellerId} 没有对应的店铺");
                }
                int storeId = storeIdNullable.Value;

                // 获取优惠券
                var coupon = await _couponRepository.GetByIdAndStoreIdAsync(couponId, storeId);
                if (coupon == null)
                {
                    throw new ArgumentException($"优惠券 {couponId} 不存在或不属于商家 {sellerId}");
                }

                // 删除优惠券
                await _couponRepository.DeleteAsync(coupon);
                await _couponRepository.SaveAsync();

                _logger.LogInformation("优惠券 {CouponId} 删除成功", couponId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商家 {SellerId} 删除优惠券 {CouponId} 失败", sellerId, couponId);
                throw;
            }
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        public async Task<int> BatchDeleteCouponsAsync(int sellerId, BatchDeleteCouponsRequestDto request)
        {
            try
            {
                _logger.LogInformation("商家 {SellerId} 批量删除优惠券，数量: {Count}", sellerId, request.Ids.Count);

                int? storeIdNullable = await _storeRepository.GetStoreIdBySellerIdAsync(sellerId);
                if (!storeIdNullable.HasValue)
                {
                    throw new ArgumentException($"商家 {sellerId} 没有对应的店铺");
                }
                int storeId = storeIdNullable.Value;

                if (request.Ids == null || !request.Ids.Any())
                {
                    throw new ArgumentException("优惠券ID列表不能为空");
                }

                // 批量删除
                var deletedCount = await _couponRepository.BatchDeleteAsync(request.Ids, storeId);

                _logger.LogInformation("批量删除完成，实际删除数量: {DeletedCount}", deletedCount);

                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "商家 {SellerId} 批量删除优惠券失败", sellerId);
                throw;
            }
        }

        /// <summary>
        /// 获取商家默认店铺ID
        /// </summary>
        private async Task<int> GetDefaultStoreIdForSeller(int sellerId)
        {
            int? storeIdNullable = await _storeRepository.GetStoreIdBySellerIdAsync(sellerId);
            if (!storeIdNullable.HasValue)
            {
                throw new ArgumentException($"商家 {sellerId} 没有对应的店铺");
            }
            return storeIdNullable.Value;
        }

        /// <summary>
        /// 验证优惠券请求数据
        /// </summary>
        private void ValidateCouponRequest(CreateCouponRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ArgumentException("优惠券名称不能为空");

            if (request.Type != "fixed" && request.Type != "discount")
                throw new ArgumentException("优惠券类型必须是 'fixed' 或 'discount'");

            if (request.Value <= 0)
                throw new ArgumentException("优惠值必须大于0");

            if (request.Type == "discount" && (request.Value <= 0 || request.Value > 10))
                throw new ArgumentException("折扣券的折扣值必须在0-10之间");

            if (request.Type == "fixed" && request.MinAmount.HasValue && request.MinAmount <= request.Value)
                throw new ArgumentException("满减券的最低消费必须大于优惠金额");

            if (request.TotalQuantity <= 0)
                throw new ArgumentException("发放数量必须大于0");

            if (!DateTime.TryParse(request.StartTime, out var startTime))
                throw new ArgumentException("开始时间格式不正确");

            if (!DateTime.TryParse(request.EndTime, out var endTime))
                throw new ArgumentException("结束时间格式不正确");

            if (endTime <= startTime)
                throw new ArgumentException("结束时间必须晚于开始时间");
        }
    }
}
