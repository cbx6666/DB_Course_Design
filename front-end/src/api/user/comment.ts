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

export async function postStoreComment(userId: number, storeId: number, rating: number, content: string) {
    return postData<StoreComment>(`/user/comment`, { userId, storeId, rating, content })
}

export async function postRiderComment(userId: number, orderId: number, content: string) {
    return postData<RiderComment>(`/user/courier/comment`, { userId, orderId, content })
}


