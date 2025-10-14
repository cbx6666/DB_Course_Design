import { getData, postData, putData } from '@/api/multiuse_function'

export interface UserAddress {
    id: number;
    name: string;
    phoneNumber: number;
    address: string;
    gender?: string;
    isDefault?: boolean;
}

export const getUserAddresses = () => getData<UserAddress[]>("/user/profile/addresses");
export const createUserAddress = (payload: Omit<UserAddress, 'id'>) => postData("/user/profile/address", payload);
export const updateUserAddress = (payload: UserAddress) => putData("/user/profile/address", payload);

// 兼容旧接口：获取默认/单个地址
export type Address = UserAddress;
export const getAddress = (userId?: number) => getData<UserAddress>("/user/profile/address");


