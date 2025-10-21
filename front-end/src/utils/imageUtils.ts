// src/utils/imageUtils.ts
import { API_CONFIG } from '@/config';

/**
 * 将相对路径的图片URL转换为完整的URL
 * @param imageUrl 图片的相对路径或完整URL
 * @returns 完整的图片URL
 */
export function normalizeImageUrl(imageUrl?: string): string {
    if (!imageUrl) return `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;

    // 如果已经是完整URL，直接返回
    if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
        return imageUrl;
    }

    // 如果是相对路径，添加baseURL
    if (imageUrl.startsWith('/')) {
        return `${API_CONFIG.BASE_URL}${imageUrl}`;
    }

    // 如果不是以/开头，添加/和baseURL
    return `${API_CONFIG.BASE_URL}/${imageUrl}`;
}

/**
 * 处理图片加载错误，设置默认图片
 * @param event 图片错误事件
 * @param defaultImage 默认图片路径
 */
export function handleImageError(event: Event, defaultImage?: string): void {
    const img = event.target as HTMLImageElement;
    img.src = defaultImage || `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;
}
