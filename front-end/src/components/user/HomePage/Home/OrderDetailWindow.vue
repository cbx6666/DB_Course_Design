<template>
    <div v-if="visible"
        class="fixed border-2 top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-full max-w-2xl max-h-[80vh] p-6 bg-white rounded-xl shadow-xl z-50 overflow-y-auto">
        <!-- 关闭按钮 -->
        <button
            class="absolute top-2 right-2 text-gray-500 hover:text-black text-xl"
            @click="close"
        >
            ✖
        </button>

        <!-- 标题 -->
        <h2 class="text-xl font-semibold text-gray-800 mb-4 text-left">订单详情</h2>

        <!-- 订单信息 -->
        <div class="space-y-4 text-left">
            <!-- 商家信息 -->
            <div class="flex items-center space-x-3 pb-3 border-b text-left">
                <img :src="normalizeImageUrl(order.storeImage)" :alt="order.storeName"
                    class="w-12 h-12 rounded-lg object-cover" @error="handleImageError" />
                <div class="text-left">
                    <h3 class="font-bold text-lg text-left">{{ order.storeName }}</h3>
                    <p class="text-gray-600 text-sm text-left">订单号：{{ order.orderId }}</p>
                </div>
            </div>

            <!-- 订单状态 -->
            <div class="pb-3 border-b text-left">
                <div class="flex items-center text-left">
                    <span class="text-gray-600 w-24 text-left">订单状态：</span>
                    <span :class="{
                        'text-gray-500': order.orderState === 0,
                        'text-orange-500': order.deliveryStatus === 1 || order.deliveryStatus === 2,
                        'text-green-500': order.deliveryStatus === 3,
                    }" class="font-medium text-left flex-1">
                        {{ getOrderStatusText(order) }}
                    </span>
                </div>
                <div class="flex items-center mt-2 text-left">
                    <span class="text-gray-600 w-24 text-left">下单时间：</span>
                    <span class="text-gray-800 text-left flex-1">{{ order.paymentTime }}</span>
                </div>
            </div>

            <!-- 菜品清单 -->
            <div class="pb-3 border-b text-left">
                <h4 class="font-semibold text-gray-800 mb-2 text-left">菜品清单</h4>
                <div class="bg-gray-50 rounded-lg overflow-hidden">
                    <div class="max-h-96 overflow-y-auto">
                        <table class="w-full text-sm">
                            <thead class="bg-gray-100 sticky top-0">
                                <tr>
                                    <th class="px-4 py-3 text-left text-gray-700 font-medium">菜品名称</th>
                                    <th class="px-4 py-3 text-center text-gray-700 font-medium">数量</th>
                                    <th class="px-4 py-3 text-right text-gray-700 font-medium">单价</th>
                                    <th class="px-4 py-3 text-right text-gray-700 font-medium">小计</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(dish, idx) in order.dishDetails" :key="idx" 
                                    class="border-t border-gray-200 hover:bg-gray-100 transition-colors">
                                    <td class="px-4 py-3">
                                        <span class="font-medium text-gray-900">{{ dish.dishName }}</span>
                                    </td>
                                    <td class="px-4 py-3 text-center text-gray-700">{{ dish.quantity }}</td>
                                    <td class="px-4 py-3 text-right text-gray-600">¥{{ (dish.price || 0).toFixed(2) }}</td>
                                    <td class="px-4 py-3 text-right font-medium text-orange-600">¥{{ ((dish.price || 0) * dish.quantity).toFixed(2) }}</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <!-- 费用明细 -->
            <div class="pb-3 border-b text-left">
                <h4 class="font-semibold text-gray-800 mb-2 text-left">费用明细</h4>
                <div class="bg-gray-50 rounded-lg p-4 space-y-2 text-sm text-left">
                    <div class="flex items-center text-left">
                        <span class="text-gray-600 w-24 text-left">商品总额：</span>
                        <span class="text-gray-800 text-left flex-1">¥{{ order.totalAmount.toFixed(2) }}</span>
                    </div>
                    <div class="flex items-center text-left">
                        <span class="text-gray-600 w-24 text-left">配送费：</span>
                        <span class="text-gray-800 text-left flex-1">¥{{ (order.deliveryFee || 0).toFixed(2) }}</span>
                    </div>
                    <div v-if="order.usedCoupon" class="flex items-center text-left">
                        <span class="text-gray-600 w-24 text-left">优惠券：</span>
                        <span class="text-yellow-600 text-left flex-1">
                            {{ order.usedCoupon.couponName || '优惠券' }}
                            <span v-if="order.usedCoupon.discountType === 'fixed'">
                                -¥{{ order.usedCoupon.discountValue.toFixed(0) }}
                            </span>
                            <span v-else-if="order.usedCoupon.discountType === 'discount'">
                                {{ (order.usedCoupon.discountValue * 10).toFixed(1) }}折
                            </span>
                        </span>
                    </div>
                    <div class="flex items-center pt-2 border-t border-gray-200 text-left">
                        <span class="font-semibold text-gray-800 w-24 text-left">实付金额：</span>
                        <span class="font-bold text-lg text-orange-500 text-left flex-1">¥{{ getActualAmount(order).toFixed(2) }}</span>
                    </div>
                </div>
            </div>
        </div>

        <!-- 操作按钮 -->
        <div class="flex justify-between items-center mt-6 pt-4 border-t">
            <div class="flex gap-2">
                <!-- 售后：已有则查看，否则申请 -->
                <button
                    @click="hasAfterSale ? goUserAfterSale('afterSale') : handleAfterSale()"
                    class="px-4 py-2 rounded-lg bg-orange-500 text-white text-sm hover:bg-orange-600 transition-colors cursor-pointer flex items-center gap-1">
                    <i class="fas fa-headset"></i>
                    <span>{{ hasAfterSale ? '查看售后' : '申请售后' }}</span>
                </button>

                <!-- 举报：有 Pending 则查看，否则发起 -->
                <button
                    @click="hasPendingReport ? goUserAfterSale('report') : handleReport()"
                    class="px-4 py-2 rounded-lg bg-red-500 text-white text-sm hover:bg-red-600 transition-colors cursor-pointer flex items-center gap-1">
                    <i class="fas fa-exclamation-circle"></i>
                    <span>{{ hasPendingReport ? '查看举报' : '举报店铺' }}</span>
                </button>

                <!-- 评论：已有则查看，否则发布 -->
                <button
                    @click="hasComment ? goUserAfterSale('comment') : handleReview()"
                    class="px-4 py-2 rounded-lg bg-blue-500 text-white text-sm hover:bg-blue-600 transition-colors cursor-pointer flex items-center gap-1">
                    <i class="fas fa-star"></i>
                    <span>{{ hasComment ? '查看评论' : '发布评论' }}</span>
                </button>
            </div>
            <button
                class="px-4 py-2 rounded-lg border border-gray-300 text-gray-700 text-sm hover:bg-gray-100"
                @click="close">
                关闭
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, computed, ref, watch } from "vue";
import { useRouter } from 'vue-router';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import type { OrderInfo } from "@/api/user";
import { getMyAfterSales, getMyStoreReports, getMyComments } from '@/api/user/afterSale';

const props = defineProps<{
    visible: boolean;
    order: OrderInfo;
}>();

const emit = defineEmits(["close", "afterSale", "report", "review"]);

const router = useRouter();

// 状态标记：是否已有售后/评论/待处理举报
const hasAfterSale = ref(false);
const hasComment = ref(false);
const hasPendingReport = ref(false);

// 打开弹窗时查询当前订单/店铺的相关记录是否存在
watch(() => props.visible, async (v) => {
    if (!v) return;
    try {
        const [afs, reps, cms] = await Promise.all([
            getMyAfterSales(),
            getMyStoreReports(),
            getMyComments()
        ]);
        hasAfterSale.value = afs.some(a => a.orderId === props.order.orderId);
        hasComment.value = cms.some(c => c.orderId === props.order.orderId);
        hasPendingReport.value = reps.some(r => r.storeId === props.order.storeId && (r.status?.toLowerCase?.() === 'pending' || r.status === '待处理'));
    } catch {
        hasAfterSale.value = false;
        hasComment.value = false;
        hasPendingReport.value = false;
    }
}, { immediate: false });

function close() {
    emit("close");
}

function handleAfterSale() {
    emit("afterSale");
    close();
}

function handleReport() {
    emit("report");
    close();
}

function handleReview() {
    emit("review");
    close();
}

// 跳转用户售后中心（带上目标tab）
function goUserAfterSale(tab?: string) {
    router.push({ name: 'AfterSale', query: tab ? { tab } : undefined });
    close();
}

// 获取订单状态文本
const getOrderStatusText = (order: OrderInfo) => {
    if (order.orderState === 0) {
        return "未接单";
    }
    
    if (order.deliveryStatus === null || order.deliveryStatus === undefined) {
        return "已接单";
    }
    
    const deliveryStatusMap: Record<number, string> = {
        0: "已接单",
        1: "配送中",
        2: "配送中",
        3: "已完成",
    };
    
    return deliveryStatusMap[order.deliveryStatus] || "未知状态";
};

// 获取订单实际支付金额
const getActualAmount = (order: OrderInfo): number => {
    const subtotal = order.totalAmount;
    const deliveryFee = order.deliveryFee || 0;
    
    if (!order.usedCoupon) {
        return subtotal + deliveryFee;
    }
    
    let discountAmount = 0;
    const coupon = order.usedCoupon;
    
    if (coupon.discountType === 'fixed') {
        discountAmount = coupon.discountValue;
    } else if (coupon.discountType === 'discount') {
        discountAmount = subtotal * (1 - coupon.discountValue);
    }
    
    discountAmount = Math.min(discountAmount, subtotal);
    return Math.max(0, subtotal + deliveryFee - discountAmount);
};
</script>

