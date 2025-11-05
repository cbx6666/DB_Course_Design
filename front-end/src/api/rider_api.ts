// src/api/rider.api.ts
import apiClient from './client'; // 导入我们共享的客户端

// --- 从 types.ts 导入骑手相关的数据类型 ---
import type {
    UserProfile,
    WorkStatus,
    Order,
    UpdateProfilePayload,
    OrderStatus,
    LocationInfo,
    Complaint
} from './types';

// --- 修正后的 API 函数 ---

/** 获取用户（骑手）个人资料 */
export const fetchUserProfile = () => {
    // 【已修正】路径从 /user/profile 改为 /courier/info/profile
    return apiClient.get<UserProfile>('/courier/info/profile');
};

/** 获取骑手工作状态 */
export const fetchWorkStatus = () => {
    // 【已修正】路径从 /user/status 改为 /courier/info/status
    // 我们的后端返回 { code, message, data: { isOnline: boolean } }
    // apiClient 应该配置为自动提取 data 字段，所以这里的类型 WorkStatus 是正确的
    return apiClient.get<WorkStatus>('/courier/info/status');
};

/** 获取收入数据 */
export const fetchIncomeData = () => {
    // 【已修正】路径从 /income/thisMonth 改为 /courier/info/income/monthly
    // 我们的后端直接返回纯数字，所以类型是 number
    return apiClient.get<number>('/courier/info/income/monthly');
};

/** 根据状态获取配送任务列表 */
export const fetchOrders = (status: OrderStatus) => {
    // 【已修正】路径从 /courier/info/orders 改为 /courier/delivery-tasks
    // 这将生成正确的 URL: /api/courier/delivery-tasks?status=pending
    return apiClient.get<Order[]>('/courier/delivery-tasks', { params: { status } });
};

/** 获取骑手当前位置信息 */
export const fetchLocationInfo = () => {
    // 【已修正】路径从 /user/location 改为 /courier/info/location
    // 后端返回 { data: { area: "..." } }，所以需要一个匹配的类型
    return apiClient.get<LocationInfo>('/courier/info/location');
};



/** 切换工作状态 (上班/下班) */
export const toggleWorkStatusAPI = (newStatus: boolean) => {
    // 【已修正】路径从 /user/status 改为 /courier/info/status/toggle
    return apiClient.post<{ success: boolean }>('/courier/info/status/toggle', { isOnline: newStatus });
};



// 在文件末尾新增这两个函数

/**
 * 骑手确认取单
 * @param taskId 配送任务ID
 */
export const pickupOrderAPI = (taskId: string) => {
    return apiClient.post<{ success: boolean }>(`/courier/delivery-tasks/${taskId}/pickup`);
};

/**
 * 骑手确认送达
 * @param taskId 配送任务ID
 */
export const deliverOrderAPI = (taskId: string) => {
    return apiClient.post<{ success: boolean }>(`/courier/delivery-tasks/${taskId}/deliver`);
};

// --- 以下是你队友原来的其他接口，可以暂时保留 ---
interface RiderInfo {
    vehicleType: string;
}
export const updateRiderInfo = (riderData: RiderInfo) => {
    return apiClient.put('/user/profile/rider', riderData);
};

/**
 * 骑手接受一个可接配送任务 (抢单)
 * @param taskId 配送任务ID
 */
export const acceptAvailableOrderAPI = (taskId: string) => {
    return apiClient.post<{ success: true }>(`/courier/delivery-tasks/${taskId}/accept`);
};


/**
 * 获取骑手的投诉记录列表
 */
export const fetchComplaints = () => {
    // 【已修正】路径从 /courier/info/complaints 改为 /courier/delivery-complaints
    return apiClient.get<Complaint[]>('/courier/delivery-complaints');
};

/**
 * 获取当前骑手附近的可接配送任务列表
 */
export const fetchAvailableOrders = () => {
    // 【已修正】路径从 /courier/info/orders/available 改为 /courier/delivery-tasks/available
    return apiClient.get<Order[]>('/courier/delivery-tasks/available');
};


/**
 * 更新骑手在服务器上的位置信息
 * @param latitude 纬度
 * @param longitude 经度
 */
export const updateCourierLocationAPI = (latitude: number, longitude: number) => {
    // 调用我们刚刚在后端创建的 POST /api/courier/info/location/update 接口
    return apiClient.post('/courier/info/location/update', { latitude, longitude });
};


/**
 * 更新用户（骑手）的个人资料
 * @param profileData 包含更新信息的用户对象
 */
export const updateUserProfile = (profileData: UpdateProfilePayload) => {
    return apiClient.put<{ success: boolean; message: string }>('/courier/info/profile', profileData);
};

/** 获取用于编辑页面的个人资料 */
export const fetchProfileForEdit = () => {
    // 后端返回 UpdateProfileDto，其结构与 UpdateProfilePayload 兼容
    return apiClient.get<UpdateProfilePayload>('/courier/info/profile/for-edit');
};

/**
 * 上传头像文件
 * @param file 图片文件对象
 */
export const uploadAvatarAPI = (file: File) => {
    const formData = new FormData();
    formData.append('file', file); // 'file' 必须与后端 IFormFile 参数名一致

    return apiClient.post<{ url: string }>('/files/upload/avatar', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });
};

