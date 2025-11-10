import { getData, postData } from '@/api/multiuse_function'

export interface showStore {
    id: number
    image: string
    averageRating: number
    name: string
    monthlySales: number
    description?: string
    category?: string
}

export interface RecomStore {
    recomStore: showStore[]
}

export interface AllStore {
    allStores: showStore[]
}

export interface SearchStore {
    searchStores: showStore[]
}

export interface OrderDish {
    dishName: string
    dishImage: string
    quantity: number
    price: number
}

export interface OrderCouponInfo {
    couponId: number
    couponName?: string
    description?: string
    discountType: string  // 'fixed' | 'discount'
    discountValue: number
    validFrom: string
    validTo: string
    isUsed: boolean
}

export interface OrderInfo {
    orderId: number
    paymentTime: string
    cartId: number
    storeId: number
    storeImage: string
    storeName: string
    dishImage: string[]
    dishDetails: OrderDish[]
    totalAmount: number  // 原始商品总价（不含优惠券、不含配送费）
    orderState: number
    deliveryStatus?: number | null
    usedCoupon?: OrderCouponInfo
    deliveryFee: number
}

export interface UserInfo {
    name: string
    phoneNumber: number
    avatar: string
}

export interface AfterSale {
    userID: number;
    orderID: number;
    content: string;
}

export async function getAllStore() {
    return getData<AllStore>(`/customer/stores`);
}

export async function getRecomStore() {
    return getData<RecomStore>(`/customer/stores/recommend`);
}

export async function getSearchStore(UserID: number, Address: string, Keyword: string) {
    return getData<SearchStore>(`/customer/stores/search`, {
        params: {
            userId: UserID,
            address: Address,
            keyword: Keyword
        }
    });
}

export async function getOrderInfo() {
    return getData<OrderInfo[]>(`/customer/orders`);
}

export async function getUserInfo() {
    return getData<UserInfo>(`/customer/info/home/userInfo`);
}

export async function postAfterSaleApplication(orderId: number, description: string, images?: string) {
    return postData<AfterSale>(`/customer/after-sales`, {
        orderId,
        description,
        images
    })
}

/**
 * 获取订单配送信息
 * @param orderId 订单ID
 */
export async function getOrderDeliveryInfo(orderId: number) {
    // getData 会自动处理 ApiResponseDto 格式，返回 data 字段
    return getData<any>(`/customer/orders/${orderId}/delivery-info`);
}

/**
 * 上传图片
 * @param file 图片文件
 * @returns 图片URL
 */
export async function uploadImage(file: File): Promise<string> {
    const formData = new FormData();
    formData.append('imageFile', file);
    // 使用 apiClient 直接调用，因为需要 multipart/form-data
    const API = (await import('@/api/index')).default;
    const response = await API.post<{ success: boolean; code: number; message: string; data: string }>(`/upload/image`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
    });
    // 处理 ApiResponseDto 格式
    if (response.data && typeof response.data === 'object' && 'data' in response.data && 'success' in response.data) {
        return response.data.data as string;
    }
    // 如果直接返回字符串
    if (typeof response.data === 'string') {
        return response.data;
    }
    throw new Error('上传失败：未返回图片URL');
}


