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
    return getData<AllStore>(`/customer/info/home/stores`);
}

export async function getRecomStore() {
    return getData<RecomStore>(`/customer/info/home/recommend`);
}

export async function getSearchStore(UserID: number, Address: string, Keyword: string) {
    return getData<SearchStore>(`/customer/info/home/search`, {
        params: {
            userId: UserID,
            address: Address,
            keyword: Keyword
        }
    });
}

export async function getOrderInfo() {
    return getData<OrderInfo[]>(`/customer/info/home/orders`);
}

export async function getUserInfo() {
    return getData<UserInfo>(`/customer/info/home/userInfo`);
}

export async function postAfterSaleApplication(orderId: number, description: string) {
    return postData<AfterSale>(`/user/applications/create`, {
        orderId,
        description
    })
}


