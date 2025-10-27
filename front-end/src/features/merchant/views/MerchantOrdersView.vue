<template>
  <div class="min-h-screen bg-gray-50">
    <header class="fixed top-0 left-0 right-0 bg-white shadow-sm z-50 h-16">
      <div class="flex items-center justify-between h-full px-6">
        <div class="flex items-center">
          <h1 class="text-xl font-bold text-[#F9771C]">{{ projectName }}</h1>
        </div>
        <div class="flex items-center space-x-4">
          <el-icon class="text-gray-600 text-xl cursor-pointer">
            <Bell />
          </el-icon>
          <div class="flex items-center space-x-2">
            <span class="text-gray-700 font-medium">商家中心</span>
          </div>
        </div>
      </div>
    </header>

    <div class="flex pt-16">
      <aside class="fixed left-0 top-16 bottom-0 w-52 bg-white shadow-sm overflow-y-auto z-50">
        <nav class="p-4">
          <div class="space-y-2">
            <div v-for="(item, index) in menuItems" :key="index" @click="handleMenuClick(item)" :class="{
                'bg-orange-50 text-[#F9771C] border-r-3 border-[#F9771C]': $route.name === item.routeName,
                'text-gray-700 hover:bg-gray-50': $route.name !== item.routeName
              }"
              class="flex items-center px-4 py-3 rounded-l-lg cursor-pointer transition-colors whitespace-nowrap !rounded-button">
              <el-icon class="mr-3 text-lg">
                <component :is="item.icon" />
              </el-icon>
              <span class="font-medium">{{ item.label }}</span>
            </div>
          </div>
        </nav>
        <div class="p-4 border-t border-gray-100">
          <div @click="handleLogout"
            class="flex items-center px-4 py-3 rounded-lg cursor-pointer transition-colors text-red-500 hover:bg-red-50">
            <el-icon class="mr-3 text-lg">
              <SwitchButton />
            </el-icon>
            <span class="font-medium">退出登录</span>
          </div>
        </div>
      </aside>

      <main class="ml-52 flex-1 p-6">
        <h2 class="text-2xl font-bold text-gray-800 mb-6">订单中心</h2>

        <div v-if="errorMessage" class="mb-4 p-4 bg-red-50 border border-red-200 rounded-lg">
          <div class="flex items-center justify-between">
            <div class="flex items-center">
              <svg class="w-5 h-5 text-red-400 mr-2" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd"
                  d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z"
                  clip-rule="evenodd"></path>
              </svg>
              <span class="text-red-800">{{ errorMessage }}</span>
            </div>
            <div class="flex items-center space-x-2">
              <button @click="retryLoad" class="btn-error">重试</button>
              <button @click="clearError" class="btn-icon text-red-400 hover:text-red-600">
                <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
                </svg>
              </button>
            </div>
          </div>
        </div>

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

        <div class="bg-white/95 backdrop-blur-xl rounded-3xl shadow-2xl border border-gray-200/30 overflow-hidden relative z-10 transform transition-all duration-300 hover:shadow-3xl hover:scale-[1.01]">
          <div class="absolute top-0 left-0 w-full h-1 bg-gradient-to-r from-[#F9771C] via-[#FF8C42] to-transparent">
          </div>

          <el-table :data="orders" style="width: 100%" class="custom-table relative z-10" v-loading="loading.orders"
            element-loading-text="加载订单中..." element-loading-spinner="el-icon-loading"
            element-loading-background="rgba(255, 255, 255, 0.8)">
            <el-table-column prop="orderId" label="订单ID" width="120" align="center" />
            <el-table-column prop="paymentTime" label="支付时间" width="160">
              <template #default="scope">
                <span>{{ formatDate(scope.row.paymentTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column prop="customerId" label="客户ID" width="100" align="center" />
            <el-table-column prop="storeId" label="门店ID" width="100" align="center" />
            <el-table-column prop="sellerId" label="商家ID" width="100" align="center" />
            <el-table-column prop="orderState" label="接单状态" width="120" align="center">
              <template #default="scope">
                <span
                  :class="[orderStateMap[scope.row.orderState]?.colorClass, 'px-3 py-1 rounded-full text-xs font-medium']">
                  {{ orderStateMap[scope.row.orderState]?.label || '未知状态' }}
                </span>
              </template>
            </el-table-column>
            <el-table-column prop="deliveryStatus" label="配送任务状态" width="140" align="center">
              <template #default="scope">
                <span
                  :class="[deliveryStatusMap[String(scope.row.deliveryStatus ?? -1)]?.colorClass, 'px-3 py-1 rounded-full text-xs font-medium']">
                  {{ deliveryStatusMap[String(scope.row.deliveryStatus ?? -1)]?.label }}
                </span>
              </template>
            </el-table-column>
            <el-table-column prop="remarks" label="备注" min-width="200" />
            <el-table-column label="操作" min-width="520">
              <template #default="scope">
                <div class="flex flex-wrap items-center gap-2">
                  <button @click="showOrderDetails(scope.row)" class="btn-primary btn-small shrink-0">
                    订单信息
                  </button>

                  <!-- 接单/出餐 按钮 -->
                  <button v-if="scope.row.orderState === 0" @click="acceptOrder(scope.row)"
                    class="btn-success btn-small shrink-0">
                    接单
                  </button>
                  <button v-else-if="scope.row.orderState === 1" @click="markAsReady(scope.row)"
                    class="btn-warning btn-small shrink-0">
                    出餐
                  </button>
                  <button v-else-if="scope.row.orderState === 2" disabled
                    class="btn-secondary btn-small shrink-0 opacity-60 cursor-not-allowed">
                    已出餐
                  </button>
                  <!-- 配送任务按钮 -->
                  <button v-if="!scope.row.deliveryTaskId && scope.row.orderState !== 0"
                    @click="openPublishDialog(scope.row)" class="btn-info btn-small shrink-0">
                    发布配送
                  </button>
                  <button v-else-if="!scope.row.deliveryTaskId && scope.row.orderState === 0" disabled
                    class="btn-secondary btn-small shrink-0 opacity-60 cursor-not-allowed">
                    请先接单
                  </button>
                  <button v-else disabled class="btn-secondary btn-small shrink-0 opacity-60 cursor-not-allowed">
                    已发布配送
                  </button>

                  <!-- 只要发布了配送任务，显示"查看配送"按钮 -->
                  <button v-if="scope.row.deliveryTaskId" @click="openDeliveryInfo(scope.row)"
                    class="btn-small shrink-0"
                    style="background-color: #f8bbd0 !important; color: white !important; border-radius: 8px !important; padding: 8px 16px !important;">
                    查看配送
                  </button>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </main>
    </div>

    <!-- 订单详情对话框 -->
    <div v-if="showOrderDetailsDialog"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
      <div
        class="bg-white rounded-2xl w-[720px] max-h-[80vh] flex flex-col overflow-hidden shadow-2xl border border-gray-100 transform transition-all duration-300 scale-100">
        <div
          class="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-orange-50 to-yellow-50">
          <div>
            <div class="text-xl font-bold text-gray-900">订单详细信息</div>
            <div class="text-sm text-orange-600 font-medium">订单ID: {{ selectedOrder?.orderId }}</div>
          </div>
          <button @click="closeOrderDetailsDialog" class="btn-icon text-gray-400 hover:text-gray-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
        </div>

        <div class="flex-1 p-4 overflow-y-auto space-y-4">
          <div class="grid grid-cols-2 gap-4">
            <div class="bg-gray-50 rounded-lg p-3">
              <div class="text-sm text-gray-700">支付时间: {{ selectedOrder?.paymentTime }}</div>
              <div class="text-sm text-gray-700">备注: {{ selectedOrder?.remarks || '-' }}</div>
            </div>
            <div class="bg-gray-50 rounded-lg p-3">
              <div class="text-sm text-gray-700">客户ID: {{ selectedOrder?.customerId }}</div>
              <div class="text-sm text-gray-700">门店ID: {{ selectedOrder?.storeId }}，商家ID: {{ selectedOrder?.sellerId }}
              </div>
            </div>
          </div>

          <div>
            <div class="text-sm font-medium text-gray-900 mb-2">订单信息</div>
            <div class="bg-gray-50 rounded-lg p-3">
              <div class="text-sm text-gray-700">订单状态: {{ getStatusText(selectedOrder?.orderState || 0) }}</div>
              <div class="text-sm text-gray-700">总金额: ¥{{ selectedOrder?.totalAmount || 0 }}</div>
              <div class="text-sm text-gray-700">配送地址: {{ selectedOrder?.deliveryAddress || '-' }}</div>
            </div>
          </div>
        </div>

        <div class="p-4 border-t border-gray-200 flex justify-end">
          <button @click="closeOrderDetailsDialog" class="btn-outline btn-medium">关闭</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { getProjectName } from '@/stores/name';
import { ref, computed, onMounted } from 'vue';
import { 
  Bell, House, List, Ticket, Warning, User, SwitchButton,
  Clock, Document, TrendCharts, Money, Loading
} from '@element-plus/icons-vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useRouter, useRoute } from 'vue-router';

// API 导入
import {
  getOrders,
  acceptOrder as acceptOrderApi,
  rejectOrder as rejectOrderApi,
  type FoodOrder
} from '@/api/merchant/orders';

import {
  getMerchantInfo,
  type MerchantInfo
} from '@/api/merchant/shop';

// 登出功能导入
import loginApi from '@/api/login_api';
import { removeToken } from '@/utils/jwt';

const useProjectName = getProjectName();
const projectName = useProjectName.projectName;
const router = useRouter();
const $route = useRoute();

// 菜单项
const menuItems = [
  { key: 'overview', label: '店铺概况', icon: House, routeName: 'MerchantHome' },
  { key: 'orders', label: '订单中心', icon: List, routeName: 'MerchantOrders' },
  { key: 'menu', label: '菜品管理', icon: Document, routeName: 'MerchantMenu' },
  { key: 'coupons', label: '配券中心', icon: Ticket, routeName: 'MerchantCoupons' },
  { key: 'aftersale', label: '订单售后', icon: Warning, routeName: 'MerchantAftersale' },
  { key: 'profile', label: '商家信息', icon: User, routeName: 'MerchantProfile' }
] as const;

// 数据加载状态
const loading = ref({ orders: false });

// 错误处理
const errorMessage = ref('');

// 订单相关
const orders = ref<(FoodOrder & { localStatus?: string; deliveryStatus?: number | null })[]>([]);
const merchantInfo = ref<MerchantInfo | null>(null);

// 订单详情对话框
const showOrderDetailsDialog = ref(false);
const selectedOrder = ref<FoodOrder | null>(null);

// 订单统计
const orderStats = ref({
  pending: 0,
  today: 0,
  monthly: 0,
  revenue: 0
});

// 计算属性
const selectedStatus = ref('all');

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
    console.log('商家信息获取成功:', merchantInfo.value);
  } catch (error) {
    console.error('获取商家信息失败:', error);
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
    console.error('加载订单失败:', error);
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
  console.log('筛选订单状态:', selectedStatus.value);
};

// 菜单点击处理
const handleMenuClick = (menuItem: typeof menuItems[number]) => {
  router.push({ name: menuItem.routeName });
};

// 错误处理
const clearError = () => {
  errorMessage.value = '';
};

const retryLoad = async () => {
  errorMessage.value = '';
  await loadOrders();
};

// 订单详情
const showOrderDetails = async (order: FoodOrder) => {
  selectedOrder.value = order;
  showOrderDetailsDialog.value = true;
};

const closeOrderDetailsDialog = () => {
  showOrderDetailsDialog.value = false;
  selectedOrder.value = null;
};

// 出餐功能 - 暂时简化
const markAsReady = async (order: any) => {
  try {
    // 这里应该调用后端API，暂时只更新前端状态
    order.orderState = 2; // 已出餐
    ElMessage.success('订单已出餐');
  } catch (error) {
    console.error('出餐失败:', error);
    ElMessage.error('出餐失败，请重试');
  }
};

// 配送相关功能暂时移除
const openPublishDialog = (order: FoodOrder) => {
  ElMessage.info('配送功能开发中');
};

const openDeliveryInfo = (order: FoodOrder) => {
  ElMessage.info('配送信息功能开发中');
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
  console.log('查看订单详情:', order);
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

// 订单状态映射
const orderStateMap: Record<number, { label: string; colorClass: string }> = {
  0: { label: '未接单', colorClass: 'bg-gray-100 text-gray-600' },
  1: { label: '备菜中', colorClass: 'bg-yellow-100 text-yellow-600' },
  2: { label: '已出餐', colorClass: 'bg-green-100 text-green-600' }
};

const deliveryStatusMap: Record<string, { label: string; colorClass: string }> = {
  '-1': { label: '未发布配送', colorClass: 'bg-gray-100 text-gray-400' },
  '0': { label: '未接单', colorClass: 'bg-gray-100 text-gray-600' },
  '1': { label: '骑手未取餐', colorClass: 'bg-yellow-100 text-yellow-600' },
  '2': { label: '配送中', colorClass: 'bg-blue-100 text-blue-600' },
  '3': { label: '已完成', colorClass: 'bg-green-100 text-green-600' },
  '4': { label: '已取消', colorClass: 'bg-red-100 text-red-600' },
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toLocaleString();
};

// 登出功能
async function handleLogout() {
  try {
    await ElMessageBox.confirm(
      '您确定要退出当前商家账号吗？',
      '退出登录',
      {
        confirmButtonText: '确定退出',
        cancelButtonText: '取消',
        type: 'warning',
      }
    );

    await loginApi.logout();
    removeToken();
    ElMessage.success('您已成功退出登录');
    router.replace('/login');

  } catch (error: any) {
    if (error === 'cancel') {
      ElMessage.info('已取消退出操作');
    } else {
      console.error('登出时发生错误:', error);
      ElMessage.warning('与服务器通信失败，但已在本地强制退出');
      removeToken();
      router.replace('/login');
    }
  }
}

// 初始化数据
onMounted(async () => {
  await fetchMerchantInfo();
  await loadOrders();
});
</script>

<style scoped>
.\!rounded-button {
  border-radius: 8px;
}
input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  appearance: none;
  margin: 0;
}
input[type="number"] {
  -moz-appearance: textfield;
  appearance: textfield;
}

/* 自定义表格样式 - 苹果风格 */
.custom-table :deep(.el-table) {
  background: transparent !important;
  border: none !important;
  position: relative !important;
  z-index: 10 !important;
}

.custom-table :deep(.el-table__header) {
  background: rgba(255, 255, 255, 0.8) !important;
  backdrop-filter: blur(10px) !important;
  border-bottom: 1px solid rgba(249, 119, 28, 0.1) !important;
  position: relative !important;
  z-index: 10 !important;
}

.custom-table :deep(.el-table__header th) {
  background: transparent !important;
  border: none !important;
  color: #374151 !important;
  font-weight: 600 !important;
  font-size: 0.875rem !important;
  padding: 1rem 0.75rem !important;
}
.custom-table :deep(.el-table__body tr) {
  background: rgba(255, 255, 255, 0.6) !important;
  backdrop-filter: blur(8px) !important;
  border: none !important;
  transition: all 0.2s ease !important;
  position: relative !important;
  z-index: 10 !important;
}

.custom-table :deep(.el-table__body tr:hover) {
  background: rgba(255, 255, 255, 0.8) !important;
  backdrop-filter: blur(12px) !important;
  transform: translateY(-1px) !important;
  box-shadow: 0 4px 12px rgba(249, 119, 28, 0.1) !important;
}

.custom-table :deep(.el-table__body td) {
  border: none !important;
  padding: 1rem 0.75rem !important;
  color: #374151 !important;
  background: transparent !important;
}

/* 状态标签优化 */
.custom-table :deep(.px-3.py-1.rounded-full) {
  backdrop-filter: blur(8px) !important;
  border: 1px solid rgba(255, 255, 255, 0.2) !important;
}

/* 按钮样式 */
.btn-primary {
  background-color: #3b82f6;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-primary:hover {
  background-color: #2563eb;
}

.btn-success {
  background-color: #10b981;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-success:hover {
  background-color: #059669;
}

.btn-warning {
  background-color: #f59e0b;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-warning:hover {
  background-color: #d97706;
}

.btn-danger {
  background-color: #ef4444;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-danger:hover {
  background-color: #dc2626;
}

.btn-info {
  background-color: #06b6d4;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-info:hover {
  background-color: #0891b2;
}

.btn-secondary {
  background-color: #6b7280;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-secondary:hover {
  background-color: #4b5563;
}

.btn-outline {
  border: 1px solid #d1d5db;
  color: #374151;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-outline:hover {
  background-color: #f9fafb;
}

.btn-small {
  font-size: 0.875rem;
  padding: 0.25rem 0.75rem;
}

.btn-medium {
  font-size: 1rem;
  padding: 0.5rem 1rem;
}

.btn-error {
  background-color: #ef4444;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  transition: all 0.2s;
}
.btn-error:hover {
  background-color: #dc2626;
}

.btn-icon {
  padding: 0.25rem;
  border-radius: 0.25rem;
  transition: all 0.2s;
}
.btn-icon:hover {
  background-color: #f3f4f6;
}
</style>