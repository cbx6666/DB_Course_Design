import apiClient from '../client';

export interface AfterSaleUserInfo {
    name: string;
    phoneNumber?: number;
    phone?: string;
    avatar?: string;
    gender?: string;
    fullName?: string;
}

export interface AfterSaleApplication {
    id: number;
    orderNo: string;
    orderId?: number;
    user?: AfterSaleUserInfo;
    accountUserName?: string;
    reason: string;
    images?: string[];
    createdAt: string;
    status?: string;
    merchantReply?: string;
    punishment?: string;
    punishmentReason?: string;
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

export const replyAfterSale = async (
    id: number,
    payload: { remark: string }
): Promise<void> => {
    await apiClient.post(`/merchant/after-sales/${id}/reply`, payload);
};


