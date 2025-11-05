import { getData, postData, deleteData } from '@/api/multiuse_function'

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

export const getMenuItem = (StoreID: string) => getData<MenuItem[]>(`/store/${StoreID}/menu`);
export const getShoppingCart = (StoreID: string, userId?: number) => getData<ShoppingCart>(`/cart/store/${StoreID}`);
export const addOrUpdateCartItem = (cartId: number, dishId: number, quantity: number) => postData<ShoppingCartItem>('/cart/item/update', { cartId, dishId, quantity });
export const removeCartItem = (cartId: number, dishId: number) => deleteData('/cart/item/remove', { cartId, dishId });

export interface Order {
    paymentTime: Date;
    customerID: number;
    cartID: number;
    storeID: number;
    deliveryFee: number;
}

export const submitOrder = (customerId: number, cartId: number, storeId: number, deliveryInfoId: number, deliveryFee: number, remarks?: string, couponId?: number | null) => {
    const requestBody: any = {
        CartId: cartId,
        CustomerId: customerId,
        StoreId: storeId,
        DeliveryInfoID: deliveryInfoId,
        PaymentTime: new Date().toISOString(),
        DeliveryFee: deliveryFee,
        Remarks: remarks || ''
    };
    // 如果提供了优惠券ID，添加到请求中
    if (couponId && couponId > 0) {
        requestBody.CouponId = couponId;
    }
    return postData<Order>('/orders/create', requestBody);
}

// 获取用户优惠券（用于结账页面选择）- 已迁移到 CustomerController
// 请使用 CustomerController 的 /customer/home/couponInfo 端点
// export const getUserCoupons = () => getData<any[]>('/user/coupons');

// 使用优惠券功能已废弃，优惠券在创建订单时直接传递
// export const useCoupon = (couponId: number | null) => {
//     if (couponId == null) return Promise.resolve();
//     return postData(`/user/checkout/coupon`, { couponId });
// }
