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
    orderTime?: string;
    totalAmount?: number;
    customerName?: string;
    customerPhone?: string;
    deliveryAddress?: string;
    items?: OrderItem[];
}

export interface OrderItem {
    dishId: number;
    quantity: number;
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
        orderTime: o.OrderTime ?? o.orderTime,
        totalAmount: o.TotalAmount ?? o.totalAmount,
        customerName: o.CustomerName ?? o.customerName,
        customerPhone: o.CustomerPhone ?? o.customerPhone,
        deliveryAddress: o.DeliveryAddress ?? o.deliveryAddress,
        items: Array.isArray(o.Items ?? o.items) ? (o.Items ?? o.items).map(mapItem) : [],
    })) as FoodOrder[];
    return list;
};

export const getOrderById = async (orderId: number) => {
    const response = await apiClient.get(`/orders/${orderId}`);
    const o = response.data;
    const mapItem = (it: any): OrderItem => ({
        dishId: it?.DishID ?? it?.dishId ?? it?.dish?.DishID ?? it?.dish?.id ?? 0,
        quantity: it?.Quantity ?? it?.quantity ?? 0,
        dish: (it?.Dish || it?.dish)
            ? {
                dishName: it?.Dish?.DishName ?? it?.dish?.dishName,
                price: it?.Dish?.Price ?? it?.dish?.price,
            }
            : undefined,
    });

    const mapped: FoodOrder = {
        orderId: o.OrderID ?? o.orderId,
        paymentTime: o.PaymentTime ?? o.paymentTime,
        remarks: o.Remarks ?? o.remarks,
        customerId: o.CustomerID ?? o.customerId,
        cartId: o.CartID ?? o.cartId,
        storeId: o.StoreID ?? o.storeId,
        sellerId: o.SellerID ?? o.sellerId,
        orderState: o.OrderState ?? o.orderState,
        deliveryTaskId: o.DeliveryTaskId ?? o.deliveryTaskId ?? null,
        deliveryStatus: o.DeliveryStatus ?? o.deliveryStatus ?? null,
        orderTime: o.OrderTime ?? o.orderTime,
        totalAmount: o.TotalAmount ?? o.totalAmount,
        customerName: o.CustomerName ?? o.customerName,
        customerPhone: o.CustomerPhone ?? o.customerPhone,
        deliveryAddress: o.DeliveryAddress ?? o.deliveryAddress,
        items: Array.isArray(o.Items ?? o.items) ? (o.Items ?? o.items).map(mapItem) : [],
    };
    return mapped;
};

export const acceptOrder = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/accept`);
};

export const rejectOrder = async (orderId: number) => {
    await apiClient.post(`/orders/${orderId}/reject`);
};


