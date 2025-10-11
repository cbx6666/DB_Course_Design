using BackEnd.Models;
using BackEnd.Models.Enums;

namespace BackEnd.DTOs.Merchant
{
    /// <summary>
    /// 优惠券映射器 - 用于模型和DTO之间的转换
    /// </summary>
    public static class CouponMapper
    {
        /// <summary>
        /// 将CouponManager模型转换为CouponDto
        /// </summary>
        /// <param name="coupon">优惠券管理器模型</param>
        /// <returns>优惠券DTO</returns>
        public static CouponDto ToDto(this CouponManager coupon)
        {
            return new CouponDto
            {
                id = coupon.CouponManagerID,
                name = coupon.CouponName,
                type = coupon.CouponType == CouponType.Fixed ? "fixed" : "discount",
                value = coupon.CouponType == CouponType.Fixed ? coupon.DiscountAmount : (coupon.DiscountRate ?? 0),
                minAmount = coupon.MinimumSpend,
                startTime = coupon.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                endTime = coupon.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                totalQuantity = coupon.TotalQuantity,
                usedQuantity = coupon.UsedQuantity,
                description = coupon.Description ?? "",
                status = coupon.Status
            };
        }

        /// <summary>
        /// 将CreateCouponRequestDto转换为CouponManager模型
        /// </summary>
        /// <param name="dto">创建优惠券请求DTO</param>
        /// <param name="sellerId">商家ID</param>
        /// <param name="storeId">店铺ID</param>
        /// <returns>优惠券管理器模型</returns>
        public static CouponManager ToModel(this CreateCouponRequestDto dto, int sellerId, int storeId)
        {
            var couponType = dto.type == "fixed" ? CouponType.Fixed : CouponType.Discount;

            return new CouponManager
            {
                /// <summary>
                /// CouponManagerID 将由数据库自动生成
                /// </summary>
                CouponName = dto.name,
                CouponType = couponType,
                MinimumSpend = dto.minAmount ?? 0,
                DiscountAmount = dto.discountAmount ?? 0,
                DiscountRate = couponType == CouponType.Discount ? dto.value : null,
                TotalQuantity = dto.totalQuantity,
                UsedQuantity = 0,
                ValidFrom = DateTime.Parse(dto.startTime),
                ValidTo = DateTime.Parse(dto.endTime),
                Description = dto.description,
                /// <summary>
                /// 使用传入的storeId参数
                /// </summary>
                StoreID = storeId,
            };
        }

        /// <summary>
        /// 更新CouponManager模型
        /// </summary>
        /// <param name="model">优惠券管理器模型</param>
        /// <param name="dto">创建优惠券请求DTO</param>
        public static void UpdateModel(this CouponManager model, CreateCouponRequestDto dto)
        {
            var couponType = dto.type == "fixed" ? CouponType.Fixed : CouponType.Discount;

            model.CouponName = dto.name;
            model.CouponType = couponType;
            model.MinimumSpend = dto.minAmount ?? 0;
            model.DiscountAmount = dto.discountAmount ?? 0;
            model.DiscountRate = couponType == CouponType.Discount ? dto.value : null;
            model.TotalQuantity = dto.totalQuantity;
            model.ValidFrom = DateTime.Parse(dto.startTime);
            model.ValidTo = DateTime.Parse(dto.endTime);
            model.Description = dto.description;
            /// <summary>
            /// 更新时不改变店铺ID，保持原有的店铺关联
            /// </summary>
            // model.StoreID = dto.storeId;
        }

        /// <summary>
        /// 将优惠券列表转换为DTO列表
        /// </summary>
        /// <param name="coupons">优惠券管理器模型列表</param>
        /// <returns>优惠券DTO列表</returns>
        public static List<CouponDto> ToDtoList(this IEnumerable<CouponManager> coupons)
        {
            return coupons.Select(c => c.ToDto()).ToList();
        }
    }
}
