import apiClient from '../client';

export interface CouponInfo {
    id: number;
    name: string;
    type: 'fixed' | 'discount';
    value: number;
    minAmount?: number;
    startTime: string;
    endTime: string;
    totalQuantity: number;
    usedQuantity: number;
    description: string;
    status: 'active' | 'expired' | 'upcoming' | 'inactive';
}

export interface CreateCouponRequest {
    name: string;
    description?: string;
    couponType: 'fixed' | 'percentage';
    minimumSpend: number;
    discountAmount: number;
    totalQuantity: number;
    validFrom: string;
    validTo: string;
}

export interface CouponListResponse {
    list: CouponInfo[];
    total: number;
    page?: number;
    pageSize?: number;
}

export const getCoupons = async (page = 1, pageSize = 10): Promise<CouponListResponse> => {
    const response = await apiClient.get('/merchant/coupons', { params: { page, pageSize } });
    return response.data.data || response.data;
};

export const createCoupon = async (couponData: CreateCouponRequest) => {
    const backendData = {
        name: couponData.name,
        description: couponData.description || '',
        type: couponData.couponType === 'percentage' ? 'discount' : 'fixed',
        value: couponData.discountAmount,  // 前端直接发送0-10的值，后端自动转换
        minAmount: couponData.minimumSpend || undefined,
        totalQuantity: couponData.totalQuantity,
        startTime: couponData.validFrom,
        endTime: couponData.validTo
    };

    const response = await apiClient.post('/merchant/coupons', backendData);
    return response.data;
};

export const updateCoupon = async (couponId: number, couponData: CreateCouponRequest) => {
    const backendData = {
        id: couponId,
        name: couponData.name,
        description: couponData.description || '',
        type: couponData.couponType === 'percentage' ? 'discount' : 'fixed',
        value: couponData.discountAmount,  // 前端直接发送0-10的值，后端自动转换
        minAmount: couponData.minimumSpend || undefined,
        totalQuantity: couponData.totalQuantity,
        startTime: couponData.validFrom,
        endTime: couponData.validTo
    };

    await apiClient.put(`/merchant/coupons/${couponId}`, backendData);
};

export const deleteCoupon = async (couponId: number) => {
    await apiClient.delete(`/merchant/coupons/${couponId}`);
};

export const getCouponStats = async () => {
    const response = await apiClient.get('/merchant/coupons/stats');
    return response.data;
};


