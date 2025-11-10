import { postData } from '@/api/multiuse_function'

export interface StoreReport {
    userId: number;
    storeId: number;
    content: string;
}

export interface RiderReport {
    userId: number;
    orderId: number;
    content: string;
}

export async function postStoreReport(userId: number, storeId: number, content: string, images?: string) {
    return postData<StoreReport>(`/customer/store-reports/${storeId}`, { userId, storeId, content, images })
}

export async function postRiderReport(orderId: number, content: string, images?: string) {
    return postData(`/customer/delivery-complaints`, {
        orderId,
        complaintReason: content,
        images
    })
}


