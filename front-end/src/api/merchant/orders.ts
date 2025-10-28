import apiClient from '../client';

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
    items?: OrderItem[];
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
        dishId: it?.DishID ?? it?.dishId ?? it?.dish?.DishID ?? it?.dish?.id ?? 0,
        quantity: it?.Quantity ?? it?.quantity ?? 0,
        totalPrice: it?.TotalPrice ?? it?.totalPrice ?? 0,
        dish: (it?.Dish || it?.dish)
            ? {
                dishName: it?.Dish?.DishName ?? it?.dish?.dishName,
                price: it?.Dish?.Price ?? it?.dish?.price,
            }
            : undefined,
    });

    const list = (response.data || []).map((o: any) => ({
        orderId: o.OrderID ?? o.orderId,
        paymentTime: o.PaymentTime ?? o.paymentTime,
        remarks: o.Remarks ?? o.remarks,
        customerId: o.CustomerID ?? o.customerId,
        cartId: o.CartID ?? o.cartId,
        storeId: o.StoreID ?? o.storeId,
        sellerId: o.SellerID ?? o.sellerId,
        orderState: o.OrderState ?? o.orderState ?? 0,
        deliveryTaskId: o.DeliveryTaskId ?? o.deliveryTaskId ?? null,
        deliveryStatus: o.DeliveryStatus ?? o.deliveryStatus ?? null,
        deliveryAddress: o.DeliveryAddress ?? o.deliveryAddress,
        deliveryName: o.DeliveryName ?? o.deliveryName,
        deliveryPhone: o.DeliveryPhone ?? o.deliveryPhone,
        items: Array.isArray(o.Items ?? o.items) ? (o.Items ?? o.items).map(mapItem) : [],
    })) as FoodOrder[];
    return list;
};

export const acceptOrder = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/accept`);
};

export const rejectOrder = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/reject`);
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

