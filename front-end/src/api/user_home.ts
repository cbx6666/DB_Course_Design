import { getData } from '@/api/multiuse_function'
import { postData } from '@/api/multiuse_function'

export interface showStore {
    id: number
    image: string
    averageRating: number
    name: string
    monthlySales: number
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

export interface OrderInfo {
    orderID: number
    paymentTime: string
    cartID: number
    storeID: number
    storeImage: string
    storeName: string
    dishImage: string[]
    totalAmount: number
    orderStatus: number
}

export interface UserInfo {
    name: string
    phoneNumber: number
    image: string
    defaultAddress: string
}

export interface AfterSale {
    userID: number;
    orderID: number;
    content: string;
}

export async function getAllStore() {
    return getData<AllStore>(`/api/user/home/stores`);
}

export async function getRecomStore() {
    return getData<RecomStore>(`/api/user/home/recommend`);
}

export async function getSearchStore(UserID: number, Address: string, Keyword: string) {
    return getData<SearchStore>(`/api/user/home/search`, {
        params: {
            userId: UserID,
            address: Address,
            keyword: Keyword
        }
    });
}

export async function getOrderInfo() {
    return getData<OrderInfo[]>(`/api/user/home/orders`);
}

export async function getUserInfo() {
    return getData<UserInfo>(`/api/user/home/userinfo`);
}

export async function postAfterSaleApplication(orderId: number, description: string) {
    return postData<AfterSale>(`/api/user/applications/create`, {
        orderId,
        description
    })
}