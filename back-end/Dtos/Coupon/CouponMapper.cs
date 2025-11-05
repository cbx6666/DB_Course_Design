using BackEnd.Models;
using BackEnd.Models.Enums;

namespace BackEnd.DTOs.Coupon
{
    /// <summary>
    /// 优惠券映射器（商家侧）：CouponManager ⇄ DTO 的转换
    /// - 对商家端展示的值做必要的换算（折扣值 0-1 ⇄ 0-10）
    /// - 将请求 DTO 转为实体，或实体转为展示 DTO
    /// </summary>
    public static class CouponMapper
    {
        /// <summary>
        /// 将CouponManager模型转换为MerchantCouponDto
        /// </summary>
        public static MerchantCouponDto ToDto(this CouponManager coupon)
        {
            // 折扣券：数据库存储0-1，返回时转换为0-10
            // 满减券：直接使用原值
            var value = coupon.CouponType == CouponType.Discount ? coupon.Value * 10 : coupon.Value;
            
            return new MerchantCouponDto
            {
                Id = coupon.CouponManagerID,
                Name = coupon.CouponName,
                Type = coupon.CouponType == CouponType.Fixed ? "fixed" : "discount",
                Value = value,
                MinAmount = coupon.MinimumSpend,
                StartTime = coupon.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = coupon.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                TotalQuantity = coupon.TotalQuantity,
                UsedQuantity = coupon.Coupons?.Count ?? 0,
                Description = coupon.Description ?? string.Empty,
                Status = coupon.Status
            };
        }

        /// <summary>
        /// 将CreateCouponRequestDto转换为CouponManager模型
        /// </summary>
        public static CouponManager ToModel(this CreateCouponRequestDto dto, int sellerId, int storeId)
        {
            var couponType = dto.Type == "fixed" ? CouponType.Fixed : CouponType.Discount;
            // 折扣券：前端发送0-10，转换为0-1存储；满减券：直接使用原值
            var value = couponType == CouponType.Discount ? dto.Value / 10 : dto.Value;

            return new CouponManager
            {
                CouponName = dto.Name,
                CouponType = couponType,
                MinimumSpend = dto.MinAmount ?? 0,
                Value = value,
                TotalQuantity = dto.TotalQuantity,
                ValidFrom = DateTime.Parse(dto.StartTime),
                ValidTo = DateTime.Parse(dto.EndTime),
                Description = dto.Description,
                StoreID = storeId,
            };
        }

        /// <summary>
        /// 更新CouponManager模型
        /// </summary>
        public static void UpdateModel(this CouponManager model, CreateCouponRequestDto dto)
        {
            var couponType = dto.Type == "fixed" ? CouponType.Fixed : CouponType.Discount;
            var value = couponType == CouponType.Discount ? dto.Value / 10 : dto.Value;

            model.CouponName = dto.Name;
            model.CouponType = couponType;
            model.MinimumSpend = dto.MinAmount ?? 0;
            model.Value = value;
            model.TotalQuantity = dto.TotalQuantity;
            model.ValidFrom = DateTime.Parse(dto.StartTime);
            model.ValidTo = DateTime.Parse(dto.EndTime);
            model.Description = dto.Description;
        }

        /// <summary>
        /// 将优惠券列表转换为DTO列表
        /// </summary>
        public static List<MerchantCouponDto> ToDtoList(this IEnumerable<CouponManager> coupons)
        {
            return coupons.Select(c => c.ToDto()).ToList();
        }
    }
}
