<template>
  <transition name="fade">
    <div v-if="props.showCouponForm" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div class="bg-white w-full max-w-4xl rounded-lg shadow-xl p-6 overflow-y-auto max-h-[90vh] relative">
        <div class="flex justify-between items-center mb-6">
          <h3 class="text-2xl font-bold text-gray-900">我的优惠券</h3>
          <button class="text-gray-500 hover:text-gray-700" @click="closeForm">
            <i class="fas fa-times text-xl"></i>
          </button>
        </div>

        <!-- 加载中 -->
        <div v-if="loading" class="flex justify-center items-center h-64">
          <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
        </div>

        <!-- 优惠券列表 -->
        <div v-else class="space-y-4 relative pb-24">
          <div v-if="coupons.length === 0" class="text-center py-12 text-gray-500">
            <i class="fas fa-ticket-alt text-4xl mb-4"></i>
            <p>暂无可用优惠券</p>
          </div>

          <div
            v-for="coupon in coupons"
            :key="coupon.couponID"
            class="bg-yellow-50 rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow border border-yellow-200"
          >
            <div class="p-6">
                <div class="flex items-start justify-between mb-4">
                  <div class="flex items-center space-x-3">
                    <div class="w-16 h-16 rounded-lg bg-gray-100 flex items-center justify-center overflow-hidden">
                      <img
                        v-if="coupon.storeImage"
                        :src="normalizeImageUrl(coupon.storeImage)"
                        :alt="coupon.storeName || '店铺'"
                        class="max-w-full max-h-full w-auto h-auto object-contain"
                        @error="handleImageError"
                      />
                      <i v-else class="fas fa-store text-2xl text-gray-400"></i>
                    </div>
                    <div>
                      <h3 class="font-bold text-lg text-gray-800">{{ coupon.storeName || '未知店铺' }}</h3>
                      <p class="text-sm text-gray-500">{{ coupon.couponName || '优惠券' }}</p>
                    </div>
                  </div>
                  <div class="text-right">
                    <div class="text-2xl font-bold text-[#F9771C]">
                      <span v-if="coupon.couponType === 'discount'">{{ (coupon.discountAmount * 10).toFixed(1) }}折</span>
                      <span v-else>¥{{ coupon.discountAmount.toFixed(0) }}</span>
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
                  <span v-if="coupon.validFrom">
                    有效期：{{ formatDate(coupon.validFrom) }} 至 {{ formatDate(coupon.validTo) }}
                  </span>
                  <span v-else>
                    有效期至 {{ formatDate(coupon.validTo) }}
                  </span>
                </div>
            </div>
          </div>

          <!-- 更多优惠券按钮 -->
          <div class="absolute bottom-6 right-6 mt-4">
            <button
              @click="goToCoupons"
              class="bg-[#F9771C] hover:bg-[#F9771C]/90 text-white px-6 py-3 rounded-lg shadow-lg font-medium transition-all duration-300 flex items-center space-x-2"
            >
              <i class="fas fa-plus-circle"></i>
              <span>更多优惠券</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, ref, watch } from 'vue'
import { useRouter } from 'vue-router'

import type { CouponInfo } from '@/api/user';
import { useUserStore } from '@/stores/user';
import { getCouponInfo } from '@/api/user';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';

const router = useRouter();

const userStore = useUserStore();
const userID = userStore.getUserID();
const coupons = ref<CouponInfo[]>([]);
const loading = ref(false);

const props = defineProps<{
    showCouponForm: Boolean;
}>();

const emit = defineEmits<{
    (e: "update:showCouponForm", value: Boolean): void;
}>();

// 关闭弹窗
function closeForm() {
    emit("update:showCouponForm", false);
}

// 加载优惠券数据
async function loadCoupons() {
  try {
    loading.value = true;
    coupons.value = await getCouponInfo(userID);
  } catch (error) {
    console.error('获取优惠券信息失败:', error);
    coupons.value = [];
  } finally {
    loading.value = false;
  }
}

// 监听弹窗打开，每次打开时重新加载数据
watch(() => props.showCouponForm, async (newVal) => {
  if (newVal) {
    await loadCoupons();
  }
})

// 格式化日期显示
function formatDate(dateStr: string) {
  const date = new Date(dateStr);
  const year = date.getFullYear();
  const month = date.getMonth() + 1;
  const day = date.getDate();
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${year}年${month}月${day}日 ${hours}:${minutes}`;
}

// 跳转到优惠页面
function goToCoupons() {
  closeForm();
  router.push('/home/coupons');
}

</script>
