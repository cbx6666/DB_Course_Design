import apiClient from '../client';

export interface MenuInfo {
    id: number;
    name: string;
    description: string;
    isActive: boolean;
    createdAt: string;
    dishCount?: number;
}

export interface CreateMenuRequest {
    name: string;
    description: string;
}

export interface MenuListResponse {
    list: MenuInfo[];
    total: number;
}

export const getMenus = async (sellerId: number): Promise<MenuListResponse> => {
    const response = await apiClient.get('/menus', { params: { sellerId: sellerId.toString() } });
    return response.data;
};

export const createMenu = async (menuData: CreateMenuRequest, sellerId: number): Promise<MenuInfo> => {
    const response = await apiClient.post('/menus', { ...menuData, sellerId });
    return response.data;
};

export const updateMenu = async (menuId: number, menuData: CreateMenuRequest): Promise<MenuInfo> => {
    const response = await apiClient.put(`/menus/${menuId}`, menuData);
    return response.data;
};

export const deleteMenu = async (menuId: number): Promise<void> => {
    await apiClient.delete(`/menus/${menuId}`);
};

export const setActiveMenu = async (menuId: number, sellerId: number): Promise<void> => {
    await apiClient.put(`/menus/${menuId}/set-active`, null, { params: { sellerId } });
};


