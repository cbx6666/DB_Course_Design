import { getData } from '@/api/multiuse_function'

export interface StoreInfo {
    id: number
    name: string
    image: string
    address: string
    businessHours: string
    rating: number
    monthlySales: number
    description: string
    category: string
    createTime: string
}

export interface DeliveryTask {
    id: number
    deliveryTime: number
    deliveryFee: number
}

export interface Comment {
    id: number;
    username: string;
    rating: number;
    date: string;
    content: string;
    avatar: string;
    images: string[];
    merchantReply?: string;
    merchantReplyTime?: string;
}

export interface CommentList {
    comments: Comment[];
}

export interface CommentStatus {
    status: number[];
}

export interface Category {
    id: number;
    name: string;
}

export async function getStoreInfo(StoreId: string): Promise<StoreInfo> {
    return getData<StoreInfo>(`/customer/stores/${StoreId}/info`);
}

export function getDeliveryTasks(storeId?: string) {
    // 如果提供了 storeId，尝试从 sessionStorage 获取该店铺的配送时间和配送费
    let deliveryTime: number;
    let deliveryFee: number;
    
    if (storeId) {
        const timeStorageKey = `deliveryTime_${storeId}`;
        const feeStorageKey = `deliveryFee_${storeId}`;
        
        // 获取配送时间
        const storedTime = sessionStorage.getItem(timeStorageKey);
        if (storedTime !== null) {
            deliveryTime = parseInt(storedTime, 10);
        } else {
            // 首次访问该店铺，生成新的配送时间并存储（10-30分钟）
            deliveryTime = Math.floor(Math.random() * 21) + 10;
            sessionStorage.setItem(timeStorageKey, deliveryTime.toString());
        }
        
        // 获取配送费
        const storedFee = sessionStorage.getItem(feeStorageKey);
        if (storedFee !== null) {
            const parsedFee = parseFloat(storedFee);
            // 如果存储的配送费是0，重新生成（修复旧数据）
            if (parsedFee === 0) {
                const deliveryFeeOptions = [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];
                const randomIndex = Math.floor(Math.random() * deliveryFeeOptions.length);
                deliveryFee = deliveryFeeOptions[randomIndex];
                sessionStorage.setItem(feeStorageKey, deliveryFee.toString());
            } else {
                deliveryFee = parsedFee;
            }
        } else {
            // 首次访问该店铺，生成新的配送费并存储
            // 移除0，设置最小配送费为0.5元，避免配送费为0的情况
            const deliveryFeeOptions = [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];
            const randomIndex = Math.floor(Math.random() * deliveryFeeOptions.length);
            deliveryFee = deliveryFeeOptions[randomIndex];
            sessionStorage.setItem(feeStorageKey, deliveryFee.toString());
        }
    } else {
        // 没有 storeId，直接生成随机配送时间和配送费
        deliveryTime = Math.floor(Math.random() * 21) + 10; // 配送时间：10-30分钟
        const deliveryFeeOptions = [0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5, 5];
        const randomIndex = Math.floor(Math.random() * deliveryFeeOptions.length);
        deliveryFee = deliveryFeeOptions[randomIndex];
    }

    return {
        id: Math.floor(Math.random() * 10000),
        deliveryTime,
        deliveryFee,
    } as DeliveryTask;
}

export async function getCommentList(StoreId: string) {
    return getData<CommentList>(`/customer/comments/store/${StoreId}/comments`);
}

export async function getCommentStatus(StoreId: string): Promise<CommentStatus> {
    return getData<CommentStatus>(`/customer/comments/store/${StoreId}/commentStatus`);
}

export async function getStoreCategories(storeId: string): Promise<Category[]> {
    return getData<Category[]>(`/customer/stores/${storeId}/categories`);
}


