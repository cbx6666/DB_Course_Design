import apiClient from '../client';
import { devLog } from '@/utils/logger';

export interface MerchantInfo {
    username: string;
    sellerId: number;
    avatar?: string;
}

export const getShopOverview = async () => {
    devLog.api('正在请求店铺概览数据...');
    const response = await apiClient.get('/merchant/store/overview');
    devLog.api('店铺概览数据响应:', response.data);
    return response.data;
};

export const getShopInfo = async () => {
    const response = await apiClient.get('/merchant/store/info');
    return response.data;
};

export const getMerchantInfo = async (): Promise<MerchantInfo> => {
    const response = await apiClient.get('/merchant/info');
    // 后端返回 MerchantProfileDto，包含 Id (string), Username, Avatar 等
    // 前端需要转换为 MerchantInfo，将 Id 转换为 sellerId (number)
    const data = response.data.data;
    return {
        username: data.username || data.Username || '',
        sellerId: data.sellerId || (data.id ? parseInt(data.id) : parseInt(data.Id || '0')),
        avatar: data.avatar || data.Avatar
    };
};

export const toggleBusinessStatus = async (status: boolean) => {
    const response = await apiClient.patch('/merchant/store/status', { isOpen: status });
    return response.data;
};

export const updateShopField = async (field: string, value: string) => {
    const response = await apiClient.patch('/merchant/store/field', {
        field,
        value
    });
    return response.data;
};

// 获取店铺种类选项
export const getStoreCategoryOptions = async () => {
    const response = await apiClient.get('/merchant/store/category-options');
    return response.data;
};

// 更新店铺种类
export const updateStoreCategory = async (category: string) => {
    const response = await apiClient.patch('/merchant/store/category', {
        category
    });
    return response.data;
};


