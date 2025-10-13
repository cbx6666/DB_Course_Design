import { getData } from '@/api/multiuse_function'
import { putData } from '@/api/multiuse_function'

export interface Address {
    id: number;
    name: string;
    phoneNumber: number;
    address: string;
}

export async function getAddress(UserId: number) {
    return getData<Address>("/user/profile/address", {
        params: {
            userId: UserId
        }
    });
}

export async function saveAddressInfo(data: Address) {
    return putData<Address>("/account/address/save", data);
}