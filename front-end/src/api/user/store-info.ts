import { getData } from '@/api/multiuse_function'

export interface StoreInfoForUser {
    storeId: number;
    storeName: string;
    storeImage: string;
}

export const getUserFacingStoreInfo = (storeId: number) => getData<StoreInfoForUser>(`/user/store/${storeId}`);


