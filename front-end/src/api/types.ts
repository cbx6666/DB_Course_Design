// src/api/types.ts
// 骑手相关的数据类型定义

export type OrderStatus = 'to_be_taken' | 'pending' | 'delivering' | 'completed' | 'cancelled';

export interface UserProfile {
    name: string;
    id: string;
    registerDate: string;
    rating: number;
    creditScore: number;
    gender?: string;
    birthday?: string;
    avatar?: string;
    vehicleType?: string;
}

export interface UpdateProfilePayload {
    name: string;
    gender?: string;
    birthday?: string;
    avatar?: string;
    vehicleType?: string;
}

export interface WorkStatus {
    isOnline: boolean;
}

export interface IncomeData {
    thisMonth: number;
}

export interface Order {
    id: string;
    status: OrderStatus;
    restaurant: string;
    pickupAddress: string;
    deliveryAddress: string;
    customer: string;
    fee: string;
    distance: string;
    time: string;
    isReadyForPickup: boolean;
}

export interface NewOrder {
    id: string;
    restaurantName: string;
    restaurantAddress: string;
    customerName: string;
    customerAddress: string;
    distance: string;
    fee: number;
    mapImageUrl: string;
}

export interface LocationInfo {
    area: string;
}

export interface Complaint {
    complaintID: string;
    deliveryTaskID: string;
    complaintTime: string;
    complaintReason: string;
    punishment?: {
        type: string;
        description: string;
        duration?: string;
    };
}

// 菜品种类相关类型定义
export interface DishCategory {
    categoryId: number;
    categoryName: string;
}