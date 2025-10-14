import apiClient from '../client';

export interface AfterSaleUserInfo {
    name: string;
    phone: string;
    avatar?: string;
}

export interface AfterSaleApplication {
    id: number;
    orderNo: string;
    user?: AfterSaleUserInfo;
    reason: string;
    createdAt: string;
}

export interface AfterSaleListResponse {
    list: AfterSaleApplication[];
    total: number;
}

export interface AfterSaleListParams {
    page: number;
    pageSize: number;
    keyword?: string;
    sellerId: number;
}

export const getAfterSaleList = async (params: AfterSaleListParams): Promise<AfterSaleListResponse> => {
    const response = await apiClient.get('/merchant/after-sales', { params });
    return response.data as AfterSaleListResponse;
};

export const getAfterSaleDetail = async (id: number): Promise<AfterSaleApplication> => {
    const response = await apiClient.get(`/merchant/after-sales/${id}`);
    return response.data as AfterSaleApplication;
};

export const decideAfterSale = async (
    id: number,
    action: 'approve' | 'reject' | 'negotiate',
    payload: { remark: string }
): Promise<void> => {
    await apiClient.post(`/merchant/after-sales/${id}/decide`, { action, ...payload });
};


