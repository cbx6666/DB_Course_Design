import { getData, postData, putData } from '@/api/multiuse_function'

export interface UserAddress {
    id: number;
    name: string;
    phoneNumber: number;
    address: string;
    gender?: string;
    isDefault?: boolean;
}

export const getUserAddresses = () => getData<UserAddress[]>("/customer/info/profile/addresses");
export const createUserAddress = (payload: Omit<UserAddress, 'id'>) => postData("/customer/info/profile/account/address/create", payload);
export const updateUserAddress = (payload: UserAddress) => putData(`/customer/info/profile/account/address/update/${payload.id}`, payload);

// 兼容旧接口：地址类型别名
export type Address = UserAddress;
