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
    return getData<AvailableCoupon[]>('/customer/coupons/available');
}

/**
 * 领取优惠券
 * @param couponManagerId 优惠券管理ID
 */
export async function claimCoupon(couponManagerId: number): Promise<ClaimCouponResponse> {
    return postData<ClaimCouponResponse>(`/customer/coupons/claim/${couponManagerId}`);
}

/**
 * 获取用户已领取的优惠券（我的优惠券页面）
 * @param userId 用户ID（保留参数以兼容现有调用，但实际不使用，因为后端从Token获取）
 */
export async function getCouponInfo(userId: number): Promise<CouponInfo[]> {
    try {
        const data = await getData<any[]>('/customer/coupons');
        // 转换后端数据格式到前端格式
        return data.map((item: any) => {
            return {
                couponID: item.couponID,
                minimumSpend: item.minimumSpend,
                discountAmount: item.value, // value 字段：满减券是金额，折扣券是比例 0-1
                validFrom: item.validFrom,
                validTo: item.validTo,
                couponType: item.couponType, // 'fixed' | 'discount'
                couponName: item.couponName,
                description: item.description,
                storeID: item.storeID,
                storeName: item.storeName,
                storeImage: item.storeImage
            };
        });
    } catch (error: any) {
        // 后端若无优惠券会返回 404，这里按空列表处理，避免结账页报错
        if (error?.response?.status === 404) {
            return [];
        }
        throw error;
    }
}
