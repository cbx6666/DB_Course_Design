import apiClient from '../client';
import { devLog } from '@/utils/logger';

export interface MerchantInfo {
    username: string;
    sellerId: number;
    avatar?: string;
}

export const getShopOverview = async () => {
    devLog.api('正在请求店铺概览数据...');
    const response = await apiClient.get('/shop/overview');
    devLog.api('店铺概览数据响应:', response.data);
    return response.data;
};

export const getShopInfo = async () => {
    const response = await apiClient.get('/shop/info');
    return response.data;
};

export const getMerchantInfo = async (): Promise<MerchantInfo> => {
    const response = await apiClient.get('/merchant/info');
    return response.data.data as MerchantInfo;
};

export const toggleBusinessStatus = async (status: boolean) => {
    const response = await apiClient.patch('/shop/status', { isOpen: status });
    return response.data;
};

export const updateShopField = async (field: string, value: string) => {
    const response = await apiClient.patch('/shop/update-field', {
        field,
        value
    });
    return response.data;
};


