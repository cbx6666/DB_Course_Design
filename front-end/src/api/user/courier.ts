import { postData } from '@/api/multiuse_function'

export interface CourierRating {
    userId: number;
    courierId: number;
    rating: number;
    orderId?: number;
    taskId?: number;
}

/**
 * 为骑手打分
 * @param userId 用户ID
 * @param courierId 骑手ID
 * @param rating 评分（1-5）
 * @param orderId 订单ID（可选）
 * @param taskId 配送任务ID（可选）
 */
export async function rateCourier(userId: number, courierId: number, rating: number, orderId?: number, taskId?: number) {
    return postData<CourierRating>(`/customer/delivery-complaints/rate/${courierId}`, { 
        userId, 
        courierId, 
        rating, 
        orderId, 
        taskId 
    })
}

