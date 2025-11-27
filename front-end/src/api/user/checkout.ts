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

export const getMenuItemPage = (StoreID: string, params: { categoryId?: number; page?: number; pageSize?: number }) =>
    getData<{ items: MenuItem[]; hasMore: boolean }>(`/customer/stores/${StoreID}/menu/basic`, { params });

export const getMenuItem = async (StoreID: string) => {
    const result = await getMenuItemPage(StoreID, { page: 1, pageSize: 1000 });
    return result.items;
};
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
    // 后端配置了 JsonNamingPolicy.CamelCase，所以字段名必须是 camelCase
    const requestBody: any = {
        cartId: cartId,
        customerId: customerId,
        storeId: storeId,
        deliveryInfoID: deliveryInfoId, // 注意：ID 是缩写，保持大写
        paymentTime: new Date().toISOString(),
        deliveryFee: deliveryFee,
        remarks: remarks || ''
    };
    // 如果提供了优惠券ID，添加到请求中
    if (couponId && couponId > 0) {
        requestBody.couponId = couponId;
    }
    // 后端路由：api/customer/orders，方法：[HttpPost]
    return postData<Order>('/customer/orders', requestBody);
}
