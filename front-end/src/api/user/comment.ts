import { postData } from '@/api/multiuse_function'

export interface StoreComment {
    userId: number;
    storeId: number;
    rating: number;
    content: string;
}

export interface RiderComment {
    userId: number;
    orderId: number;
    content: string;
}

export async function postStoreComment(userId: number, storeId: number, rating: number, content: string, images?: string, orderId?: number) {
    return postData<StoreComment>(`/customer/comments/comment`, { userId, storeId, rating, content, images, orderId })
}
