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
    return postData<StoreComment>(`/review/comment`, { userId, storeId, rating, content })
}

// 配送员评论功能 - 后端暂无对应端点，暂时保留接口定义但标记为废弃
// export async function postRiderComment(userId: number, orderId: number, content: string) {
//     return postData<RiderComment>(`/user/courier/comment`, { userId, orderId, content })
// }
