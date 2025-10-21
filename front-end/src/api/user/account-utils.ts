import { getData } from '@/api/multiuse_function'

export async function getUserID(account: string | number) {
    const params: Record<string, string | number> = {}

    if (typeof account === 'number') {
        params.phoneNumber = account
    } else if (typeof account === 'string') {
        params.email = account
    } else {
        throw new Error('account 必须是手机号或邮箱')
    }

    return getData<number>('/user/account', { params })
}


