import { getData } from '@/api/multiuse_function'
import API from '@/api/index';

export interface AccountInfo {
    id: number;
    name: string;
    avatar: string;
}

export interface AccountUpdateData {
    id: number;
    name: string;
    avatarFile: File;
}

export async function getAccountInfo() {
    return getData<AccountInfo>("/customer/info/profile/userProfile");
}

export async function saveAccountInfo(data: AccountUpdateData) {
    const formData = new FormData();
    formData.append('Id', data.id.toString());
    formData.append('Name', data.name);
    // 只有当文件存在且大小大于0时才添加头像文件
    if (data.avatarFile && data.avatarFile.size > 0) {
        formData.append('AvatarFile', data.avatarFile);
    }

    const response = await API.put('/customer/info/profile/account/update', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
    
    return response.data;
}
