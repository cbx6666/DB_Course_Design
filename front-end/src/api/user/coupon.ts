import { getData, postData } from '@/api/multiuse_function';

// 保留原有的接口定义，以便向后兼容
export interface CouponInfo {
    couponID: number
    minimumSpend: number
    discountAmount: number
    validFrom?: string // 有效期起始时间
    validTo: string
    couponType?: 'fixed' | 'discount' // 可选字段，用于区分满减券和折扣券
    couponName?: string // 优惠券名称
    description?: string // 优惠券描述
    storeID?: number // 店铺ID
    storeName?: string // 店铺名称
    storeImage?: string // 店铺图片
}

export interface AvailableCoupon {
    couponManagerID: number;
    couponName: string;
    type: 'fixed' | 'discount';
    minimumSpend: number;
    value: number;
    validFrom: string;
    validTo: string;
    description?: string;
    storeID: number;
    storeName: string;
    storeImage?: string;
    remainingQuantity: number;
    isClaimed: boolean;
}

export interface ClaimCouponResponse {
    success: boolean;
    message?: string;
}

/**
 * 获取所有可领取的优惠券
 */
export async function getAvailableCoupons(): Promise<AvailableCoupon[]> {
    return getData<AvailableCoupon[]>('/user/home/available-coupons');
}

/**
 * 领取优惠券
 * @param couponManagerId 优惠券管理ID
 */
export async function claimCoupon(couponManagerId: number): Promise<ClaimCouponResponse> {
    return postData<ClaimCouponResponse>(`/user/home/claim-coupon/${couponManagerId}`);
}

/**
 * 获取用户已领取的优惠券
 * @param userId 用户ID（保留参数以兼容现有调用，但实际不使用，因为后端从Token获取）
 */
export async function getCouponInfo(userId: number): Promise<CouponInfo[]> {
    const data = await getData<any[]>('/user/coupons');
    // 转换后端数据格式到前端格式
    return data.map((item: any) => {
        // 根据优惠券类型计算折扣金额
        // 满减券：value 就是金额（例如 20 表示减20元）
        // 折扣券：value 是比例 0-1（例如 0.8 表示8折，需要转换为金额显示时可能需要订单金额，这里暂时用 value * 100 显示为百分比）
        // 注意：CouponInfo.discountAmount 是金额，所以折扣券可能需要特殊处理
        // 但为了兼容现有显示逻辑，满减券直接用 value，折扣券暂时也用 value（前端应该知道这是折扣比例）
        const discountAmount = item.value;
        // 如果是折扣券，前端显示时可能需要计算实际折扣金额，但这里只返回原始值
        // 前端可以根据需要自行处理显示逻辑
        return {
            couponID: item.couponID,
            minimumSpend: item.minimumSpend,
            discountAmount: discountAmount,
            validFrom: item.validFrom,
            validTo: item.validTo,
            couponType: item.couponType, // 添加类型信息以便前端判断
            couponName: item.couponName,
            description: item.description,
            storeID: item.storeID,
            storeName: item.storeName,
            storeImage: item.storeImage
        };
    });
}
