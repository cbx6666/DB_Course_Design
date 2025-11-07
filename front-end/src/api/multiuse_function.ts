import axios, { AxiosRequestConfig } from 'axios';
import API from './index'

export async function getData<T>(url: string, config?: AxiosRequestConfig): Promise<T> {
    try {
        const response = await API.get<any>(url, config);
        // 后端统一返回 ApiResponseDto<T> 格式（camelCase: { success, code, message, data }）
        // 如果响应是 ApiResponseDto 格式，则提取 data 字段；否则直接返回响应数据
        if (response.data && typeof response.data === 'object' && 'data' in response.data && 'success' in response.data) {
            return response.data.data as T;
        }
        return response.data as T;
    } catch (error: unknown) {
        handleAxiosError(error);
        throw error;
    }
}

export async function postData<T, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    try {
        const response = await API.post<any>(url, data, config);
        // 后端统一返回 ApiResponseDto<T> 格式（camelCase: { success, code, message, data }）
        // 如果响应是 ApiResponseDto 格式，则提取 data 字段；否则直接返回响应数据
        if (response.data && typeof response.data === 'object' && 'data' in response.data && 'success' in response.data) {
            return response.data.data as T;
        }
        return response.data as T;
    } catch (error: unknown) {
        handleAxiosError(error);
        throw error;
    }
}

export async function putData<T, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    try {
        const response = await API.put<any>(url, data, config);
        // 后端统一返回 ApiResponseDto<T> 格式（camelCase: { success, code, message, data }）
        // 如果响应是 ApiResponseDto 格式，则提取 data 字段；否则直接返回响应数据
        if (response.data && typeof response.data === 'object' && 'data' in response.data && 'success' in response.data) {
            return response.data.data as T;
        }
        return response.data as T;
    } catch (error: unknown) {
        handleAxiosError(error);
        throw error;
    }
}

export async function deleteData<T, D = any>(url: string, data?: D, config?: AxiosRequestConfig): Promise<T> {
    try {
        const response = await API.delete<any>(url, { ...config, data });
        // 后端统一返回 ApiResponseDto<T> 格式（camelCase: { success, code, message, data }）
        // 如果响应是 ApiResponseDto 格式，则提取 data 字段；否则直接返回响应数据
        if (response.data && typeof response.data === 'object' && 'data' in response.data && 'success' in response.data) {
            return response.data.data as T;
        }
        return response.data as T;
    } catch (error: unknown) {
        handleAxiosError(error);
        throw error;
    }
}

function handleAxiosError(error: unknown) {
    let message = '请求失败，未知错误'
    if (axios.isAxiosError(error)) {
        message = error.message ?? message
    } else if (error instanceof Error) {
        message = error.message ?? message
    }
    console.warn(message)
}