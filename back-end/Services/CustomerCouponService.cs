using BackEnd.Data;
using BackEnd.DTOs.Coupon;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    /// <summary>
    /// 优惠券服务实现（消费者侧）
    /// </summary>
    public class CustomerCouponService : ICustomerCouponService
    {
        private readonly ICouponRepository _userCouponRepository;
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerCouponService(
            ICouponRepository userCouponRepository,
            IUserRepository userRepository,
            AppDbContext context)
        {
            _userCouponRepository = userCouponRepository;
            _userRepository = userRepository;
            _context = context;
        }

        /// <summary>
        /// 获取用户优惠券列表（用于结账页面）
        /// </summary>
        public async Task<List<CustomerCouponDto>> GetUserCouponsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("用户不存在");
            }

            if (user.Customer == null)
            {
                return new List<CustomerCouponDto>();
            }

            var coupons = await _userCouponRepository.GetByCustomerIdAsync(user.Customer.UserID);

            return coupons
                .Where(c => GetActualCouponState(c) == CouponState.Unused)
                .Select(c => new CustomerCouponDto
                {
                    CouponID = c.CouponID,
                    CouponState = GetActualCouponState(c),
                    CouponManagerID = c.CouponManagerID,
                    MinimumSpend = c.CouponManager.MinimumSpend,
                    Value = c.CouponManager.Value,
                    CouponType = c.CouponManager.CouponType == CouponType.Fixed ? "fixed" : "discount",
                    ValidFrom = c.CouponManager.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ss"),
                    ValidTo = c.CouponManager.ValidTo.ToString("yyyy-MM-ddTHH:mm:ss"),
                    CouponName = c.CouponManager.CouponName,
                    Description = c.CouponManager.Description,
                    StoreID = c.CouponManager.StoreID,
                    StoreName = c.CouponManager.Store?.StoreName,
                    StoreImage = c.CouponManager.Store?.StoreImage
                }).ToList();
        }

        /// <summary>
        /// 获取用户优惠券信息（用于我的优惠券页面）
        /// </summary>
        public async Task<IEnumerable<CouponDto>> GetUserCouponListAsync(int userId)
        {
            var coupons = await _userCouponRepository.GetAllAsync();

            return coupons
                .Where(c => c.Customer.UserID == userId)
                .Select(c => new CouponDto
                {
                    CouponID = c.CouponID,
                    CouponState = c.CouponState,
                    OrderID = c.OrderID,
                    CouponManagerID = c.CouponManagerID,
                    MinimumSpend = c.CouponManager.MinimumSpend,
                    Value = c.CouponManager.Value,
                    ValidTo = c.CouponManager.ValidTo
                });
        }

        /// <summary>
        /// 获取所有可领取的优惠券
        /// </summary>
        public async Task<List<AvailableCouponDto>> GetAvailableCouponsAsync(int userId)
        {
            var now = DateTime.Now;
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Customer == null)
            {
                return new List<AvailableCouponDto>();
            }

            var customerId = user.Customer.UserID;

            var allCouponManagers = await _context.CouponManagers
                .Include(cm => cm.Store)
                .Include(cm => cm.Coupons)
                .Where(cm => cm.ValidTo >= now)
                .ToListAsync();
            
            allCouponManagers = allCouponManagers
                .Where(cm => (cm.Coupons?.Count ?? 0) < cm.TotalQuantity)
                .ToList();

            var claimedCouponManagerIds = await _context.Coupons
                .Where(c => c.CustomerID == customerId)
                .Select(c => c.CouponManagerID)
                .Distinct()
                .ToListAsync();

            var result = new List<AvailableCouponDto>();

            foreach (var cm in allCouponManagers)
            {
                var totalClaimedCount = cm.Coupons?.Count ?? 0;
                var remainingQuantity = cm.TotalQuantity - totalClaimedCount;
                var isClaimed = claimedCouponManagerIds.Contains(cm.CouponManagerID);

                result.Add(new AvailableCouponDto
                {
                    CouponManagerID = cm.CouponManagerID,
                    CouponName = cm.CouponName,
                    Type = cm.CouponType == CouponType.Fixed ? "fixed" : "discount",
                    MinimumSpend = cm.MinimumSpend,
                    Value = cm.Value,
                    ValidFrom = cm.ValidFrom.ToString("yyyy-MM-dd HH:mm:ss"),
                    ValidTo = cm.ValidTo.ToString("yyyy-MM-dd HH:mm:ss"),
                    Description = cm.Description,
                    StoreID = cm.StoreID,
                    StoreName = cm.Store?.StoreName ?? "",
                    StoreImage = cm.Store?.StoreImage,
                    RemainingQuantity = remainingQuantity,
                    IsClaimed = isClaimed
                });
            }

            return result;
        }

        /// <summary>
        /// 领取优惠券
        /// </summary>
        public async Task<bool> ClaimCouponAsync(int userId, int couponManagerId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Customer == null)
            {
                return false;
            }

            var customerId = user.Customer.UserID;
            var couponManager = await _context.CouponManagers.FindAsync(couponManagerId);
            if (couponManager == null)
            {
                return false;
            }

            var now = DateTime.Now;
            if (now > couponManager.ValidTo)
            {
                return false;
            }

            await _context.Entry(couponManager)
                .Collection(cm => cm.Coupons!)
                .LoadAsync();

            var totalClaimed = couponManager.Coupons?.Count ?? 0;
            if (totalClaimed >= couponManager.TotalQuantity)
            {
                return false;
            }

            var existingCoupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.CustomerID == customerId && c.CouponManagerID == couponManagerId);
            if (existingCoupon != null)
            {
                return false;
            }

            var newCoupon = new Coupon
            {
                CustomerID = customerId,
                CouponManagerID = couponManagerId,
                CouponState = CouponState.Unused
            };

            await _userCouponRepository.AddAsync(newCoupon);
            await _userCouponRepository.SaveAsync();

            return true;
        }

        /// <summary>
        /// 获取实际优惠券状态
        /// </summary>
        private CouponState GetActualCouponState(Coupon coupon)
        {
            if (coupon.IsExpired && coupon.CouponState == CouponState.Unused)
            {
                return CouponState.Expired;
            }

            return coupon.CouponState;
        }
    }
}
