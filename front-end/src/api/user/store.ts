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
    images: string[]
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
    return getData<StoreInfo>(`/store/${StoreId}/info`);
}

export function getDeliveryTasks(storeId?: string) {
    const deliveryTime = Math.floor(Math.random() * 21) + 10; // 配送时间：10-30分钟

    // 如果提供了 storeId，尝试从 sessionStorage 获取该店铺的配送费
    let deliveryFee: number;
    if (storeId) {
        const storageKey = `deliveryFee_${storeId}`;
        const storedFee = sessionStorage.getItem(storageKey);

        if (storedFee !== null) {
            deliveryFee = parseFloat(storedFee);
        } else {
            // 首次访问该店铺，生成新的配送费并存储
            const deliveryFeeOptions = [0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5];
            const randomIndex = Math.floor(Math.random() * deliveryFeeOptions.length);
            deliveryFee = deliveryFeeOptions[randomIndex];
            sessionStorage.setItem(storageKey, deliveryFee.toString());
        }
    } else {
        // 没有 storeId，直接生成随机配送费
        const deliveryFeeOptions = [0, 0.5, 1, 1.5, 2, 2.5, 3, 3.5, 4, 4.5];
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
    return getData<CommentList>(`/review/store/${StoreId}/comments`);
}

export async function getCommentStatus(StoreId: string): Promise<CommentStatus> {
    return getData<CommentStatus>(`/review/store/${StoreId}/commentStatus`);
}

export async function getStoreCategories(storeId: string): Promise<Category[]> {
    return getData<Category[]>(`/store/${storeId}/categories`);
}


