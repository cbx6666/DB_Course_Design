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
        @click="emit('checkout')"
        :disabled="total === 0"
      >
        {{ `立即支付 ¥${total.toFixed(2)}` }}
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, defineProps, defineEmits } from 'vue';

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

// 计算优惠金额
const discount = computed(() => {
  if (!props.selectedCoupon) return 0;
  
  const coupon = props.selectedCoupon;
  
  // 满减券：discountAmount 就是优惠金额
  if (coupon.couponType === 'fixed') {
    return coupon.discountAmount;
  }
  
  // 折扣券：discountAmount 是折扣比例（0-1），需要计算优惠金额
  // 例如：discountAmount = 0.8 表示8折，优惠金额 = subtotal * (1 - 0.8) = subtotal * 0.2
  if (coupon.couponType === 'discount') {
    const discountRatio = coupon.discountAmount; // 已经是 0-1 的比例
    const discountAmount = props.subtotal * (1 - discountRatio);
    // 确保优惠金额不超过订单总额
    return Math.min(discountAmount, props.subtotal);
  }
  
  // 兼容旧数据（如果没有 couponType）
  return coupon.discountAmount;
});

// 计算实付金额 = 商品总价 + 配送费 - 优惠金额
const total = computed(() => Math.max(0, props.subtotal + props.deliveryFee - discount.value));
</script>