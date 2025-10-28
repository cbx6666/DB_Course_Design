import { getData, postData, deleteData } from '@/api/multiuse_function'

export interface CheckoutPayload {
    cartId: number;
    couponId?: number;
    addressId: number;
    remarks?: string;
}

export const getCheckoutSummary = (cartId: number) => getData<any>(`/user/checkout/${cartId}/summary`);
export const submitCheckout = (payload: CheckoutPayload) => postData(`/user/checkout/submit`, payload);

// 兼容旧购物车相关 API
export interface ShoppingCartItem {
    itemId: number;
    dishId: number;
    quantity: number;
    totalPrice: number;
}

export interface ShoppingCart {
    cartId: number;
    totalPrice: number;
    items: ShoppingCartItem[];
}

export interface MenuItem {
    id: number;
    name: string;
    description: string;
    price: number;
    image: string;
    isSoldOut: number;
    categoryId?: number;
}

export const getMenuItem = (StoreID: string) => getData<MenuItem[]>("/store/dish", { params: { storeId: StoreID } });
export const getShoppingCart = (StoreID: string, userId?: number) => getData<ShoppingCart>("/store/shoppingcart", { params: { storeId: StoreID } });
export const addOrUpdateCartItem = (cartId: number, dishId: number, quantity: number) => postData<ShoppingCartItem>('/store/cart/change', { cartId, dishId, quantity });
export const removeCartItem = (cartId: number, dishId: number) => deleteData<ShoppingCartItem>('/store/cart/remove', { cartId, dishId });

export interface Order {
    paymentTime: Date;
    customerID: number;
    cartID: number;
    storeID: number;
    deliveryFee: number;
}

export const submitOrder = (customerId: number, cartId: number, storeId: number, deliveryInfoId: number, deliveryFee: number, remarks?: string) => {
    const requestBody = {
        CartId: cartId,
        CustomerId: customerId,
        StoreId: storeId,
        DeliveryInfoID: deliveryInfoId,
        PaymentTime: new Date().toISOString(),
        DeliveryFee: deliveryFee,
        Remarks: remarks || ''
    };
    return postData<Order>('/store/order/create', requestBody);
}

export const useCoupon = (couponId: number | null) => {
    if (couponId == null) return Promise.resolve();
    return postData(`/user/checkout/coupon`, { couponId });
}


