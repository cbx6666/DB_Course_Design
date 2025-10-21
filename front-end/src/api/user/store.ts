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

export async function getStoreInfo(StoreId: string): Promise<StoreInfo> {
    return getData<StoreInfo>(`/user/store/storeInfo`, {
        params: {
            storeId: StoreId
        }
    });
}

export function getDeliveryTasks() {
    const deliveryTime = Math.floor(Math.random() * 51) + 20;
    const deliveryFee = parseFloat((deliveryTime * 0.5 + 5).toFixed(2));

    return {
        id: Math.floor(Math.random() * 10000),
        deliveryTime,
        deliveryFee,
    } as DeliveryTask;
}

export async function getCommentList(StoreId: string) {
    return getData<CommentList>("/user/store/commentList", {
        params: {
            storeId: StoreId
        }
    });
}

export async function getCommentStatus(StoreId: string): Promise<CommentStatus> {
    return getData<CommentStatus>("/user/store/commentStatus", {
        params: {
            storeId: StoreId
        }
    });
}


