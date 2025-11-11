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

    // 如果看起来像是文件名（包含扩展名且没有路径分隔符），可能是头像文件
    // 检查是否是头像文件名格式（可能包含下划线和GUID）
    if (imageUrl.includes('_') && (imageUrl.endsWith('.jpg') || imageUrl.endsWith('.jpeg') || imageUrl.endsWith('.png'))) {
        // 假设是头像文件，添加到 /avatars/ 路径
        return `${API_CONFIG.BASE_URL}/avatars/${imageUrl}`;
    }

    // 如果不是以/开头，添加/和baseURL
    return `${API_CONFIG.BASE_URL}/${imageUrl}`;
}

/**
 * 验证头像URL是否有效（用于头像显示）
 * 如果路径看起来像旧格式或无效，返回默认头像
 * @param imageUrl 头像URL
 * @returns 有效的头像URL
 */
export function validateAvatarUrl(imageUrl?: string): string {
    if (!imageUrl) return `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;
    
    // 如果路径看起来像旧格式（包含用户ID前缀，如 "21_xxx.jpg"），
    // 并且不是以 /avatars/ 开头，可能需要验证文件是否存在
    // 但为了性能，我们直接返回规范化后的URL，让 handleImageError 处理404
    return normalizeImageUrl(imageUrl);
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
