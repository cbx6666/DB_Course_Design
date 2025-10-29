<template>
  <main class="pt-20 min-h-screen max-w-screen-xl mx-auto px-6 py-8">
    <h1 class="text-3xl font-bold text-gray-800 mb-8 text-left">优惠券</h1>

    <!-- 加载中 -->
    <div v-if="loading" class="flex justify-center items-center h-64">
      <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
    </div>

    <!-- 优惠券列表 -->
    <div v-else class="space-y-4">
      <div v-if="coupons.length === 0" class="text-center py-12 text-gray-500">
        <i class="fas fa-ticket-alt text-4xl mb-4"></i>
        <p>暂无可用优惠券</p>
      </div>

      <div
        v-for="coupon in coupons"
        :key="coupon.couponManagerID"
        class="bg-yellow-50 rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow border border-yellow-200"
      >
        <div class="flex">
          <!-- 左侧：优惠券信息 -->
          <div class="flex-1 p-6 border-r-2 border-dashed border-gray-300">
            <div class="flex items-start justify-between mb-4">
              <div class="flex items-center space-x-3">
                <div class="w-16 h-16 rounded-lg bg-gray-100 flex items-center justify-center overflow-hidden">
                  <img
                    v-if="coupon.storeImage"
                    :src="normalizeImageUrl(coupon.storeImage)"
                    :alt="coupon.storeName"
                    class="max-w-full max-h-full w-auto h-auto object-contain"
                    @error="handleImageError"
                  />
                  <i v-else class="fas fa-store text-2xl text-gray-400"></i>
                </div>
                <div>
                  <h3 class="font-bold text-lg text-gray-800">{{ coupon.storeName }}</h3>
                  <p class="text-sm text-gray-500">{{ coupon.couponName }}</p>
                </div>
              </div>
              <div class="text-right">
                <div class="text-2xl font-bold text-[#F9771C]">
                  <span v-if="coupon.type === 'fixed'">¥{{ coupon.value.toFixed(0) }}</span>
                  <span v-else>{{ (coupon.value * 10).toFixed(1) }}折</span>
                </div>
                <p class="text-xs text-gray-500 mt-1">
                  <span v-if="coupon.minimumSpend === 0">无门槛</span>
                  <span v-else>满{{ coupon.minimumSpend.toFixed(0) }}元可用</span>
                </p>
              </div>
            </div>

            <div v-if="coupon.description" class="text-sm text-gray-600 mb-3">
              {{ coupon.description }}
            </div>

            <div class="flex items-center justify-between text-xs text-gray-500">
              <span>有效期：{{ formatDate(coupon.validFrom) }} 至 {{ formatDate(coupon.validTo) }}</span>
              <span v-if="coupon.remainingQuantity > 0" class="text-orange-500">
                剩余 {{ coupon.remainingQuantity }} 张
              </span>
              <span v-else class="text-red-500">已领完</span>
            </div>
          </div>

          <!-- 右侧：领取按钮 -->
          <div class="w-40 flex items-center justify-center p-6 bg-orange-50">
            <button
              v-if="!coupon.isClaimed && coupon.remainingQuantity > 0"
              @click="claimCoupon(coupon.couponManagerID)"
              :disabled="claimingCouponId === coupon.couponManagerID"
              class="bg-orange-500 hover:bg-orange-600 disabled:bg-gray-400 text-white px-4 py-3 rounded-lg font-medium transition-colors cursor-pointer whitespace-nowrap w-full text-center"
            >
              <span v-if="claimingCouponId === coupon.couponManagerID">
                <i class="fas fa-spinner fa-spin mr-2"></i>领取中
              </span>
              <span v-else>立即领取</span>
            </button>
            <div
              v-else-if="coupon.isClaimed"
              class="text-center"
            >
              <div class="text-green-500 font-medium mb-1">
                <i class="fas fa-check-circle text-xl mb-2"></i>
              </div>
              <p class="text-sm text-gray-600">已领取</p>
            </div>
            <div
              v-else
              class="text-center"
            >
              <p class="text-sm text-gray-500">已领完</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { getAvailableCoupons, claimCoupon as claimCouponApi, type AvailableCoupon } from '@/api/user/coupon';

const loading = ref(true);
const coupons = ref<AvailableCoupon[]>([]);
const claimingCouponId = ref<number | null>(null);

onMounted(() => {
  loadCoupons();
});

const loadCoupons = async () => {
  try {
    loading.value = true;
    coupons.value = await getAvailableCoupons();
  } catch (error) {
    console.error('加载优惠券失败:', error);
    ElMessage.error('加载优惠券失败，请重试');
  } finally {
    loading.value = false;
  }
};

const claimCoupon = async (couponManagerId: number) => {
  try {
    claimingCouponId.value = couponManagerId;
    const result = await claimCouponApi(couponManagerId);
    
    if (result.success) {
      ElMessage.success('优惠券领取成功！');
      // 刷新列表
      await loadCoupons();
    } else {
      ElMessage.error(result.message || '优惠券领取失败');
    }
  } catch (error: any) {
    console.error('领取优惠券失败:', error);
    ElMessage.error(error.response?.data?.message || '优惠券领取失败，请重试');
  } finally {
    claimingCouponId.value = null;
  }
};

// 格式化日期显示（去掉秒数）
function formatDate(dateStr: string) {
  const date = new Date(dateStr);
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}`;
}
</script>

