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
            // 折扣券：数据库存储0-1，返回时转换为0-10
            // 满减券：直接使用原值
            var value = coupon.CouponType == CouponType.Discount ? coupon.Value * 10 : coupon.Value;
            
            return new CouponDto
            {
                Id = coupon.CouponManagerID,
                Name = coupon.CouponName,
                Type = coupon.CouponType == CouponType.Fixed ? "fixed" : "discount",
                Value = value,
                MinAmount = coupon.MinimumSpend,
                StartTime = coupon.ValidFrom.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                EndTime = coupon.ValidTo.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                TotalQuantity = coupon.TotalQuantity,
                UsedQuantity = coupon.UsedQuantity,
                Description = coupon.Description ?? "",
                Status = coupon.Status
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
            var couponType = dto.Type == "fixed" ? CouponType.Fixed : CouponType.Discount;
            
            // 折扣券：前端发送0-10，转换为0-1存储
            // 满减券：直接使用原值
            var value = couponType == CouponType.Discount ? dto.Value / 10 : dto.Value;

            return new CouponManager
            {
                /// <summary>
                /// CouponManagerID 将由数据库自动生成
                /// </summary>
                CouponName = dto.Name,
                CouponType = couponType,
                MinimumSpend = dto.MinAmount ?? 0,
                Value = value,
                TotalQuantity = dto.TotalQuantity,
                UsedQuantity = 0,
                ValidFrom = DateTime.Parse(dto.StartTime),
                ValidTo = DateTime.Parse(dto.EndTime),
                Description = dto.Description,
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
            var couponType = dto.Type == "fixed" ? CouponType.Fixed : CouponType.Discount;
            
            // 折扣券：前端发送0-10，转换为0-1存储
            // 满减券：直接使用原值
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
        /// <param name="coupons">优惠券管理器模型列表</param>
        /// <returns>优惠券DTO列表</returns>
        public static List<CouponDto> ToDtoList(this IEnumerable<CouponManager> coupons)
        {
            return coupons.Select(c => c.ToDto()).ToList();
        }
    }
}
