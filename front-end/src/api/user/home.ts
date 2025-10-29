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
    orderID: number
    paymentTime: string
    cartID: number
    storeID: number
    storeImage: string
    storeName: string
    dishImage: string[]
    dishDetails: OrderDish[]
    totalAmount: number  // 原始商品总价（不含优惠券折扣，不含配送费）
    orderStatus: number
    deliveryStatus?: number | null  // 配送状态：0=待取件, 1=待取单, 2=配送中, 3=已完成, null=无配送任务
    usedCoupon?: OrderCouponInfo  // 使用的优惠券信息
    deliveryFee: number  // 配送费
}

export interface UserInfo {
    name: string
    phoneNumber: number
    image: string
}

export interface AfterSale {
    userID: number;
    orderID: number;
    content: string;
}

export async function getAllStore() {
    return getData<AllStore>(`/user/home/stores`);
}

export async function getRecomStore() {
    return getData<RecomStore>(`/user/home/recommend`);
}

export async function getSearchStore(UserID: number, Address: string, Keyword: string) {
    return getData<SearchStore>(`/user/home/search`, {
        params: {
            userId: UserID,
            address: Address,
            keyword: Keyword
        }
    });
}

export async function getOrderInfo() {
    return getData<OrderInfo[]>(`/user/home/orders`);
}

export async function getUserInfo() {
    return getData<UserInfo>(`/user/home/userinfo`);
}

export async function postAfterSaleApplication(orderId: number, description: string) {
    return postData<AfterSale>(`/user/applications/create`, {
        orderId,
        description
    })
}


