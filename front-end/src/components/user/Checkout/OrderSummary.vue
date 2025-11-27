<template>
  <div class="bg-white shadow-lg rounded-lg border-0">
    <div class="px-4 py-3 border-b">
      <h3 class="text-base font-semibold">费用明细</h3>
    </div>
    <div class="p-4 space-y-4">
      <div class="space-y-2">
        <div class="flex justify-between text-sm">
          <span class="text-gray-600">商品总价</span>
          <span>¥{{ subtotal.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-gray-600">配送费</span>
          <span>¥{{ deliveryFee.toFixed(2) }}</span>
        </div>
        <div class="flex justify-between text-sm">
          <span class="text-gray-600">优惠金额</span>
          <span class="text-[#F9771C] font-semibold">-¥{{ discount.toFixed(2) }}</span>
        </div>
      </div>

      <div class="border-t border-gray-200"></div>

      <div class="flex justify-between items-center">
        <span class="font-semibold text-lg">实付金额</span>
        <span class="font-bold text-xl text-[#F9771C]">¥{{ total.toFixed(2) }}</span>
      </div>

      <button
        class="w-full bg-[#F9771C] hover:bg-[#F9771C]/90 text-white font-semibold py-3 text-lg rounded"
        @click="showConfirmDialog = true"
        :disabled="total === 0"
      >
        {{ `立即支付 ¥${total.toFixed(2)}` }}
      </button>
    </div>
  </div>

  <!-- 支付确认弹窗 -->
  <div v-if="showConfirmDialog" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="showConfirmDialog = false">
    <div class="bg-white rounded-2xl shadow-2xl p-8 w-[500px] mx-4 transform transition-all">
      <div class="text-center mb-6">
        <div class="w-16 h-16 bg-orange-100 rounded-full flex items-center justify-center mx-auto mb-4">
          <i class="fas fa-money-bill-wave text-orange-500 text-2xl"></i>
        </div>
        <h3 class="text-lg font-bold text-gray-900 mb-2">确认支付</h3>
        <p class="text-sm text-gray-600 mb-4">请确认订单信息及支付金额</p>
        
        <!-- 支付详情 -->
        <div class="bg-gray-50 rounded-lg p-4 space-y-2 text-sm text-left">
          <div class="flex justify-between">
            <span class="text-gray-600">商品总价</span>
            <span class="font-medium">¥{{ subtotal.toFixed(2) }}</span>
          </div>
          <div class="flex justify-between">
            <span class="text-gray-600">配送费</span>
            <span class="font-medium">¥{{ deliveryFee.toFixed(2) }}</span>
          </div>
          <div v-if="discount > 0" class="flex justify-between">
            <span class="text-gray-600">优惠金额</span>
            <span class="font-medium text-orange-500">-¥{{ discount.toFixed(2) }}</span>
          </div>
          <div class="border-t border-gray-200 pt-2 flex justify-between items-center">
            <span class="text-gray-900 font-semibold">实付金额</span>
            <span class="text-xl font-bold text-orange-500">¥{{ total.toFixed(2) }}</span>
          </div>
        </div>
      </div>
      
      <div class="flex gap-3">
        <button 
          @click="showConfirmDialog = false"
          class="flex-1 bg-white border border-gray-200 hover:bg-gray-50 text-gray-700 px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer">
          取消
        </button>
        <button 
          @click="confirmPayment"
          class="flex-1 bg-orange-500 hover:bg-orange-600 text-white px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer shadow-sm">
          确认支付
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, ref, defineProps, defineEmits } from 'vue';

import type { CouponInfo } from '@/api/user';

const props = defineProps<{
  subtotal: number;
  selectedCoupon?: CouponInfo | null;
  deliveryFee: number;
}>();

console.log(props);

const emit = defineEmits<{
  (e: 'checkout'): void;
}>();

// 支付确认对话框状态
const showConfirmDialog = ref(false);

// 确认支付
const confirmPayment = () => {
  showConfirmDialog.value = false;
  emit('checkout');
};

// 计算优惠金额
const discount = computed(() => {
  if (!props.selectedCoupon) return 0;
  
  const coupon = props.selectedCoupon;
  
  // 满减券：discountAmount 就是优惠金额（单位：元）
  if (coupon.couponType === 'fixed') {
    return coupon.discountAmount;
  }
  
  // 折扣券：discountAmount 是折扣比例（0-1），需要计算优惠金额
  // 例如：discountAmount = 0.8 表示8折，优惠金额 = subtotal * (1 - 0.8) = subtotal * 0.2
  if (coupon.couponType === 'discount') {
    const discountRatio = coupon.discountAmount; // 已经是 0-1 的比例（例如 0.8 表示 8 折）
    const discountAmount = props.subtotal * (1 - discountRatio);
    // 确保优惠金额不超过订单总额
    return Math.min(discountAmount, props.subtotal);
  }
  
  // 兼容旧数据：如果没有 couponType，根据 discountAmount 的值判断
  // 如果 discountAmount <= 1，可能是折扣比例（0-1），否则是满减金额
  // 但这种方式不可靠，建议后端返回 couponType
  if (coupon.discountAmount <= 1 && coupon.discountAmount > 0) {
    // 可能是折扣券，按折扣比例计算
    console.warn('优惠券缺少 couponType 字段，按折扣券处理');
    const discountAmount = props.subtotal * (1 - coupon.discountAmount);
    return Math.min(discountAmount, props.subtotal);
  }
  
  // 默认当作满减券处理
  return coupon.discountAmount;
});

// 计算实付金额 = 商品总价 + 配送费 - 优惠金额
const total = computed(() => Math.max(0, props.subtotal + props.deliveryFee - discount.value));
</script>