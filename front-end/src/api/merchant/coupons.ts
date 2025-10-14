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
    discountRate?: number;
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
    return response.data;
};

export const createCoupon = async (couponData: CreateCouponRequest) => {
    const response = await apiClient.post('/merchant/coupons', couponData);
    return response.data;
};

export const updateCoupon = async (couponId: number, couponData: CreateCouponRequest) => {
    await apiClient.put(`/merchant/coupons/${couponId}`, couponData);
};

export const deleteCoupon = async (couponId: number) => {
    await apiClient.delete(`/merchant/coupons/${couponId}`);
};

export const getCouponStats = async () => {
    const response = await apiClient.get('/merchant/coupons/stats');
    return response.data;
};


