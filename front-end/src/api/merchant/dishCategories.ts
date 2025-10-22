import apiClient from '../client';

export interface DishCategoryInfo {
    categoryID: number;
    categoryName: string;
    dishCount: number;
}

export interface CreateDishCategoryRequest {
    categoryName: string;
    menuId: number;
}

export interface DishCategoryListResponse {
    list: DishCategoryInfo[];
    total: number;
}

export const getDishCategories = async (menuId: number): Promise<DishCategoryListResponse> => {
    const response = await apiClient.get('/dish-categories', { params: { menuId } });
    return response.data;
};

export const createDishCategory = async (categoryData: CreateDishCategoryRequest): Promise<DishCategoryInfo> => {
    const response = await apiClient.post('/dish-categories', categoryData);
    return response.data;
};

export const updateDishCategory = async (categoryId: number, categoryData: CreateDishCategoryRequest): Promise<DishCategoryInfo> => {
    const response = await apiClient.put(`/dish-categories/${categoryId}`, categoryData);
    return response.data;
};

export const deleteDishCategory = async (categoryId: number): Promise<void> => {
    await apiClient.delete(`/dish-categories/${categoryId}`);
};

export const addCategoryToMenu = async (categoryId: number, menuId: number): Promise<void> => {
    await apiClient.post(`/dish-categories/${categoryId}/add-to-menu`, null, { params: { menuId } });
};

export const removeCategoryFromMenu = async (categoryId: number, menuId: number): Promise<void> => {
    await apiClient.delete(`/dish-categories/${categoryId}/remove-from-menu`, { params: { menuId } });
};
