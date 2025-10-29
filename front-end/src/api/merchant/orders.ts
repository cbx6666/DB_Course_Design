import apiClient from '../client';

export interface OrderCouponInfo {
    couponId: number;
    couponName?: string;
    description?: string;
    discountType: string; // 'fixed' | 'discount'
    discountValue: number;
    validFrom: string;
    validTo: string;
    isUsed: boolean;
}

export interface FoodOrder {
    orderId: number;
    paymentTime: string;
    remarks?: string;
    customerId: number;
    cartId: number;
    storeId: number;
    sellerId: number;
    orderState: number;
    deliveryTaskId?: number | null;
    deliveryStatus?: number | null;
    deliveryAddress?: string;
    deliveryName?: string;
    deliveryPhone?: string;
    deliveryFee?: number;
    items?: OrderItem[];
    usedCoupon?: OrderCouponInfo;
}

export interface OrderItem {
    dishId: number;
    quantity: number;
    totalPrice?: number;
    dish?: {
        dishName?: string;
        price?: number;
    };
}

export const getOrders = async (params?: { sellerId?: number; storeId?: number }) => {
    const response = await apiClient.get('/orders', { params });
    const mapItem = (it: any): OrderItem => ({
        dishId: it?.dishId ?? it?.dish?.id ?? 0,
        quantity: it?.quantity ?? 0,
        totalPrice: it?.totalPrice ?? 0,
        dish: it?.dish ? {
            dishName: it.dish.dishName,
            price: it.dish.price,
        } : undefined,
    });

    const list = (response.data || []).map((o: any) => ({
        orderId: o.orderId,
        paymentTime: o.paymentTime,
        remarks: o.remarks,
        customerId: o.customerId,
        cartId: o.cartId,
        storeId: o.storeId,
        sellerId: o.sellerId,
        orderState: o.orderState ?? 0,
        deliveryTaskId: o.deliveryTaskId ?? null,
        deliveryStatus: o.deliveryStatus ?? null,
        deliveryAddress: o.deliveryAddress,
        deliveryName: o.deliveryName,
        deliveryPhone: o.deliveryPhone,
        deliveryFee: o.deliveryFee ?? 0,
        items: Array.isArray(o.items) ? o.items.map(mapItem) : [],
        usedCoupon: o.usedCoupon ? {
            couponId: o.usedCoupon.couponId ?? 0,
            couponName: o.usedCoupon.couponName,
            description: o.usedCoupon.description,
            discountType: o.usedCoupon.discountType ?? '',
            discountValue: o.usedCoupon.discountValue ?? 0,
            validFrom: o.usedCoupon.validFrom ?? '',
            validTo: o.usedCoupon.validTo ?? '',
            isUsed: o.usedCoupon.isUsed ?? false,
        } : undefined,
    })) as FoodOrder[];
    return list;
};

export const acceptOrder = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/accept`);
};

// 标记为已准备（出餐）
export const markAsReady = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/ready`);
};

// 发布配送任务
export const publishDeliveryTask = async (orderId: number, estimatedArrivalTime: string, estimatedDeliveryTime: string) => {
    const response = await apiClient.post('/delivery-tasks/publish', {
        OrderId: orderId,
        EstimatedArrivalTime: estimatedArrivalTime,
        EstimatedDeliveryTime: estimatedDeliveryTime
    });
    return response.data;
};

// 获取订单配送信息
export const getOrderDeliveryInfo = async (orderId: number) => {
    const response = await apiClient.get(`/delivery-tasks/order/${orderId}`);
    return response.data;
};

