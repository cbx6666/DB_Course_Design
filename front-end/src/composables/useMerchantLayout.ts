// src/composables/useMerchantLayout.ts
import { ref, onMounted } from 'vue';
import { API_CONFIG } from '@/config';
import { getMerchantInfo, type MerchantInfo } from '@/api/merchant';
import { devLog } from '@/utils/logger';

/**
 * 商家布局组合式函数
 * 提供商家页面通用的数据和逻辑
 */
export function useMerchantLayout() {
    // 使用模块级单例状态，确保不同组件间共享
    if (!(useMerchantLayout as any)._state) {
        (useMerchantLayout as any)._state = {
            merchantInfo: ref<MerchantInfo | null>(null),
            isLoading: ref(true),
            error: ref<string>('')
        };
    }
    const { merchantInfo, isLoading, error } = (useMerchantLayout as any)._state as {
        merchantInfo: ReturnType<typeof ref<MerchantInfo | null>>,
        isLoading: ReturnType<typeof ref<boolean>>,
        error: ReturnType<typeof ref<string>>
    };

    /**
     * 获取商家信息
     */
    const fetchMerchantInfo = async () => {
        try {
            devLog.component('MerchantLayout', '开始获取商家信息');
            isLoading.value = true;
            error.value = '';

            const info = await getMerchantInfo();
            // 规范化头像 URL
            if (info && (info as any).avatar) {
                const u = (info as any).avatar as string;
                (info as any).avatar = normalizeAssetUrl(u);
            }
            merchantInfo.value = info;

            devLog.user('商家信息获取成功:', info);
        } catch (err: any) {
            devLog.error('获取商家信息失败:', err);
            error.value = '获取商家信息失败，请刷新页面重试';
        } finally {
            isLoading.value = false;
        }
    };

    /**
     * 初始化
     */
    onMounted(() => {
        if (!merchantInfo.value) {
            fetchMerchantInfo();
        }
    });

    return {
        merchantInfo,
        isLoading,
        error,
        fetchMerchantInfo
    };
}

function normalizeAssetUrl(u?: string): string {
    if (!u) return '';
    if (u.startsWith('http://') || u.startsWith('https://')) return u;
    if (u.startsWith('/')) return `${API_CONFIG.BASE_URL}${u}`;
    return `${API_CONFIG.BASE_URL}/${u}`;
}
