import { postData } from '@/api/multiuse_function'

export interface StoreReport {
    userId: number;
    storeId: number;
    content: string;
    images?: string;
}

export async function postStoreReport(userId: number, storeId: number, content: string, images?: string) {
    return postData<StoreReport>(`/customer/store-reports/${storeId}`, { userId, storeId, content, images })
}


