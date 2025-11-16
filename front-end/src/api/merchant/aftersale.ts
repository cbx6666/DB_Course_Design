import apiClient from '../client';

export interface AfterSaleUserInfo {
    name: string;
    phone: string;
    avatar?: string;
}

export interface AfterSaleApplication {
    id: number;
    orderNo: string;
    orderId?: number;
    user?: AfterSaleUserInfo;
    reason: string;
    images?: string[];
    createdAt: string;
    dishDetails?: Array<{
        dishName: string;
        dishImage: string;
        quantity: number;
        price: number;
    }>;
}

export interface AfterSaleListResponse {
    list: AfterSaleApplication[];
    total: number;
}

export interface AfterSaleListParams {
    page: number;
    pageSize: number;
    keyword?: string;
    field?: string;
    sellerId: number;
}

export const getAfterSaleList = async (params: AfterSaleListParams): Promise<AfterSaleListResponse> => {
    const requestParams = {
        page: params.page,
        pageSize: params.pageSize,
        sellerId: params.sellerId,
        ...(params.keyword && { keyword: params.keyword }),
        ...(params.field && { field: params.field })
    };
    const response = await apiClient.get('/merchant/after-sales', { params: requestParams });
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


