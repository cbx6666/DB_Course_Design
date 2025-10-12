// src/utils/errorHandler.ts
import { ElMessage } from 'element-plus';
import { API_CONFIG } from '@/config';

/**
 * 统一的错误处理函数
 */
export function handleApiError(error: any, defaultMessage = '操作失败，请重试') {
    console.error('API错误:', error);

    let message = defaultMessage;

    if (error.response) {
        // 服务器响应了错误状态码
        const status = error.response.status;
        const data = error.response.data;

        switch (status) {
            case 400:
                message = data?.message || '请求参数错误';
                break;
            case 401:
                message = '登录已过期，请重新登录';
                // 可以在这里处理自动跳转到登录页
                break;
            case 403:
                message = '没有权限执行此操作';
                break;
            case 404:
                message = '请求的资源不存在';
                break;
            case 500:
                message = '服务器内部错误';
                break;
            default:
                message = data?.message || `请求失败 (${status})`;
        }
    } else if (error.request) {
        // 请求已发出但没有收到响应
        message = '网络连接失败，请检查网络';
    } else {
        // 其他错误
        message = error.message || defaultMessage;
    }

    ElMessage.error(message);
    return message;
}

/**
 * 处理图片加载错误
 */
export function handleImageError(event: Event, defaultImageUrl?: string) {
    const img = event.target as HTMLImageElement;
    if (defaultImageUrl) {
        img.src = defaultImageUrl;
    } else {
        // 使用默认头像
        img.src = `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;
    }
}
