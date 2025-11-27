<template>
    <div class="min-h-screen bg-gray-100 pt-20 pb-12">
        <main class="orders-layout max-w-6xl mx-auto px-4">
            <section class="orders-main space-y-6">
            <h1 class="text-xl font-bold text-gray-900 mb-4 text-center">我的订单</h1>

            <!-- 订单状态标签 -->
            <div class="flex overflow-x-auto space-x-2 mb-6 scrollbar-hide sticky top-20 z-10 py-2">
                <button v-for="(status, index) in orderStatuses" :key="index" @click="activeOrderStatus = status.key"
                    :class="{
                        'bg-orange-500 text-white font-bold shadow-md transform scale-105': activeOrderStatus === status.key,
                        'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50': activeOrderStatus !== status.key
                    }"
                    class="flex-1 px-4 py-2.5 rounded-full text-sm transition-all duration-200 whitespace-nowrap text-center min-w-[100px]">
                    {{ status.label }}
                </button>
            </div>

            <!-- 加载中 -->
            <div v-if="showLoading" class="flex justify-center items-center h-64">
                <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
            </div>

            <!-- 订单列表 -->
            <div v-else>
                <div
                    v-if="paginatedOrders.length === 0"
                    class="bg-white rounded-2xl border border-dashed border-gray-200 shadow-sm py-14 px-6 text-center text-gray-500"
                >
                    <i class="fas fa-clipboard-list text-4xl text-orange-400 mb-3"></i>
                    <p class="text-base font-semibold text-gray-800">暂时没有相关订单</p>
                    <p class="text-sm text-gray-500 mt-1">可以去首页挑选喜欢的美食再回来查看哦～</p>
                </div>

                <div v-else class="space-y-6">
                <div v-for="order in paginatedOrders" :key="order.orderId"
                    class="bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm hover:shadow-md">

                    <!-- 顶部商家信息 -->
                    <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
                        <div class="flex items-center space-x-3">
                            <div class="w-10 h-10 rounded-md bg-gray-50 flex items-center justify-center overflow-hidden border border-gray-100">
                              <img :src="normalizeImageUrl(order.storeImage)" :alt="order.storeName"
                                  class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
                            </div>
                            <div>
                                <h3 class="font-bold text-base text-gray-900">{{ order.storeName }}</h3>
                                <p class="text-xs text-gray-500 mt-0.5">订单号：<span class="font-mono">{{ order.orderId }}</span></p>
                                <p class="text-xs text-gray-500">下单时间：{{ order.paymentTime }}</p>
                            </div>
                        </div>
                        <span :class="{
                            'text-gray-500': order.orderState === 0,
                            'text-yellow-600': (order.orderState !== 0 && (order.deliveryStatus === null || order.deliveryStatus === undefined || order.deliveryStatus === 0)),
                            'text-orange-600': order.deliveryStatus === 1 || order.deliveryStatus === 2,
                            'text-green-600': order.deliveryStatus === 3,
                        }" class="px-2 py-1 rounded text-xs font-medium ml-2 whitespace-nowrap bg-gray-50">
                            {{ getOrderStatusText(order) }}
                        </span>
                    </div>

                    <!-- 菜品展示 + 金额 + 操作按钮 -->
                    <div class="flex flex-col gap-3">
                        <div class="flex justify-between items-start">
                        <!-- 左边：菜品 -->
                            <div class="flex gap-2 overflow-x-auto scrollbar-hide pb-1 flex-1 min-w-0">
                                <div v-for="(dish, idx) in order.dishDetails.slice(0, 8)" :key="idx" class="flex flex-col items-center min-w-[4.5rem]">
                                    <div class="relative w-16 h-16 rounded-lg bg-gray-50 flex items-center justify-center overflow-hidden border border-gray-100">
                                    <img :src="normalizeImageUrl(dish.dishImage)" :alt="dish.dishName"
                                            class="w-full h-full object-cover" @error="handleImageError" />
                                        <span v-if="dish.quantity > 1" class="absolute top-0 right-0 bg-red-500 text-white text-[10px] px-1 rounded-bl-lg font-bold">x{{ dish.quantity }}</span>
                                </div>
                                    <div class="w-16 mt-1.5 text-center">
                                        <p class="text-xs text-gray-800 truncate w-full font-medium" :title="dish.dishName">{{ dish.dishName }}</p>
                                        <p class="text-[10px] text-gray-500 mt-0.5 font-mono">
                                            ¥{{ Number.isInteger(dish.price) ? dish.price : dish.price.toFixed(2) }}
                                        </p>
                                </div>
                            </div>
                            <!-- 超过 8 个时显示省略 -->
                            <div v-if="order.dishDetails.length > 8"
                                    class="w-16 h-16 flex flex-col items-center justify-center rounded-lg bg-gray-50 text-gray-500 text-xs border border-gray-100 min-w-[4.5rem] cursor-pointer hover:bg-gray-100 transition-colors"
                                    @click="openOrderDetail(order.orderId)">
                                    <span class="text-lg font-bold">+{{ order.dishDetails.length - 8 }}</span>
                                    <span class="text-[10px]">查看更多</span>
                                </div>
                            </div>
                            <!-- 右边：共X件 -->
                            <div class="ml-3 flex h-16 items-center shrink-0">
                                <span class="text-xs text-gray-500 bg-gray-50 px-2 py-1 rounded">共 {{ order.dishDetails.reduce((acc, d) => acc + d.quantity, 0) }} 件</span>
                            </div>
                        </div>

                        <!-- 右边：金额 + 操作按钮 -->
                        <div class="flex flex-col items-end border-t border-gray-50 pt-3">
                            <!-- 费用明细 -->
                            <div class="w-full mb-3 text-xs text-gray-500 flex flex-col gap-1 items-end">
                                <!-- 商品原始总价（不含优惠券折扣） -->
                                <div class="flex items-center justify-end w-full">
                                    <span>商品</span>
                                    <span class="ml-2 text-gray-900">¥{{ Number.isInteger(order.totalAmount) ? order.totalAmount : order.totalAmount.toFixed(2) }}</span>
                                </div>
                                <!-- 配送费 -->
                                <div class="flex items-center justify-end w-full">
                                    <span>配送费</span>
                                    <span class="ml-2 text-gray-900">¥{{ Number.isInteger(order.deliveryFee || 0) ? (order.deliveryFee || 0) : (order.deliveryFee || 0).toFixed(2) }}</span>
                                </div>
                                <!-- 优惠券信息 -->
                                <div v-if="order.usedCoupon" class="flex items-center justify-end w-full mt-1">
                                    <span class="inline-flex items-center px-1.5 py-0.5 rounded bg-yellow-50 text-yellow-700 border border-yellow-200 text-[10px]">
                                        <i class="fas fa-ticket-alt mr-1 text-[9px]"></i>
                                        <span>{{ order.usedCoupon.couponName || '优惠券' }}</span>
                                        <span class="ml-1 font-bold">
                                            <span v-if="order.usedCoupon.discountType === 'fixed'">
                                                -¥{{ Number.isInteger(order.usedCoupon.discountValue) ? order.usedCoupon.discountValue : order.usedCoupon.discountValue.toFixed(2) }}
                                            </span>
                                            <span v-else-if="order.usedCoupon.discountType === 'discount'">
                                                {{ (order.usedCoupon.discountValue * 10).toFixed(1) }}折
                                            </span>
                                        </span>
                                    </span>
                                </div>
                                <!-- 实付金额 -->
                                <div class="flex items-center justify-end w-full mt-2 pt-2 border-t border-dashed border-gray-100">
                                    <span class="text-gray-900 font-medium flex items-baseline">
                                        <span class="text-xs mr-1">实付</span>
                                        <span class="text-lg font-bold">¥{{ Number.isInteger(getActualAmount(order)) ? getActualAmount(order) : getActualAmount(order).toFixed(2) }}</span>
                                    </span>
                                </div>
                            </div>

                            <!-- 统一的操作按钮区域 -->
                            <div class="flex justify-end gap-2 flex-wrap">
                                <!-- 联系商家按钮 -->
                                <button @click="openMerchantDialog(order.orderId)"
                                    class="bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 px-3 py-1.5 rounded-full text-xs transition-colors cursor-pointer whitespace-nowrap flex items-center gap-1 shadow-sm hover:shadow">
                                    <i class="fas fa-store"></i>
                                    <span>联系商家</span>
                                </button>

                                <!-- 订单售后按钮 -->
                                <button 
                                    @click="openOrderDetail(order.orderId)"
                                    :disabled="order.deliveryStatus !== 3"
                                    :class="[
                                        order.deliveryStatus === 3
                                            ? 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                                            : 'bg-gray-50 border border-gray-200 text-gray-400 cursor-not-allowed'
                                    ]"
                                    class="px-3 py-1.5 rounded-full text-xs transition-colors whitespace-nowrap flex items-center gap-1">
                                    <span>售后服务</span>
                                </button>

                                <!-- 查看配送信息按钮 -->
                                <button 
                                    @click="openDeliveryInfo(order.orderId)"
                                    :disabled="order.deliveryStatus === null || order.deliveryStatus === undefined || order.deliveryStatus === 0"
                                    :class="[
                                        (order.deliveryStatus !== null && order.deliveryStatus !== undefined && order.deliveryStatus !== 0)
                                            ? 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                                            : 'bg-gray-50 border border-gray-200 text-gray-400 cursor-not-allowed'
                                    ]"
                                    class="px-3 py-1.5 rounded-full text-xs transition-colors whitespace-nowrap flex items-center gap-1">
                                    <span>配送信息</span>
                                </button>

                                <!-- 再来一单按钮 -->
                                <button @click="reorder(order)"
                                    class="bg-orange-500 hover:bg-orange-600 text-white px-3 py-1.5 rounded-full text-xs font-medium transition-colors cursor-pointer whitespace-nowrap flex items-center gap-1 shadow-sm hover:shadow">
                                    <span>再来一单</span>
                                </button>
                            </div>

                            <!-- 联系商家对话框（每个订单独立） -->
                            <ReplyDialog 
                                :model-value="!!dialogVisibleMerchant[order.orderId]"
                                @update:model-value="dialogVisibleMerchant[order.orderId] = $event"
                                title="联系商家" 
                                identity="user"
                                :chatMessages="merchantChat" 
                                :quickPhrases="['您好，有什么能帮您？', '请稍等一下']"
                                :emojis="['😊', '👍', '❤️', '🎉']" 
                                @submit="handleMerchantReply" />

                            <!-- 联系骑手对话框（配送中时显示） -->
                            <ReplyDialog 
                                v-if="order.deliveryStatus === 1 || order.deliveryStatus === 2"
                                :model-value="!!dialogVisibleRider[order.orderId]"
                                @update:model-value="dialogVisibleRider[order.orderId] = $event"
                                title="联系骑手" 
                                identity="user"
                                :chatMessages="riderChat" 
                                :quickPhrases="['请尽快送达哦', '麻烦放到门口，谢谢']"
                                :emojis="['🚴', '🙏', '😁', '👌']" 
                                @submit="handleRiderReply" />

                            <!-- 订单详情弹窗 -->
                            <OrderDetailWindow 
                                :visible="!!showOrderDetail[order.orderId]" 
                                :order="order"
                                @close="showOrderDetail[order.orderId] = false"
                                @afterSale="() => openAfterSale(order.orderId)"
                                @report="() => openReportWindow(order.orderId)"
                                @review="() => openReviewWindow(order.orderId)" />

                            <!-- 配送信息弹窗 -->
                            <RevealDelivery 
                                :visible="!!showRevealDelivery[order.orderId]"
                                :order="order"
                                @close="showRevealDelivery[order.orderId] = false" />

                            <!-- 其他弹窗（已完成订单时显示） -->
                            <AfterSaleWindow 
                                v-if="order.deliveryStatus === 3"
                                :visible="!!showAfterSale[order.orderId]" 
                                :order="order"
                                @close="showAfterSale[order.orderId] = false" />
                            <ReportWindow 
                                v-if="order.deliveryStatus === 3"
                                :visible="!!showReportWindow[order.orderId]" 
                                :order="order"
                                @close="showReportWindow[order.orderId] = false" />
                            <ReviewWindow 
                                v-if="order.deliveryStatus === 3"
                                :visible="!!showReviewWindow[order.orderId]" 
                                :order="order"
                                @close="showReviewWindow[order.orderId] = false" />
                        </div>
                    </div>
                </div>
                
                <!-- 分页控制 -->
                <div v-if="totalPages > 1" class="flex justify-center items-center gap-2 mt-8 mb-4">
                    <button 
                        @click="currentPage--"
                        :disabled="currentPage === 1"
                        :class="[
                            currentPage === 1
                                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                                : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                        ]"
                        class="px-3 py-1.5 rounded-full text-xs transition-colors">
                        <i class="fas fa-chevron-left"></i>
                    </button>
                    
                    <div class="flex gap-1">
                        <button 
                            v-for="page in totalPages" 
                            :key="page"
                            @click="currentPage = page"
                            :class="[
                                currentPage === page
                                    ? 'bg-orange-500 text-white font-bold shadow-sm'
                                    : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                            ]"
                            class="w-8 h-8 rounded-full text-xs transition-colors cursor-pointer flex items-center justify-center">
                            {{ page }}
                        </button>
                    </div>
                    
                    <button 
                        @click="currentPage++"
                        :disabled="currentPage === totalPages"
                        :class="[
                            currentPage === totalPages
                                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                                : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                        ]"
                        class="px-3 py-1.5 rounded-full text-xs transition-colors">
                        <i class="fas fa-chevron-right"></i>
                    </button>
                </div>
                </div>
            </div>
            </section>

            <aside class="orders-aside space-y-4">
                <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
                    <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i class="fas fa-clipboard-check text-orange-500"></i>
                        最近订单概况
                    </h3>
                    <div class="space-y-3 text-xs text-gray-600">
                        <div class="flex items-center justify-between">
                            <span>全部订单</span>
                            <span class="font-semibold text-gray-900">{{ orders.length }}</span>
                        </div>
                        <div class="flex items-center justify-between">
                            <span>本月完成</span>
                            <span class="font-semibold text-gray-900">{{ completedCount }}</span>
                        </div>
                        <div class="flex items-center justify-between">
                            <span>待配送</span>
                            <span class="font-semibold text-gray-900">{{ pendingDeliveryCount }}</span>
                        </div>
                    </div>
                </div>

                <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
                    <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i class="fas fa-lightbulb text-yellow-500"></i>
                        温馨提示
                    </h3>
                    <ul class="space-y-2 text-xs text-gray-600 list-disc pl-4 text-left">
                        <li>订单完成后，若对菜品不满意，请及时进行售后申请，会由商家反馈和客服审核处理</li>
                        <li>配送信息可实时查看骑手位置，可通过平台及时与骑手联系</li>
                        <li>若对配送不满意，可对骑手进行配送投诉，每个配送任务只能进行一次投诉</li>
                        <li>若发现商家存在违规行为，请及时对店铺举报</li>
                    </ul>
                </div>

                <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
                    <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center justify-between">
                        <span class="flex items-center gap-2">
                            <i class="fas fa-brain text-purple-500"></i>
                            智能分析
                        </span>
                        <span class="text-xs text-gray-400">本月</span>
                    </h3>
                    <div class="space-y-3 text-xs text-gray-600">
                        <div class="flex items-center justify-between">
                            <span>本月总花销</span>
                            <span class="font-semibold text-gray-900">¥{{ monthlyExpense }}</span>
                        </div>
                        <div class="flex items-center justify-between">
                            <span>已领优惠</span>
                            <span class="font-semibold text-green-600">-¥{{ monthlyDiscount }}</span>
                        </div>
                        <div class="pt-2 border-t border-dashed border-gray-100">
                            <p class="text-[11px] text-gray-400 mb-1">最常下单店铺</p>
                            <p class="text-sm font-semibold text-gray-900">{{ favoriteStore?.name || '暂无数据' }}</p>
                            <p class="text-xs text-gray-500">本月下单 {{ favoriteStore?.count || 0 }} 次</p>
                        </div>
                        <div class="pt-2 border-t border-dashed border-gray-100">
                            <p class="text-[11px] text-gray-400 mb-1">常点菜品</p>
                            <p class="text-sm font-semibold text-gray-900">{{ favoriteDish?.name || '暂无数据' }}</p>
                            <p class="text-xs text-gray-500">本月点单 {{ favoriteDish?.count || 0 }} 次</p>
                </div>
            </div>
        </div>
            </aside>
    </main>
    </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, computed, watch } from "vue";
import { useRouter } from "vue-router";
import { useUserStore } from "@/stores/user";
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils'
import { ElMessage } from 'element-plus';

import type { OrderInfo } from "@/api/user";
import { getOrderInfo } from "@/api/user";
import { getMyDeliveryComplaints, type DeliveryComplaintListItem } from '@/api/user/afterSale';

import ReportWindow from "@/components/user/HomePage/Home/ReportWindow.vue";
import ReviewWindow from "@/components/user/HomePage/Home/ReviewWindow.vue";
import AfterSaleWindow from "@/components/user/HomePage/Home/AfterSaleWindow.vue";
import RevealDelivery from "@/components/user/HomePage/Home/RevealDelivery.vue";
import ReplyDialog from "@/components/user/HomePage/Home/ReplyDialog.vue";
import OrderDetailWindow from "@/components/user/HomePage/Home/OrderDetailWindow.vue";

const router = useRouter();
const userStore = useUserStore();
const userID = userStore.getUserID();

const orders = ref<OrderInfo[]>([]);
const myComplaints = ref<DeliveryComplaintListItem[]>([]);
const activeOrderStatus = ref("all");
const showLoading = ref(true);
const showReviewWindow = ref<Record<number, boolean>>({});
const showReportWindow = ref<Record<number, boolean>>({});
const showAfterSale = ref<Record<number, boolean>>({});
const showRevealDelivery = ref<Record<number, boolean>>({});
const showOrderDetail = ref<Record<number, boolean>>({});
const dialogVisibleMerchant = ref<Record<number, boolean>>({});
const dialogVisibleRider = ref<Record<number, boolean>>({});

// 分页相关
const currentPage = ref(1);
const itemsPerPage = ref(5);

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
    if (order.orderState === 0) {
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
        const [res, complaints] = await Promise.all([
            getOrderInfo(),
            getMyDeliveryComplaints()
        ]);
        orders.value = res as OrderInfo[];
        myComplaints.value = complaints || [];
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
                return order.orderState === 0;
            
            case "accepted":
                // 已接单：订单已接单（orderStatus !== 0）且（没有配送任务 或 配送状态是 To_Be_Taken=0）
                return order.orderState !== 0 && 
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

const completedCount = computed(() => orders.value.filter(order => order.deliveryStatus === 3).length);
const pendingDeliveryCount = computed(() => orders.value.filter(order => order.deliveryStatus === 1 || order.deliveryStatus === 2).length);

const monthlyExpense = computed(() => {
    const now = new Date();
    return orders.value
        .filter(order => {
            const date = new Date(order.paymentTime);
            return date.getFullYear() === now.getFullYear() && date.getMonth() === now.getMonth();
        })
        .reduce((sum, order) => sum + getActualAmount(order), 0)
        .toFixed(2);
});

const monthlyDiscount = computed(() => {
    const now = new Date();
    return orders.value
        .filter(order => {
            const date = new Date(order.paymentTime);
            return date.getFullYear() === now.getFullYear() && date.getMonth() === now.getMonth();
        })
        .reduce((sum, order) => {
            if (!order.usedCoupon) return sum;
            const coupon = order.usedCoupon;
            if (coupon.discountType === 'fixed') {
                return sum + coupon.discountValue;
            }
            if (coupon.discountType === 'discount') {
                return sum + order.totalAmount * (1 - coupon.discountValue);
            }
            return sum;
        }, 0)
        .toFixed(2);
});

const favoriteStore = computed(() => {
    if (!orders.value.length) return null;
    const counts: Record<string, { name: string; count: number }> = {};
    orders.value.forEach(order => {
        if (!counts[order.storeId]) {
            counts[order.storeId] = { name: order.storeName, count: 0 };
        }
        counts[order.storeId].count += 1;
    });
    return Object.values(counts).sort((a, b) => b.count - a.count)[0];
});

const favoriteDish = computed(() => {
    if (!orders.value.length) return null;
    const counts: Record<string, { name: string; count: number }> = {};
    orders.value.forEach(order => {
        order.dishDetails.forEach(dish => {
            if (!counts[dish.dishName]) {
                counts[dish.dishName] = { name: dish.dishName, count: 0 };
            }
            counts[dish.dishName].count += dish.quantity;
        });
    });
    return Object.values(counts).sort((a, b) => b.count - a.count)[0];
});

// 分页后的订单列表
const paginatedOrders = computed(() => {
    const start = (currentPage.value - 1) * itemsPerPage.value;
    const end = start + itemsPerPage.value;
    return filteredOrders.value.slice(start, end);
});

// 总页数
const totalPages = computed(() => {
    return Math.ceil(filteredOrders.value.length / itemsPerPage.value);
});

// 监听筛选状态变化，重置到第一页
watch(activeOrderStatus, () => {
    currentPage.value = 1;
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
function hasDeliveryComplaint(order: OrderInfo): boolean {
    return myComplaints.value.some(c => c.orderId === order.orderId);
}
function goUserAfterSale(tab?: string) {
    router.push({ name: 'AfterSale', query: tab ? { tab } : undefined });
}
function openDeliveryInfo(orderID: number) {
    showRevealDelivery.value[orderID] = true;
}
function openOrderDetail(orderID: number) {
    showOrderDetail.value[orderID] = true;
}
function openMerchantDialog(orderID: number) {
    dialogVisibleMerchant.value[orderID] = true;
}
function openRiderDialog(orderID: number) {
    dialogVisibleRider.value[orderID] = true;
}

// 再来一单：跳转到店铺页面
function reorder(order: OrderInfo) {
    router.push({ 
        name: 'InStore', 
        params: { id: order.storeId.toString() } 
    });
    ElMessage.success('正在跳转到店铺页面...');
}

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

<style scoped>
/* Hide scrollbar for Chrome, Safari and Opera */
.scrollbar-hide::-webkit-scrollbar {
    display: none;
}

/* Hide scrollbar for IE, Edge and Firefox */
.scrollbar-hide {
    -ms-overflow-style: none;  /* IE and Edge */
    scrollbar-width: none;  /* Firefox */
}

.orders-layout {
    display: flex;
    flex-direction: column;
    gap: 24px;
}

.orders-main,
.orders-aside {
    width: 100%;
}

@media (min-width: 1024px) {
    .orders-layout {
        flex-direction: row;
        gap: 28px;
        align-items: flex-start;
    }

    .orders-main {
        flex: 0 0 820px;
        max-width: 820px;
    }

    .orders-aside {
        flex: 0 0 300px;
        max-width: 300px;
        position: sticky;
        top: 120px;
    }
}
</style>
