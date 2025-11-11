<template>
  <main class="pt-20 min-h-screen max-w-screen-xl mx-auto px-6 py-8">
    <h1 class="text-3xl font-bold text-gray-800 mb-8 text-left">售后</h1>

    <!-- 标签页 -->
    <div class="flex space-x-1 mb-8 bg-white rounded-lg p-2 shadow-sm">
      <button
        v-for="(tab, index) in tabs"
        :key="index"
        @click="activeTab = tab.key"
        :class="{
          'bg-orange-500 text-white': activeTab === tab.key,
          'text-gray-600 hover:bg-gray-100': activeTab !== tab.key
        }"
        class="px-6 py-2 rounded-lg font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- 加载中 -->
    <div v-if="loading" class="flex justify-center items-center h-64">
      <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
    </div>

    <!-- 内容区域 -->
    <div v-else class="space-y-6">
      <!-- 售后申请 -->
      <div v-if="activeTab === 'afterSale'">
        <div v-if="afterSales.length === 0" class="text-center py-12 text-gray-500">
          <i class="fas fa-clipboard-list text-4xl mb-4"></i>
          <p>暂无售后申请</p>
        </div>
        <div
          v-for="item in afterSales"
          :key="item.applicationId"
          :class="[
            'bg-white rounded-lg shadow-md border-l-4 p-6 text-left hover:shadow-lg transition-all duration-200 mb-4',
            item.status === 'Pending' ? 'border-orange-500' : 'border-gray-400'
          ]"
        >
          <div class="flex justify-between items-start mb-4 pb-4 border-b border-gray-200">
            <div class="flex-1">
              <h3 class="font-bold text-lg text-gray-800 mb-2">{{ item.storeName }}</h3>
              <div class="space-y-1">
                <p class="text-sm text-gray-600">订单号：<span class="font-medium text-gray-800">{{ item.orderId }}</span></p>
                <p class="text-sm text-gray-600">申请时间：<span class="font-medium text-gray-800">{{ formatDateTime(item.applicationTime) }}</span></p>
              </div>
            </div>
            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm font-medium ml-4 whitespace-nowrap">
              {{ getStatusText(item.status) }}
            </span>
          </div>
          <div class="mb-4">
            <p class="text-gray-700">{{ item.description }}</p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="售后图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 订单菜品列表 -->
          <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mb-4">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">订单菜品：</h4>
            <div class="bg-gray-50 rounded-lg p-3">
              <div class="space-y-2">
                <div
                  v-for="(dish, dishIdx) in item.dishDetails"
                  :key="dishIdx"
                  class="flex items-center gap-3 py-2 border-b border-gray-200 last:border-b-0"
                >
                  <img
                    :src="normalizeImageUrl(dish.dishImage)"
                    :alt="dish.dishName"
                    class="w-16 h-16 object-cover rounded border border-gray-300"
                    @error="handleImageError"
                  />
                  <div class="flex-1">
                    <p class="font-medium text-gray-800">{{ dish.dishName }}</p>
                    <p class="text-sm text-gray-600">单价：¥{{ dish.price.toFixed(2) }}</p>
                  </div>
                  <div class="text-right">
                    <p class="font-medium text-gray-800">×{{ dish.quantity }}</p>
                    <p class="text-sm text-gray-600">小计：¥{{ (dish.price * dish.quantity).toFixed(2) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div v-if="item.processingResult || item.processingReason || item.status === 'Completed'" class="mt-4 p-3 bg-gray-50 rounded-lg space-y-2">
            <p class="text-sm text-gray-600">
              <span class="font-medium">处理结果：</span>{{ item.processingResult || '-' }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">处理原因：</span>{{ item.processingReason || '-' }}
            </p>
          </div>
        </div>
      </div>

      <!-- 配送投诉 -->
      <div v-if="activeTab === 'complaint'">
        <div v-if="complaints.length === 0" class="text-center py-12 text-gray-500">
          <i class="fas fa-exclamation-triangle text-4xl mb-4"></i>
          <p>暂无配送投诉</p>
        </div>
        <div
          v-for="item in complaints"
          :key="item.complaintId"
          :class="[
            'bg-white rounded-lg shadow-md border-l-4 p-6 text-left hover:shadow-lg transition-all duration-200 mb-4',
            item.status === 'Pending' ? 'border-red-500' : 'border-gray-400'
          ]"
        >
          <div class="flex justify-between items-start mb-4 pb-4 border-b border-gray-200">
            <div class="flex-1">
              <h3 class="font-bold text-lg text-gray-800 mb-2">配送投诉</h3>
              <div class="space-y-1">
                <p class="text-sm text-gray-600">订单号：<span class="font-medium text-gray-800">{{ item.orderId }}</span></p>
                <p class="text-sm text-gray-600">配送任务ID：<span class="font-medium text-gray-800">{{ item.deliveryTaskId }}</span></p>
                <p class="text-sm text-gray-600">投诉时间：<span class="font-medium text-gray-800">{{ formatDateTime(item.complaintTime) }}</span></p>
              </div>
            </div>
            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm font-medium ml-4 whitespace-nowrap">
              {{ getStatusText(item.status) }}
            </span>
          </div>
          <div class="mb-4">
            <p class="text-gray-700">{{ item.complaintReason }}</p>
          </div>
          <!-- 配送信息板块 -->
          <div class="mb-4 p-3 bg-gray-50 rounded-lg space-y-2">
            <p class="text-sm text-gray-600">骑手：<span class="font-medium text-gray-800">{{ item.courierName || '-' }}</span></p>
            <p class="text-sm text-gray-600">骑手电话：<span class="font-medium text-gray-800">{{ item.courierPhone || '-' }}</span></p>
            <p class="text-sm text-gray-600">接单时间：<span class="font-medium text-gray-800">{{ item.acceptTime ? formatDateTime(item.acceptTime) : '-' }}</span></p>
            <p class="text-sm text-gray-600">实际到店时间：<span class="font-medium text-gray-800">{{ item.pickupTime ? formatDateTime(item.pickupTime) : '-' }}</span></p>
            <p class="text-sm text-gray-600">实际送达时间：<span class="font-medium text-gray-800">{{ item.completionTime ? formatDateTime(item.completionTime) : '-' }}</span></p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="投诉图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <div v-if="item.processingResult || item.processingReason || item.status === 'Completed'" class="mt-4 p-3 bg-gray-50 rounded-lg space-y-2">
            <p class="text-sm text-gray-600">
              <span class="font-medium">处理结果：</span>{{ item.processingResult || '-' }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">处理原因：</span>{{ item.processingReason || '-' }}
            </p>
          </div>
        </div>
      </div>

      <!-- 店铺举报 -->
      <div v-if="activeTab === 'report'">
        <div v-if="reports.length === 0" class="text-center py-12 text-gray-500">
          <i class="fas fa-flag text-4xl mb-4"></i>
          <p>暂无店铺举报</p>
        </div>
        <div
          v-for="item in reports"
          :key="item.penaltyId"
          :class="[
            'bg-white rounded-lg shadow-md border-l-4 p-6 text-left hover:shadow-lg transition-all duration-200 mb-4',
            item.status === 'Pending' ? 'border-purple-500' : 'border-gray-400'
          ]"
        >
          <div class="flex justify-between items-start mb-4 pb-4 border-b border-gray-200">
            <div class="flex-1">
              <h3 class="font-bold text-lg text-gray-800 mb-2">{{ item.storeName }}</h3>
              <div class="space-y-1">
                <p class="text-sm text-gray-600">店铺ID：<span class="font-medium text-gray-800">{{ item.storeId }}</span></p>
                <p class="text-sm text-gray-600">举报时间：<span class="font-medium text-gray-800">{{ formatDateTime(item.reportTime) }}</span></p>
              </div>
            </div>
            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm font-medium ml-4 whitespace-nowrap">
              {{ getStatusText(item.status) }}
            </span>
          </div>
          <div class="mb-4">
            <p class="text-gray-700">{{ item.content }}</p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="举报图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 处罚信息 -->
          <div class="mt-4 p-3 bg-gray-50 rounded-lg space-y-2">
            <p class="text-sm text-gray-600">
              <span class="font-medium">店铺处罚：</span>{{ item.storePunishment || '-' }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">商家处罚：</span>{{ item.merchantPunishment || '-' }}
            </p>
            <p class="text-sm text-gray-600">
              <span class="font-medium">处罚时间：</span>{{ item.penaltyTime ? formatDateTime(item.penaltyTime) : '-' }}
            </p>
            <p v-if="item.processingReason" class="text-sm text-gray-600">
              <span class="font-medium">处理原因：</span>{{ item.processingReason }}
            </p>
          </div>
        </div>
      </div>

      <!-- 评论 -->
      <div v-if="activeTab === 'comment'">
        <div v-if="comments.length === 0" class="text-center py-12 text-gray-500">
          <i class="fas fa-comment text-4xl mb-4"></i>
          <p>暂无评论</p>
        </div>
        <div
          v-for="item in comments"
          :key="item.commentId"
          :class="[
            'bg-white rounded-lg shadow-md border-l-4 p-6 text-left hover:shadow-lg transition-all duration-200 mb-4',
            item.status === 'Pending' ? 'border-yellow-500' : item.status === 'Completed' ? 'border-green-500' : item.status === 'Illegal' ? 'border-red-500' : 'border-gray-400'
          ]"
        >
          <div class="flex justify-between items-start mb-4 pb-4 border-b border-gray-200">
            <div class="flex-1">
              <h3 class="font-bold text-lg text-gray-800 mb-2">{{ item.storeName }}</h3>
              <div class="space-y-1">
                <p v-if="item.orderId" class="text-sm text-gray-600">订单号：<span class="font-medium text-gray-800">{{ item.orderId }}</span></p>
                <p class="text-sm text-gray-600">店铺ID：<span class="font-medium text-gray-800">{{ item.storeId }}</span></p>
                <p class="text-sm text-gray-600">评论时间：<span class="font-medium text-gray-800">{{ formatDateTime(item.postedAt) }}</span></p>
              </div>
            </div>
            <div class="flex items-center space-x-3 ml-4">
              <div class="flex items-center">
                <span v-for="i in 5" :key="i" class="text-lg">
                  <i
                    :class="i <= item.rating ? 'fas fa-star text-yellow-400' : 'far fa-star text-gray-300'"
                  ></i>
                </span>
              </div>
              <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm font-medium whitespace-nowrap">
                {{ getStatusText(item.status) }}
              </span>
            </div>
          </div>
          <div class="mb-4">
            <p class="text-gray-700">{{ item.content }}</p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="评论图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 订单菜品列表 -->
          <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mb-4">
            <h4 class="text-sm font-semibold text-gray-700 mb-2">订单菜品：</h4>
            <div class="bg-gray-50 rounded-lg p-3">
              <div class="space-y-2">
                <div
                  v-for="(dish, dishIdx) in item.dishDetails"
                  :key="dishIdx"
                  class="flex items-center gap-3 py-2 border-b border-gray-200 last:border-b-0"
                >
                  <img
                    :src="normalizeImageUrl(dish.dishImage)"
                    :alt="dish.dishName"
                    class="w-16 h-16 object-cover rounded border border-gray-300"
                    @error="handleImageError"
                  />
                  <div class="flex-1">
                    <p class="font-medium text-gray-800">{{ dish.dishName }}</p>
                    <p class="text-sm text-gray-600">单价：¥{{ dish.price.toFixed(2) }}</p>
                  </div>
                  <div class="text-right">
                    <p class="font-medium text-gray-800">×{{ dish.quantity }}</p>
                    <p class="text-sm text-gray-600">小计：¥{{ (dish.price * dish.quantity).toFixed(2) }}</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </main>
</template>

<script lang="ts" setup>
import { ref, onMounted, watch } from 'vue';
import { ElMessage } from 'element-plus';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { API_CONFIG } from '@/config/index';
import {
  getMyAfterSales,
  getMyDeliveryComplaints,
  getMyStoreReports,
  getMyComments,
  type AfterSaleListItem,
  type DeliveryComplaintListItem,
  type StoreReportListItem,
  type CommentListItem
} from '@/api/user/afterSale';

const loading = ref(true);
const activeTab = ref('afterSale');
const afterSales = ref<AfterSaleListItem[]>([]);
const complaints = ref<DeliveryComplaintListItem[]>([]);
const reports = ref<StoreReportListItem[]>([]);
const comments = ref<CommentListItem[]>([]);

const tabs = [
  { key: 'afterSale', label: '售后申请' },
  { key: 'complaint', label: '配送投诉' },
  { key: 'report', label: '店铺举报' },
  { key: 'comment', label: '评论' }
];

onMounted(() => {
  loadData();
});

watch(activeTab, () => {
  loadData();
});

const loadData = async () => {
  try {
    loading.value = true;
    switch (activeTab.value) {
      case 'afterSale':
        afterSales.value = await getMyAfterSales();
        break;
      case 'complaint':
        complaints.value = await getMyDeliveryComplaints();
        break;
      case 'report':
        reports.value = await getMyStoreReports();
        console.log('店铺举报数据:', reports.value);
        break;
      case 'comment':
        comments.value = await getMyComments();
        break;
    }
  } catch (error) {
    console.error('加载数据失败:', error);
    ElMessage.error('加载数据失败，请重试');
  } finally {
    loading.value = false;
  }
};

const formatDateTime = (dateStr: string) => {
  const date = new Date(dateStr);
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}`;
};

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': '待处理',
    'Processing': '处理中',
    'Approved': '已通过',
    'Rejected': '已拒绝',
    'Completed': '已完成',
    'Illegal': '违规'
  };
  return statusMap[status] || status;
};

const getStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    'Pending': 'bg-yellow-100 text-yellow-700',
    'Processing': 'bg-blue-100 text-blue-700',
    'Approved': 'bg-green-100 text-green-700',
    'Rejected': 'bg-red-100 text-red-700',
    'Completed': 'bg-green-100 text-green-700',
    'Illegal': 'bg-red-100 text-red-700'
  };
  return classMap[status] || 'bg-gray-100 text-gray-700';
};

const previewImage = (imageUrl: string) => {
  const fullUrl = imageUrl.startsWith('http') ? imageUrl : `${API_CONFIG.BASE_URL}${imageUrl}`;
  window.open(fullUrl, '_blank');
};
</script>

