import { postData } from '@/api/multiuse_function'

export interface DeliveryComplaint {
    orderId?: number;
    deliveryTaskId?: number;
    complaintReason: string;
    images?: string;
}

/**
 * 提交配送投诉
 * @param userId 用户ID（后端从Token获取，不需要传）
 * @param orderId 订单ID（可选）
 * @param taskId 配送任务ID（可选）
 * @param complaintReason 投诉原因
 * @param images 图片URL列表（逗号分隔，可选）
 */
export async function postDeliveryComplaint(
    userId: number, 
    orderId?: number, 
    taskId?: number, 
    complaintReason?: string, 
    images?: string
) {
    return postData<DeliveryComplaint>(`/customer/delivery-complaints`, { 
        orderId, 
        deliveryTaskId: taskId,
        complaintReason,
        images 
    })
}

