import { getData } from '@/api/multiuse_function';

export interface FavoriteItem {
    itemID: number;
    storeID: number;
    storeName: string;
    storeImage?: string;
    favoritedAt: string;
    favoriteReason: string;
}

export interface FavoritesFolder {
    folderID: number;
    folderName: string;
    favoriteItems: FavoriteItem[];
}

/**
 * 获取用户的收藏夹列表
 */
export async function getFavoritesFolders(): Promise<FavoritesFolder[]> {
    // getData 已经会从 ApiResponseDto 中提取 data 字段，这里直接拿结果即可
    return await getData<FavoritesFolder[]>('/customer/info/favorites');
}

