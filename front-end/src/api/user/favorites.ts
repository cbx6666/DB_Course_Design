import { getData } from '@/api/multiuse_function';
import { postData } from '@/api/multiuse_function';
import { deleteData } from '@/api/multiuse_function';

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

/**
 * 新建收藏夹
 */
export async function createFavoritesFolder(folderName: string): Promise<void> {
    await postData('/customer/info/favorites', { folderName });
}

/**
 * 删除收藏夹
 */
export async function deleteFavoritesFolder(folderId: number): Promise<void> {
    await deleteData(`/customer/info/favorites/${folderId}`);
}

/**
 * 将店铺加入收藏夹
 */
export async function addFavoriteToFolder(folderId: number, storeId: number, favoriteReason?: string): Promise<void> {
    await postData(`/customer/info/favorites/${folderId}/items`, { storeId, favoriteReason });
}

/**
 * 从收藏夹移除店铺
 */
export async function removeFavoriteFromFolder(folderId: number, storeId: number): Promise<void> {
    await deleteData(`/customer/info/favorites/${folderId}/items`, { storeId });
}

