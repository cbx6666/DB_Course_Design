// src/composables/useMerchantLayout.ts
import { ref, onMounted } from 'vue';
import { getMerchantInfo, type MerchantInfo } from '@/api/merchant_api';
import { devLog } from '@/utils/logger';

/**
 * 商家布局组合式函数
 * 提供商家页面通用的数据和逻辑
 */
export function useMerchantLayout() {
    const merchantInfo = ref<MerchantInfo | null>(null);
    const isLoading = ref(true);
    const error = ref<string>('');

    /**
     * 获取商家信息
     */
    const fetchMerchantInfo = async () => {
        try {
            devLog.component('MerchantLayout', '开始获取商家信息');
            isLoading.value = true;
            error.value = '';

            const info = await getMerchantInfo();
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
        fetchMerchantInfo();
    });

    return {
        merchantInfo,
        isLoading,
        error,
        fetchMerchantInfo
    };
}
