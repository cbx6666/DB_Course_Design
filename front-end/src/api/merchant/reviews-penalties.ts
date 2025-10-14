import apiClient from '../client';

export interface Review {
    id: number;
    orderNo: string;
    user: { name: string; phone: string; avatar?: string };
    content: string;
    createdAt: string;
}

export interface PageResult<T> {
    list: T[];
    total: number;
}

export const getReviewList = async (params: {
    page: number;
    pageSize: number;
    keyword?: string;
    sellerId: number;
}): Promise<PageResult<Review>> => {
    const requestParams = {
        page: params.page.toString(),
        pageSize: params.pageSize.toString(),
        sellerId: params.sellerId.toString(),
        ...(params.keyword && { keyword: params.keyword })
    };
    const response = await apiClient.get('/reviews', { params: requestParams });
    return response.data;
};

export const replyReview = async (id: number, content: string) => {
    const response = await apiClient.post(`/reviews/${id}/reply`, { content });
    return response.data;
};

export interface PenaltyRecord {
    id: string;
    reason: string;
    time: string;
    merchantAction: string;
    platformAction: string;
}

export const getPenaltyList = async (params?: { keyword?: string }) => {
    const response = await apiClient.get('/penalties', { params: { ...(params?.keyword && { keyword: params.keyword }) } });
    return response.data as PenaltyRecord[];
};

export const getPenaltyDetail = async (id: string) => {
    const response = await apiClient.get(`/penalties/${id}`);
    return response.data as PenaltyRecord;
};

export const appealPenalty = async (id: string, reason: string) => {
    const response = await apiClient.post(`/penalties/${id}/appeal`, { reason });
    return response.data;
};


