import apiClient from '../client';

export interface Dish {
    dishId: number;
    dishName: string;
    price: number;
    description: string;
    isSoldOut: number;
    categoryId: number;
}

export interface NewDishData {
    dishName: string;
    price: string | number;
    description: string;
    isSoldOut?: number;
    categoryId: number;
}

export const getDishes = async (sellerId: number) => {
    const response = await apiClient.get('/dishes', {
        params: { sellerId: sellerId.toString() }
    });
    const list = (response.data || []).map((d: any) => ({
        dishId: d.DishID ?? d.dishId,
        dishName: d.DishName ?? d.dishName,
        price: d.Price ?? d.price,
        description: d.Description ?? d.description,
        isSoldOut: d.IsSoldOut ?? d.isSoldOut,
        categoryId: d.CategoryID ?? d.categoryId ?? 1,
    })) as Dish[];
    return list;
};

export const createDish = async (dishData: NewDishData, sellerId: number) => {
    const payload = {
        DishName: dishData.dishName,
        Price: Number(dishData.price),
        Description: dishData.description,
        IsSoldOut: dishData.isSoldOut ?? 2,
        SellerID: sellerId
    };
    const response = await apiClient.post('/dishes', payload);
    const d = response.data;
    const mapped: Dish = {
        dishId: d.DishID ?? d.dishId,
        dishName: d.DishName ?? d.dishName,
        price: d.Price ?? d.price,
        description: d.Description ?? d.description,
        isSoldOut: d.IsSoldOut ?? d.isSoldOut,
        categoryId: d.CategoryID ?? d.categoryId ?? 1,
    };
    return mapped;
};

export const updateDish = async (
    dishId: number,
    dishData: { dishName?: string; price?: number; description?: string; isSoldOut?: number },
    sellerId: number
) => {
    const payload: any = { SellerID: sellerId };
    if (dishData.dishName !== undefined) payload.DishName = dishData.dishName;
    if (dishData.price !== undefined) payload.Price = dishData.price;
    if (dishData.description !== undefined) payload.Description = dishData.description;
    if (dishData.isSoldOut !== undefined) payload.IsSoldOut = dishData.isSoldOut;

    const response = await apiClient.patch(`/dishes/${dishId}`, payload);
    const d = response.data;
    const mapped: Dish = {
        dishId: d.DishID ?? d.dishId,
        dishName: d.DishName ?? d.dishName,
        price: d.Price ?? d.price,
        description: d.Description ?? d.description,
        isSoldOut: d.IsSoldOut ?? d.isSoldOut,
        categoryId: d.CategoryID ?? d.categoryId ?? 1,
    };
    return mapped;
};

export const toggleDishSoldOut = async (dishId: number, isSoldOut: number, sellerId: number) => {
    const payload = { IsSoldOut: isSoldOut, SellerID: sellerId };
    const response = await apiClient.patch(`/dishes/${dishId}/soldout`, payload);
    return response.data;
};

export const deleteDish = async (dishId: number): Promise<void> => {
    await apiClient.delete(`/dishes/${dishId}`);
};


