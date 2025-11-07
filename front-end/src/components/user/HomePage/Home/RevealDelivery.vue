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
        <button @click="close" class="btn-icon text-gray-400 hover:text-gray-600 ml-4">
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
      <div class="p-4 border-t border-gray-200 flex justify-end">
        <button @click="close" class="btn-outline btn-medium">关闭</button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, watch, defineProps, defineEmits } from "vue";
import type { OrderInfo } from "@/api/user";
import { getOrderDeliveryInfo } from "@/api/user/home";
import { ElMessage } from "element-plus";

const props = defineProps<{ 
    visible: boolean;
    order?: OrderInfo;
}>();

const emit = defineEmits(["close"]);

const loading = ref(false);
const error = ref<string | null>(null);
const deliveryInfo = ref<any>(null);

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

// 监听弹窗打开，获取配送信息
watch(
  () => props.visible,
  async (val) => {
    if (val && props.order) {
      loading.value = true;
      error.value = null;
      deliveryInfo.value = null;
      
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

<style scoped>
.btn-outline {
  @apply px-4 py-2 rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 transition-colors;
}

.btn-medium {
  @apply text-sm font-medium;
}

.btn-icon {
  @apply p-2 rounded-lg hover:bg-gray-100 transition-colors;
}
</style>
