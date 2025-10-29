<template>
    <main class="pt-20 min-h-screen">
        <div class="max-w-screen-xl mx-auto px-6 py-8">
            <h1 class="text-3xl font-bold text-gray-800 mb-8 text-left">我的订单</h1>

            <!-- 订单状态标签 -->
            <div class="flex space-x-1 mb-8 bg-white rounded-lg p-2 shadow-sm">
                <button v-for="(status, index) in orderStatuses" :key="index" @click="activeOrderStatus = status.key"
                    :class="{
                        'bg-orange-500 text-white': activeOrderStatus === status.key,
                        'text-gray-600 hover:bg-gray-100': activeOrderStatus !== status.key
                    }"
                    class="px-6 py-2 rounded-lg font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap">
                    {{ status.label }}
                </button>
            </div>

            <!-- 加载中 -->
            <div v-if="showLoading" class="flex justify-center items-center h-64">
                <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
            </div>

            <!-- 订单列表 -->
            <div v-else class="space-y-4">
                <div v-for="order in filteredOrders" :key="order.orderID"
                    class="bg-white rounded-lg shadow-md p-6 text-left">

                    <!-- 顶部商家信息 -->
                    <div class="flex justify-between items-start mb-4">
                        <div class="flex items-center space-x-4">
                            <div class="w-16 h-16 rounded-lg bg-gray-100 flex items-center justify-center overflow-hidden">
                              <img :src="normalizeImageUrl(order.storeImage)" :alt="order.storeName"
                                  class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
                            </div>
                            <div>
                                <h3 class="font-bold text-lg">{{ order.storeName }}</h3>
                                <p class="text-gray-600 text-sm">下单时间：{{ order.paymentTime }}</p>
                            </div>
                        </div>
                        <span :class="{
                            'text-gray-500': order.orderStatus === 0,
                            'text-orange-500': order.deliveryStatus === 1 || order.deliveryStatus === 2,
                            'text-green-500': order.deliveryStatus === 3,
                        }" class="font-medium">
                            {{ getOrderStatusText(order) }}
                        </span>
                    </div>

                    <!-- 菜品展示 + 金额 + 操作按钮 -->
                    <div class="border-t pt-4 flex justify-between items-center">
                        <!-- 左边：菜品 -->
                        <div class="flex space-x-2 items-start">
                            <div v-for="(dish, idx) in order.dishDetails.slice(0, 8)" :key="idx" class="flex flex-col items-center">
                                <div class="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center overflow-hidden">
                                    <img :src="normalizeImageUrl(dish.dishImage)" :alt="dish.dishName"
                                        class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
                                </div>
                                <div class="w-12 mt-1 text-center">
                                    <p class="text-xs text-gray-700 truncate" :title="dish.dishName">{{ dish.dishName }}</p>
                                    <p v-if="dish.quantity > 1" class="text-xs text-gray-500">×{{ dish.quantity }}</p>
                                </div>
                            </div>
                            <!-- 超过 8 个时显示省略 -->
                            <div v-if="order.dishDetails.length > 8"
                                class="w-12 h-12 flex items-center justify-center rounded-lg bg-gray-100 text-gray-500 text-sm">
                                +{{ order.dishDetails.length - 8 }}
                            </div>
                        </div>

                        <!-- 右边：金额 + 操作按钮 -->
                        <div class="text-right">
                            <!-- 费用明细 -->
                            <div class="mb-2 text-xs text-gray-600 space-y-1">
                                <!-- 商品原始总价（不含优惠券折扣） -->
                                <div class="flex items-center justify-end">
                                    <span>商品：</span>
                                    <span class="ml-1">¥{{ order.totalAmount.toFixed(2) }}</span>
                                </div>
                                <!-- 配送费 -->
                                <div class="flex items-center justify-end">
                                    <span>配送费：</span>
                                    <span class="ml-1">¥{{ (order.deliveryFee || 0).toFixed(2) }}</span>
                                </div>
                                <!-- 优惠券信息 -->
                                <div v-if="order.usedCoupon" class="flex items-center justify-end">
                                    <span class="inline-flex items-center px-2 py-1 rounded bg-yellow-50 text-yellow-700 border border-yellow-200">
                                        <i class="fas fa-ticket-alt mr-1"></i>
                                        <span>{{ order.usedCoupon.couponName || '优惠券' }}</span>
                                        <span class="ml-1">
                                            <span v-if="order.usedCoupon.discountType === 'fixed'">
                                                -¥{{ order.usedCoupon.discountValue.toFixed(0) }}
                                            </span>
                                            <span v-else-if="order.usedCoupon.discountType === 'discount'">
                                                {{ (order.usedCoupon.discountValue * 10).toFixed(1) }}折
                                            </span>
                                        </span>
                                    </span>
                                </div>
                            </div>
                            <!-- 实付金额 -->
                            <p class="font-bold text-lg">¥{{ getActualAmount(order).toFixed(2) }}</p>

                            <!-- 未接单 -->
                            <div v-if="order.orderStatus === 0" class="flex justify-end gap-2 mt-2">
                                <button @click="dialogVisibleMerchant = true"
                                    class="bg-orange-500 hover:bg-orange-600 text-white w-8 h-8 rounded-full text-sm transition-colors cursor-pointer"
                                    title="联系商家">
                                    <i class="fas fa-store"></i>
                                </button>
                                <!-- 联系商家对话框 -->
                                <ReplyDialog v-model="dialogVisibleMerchant" title="联系商家" identity="user"
                                    :chatMessages="merchantChat" :quickPhrases="['您好，有什么能帮您？', '请稍等一下']"
                                    :emojis="['😊', '👍', '❤️', '🎉']" @submit="handleMerchantReply" />
                            </div>

                            <!-- 已接单（已接单但还未开始配送） -->
                            <div v-if="order.orderStatus !== 0 && (order.deliveryStatus === null || order.deliveryStatus === undefined || order.deliveryStatus === 0)" 
                                class="flex justify-end gap-2 mt-2">
                                <button @click="dialogVisibleMerchant = true"
                                    class="bg-orange-500 hover:bg-orange-600 text-white w-8 h-8 rounded-full text-sm transition-colors cursor-pointer"
                                    title="联系商家">
                                    <i class="fas fa-store"></i>
                                </button>
                                <!-- 联系商家对话框 -->
                                <ReplyDialog v-model="dialogVisibleMerchant" title="联系商家" identity="user"
                                    :chatMessages="merchantChat" :quickPhrases="['您好，有什么能帮您？', '请稍等一下']"
                                    :emojis="['😊', '👍', '❤️', '🎉']" @submit="handleMerchantReply" />
                            </div>

                            <!-- 配送中 -->
                            <div v-if="order.deliveryStatus === 1 || order.deliveryStatus === 2" class="flex justify-end gap-2 mt-2">
                                <button @click="dialogVisibleMerchant = true"
                                    class="bg-orange-500 hover:bg-orange-600 text-white w-8 h-8 rounded-full text-sm transition-colors cursor-pointer"
                                    title="联系商家">
                                    <i class="fas fa-store"></i>
                                </button>
                                <button @click="dialogVisibleRider = true"
                                    class="bg-orange-500 hover:bg-orange-600 text-white w-8 h-8 rounded-full text-sm transition-colors cursor-pointer"
                                    title="联系骑手">
                                    <i class="fas fa-motorcycle"></i>
                                </button>
                                <button @click="openRevealDelivery()"
                                    class="bg-blue-500 hover:bg-blue-600 text-white px-4 py-1 rounded text-sm transition-colors cursor-pointer whitespace-nowrap">
                                    查看物流
                                </button>

                                <!-- 联系商家 -->
                                <ReplyDialog v-model="dialogVisibleMerchant" title="联系商家" identity="user"
                                    :chatMessages="merchantChat" :quickPhrases="['您好，有什么能帮您？', '请稍等一下']"
                                    :emojis="['😊', '👍', '❤️', '🎉']" @submit="handleMerchantReply" />

                                <!-- 联系骑手 -->
                                <ReplyDialog v-model="dialogVisibleRider" title="联系骑手" identity="user"
                                    :chatMessages="riderChat" :quickPhrases="['请尽快送达哦', '麻烦放到门口，谢谢']"
                                    :emojis="['🚴', '🙏', '😁', '👌']" @submit="handleRiderReply" />

                                <!-- 显示物流弹窗 -->
                                <RevealDelivery :visible="showRevealDelivery"
                                    @close="showRevealDelivery = false" />
                            </div>

                            <!-- 已完成 -->
                            <div v-if="order.deliveryStatus === 3" class="flex justify-end gap-2 mt-2">
                                <!-- 售后按钮 -->
                                <button @click="openAfterSale(order.orderID)"
                                    class="relative w-8 h-8 flex items-center justify-center cursor-pointer"
                                    title="提起售后">
                                    <i class="fas fa-headset text-orange-500 hover:text-orange-600 text-2xl"></i>
                                </button>

                                <!-- 举报按钮 -->
                                <button @click="openReportWindow(order.orderID)"
                                    class="relative w-8 h-8 flex items-center justify-center cursor-pointer"
                                    title="对此订单有意见">
                                    <i
                                        class="fas fa-exclamation-circle text-orange-500 hover:text-orange-600 text-2xl"></i>
                                </button>

                                <!-- 评价按钮 -->
                                <button @click="openReviewWindow(order.orderID)"
                                    class="bg-orange-500 hover:bg-orange-600 text-white px-4 py-1 rounded text-sm transition-colors cursor-pointer whitespace-nowrap">
                                    评价
                                </button>

                                <!-- 弹窗们 -->
                                <AfterSaleWindow :visible="showAfterSale[order.orderID]" :order="order"
                                    @close="showAfterSale[order.orderID] = false" />
                                <ReportWindow :visible="showReportWindow[order.orderID]" :order="order"
                                    @close="showReportWindow[order.orderID] = false" />
                                <ReviewWindow :visible="showReviewWindow[order.orderID]" :order="order"
                                    @close="showReviewWindow[order.orderID] = false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </main>
</template>

<script lang="ts" setup>
import { ref, onMounted, computed } from "vue";
import { useUserStore } from "@/stores/user";
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils'

import type { OrderInfo } from "@/api/user";
import { getOrderInfo } from "@/api/user";

import ReportWindow from "@/components/user/HomePage/Home/ReportWindow.vue";
import ReviewWindow from "@/components/user/HomePage/Home/ReviewWindow.vue";
import AfterSaleWindow from "@/components/user/HomePage/Home/AfterSaleWindow.vue";
import RevealDelivery from "@/components/user/HomePage/Home/RevealDelivery.vue";
import ReplyDialog from "@/components/user/HomePage/Home/ReplyDialog.vue";

const userStore = useUserStore();
const userID = userStore.getUserID();

const orders = ref<OrderInfo[]>([]);
const activeOrderStatus = ref("all");
const showLoading = ref(true);
const showReviewWindow = ref<Record<number, boolean>>({});
const showReportWindow = ref<Record<number, boolean>>({});
const showAfterSale = ref<Record<number, boolean>>({});
const showRevealDelivery = ref(false);

const orderStatuses = [
    { key: "all", label: "全部订单" },
    { key: "unaccepted", label: "未接单" },
    { key: "accepted", label: "已接单" },
    { key: "delivering", label: "配送中" },
    { key: "completed", label: "已完成" },
];

onMounted(() => {
    fetchOrders();
});

const getOrderStatusText = (order: OrderInfo) => {
    // 未接单：订单状态为 Pending (0)
    if (order.orderStatus === 0) {
        return "未接单";
    }
    
    // 如果订单已接单，检查配送状态
    if (order.deliveryStatus === null || order.deliveryStatus === undefined) {
        return "已接单";
    }
    
    // 配送状态映射
    const deliveryStatusMap: Record<number, string> = {
        0: "已接单",      // To_Be_Taken: 待取件
        1: "配送中",      // Pending: 待取单
        2: "配送中",      // Delivering: 配送中
        3: "已完成",      // Completed: 已完成
    };
    
    return deliveryStatusMap[order.deliveryStatus] || "未知状态";
};

const fetchOrders = async () => {
    try {
        const res: OrderInfo[] = await getOrderInfo();
        orders.value = res;
        showLoading.value = false;
    } catch (err) {
        alert("获取订单失败");
        console.error("获取订单失败:", err);
    }
};

const filteredOrders = computed(() => {
    if (activeOrderStatus.value === "all") {
        return orders.value;
    }
    
    return orders.value.filter((order) => {
        switch (activeOrderStatus.value) {
            case "unaccepted":
                // 未接单：订单状态为 Pending (0)
                return order.orderStatus === 0;
            
            case "accepted":
                // 已接单：订单已接单（orderStatus !== 0）且（没有配送任务 或 配送状态是 To_Be_Taken=0）
                return order.orderStatus !== 0 && 
                       (order.deliveryStatus === null || 
                        order.deliveryStatus === undefined || 
                        order.deliveryStatus === 0);
            
            case "delivering":
                // 配送中：配送状态是 Pending=1 或 Delivering=2
                return order.deliveryStatus === 1 || order.deliveryStatus === 2;
            
            case "completed":
                // 已完成：配送状态是 Completed=3
                return order.deliveryStatus === 3;
            
            default:
                return true;
        }
    });
});

// 获取订单实际支付金额 = 原始商品总价 + 配送费 - 优惠券折扣
const getActualAmount = (order: OrderInfo): number => {
    const subtotal = order.totalAmount; // 原始商品总价
    const deliveryFee = order.deliveryFee || 0;
    
    // 如果没有优惠券，直接返回商品总价 + 配送费
    if (!order.usedCoupon) {
        return subtotal + deliveryFee;
    }
    
    // 计算优惠金额
    let discountAmount = 0;
    const coupon = order.usedCoupon;
    
    if (coupon.discountType === 'fixed') {
        // 满减券：discountValue 就是优惠金额
        discountAmount = coupon.discountValue;
    } else if (coupon.discountType === 'discount') {
        // 折扣券：discountValue 是折扣比例（0-1），计算优惠金额
        discountAmount = subtotal * (1 - coupon.discountValue);
    }
    
    // 确保优惠金额不超过商品总价
    discountAmount = Math.min(discountAmount, subtotal);
    
    // 实付金额 = 原始商品总价 + 配送费 - 优惠金额
    return Math.max(0, subtotal + deliveryFee - discountAmount);
};

function openReviewWindow(orderID: number) {
    showReviewWindow.value[orderID] = true;
}
function openReportWindow(orderID: number) {
    showReportWindow.value[orderID] = true;
}
function openAfterSale(orderID: number) {
    showAfterSale.value[orderID] = true;
}
function openRevealDelivery() {
    showRevealDelivery.value = true;
}

const dialogVisibleMerchant = ref(false);
const dialogVisibleRider = ref(false);

const merchantChat = ref([
    { sender: "user", content: "你好，有优惠吗？", time: "14:00" },
    { sender: "merchant", content: "有的，满50减10", time: "14:01" },
]);

const riderChat = ref([
    { sender: "user", content: "请放门口，谢谢", time: "14:02" },
    { sender: "rider", content: "好的，马上到", time: "14:03" },
]);

function handleMerchantReply(content: string) {
    merchantChat.value.push({
        sender: "user",
        content,
        time: new Date().toLocaleTimeString(),
    });
}
function handleRiderReply(content: string) {
    riderChat.value.push({
        sender: "user",
        content,
        time: new Date().toLocaleTimeString(),
    });
}
</script>
