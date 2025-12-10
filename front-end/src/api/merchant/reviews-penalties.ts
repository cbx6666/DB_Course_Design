import apiClient from '../client';

export interface Review {
    id: number;
    orderNo: string;
    orderId?: number;
    user?: { name: string; phone: string; avatar?: string };
    content: string;
    rating?: number;
    images?: string[];
    createdAt: string;
    dishDetails?: Array<{
        dishName: string;
        dishImage: string;
        quantity: number;
        price: number;
    }>;
    merchantReply?: string;
    merchantReplyTime?: string;
    merchantReplyStatus?: string;
    replies?: number;
}

export interface PageResult<T> {
    list: T[];
    total: number;
}

export const getReviewList = async (params: {
    page: number;
    pageSize: number;
    keyword?: string;
    field?: string;
    sellerId: number;
}): Promise<PageResult<Review>> => {
    const requestParams = {
        page: params.page.toString(),
        pageSize: params.pageSize.toString(),
        sellerId: params.sellerId.toString(),
        ...(params.keyword && { keyword: params.keyword }),
        ...(params.field && { field: params.field })
    };
    const response = await apiClient.get('/merchant/comments', { params: requestParams });
    // 后端返回 ApiResponseDto<PageResult<Review>>，需要提取 data
    return (response.data?.data ?? response.data) as PageResult<Review>;
};

export const replyReview = async (id: number, content: string) => {
    const response = await apiClient.post(`/merchant/comments/${id}/reply`, { content });
    // 后端返回 ApiResponseDto，需要提取 data
    return response.data?.data ?? response.data;
};

export interface PenaltyRecord {
    id: string;
    reason: string;
    time: string;
    merchantAction: string;
    platformAction: string;
}

export const getPenaltyList = async (params?: { keyword?: string; field?: string }) => {
    const query = {
        ...(params?.keyword && { keyword: params.keyword }),
        ...(params?.field && { field: params.field })
    };
    const response = await apiClient.get('/merchant/penalties', { params: query });
    // 后端返回 ApiResponseDto<PenaltyRecord[]>，需要提取 data
    return (response.data?.data ?? response.data) as PenaltyRecord[];
};

export const getPenaltyDetail = async (id: string) => {
    const response = await apiClient.get(`/merchant/penalties/${id}`);
    // 后端返回 ApiResponseDto<PenaltyRecord>，需要提取 data
    return (response.data?.data ?? response.data) as PenaltyRecord;
};


