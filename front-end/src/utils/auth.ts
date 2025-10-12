// src/utils/auth.ts
import { getToken, getUserIdFromToken } from './jwt';
import { useUserStore } from '@/stores/user';

/**
 * 恢复用户登录状态
 * 统一的用户状态恢复逻辑，避免在多个组件中重复
 */
export function restoreUserState(): boolean {
    const userStore = useUserStore();
    const token = getToken();

    if (!token) {
        return false;
    }

    const tokenUserId = getUserIdFromToken(token);
    if (tokenUserId && userStore.getUserID() <= 0) {
        const numericUserID = parseInt(tokenUserId, 10);
        userStore.login(numericUserID);
        return true;
    }

    return false;
}

/**
 * 检查用户是否已登录
 */
export function isUserLoggedIn(): boolean {
    const userStore = useUserStore();
    return userStore.getUserID() > 0;
}

/**
 * 获取当前用户ID
 */
export function getCurrentUserId(): number {
    const userStore = useUserStore();
    return userStore.getUserID();
}
