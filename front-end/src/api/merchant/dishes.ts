import apiClient from '../client';

export interface Dish {
    dishId: number;
    dishName: string;
    price: number;
    description: string;
    isSoldOut: number;
    categoryId: number;
    image?: string;
}

export interface NewDishData {
    dishName: string;
    price: string | number;
    description: string;
    isSoldOut?: number;
    categoryId: number;
    image?: string;
}

export const getDishes = async (categoryId: number) => {
    const response = await apiClient.get('/merchant/dishes', {
        params: { categoryId: categoryId.toString() }
    });
    const list = (response.data || []).map((d: any) => ({
        dishId: d.dishId,
        dishName: d.dishName,
        price: d.price,
        description: d.description,
        isSoldOut: d.isSoldOut,
        categoryId: d.categoryId ?? 1,
        image: d.dishImage ?? d.image,
    })) as Dish[];
    return list;
};

export const createDish = async (dishData: NewDishData, sellerId: number) => {
    const payload = {
        DishName: dishData.dishName,
        Price: Number(dishData.price),
        Description: dishData.description,
        IsSoldOut: dishData.isSoldOut ?? 2,
        CategoryID: dishData.categoryId,
        DishImage: dishData.image
    };

    const response = await apiClient.post('/merchant/dishes', payload);
    const d = response.data;
    const mapped: Dish = {
        dishId: d.dishId,
        dishName: d.dishName,
        price: d.price,
        description: d.description,
        isSoldOut: d.isSoldOut,
        categoryId: d.categoryId ?? 1,
        image: d.dishImage ?? d.image,
    };
    return mapped;
};

export const updateDish = async (
    dishId: number,
    dishData: { dishName?: string; price?: number; description?: string; isSoldOut?: number; image?: string },
    sellerId: number
) => {
    const payload: any = { SellerID: sellerId };
    if (dishData.dishName !== undefined) payload.DishName = dishData.dishName;
    if (dishData.price !== undefined) payload.Price = dishData.price;
    if (dishData.description !== undefined) payload.Description = dishData.description;
    if (dishData.isSoldOut !== undefined) payload.IsSoldOut = dishData.isSoldOut;
    if (dishData.image !== undefined) payload.DishImage = dishData.image;

    const response = await apiClient.patch(`/merchant/dishes/${dishId}`, payload);
    const d = response.data;
    const mapped: Dish = {
        dishId: d.dishId,
        dishName: d.dishName,
        price: d.price,
        description: d.description,
        isSoldOut: d.isSoldOut,
        categoryId: d.categoryId ?? 1,
        image: d.dishImage ?? d.image,
    };
    return mapped;
};

export const toggleDishSoldOut = async (dishId: number, isSoldOut: number, sellerId: number) => {
    const payload = { IsSoldOut: isSoldOut, SellerID: sellerId };
    const response = await apiClient.patch(`/merchant/dishes/${dishId}/soldout`, payload);
    return response.data;
};

export const deleteDish = async (dishId: number): Promise<void> => {
    await apiClient.delete(`/merchant/dishes/${dishId}`);
};

export const uploadDishImage = async (imageFile: File): Promise<string> => {
    const formData = new FormData();
    formData.append('imageFile', imageFile);

    const response = await apiClient.post('/merchant/dishes/upload-image', formData, {
        headers: {
            'Content-Type': 'multipart/form-data',
        },
    });

    // 后端返回格式: ApiResponseDto<string>，图片URL在 Data 字段中
    if (response.data.success && response.data.data) {
        return response.data.data;
    } else {
        throw new Error(response.data.message || '图片上传失败');
    }
};
