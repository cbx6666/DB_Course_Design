<template>
  <div
    v-if="visible"
    class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50"
    @click.self="close"
  >
    <div
      class="bg-white rounded-2xl w-[600px] max-h-[80vh] flex flex-col overflow-hidden shadow-2xl border border-gray-100 transform transition-all duration-300 scale-100"
    >
      <!-- 头部 -->
      <div
        class="flex items-start justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-blue-50 to-purple-50"
      >
        <div>
          <div class="text-xl font-bold text-gray-900 text-left">配送信息</div>
          <div class="text-sm text-blue-600 font-medium mt-3 ml-4 text-left">
            配送任务ID: {{ deliveryInfo?.taskId || deliveryInfo?.TaskId || '-' }}
          </div>
        </div>
        <button @click="close" class="p-2 rounded-lg hover:bg-gray-100 transition-colors text-gray-400 hover:text-gray-600 ml-4">
          <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
          </svg>
        </button>
      </div>

      <!-- 内容区域 -->
      <div class="flex-1 p-6 overflow-y-auto">
        <!-- 加载中 -->
        <div v-if="loading" class="text-center text-gray-500 py-8">
          <i class="fas fa-spinner fa-spin text-2xl mb-2"></i>
          <div>正在加载配送信息...</div>
        </div>

        <!-- 错误提示 -->
        <div v-else-if="error" class="text-center text-red-500 py-8">
          <i class="fas fa-exclamation-circle text-2xl mb-2"></i>
          <div>{{ error }}</div>
        </div>

        <!-- 配送信息内容 -->
        <div v-else>
          <!-- 配送状态 -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-3 text-left">配送状态</h3>
            <div class="bg-gray-50 rounded-lg p-4 text-left">
              <span
                :class="[
                  deliveryStatusMap[String(deliveryInfo?.status ?? -1)]?.colorClass || 'bg-gray-100 text-gray-600',
                  'px-4 py-2 rounded-full text-sm font-medium inline-block'
                ]"
              >
                {{ deliveryStatusMap[String(deliveryInfo?.status ?? -1)]?.label || '未知状态' }}
              </span>
            </div>
            <!-- 配送打分（仅已完成时显示） -->
            <div v-if="deliveryInfo?.status === 3" class="mt-3 text-left">
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 text-left">配送打分：</span>
                <span v-if="deliveryInfo?.taskRating" class="text-left flex items-center">
                  <span class="text-yellow-400">
                    <i v-for="i in 5" :key="i" :class="i <= deliveryInfo.taskRating ? 'fas fa-star' : 'far fa-star text-gray-300'"></i>
                  </span>
                </span>
                <span v-else class="text-gray-500">未打分</span>
              </div>
            </div>
          </div>

          <!-- 收货信息 -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-3 text-left">收货信息</h3>
            <div class="bg-gray-50 rounded-lg p-4 space-y-2 text-left">
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">收货人：</span>
                <span class="text-left flex-1">{{ deliveryInfo?.order?.deliveryName || deliveryInfo?.order?.DeliveryName || '未提供' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">联系电话：</span>
                <span class="text-left flex-1">{{ deliveryInfo?.order?.deliveryPhone || deliveryInfo?.order?.DeliveryPhone || '-' }}</span>
              </div>
              <div class="flex items-start text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">收货地址：</span>
                <span class="flex-1 text-left">{{ deliveryInfo?.order?.deliveryAddress || deliveryInfo?.order?.DeliveryAddress || '未提供' }}</span>
              </div>
            </div>
          </div>

          <!-- 骑手信息 -->
          <div>
            <h3 class="text-lg font-semibold text-gray-900 mb-3 text-left">骑手信息</h3>
            <div v-if="deliveryInfo?.courier" class="bg-gray-50 rounded-lg p-4 space-y-3 text-left">
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">姓名：</span>
                <span class="text-left flex-1">{{ deliveryInfo.courier.fullName || '未知' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">电话：</span>
                <span class="text-left flex-1">{{ deliveryInfo.courier.phoneNumber || '-' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">交通工具：</span>
                <span class="text-left flex-1">{{ getVehicleTypeLabel(deliveryInfo.courier.vehicleType) }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2 w-24 text-left">评分：</span>
                <span class="text-left flex-1">{{ deliveryInfo.courier.averageRating && deliveryInfo.courier.averageRating > 0 ? deliveryInfo.courier.averageRating.toFixed(1) + ' 分' : '暂未获得评价' }}</span>
              </div>
              
              <!-- 配送时间信息 -->
              <div class="border-t border-gray-200 pt-3 space-y-2">
                <div class="flex items-center text-gray-700">
                  <span class="font-medium mr-2 w-32 text-left">骑手接单时间：</span>
                  <span class="flex-1">{{ deliveryInfo.acceptTime || '未接单' }}</span>
                </div>
                <div class="flex items-center text-gray-700">
                  <span class="font-medium mr-2 w-32 text-left">预计到店时间：</span>
                  <span class="flex-1">{{ deliveryInfo.estimatedArrivalTime || '-' }}</span>
                </div>
                <div v-if="deliveryInfo.actualPickupTime" class="flex items-center">
                  <span class="font-medium mr-2 w-32 text-left text-gray-700">实际到店时间：</span>
                  <span class="text-green-600 font-medium flex-1">{{ deliveryInfo.actualPickupTime }}</span>
                </div>
                <div class="flex items-center text-gray-700">
                  <span class="font-medium mr-2 w-32 text-left">预计送达时间：</span>
                  <span class="flex-1">{{ deliveryInfo.estimatedDeliveryTime || '-' }}</span>
                </div>
                <div v-if="deliveryInfo.actualDeliveryTime" class="flex items-center">
                  <span class="font-medium mr-2 w-32 text-left text-gray-700">实际送达时间：</span>
                  <span class="text-green-600 font-medium flex-1">{{ deliveryInfo.actualDeliveryTime }}</span>
                </div>
              </div>
              
              <!-- 骑手位置地图 -->
              <div v-if="deliveryInfo.status !== 3 && deliveryInfo.courier.longitude && deliveryInfo.courier.latitude" class="border-t border-gray-200 pt-3">
                <div class="mb-2">
                  <span class="font-medium text-gray-700">实时位置：</span>
                </div>
                <div class="w-full h-64 rounded-lg overflow-hidden">
                  <iframe
                    class="w-full h-full"
                    frameborder="0"
                    style="border:0"
                    :src="getCourierMapUrl(deliveryInfo.courier.latitude, deliveryInfo.courier.longitude)"
                    allowfullscreen="true"
                  ></iframe>
                </div>
              </div>
              <div v-else-if="deliveryInfo.status === 3" class="border-t border-gray-200 pt-3">
                <div class="text-gray-500 text-sm text-center py-2">
                  配送已完成，不再显示位置信息
                </div>
              </div>
              <div v-else class="border-t border-gray-200 pt-3">
                <div class="text-gray-400 text-sm text-center py-2">
                  位置信息未提供
                </div>
              </div>
            </div>
            <div v-else class="bg-gray-50 rounded-lg p-4 text-center">
              <div class="text-gray-500 flex items-center justify-center">
                <svg class="w-8 h-8 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"></path>
                </svg>
                <div>
                  <div class="font-medium">暂无骑手接单</div>
                  <div class="text-sm mt-1">请等待骑手接单</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 底部按钮 -->
      <div class="p-4 border-t border-gray-200">
        <!-- 左下方三个按钮 -->
        <div v-if="deliveryInfo?.courier" class="flex gap-3 mb-4">
          <button 
            @click="openContactRider"
            class="px-4 py-2 bg-blue-500 hover:bg-blue-600 text-white rounded-lg text-sm transition-colors cursor-pointer flex items-center gap-2">
            <i class="fas fa-comments"></i>
            <span>联系骑手</span>
          </button>
          <button 
            @click="openRateCourier"
            :disabled="hasRatedCourier"
            :class="{
              'bg-gray-300 cursor-not-allowed': hasRatedCourier,
              'bg-yellow-500 hover:bg-yellow-600 cursor-pointer': !hasRatedCourier
            }"
            class="px-4 py-2 text-white rounded-lg text-sm transition-colors flex items-center gap-2">
            <i class="fas fa-star"></i>
            <span>骑手打分</span>
          </button>
          <button 
            @click="openDeliveryComplaint"
            class="px-4 py-2 bg-red-500 hover:bg-red-600 text-white rounded-lg text-sm transition-colors cursor-pointer flex items-center gap-2">
            <i class="fas fa-exclamation-triangle"></i>
            <span>配送投诉</span>
          </button>
        </div>
        <!-- 右侧关闭按钮 -->
        <div class="flex justify-end">
          <button @click="close" class="px-4 py-2 rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 transition-colors text-sm font-medium">关闭</button>
        </div>
      </div>
    </div>

    <!-- 联系骑手对话框 -->
    <ReplyDialog 
      :model-value="showContactRider"
      @update:model-value="showContactRider = $event"
      title="联系骑手" 
      identity="user"
      :chat-messages="riderChatMessages" 
      :quick-phrases="['您好，请问配送进度如何？', '请稍等一下']"
      :emojis="['😊', '👍', '❤️', '🎉']" 
      @submit="handleRiderReply" />

    <!-- 骑手打分对话框 -->
    <CourierRatingWindow
      v-if="deliveryInfo?.courier"
      :visible="showRateCourier"
      :courier-id="deliveryInfo.courier.userId || deliveryInfo.courier.userID"
      :order-id="props.order?.orderId"
      :task-id="deliveryInfo?.taskId || deliveryInfo?.TaskId"
      @close="showRateCourier = false"
      @rated="handleCourierRated" />

    <!-- 配送投诉对话框 -->
    <DeliveryComplaintWindow
      v-if="deliveryInfo?.courier"
      :visible="showDeliveryComplaint"
      :order-id="props.order?.orderId"
      :task-id="deliveryInfo?.taskId || deliveryInfo?.TaskId"
      :courier-id="deliveryInfo.courier.userId || deliveryInfo.courier.userID"
      @close="showDeliveryComplaint = false"
      @submitted="handleComplaintSubmitted" />
  </div>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits, computed } from "vue";
import type { OrderInfo } from "@/api/user";
import { getOrderDeliveryInfo } from "@/api/user/home";
import { ElMessage } from "element-plus";
import ReplyDialog from "./ReplyDialog.vue";
import CourierRatingWindow from "./CourierRatingWindow.vue";
import DeliveryComplaintWindow from "./DeliveryComplaintWindow.vue";

const props = defineProps<{ 
    visible: boolean;
    order?: OrderInfo;
}>();

const emit = defineEmits(["close", "contactRider", "rateCourier", "deliveryComplaint"]);

const loading = ref(false);
const error = ref<string | null>(null);
const deliveryInfo = ref<any>(null);

// 对话框状态
const showContactRider = ref(false);
const showRateCourier = ref(false);
const showDeliveryComplaint = ref(false);
const hasRatedCourier = ref(false); // 是否已评分

// 联系骑手聊天记录（模拟数据）
const riderChatMessages = ref([
  { sender: "rider", content: "您好，我是配送骑手，正在为您配送订单", time: "10:30" },
  { sender: "user", content: "好的，谢谢", time: "10:32" }
]);

// 配送状态映射
const deliveryStatusMap: Record<string, { label: string; colorClass: string }> = {
  '0': { label: '待接单', colorClass: 'bg-yellow-100 text-yellow-700' },
  '1': { label: '待取餐', colorClass: 'bg-orange-100 text-orange-700' },
  '2': { label: '配送中', colorClass: 'bg-blue-100 text-blue-700' },
  '3': { label: '已完成', colorClass: 'bg-green-100 text-green-700' }
};

// 交通工具类型标签（与商家端、骑手端保持一致）
const getVehicleTypeLabel = (vehicleType?: string) => {
  const vehicleMap: Record<string, string> = {
    'electric_bike': '电动自行车',
    'motorcycle': '摩托车',
    'car': '小型汽车'
  };
  return vehicleMap[vehicleType || ''] || vehicleType || '未知';
};

// 获取骑手地图URL
const getCourierMapUrl = (latitude: number, longitude: number) => {
  return `https://maps.google.com/maps?q=${latitude},${longitude}&z=15&output=embed`;
};

function close() {
  emit("close");
}

// 打开联系骑手对话框
function openContactRider() {
  showContactRider.value = true;
}

// 处理骑手回复
function handleRiderReply(content: string) {
  const now = new Date();
  const timeStr = `${now.getHours().toString().padStart(2, '0')}:${now.getMinutes().toString().padStart(2, '0')}`;
  riderChatMessages.value.push({
    sender: "user",
    content: content,
    time: timeStr
  });
  // 模拟骑手自动回复
  setTimeout(() => {
    riderChatMessages.value.push({
      sender: "rider",
      content: "收到，我会尽快处理",
      time: timeStr
    });
  }, 1000);
  showContactRider.value = false;
}

// 打开骑手打分对话框
function openRateCourier() {
  if (hasRatedCourier.value) {
    ElMessage.warning("您已经为该骑手打过分了");
    return;
  }
  showRateCourier.value = true;
}

// 打开配送投诉对话框
function openDeliveryComplaint() {
  showDeliveryComplaint.value = true;
}

// 重新获取配送信息
async function refreshDeliveryInfo() {
  if (!props.order) return;
  
  try {
    loading.value = true;
    const response = await getOrderDeliveryInfo(props.order.orderId);
    
    // 检查返回的配送信息是否有效
    if (!response || (!response.taskId && !response.TaskId)) {
      return;
    }
    
    deliveryInfo.value = {
      ...response,
      order: response.order || props.order
    };
    
    // 检查是否已评分
    if (response.taskRating !== undefined && response.taskRating !== null) {
      hasRatedCourier.value = true;
    }
  } catch (err: any) {
    console.error('刷新配送信息失败:', err);
  } finally {
    loading.value = false;
  }
}

// 处理骑手评分完成
async function handleCourierRated() {
  hasRatedCourier.value = true;
  showRateCourier.value = false;
  await refreshDeliveryInfo();
}

// 处理配送投诉完成
function handleComplaintSubmitted() {
  showDeliveryComplaint.value = false;
}

// 监听弹窗打开，获取配送信息
watch(
  () => props.visible,
  async (val) => {
    if (val && props.order) {
      loading.value = true;
      error.value = null;
      deliveryInfo.value = null;
      hasRatedCourier.value = false; // 重置评分状态
      
      try {
        const response = await getOrderDeliveryInfo(props.order.orderId);
        
        // 检查返回的配送信息是否有效
        if (!response || (!response.taskId && !response.TaskId)) {
          error.value = '未找到配送任务信息';
          loading.value = false;
          return;
        }
        
        // 调试：打印返回的数据
        console.log('配送信息响应:', response);
        console.log('Order 信息:', response.order);
        
        deliveryInfo.value = {
          ...response,
          // 保留后端返回的 order 信息（包含收货信息），如果没有则使用 props.order
          order: response.order || props.order
        };
        
        // 检查是否已评分（如果配送任务有 taskRating 字段）
        if (response.taskRating !== undefined && response.taskRating !== null) {
          hasRatedCourier.value = true;
        }
        
        // 调试：打印最终的 deliveryInfo
        console.log('最终的 deliveryInfo:', deliveryInfo.value);
        console.log('deliveryInfo.order:', deliveryInfo.value.order);
      } catch (err: any) {
        console.error('获取配送信息失败:', err);
        error.value = '获取配送信息失败：' + (err?.message || '未知错误');
        ElMessage.error(error.value);
      } finally {
        loading.value = false;
      }
    }
  }
);
</script>
