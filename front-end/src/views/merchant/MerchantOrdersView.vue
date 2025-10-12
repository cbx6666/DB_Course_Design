<template>
  <MerchantLayout>
    <!-- 订单中心 -->
    <div>
      <h2 class="text-2xl font-bold text-gray-800 mb-6">订单中心</h2>

      <!-- 订单统计卡片 -->
      <div class="grid grid-cols-4 gap-6 mb-8">
        <div class="bg-white rounded-lg shadow-sm p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">待处理订单</p>
              <p class="text-2xl font-bold text-orange-500">{{ orderStats.pending }}</p>
            </div>
            <el-icon class="text-orange-500 text-3xl">
              <Clock />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">今日订单</p>
              <p class="text-2xl font-bold text-blue-500">{{ orderStats.today }}</p>
            </div>
            <el-icon class="text-blue-500 text-3xl">
              <Document />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">本月订单</p>
              <p class="text-2xl font-bold text-green-500">{{ orderStats.monthly }}</p>
            </div>
            <el-icon class="text-green-500 text-3xl">
              <TrendCharts />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-gray-600 text-sm">总收入</p>
              <p class="text-2xl font-bold text-purple-500">¥{{ orderStats.revenue }}</p>
            </div>
            <el-icon class="text-purple-500 text-3xl">
              <Money />
            </el-icon>
          </div>
        </div>
      </div>

      <!-- 订单列表 -->
      <div class="bg-white rounded-lg shadow-sm">
        <div class="p-6 border-b border-gray-200">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-semibold text-gray-800">订单列表</h3>
            <div class="flex items-center space-x-4">
              <el-select v-model="selectedStatus" placeholder="筛选状态" @change="filterOrders">
                <el-option label="全部" value="all" />
                <el-option label="待处理" value="pending" />
                <el-option label="已接单" value="accepted" />
                <el-option label="制作中" value="preparing" />
                <el-option label="已完成" value="completed" />
              </el-select>
              <el-button @click="refreshOrders" :loading="loading.orders" icon="Refresh" />
            </div>
          </div>
        </div>

        <div class="p-6">
          <!-- 错误提示 -->
          <div v-if="errorMessage" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
            <div class="flex items-center justify-between">
              <p class="text-red-600">{{ errorMessage }}</p>
              <el-button @click="retryLoad" size="small" type="primary">重试</el-button>
            </div>
          </div>

          <!-- 订单列表 -->
          <div v-if="!loading.orders && orders.length > 0" class="space-y-4">
            <div 
              v-for="order in filteredOrders" 
              :key="order.orderId"
              class="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow"
            >
              <div class="flex items-center justify-between mb-3">
                <div class="flex items-center space-x-4">
                  <span class="font-semibold text-gray-800">订单 #{{ order.orderId }}</span>
                  <el-tag :type="getStatusType(order.orderState)" size="small">
                    {{ getStatusText(order.orderState) }}
                  </el-tag>
                </div>
                <div class="text-right">
                  <p class="text-lg font-bold text-gray-800">¥{{ order.totalAmount }}</p>
                  <p class="text-sm text-gray-500">{{ order.orderTime ? formatTime(order.orderTime) : '未知时间' }}</p>
                </div>
              </div>

              <!-- 订单详情 -->
              <div class="grid grid-cols-2 gap-4 mb-3">
                <div>
                  <p class="text-sm text-gray-600">客户信息</p>
                  <p class="font-medium">{{ order.customerName }}</p>
                  <p class="text-sm text-gray-500">{{ order.customerPhone }}</p>
                </div>
                <div>
                  <p class="text-sm text-gray-600">配送地址</p>
                  <p class="font-medium">{{ order.deliveryAddress }}</p>
                </div>
              </div>

              <!-- 菜品列表 -->
              <div class="mb-3">
                <p class="text-sm text-gray-600 mb-2">订单菜品</p>
                <div class="space-y-1">
                  <div 
                    v-for="item in order.items" 
                    :key="item.dishId"
                    class="flex items-center justify-between text-sm"
                  >
                    <span>{{ item.dish?.dishName || '未知菜品' }} × {{ item.quantity }}</span>
                    <span class="text-gray-600">¥{{ (item.dish?.price || 0) * item.quantity }}</span>
                  </div>
                </div>
              </div>

              <!-- 操作按钮 -->
              <div class="flex items-center justify-end space-x-2">
                <el-button 
                  v-if="order.orderState === 0"
                  @click="acceptOrder(order.orderId)"
                  type="primary"
                  size="small"
                >
                  接单
                </el-button>
                <el-button 
                  v-if="order.orderState === 0"
                  @click="rejectOrder(order.orderId)"
                  type="danger"
                  size="small"
                >
                  拒单
                </el-button>
                <el-button 
                  v-if="order.orderState === 1"
                  @click="startPreparing(order.orderId)"
                  type="warning"
                  size="small"
                >
                  开始制作
                </el-button>
                <el-button 
                  v-if="order.orderState === 2"
                  @click="finishOrder(order.orderId)"
                  type="success"
                  size="small"
                >
                  制作完成
                </el-button>
                <el-button @click="viewOrderDetail(order)" size="small">
                  查看详情
                </el-button>
              </div>
            </div>
          </div>

          <!-- 空状态 -->
          <div v-else-if="!loading.orders && orders.length === 0" class="text-center py-12">
            <el-icon class="text-gray-400 text-6xl mb-4">
              <Document />
            </el-icon>
            <p class="text-gray-500 text-lg">暂无订单</p>
            <p class="text-gray-400 text-sm">当有新订单时，会在这里显示</p>
          </div>

          <!-- 加载状态 -->
          <div v-else-if="loading.orders" class="text-center py-12">
            <el-icon class="text-gray-400 text-6xl mb-4 animate-spin">
              <Loading />
            </el-icon>
            <p class="text-gray-500">加载订单中...</p>
          </div>
        </div>
      </div>
    </div>
  </MerchantLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { 
  Clock, 
  Document, 
  TrendCharts, 
  Money, 
  Refresh, 
  Loading 
} from '@element-plus/icons-vue';

import MerchantLayout from '@/components/merchant/MerchantLayout.vue';
import {
  getOrders,
  acceptOrder as acceptOrderApi,
  rejectOrder as rejectOrderApi,
  getMerchantInfo,
  type FoodOrder,
  type MerchantInfo
} from '@/api/merchant_api';
import { devLog } from '@/utils/logger';

// 响应式数据
const orders = ref<FoodOrder[]>([]);
const merchantInfo = ref<MerchantInfo | null>(null);
const selectedStatus = ref('all');
const loading = ref({ orders: false });
const errorMessage = ref('');

// 订单统计
const orderStats = ref({
  pending: 0,
  today: 0,
  monthly: 0,
  revenue: 0
});

// 计算属性
const filteredOrders = computed(() => {
  if (selectedStatus.value === 'all') {
    return orders.value;
  }
  // 将字符串状态转换为数字进行比较
  const statusMap: Record<string, number> = {
    'pending': 0,
    'accepted': 1,
    'preparing': 2,
    'completed': 3,
    'cancelled': 4
  };
  const targetStatus = statusMap[selectedStatus.value];
  return orders.value.filter(order => order.orderState === targetStatus);
});

// 获取商家信息
const fetchMerchantInfo = async () => {
  try {
    merchantInfo.value = await getMerchantInfo();
    devLog.user('商家信息获取成功:', merchantInfo.value);
  } catch (error) {
    devLog.error('获取商家信息失败:', error);
  }
};

// 加载订单数据
const loadOrders = async () => {
  try {
    loading.value.orders = true;
    errorMessage.value = '';
    
    if (!merchantInfo.value?.sellerId) {
      throw new Error('商家ID不存在');
    }

    const apiOrders = await getOrders({ sellerId: merchantInfo.value.sellerId });

    if (apiOrders && (apiOrders as any).length > 0) {
      orders.value = (apiOrders as any).map((order: FoodOrder) => ({
        ...order,
        localStatus: 'accepted',
      }));
      
      // 计算订单统计
      calculateOrderStats();
    } else {
      orders.value = [];
    }
  } catch (error) {
    devLog.error('加载订单失败:', error);
    errorMessage.value = '加载订单失败，请重试';
    orders.value = [];
  } finally {
    loading.value.orders = false;
  }
};

// 计算订单统计
const calculateOrderStats = () => {
  const now = new Date();
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
  const thisMonth = new Date(now.getFullYear(), now.getMonth(), 1);

  orderStats.value = {
    pending: orders.value.filter(order => order.orderState === 0).length,
    today: orders.value.filter(order => order.orderTime ? new Date(order.orderTime) >= today : false).length,
    monthly: orders.value.filter(order => order.orderTime ? new Date(order.orderTime) >= thisMonth : false).length,
    revenue: orders.value.reduce((sum, order) => sum + (order.totalAmount || 0), 0)
  };
};

// 筛选订单
const filterOrders = () => {
  devLog.component('MerchantOrders', `筛选订单状态: ${selectedStatus.value}`);
};

// 刷新订单
const refreshOrders = async () => {
  await loadOrders();
};

// 重试加载
const retryLoad = async () => {
  errorMessage.value = '';
  await loadOrders();
};

// 接单
const acceptOrder = async (orderId: number) => {
  try {
    await ElMessageBox.confirm('确定要接受这个订单吗？', '确认接单', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await acceptOrderApi(orderId);
    ElMessage.success('订单已接受');
    await loadOrders();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('接单失败，请重试');
    }
  }
};

// 拒单
const rejectOrder = async (orderId: number) => {
  try {
    await ElMessageBox.confirm('确定要拒绝这个订单吗？', '确认拒单', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await rejectOrderApi(orderId);
    ElMessage.success('订单已拒绝');
    await loadOrders();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('拒单失败，请重试');
    }
  }
};

// 开始制作
const startPreparing = async (orderId: number) => {
  try {
    // 这里应该调用相应的API
    ElMessage.success('开始制作');
    await loadOrders();
  } catch (error) {
    ElMessage.error('操作失败，请重试');
  }
};

// 完成订单
const finishOrder = async (orderId: number) => {
  try {
    // 这里应该调用相应的API
    ElMessage.success('订单制作完成');
    await loadOrders();
  } catch (error) {
    ElMessage.error('操作失败，请重试');
  }
};

// 查看订单详情
const viewOrderDetail = (order: FoodOrder) => {
  // 这里可以打开订单详情弹窗或跳转到详情页面
  devLog.component('MerchantOrders', '查看订单详情:', order);
};

// 获取状态类型
// 将数字状态转换为字符串状态
const getStatusString = (orderState: number): string => {
  const statusMap: Record<number, string> = {
    0: 'pending',    // 待处理
    1: 'accepted',   // 已接单
    2: 'preparing',  // 制作中
    3: 'completed',  // 已完成
    4: 'cancelled'   // 已取消
  };
  return statusMap[orderState] || 'unknown';
};

const getStatusType = (orderState: number) => {
  const statusString = getStatusString(orderState);
  const statusMap: Record<string, string> = {
    pending: 'warning',
    accepted: 'primary',
    preparing: 'info',
    completed: 'success',
    cancelled: 'danger',
    unknown: 'info'
  };
  return statusMap[statusString] || 'info';
};

// 获取状态文本
const getStatusText = (orderState: number) => {
  const statusString = getStatusString(orderState);
  const statusMap: Record<string, string> = {
    pending: '待处理',
    accepted: '已接单',
    preparing: '制作中',
    completed: '已完成',
    cancelled: '已取消',
    unknown: '未知状态'
  };
  return statusMap[statusString] || '未知状态';
};

// 格式化时间
const formatTime = (time: string) => {
  return new Date(time).toLocaleString('zh-CN');
};

// 初始化数据
onMounted(async () => {
  await fetchMerchantInfo();
  await loadOrders();
});
</script>
