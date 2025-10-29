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
             <el-table-column label="菜品信息" min-width="220">
               <template #default="scope">
                 <div class="text-sm">
                   <div v-if="scope.row.items && scope.row.items.length > 0">
                     <!-- 显示前2个菜品，包含价格信息 -->
                     <div v-for="(item, index) in scope.row.items.slice(0, 2)" :key="index" class="mb-1">
                       <div class="flex justify-between items-center">
                         <div class="flex-1">
                           <span class="font-medium">{{ item.dish?.dishName || '未知菜品' }}</span>
                           <span class="text-gray-500 ml-2">×{{ item.quantity }}</span>
                         </div>
                         <div class="text-right text-xs">
                           <div class="text-gray-600">¥{{ item.dish?.price || 0 }}</div>
                           <div class="font-medium text-orange-600">¥{{ item.totalPrice || 0 }}</div>
            </div>
          </div>
        </div>

                     <!-- 如果菜品超过2个，显示省略信息 -->
                     <div v-if="scope.row.items.length > 2" class="text-gray-400 text-xs mt-1">
                       <span>等{{ scope.row.items.length }}种菜品</span>
                       <el-button 
                         type="text" 
                         size="small" 
                         class="ml-2 text-blue-500 hover:text-blue-700 p-0 text-xs"
                         @click="showDishDetails(scope.row)">
                         查看全部
                       </el-button>
          </div>

                     <!-- 显示总数量和总金额 -->
                     <div class="text-gray-500 text-xs mt-2 border-t border-gray-200 pt-1 flex justify-between">
                       <span>共{{ getTotalQuantity(scope.row.items) }}件商品</span>
                       <span class="font-medium text-orange-600">
                         合计：¥{{ getSubtotal(scope.row.items).toFixed(2) }}
                       </span>
                     </div>
                </div>
                   <div v-else class="text-gray-400 text-center py-2">
                     <div class="text-xs">暂无菜品信息</div>
                     <div class="text-xs mt-1">订单ID: {{ scope.row.orderId }}</div>
                </div>
              </div>
               </template>
             </el-table-column>
            <el-table-column prop="paymentTime" label="支付时间" min-width="160">
              <template #default="scope">
                <span>{{ formatDate(scope.row.paymentTime) }}</span>
              </template>
            </el-table-column>
            <el-table-column label="收货人" min-width="160" align="left">
              <template #default="scope">
                <div class="text-xs space-y-0.5">
                  <div class="font-medium text-gray-900 truncate">{{ scope.row.deliveryName || '未提供' }}</div>
                  <div class="text-gray-500 truncate">{{ scope.row.deliveryPhone || '-' }}</div>
                </div>
              </template>
            </el-table-column>
            <el-table-column label="备注" min-width="160">
              <template #default="scope">
                <span class="text-gray-600">{{ scope.row.remarks || '无' }}</span>
              </template>
            </el-table-column>
            <el-table-column label="优惠券" min-width="140">
              <template #default="scope">
                <div v-if="scope.row.usedCoupon" class="text-xs">
                  <div class="font-medium text-[#F9771C] mb-1">
                    <span v-if="scope.row.usedCoupon.discountType === 'fixed'">
                      ¥{{ scope.row.usedCoupon.discountValue.toFixed(0) }}
                    </span>
                    <span v-else>
                      {{ (scope.row.usedCoupon.discountValue * 10).toFixed(1) }}折
                    </span>
                  </div>
                  <div class="text-gray-500 truncate" :title="scope.row.usedCoupon.couponName">
                    {{ scope.row.usedCoupon.couponName || '优惠券' }}
                  </div>
                </div>
                <span v-else class="text-gray-400 text-xs">未使用</span>
              </template>
            </el-table-column>
            <el-table-column label="订单管理" min-width="160" align="left">
              <template #default="scope">
                <div class="space-y-1">
                  <!-- 订单状态 -->
                  <div>
                    <span
                      :class="[orderStateMap[scope.row.orderState]?.colorClass, 'px-2 py-0.5 rounded-full text-xs font-medium']">
                      {{ orderStateMap[scope.row.orderState]?.label || '未知状态' }}
                    </span>
                  </div>
                  
                  <!-- 接单/出餐按钮 -->
                  <div class="flex flex-col">
                    <button v-if="scope.row.orderState === 0" @click="acceptOrder(scope.row.orderId)"
                      class="btn-small shrink-0 w-full py-1.5"
                      style="background-color: #f59e0b !important; color: white !important; border-radius: 8px !important;">
                      接单
                    </button>
                    <button v-else-if="scope.row.orderState === 1" @click="markAsReady(scope.row.orderId)"
                      class="btn-small shrink-0 w-full py-1.5"
                      style="background-color: #f59e0b !important; color: white !important; border-radius: 8px !important;">
                      出餐
                    </button>
                    <button v-else-if="scope.row.orderState === 2" disabled
                      class="btn-secondary btn-small shrink-0 opacity-60 cursor-not-allowed w-full py-1.5">
                      已出餐
                    </button>
                  </div>
                </div>
              </template>
            </el-table-column>
            <el-table-column label="配送管理" min-width="160">
              <template #default="scope">
                <div class="space-y-1">
                  <!-- 简单的发布状态 -->
                  <div>
                    <span v-if="scope.row.deliveryTaskId"
                      class="bg-green-100 text-green-600 px-2 py-0.5 rounded-full text-xs font-medium inline-block">
                      已发布
                    </span>
                    <span v-else
                      class="bg-gray-100 text-gray-400 px-2 py-0.5 rounded-full text-xs font-medium inline-block">
                      未发布
                    </span>
                  </div>
                  
                  <!-- 配送操作按钮 -->
                  <div class="flex flex-col">
                    <button v-if="!scope.row.deliveryTaskId && scope.row.orderState !== 0"
                      @click="openPublishDialog(scope.row)" class="btn-small shrink-0 w-full py-1.5"
                      style="background-color: #f59e0b !important; color: white !important; border-radius: 8px !important;">
                      发布配送
                    </button>
                    <button v-else-if="!scope.row.deliveryTaskId && scope.row.orderState === 0" disabled
                      class="btn-secondary btn-small shrink-0 opacity-60 cursor-not-allowed w-full py-1.5">
                      请先接单
                    </button>
                    
                    <!-- 查看配送按钮 -->
                    <button v-if="scope.row.deliveryTaskId" @click="openDeliveryInfo(scope.row)"
                      class="btn-small shrink-0 w-full py-1.5"
                      style="background-color: #f8bbd0 !important; color: white !important; border-radius: 8px !important;">
                      查看配送
                    </button>
                  </div>
                </div>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </main>
    </div>

    <!-- 菜品详情对话框 -->
    <div v-if="showDishDetailsDialog"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
      <div
        class="bg-white rounded-2xl w-[720px] max-h-[80vh] flex flex-col overflow-hidden shadow-2xl border border-gray-100 transform transition-all duration-300 scale-100">
        <div
          class="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-orange-50 to-yellow-50">
          <div>
            <div class="text-xl font-bold text-gray-900">菜品详情</div>
            <div class="text-sm text-orange-600 font-medium">订单ID: {{ selectedDishOrder?.orderId }}</div>
          </div>
          <button @click="closeDishDetailsDialog" class="btn-icon text-gray-400 hover:text-gray-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
              </div>

        <div class="flex-1 p-4 overflow-y-auto">
          <!-- 菜品详情表格 -->
          <div v-if="selectedDishOrder?.items && selectedDishOrder.items.length > 0">
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
                    <tr v-for="(item, index) in selectedDishOrder.items" :key="index" 
                        class="border-t border-gray-200 hover:bg-gray-100 transition-colors">
                      <td class="px-4 py-3">
                        <span class="font-medium text-gray-900">{{ item.dish?.dishName || '未知菜品' }}</span>
                      </td>
                      <td class="px-4 py-3 text-center text-gray-700">{{ item.quantity }}</td>
                      <td class="px-4 py-3 text-right text-gray-600">¥{{ item.dish?.price || 0 }}</td>
                      <td class="px-4 py-3 text-right font-medium text-orange-600">¥{{ item.totalPrice || 0 }}</td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div class="bg-gray-100 px-4 py-3 border-t border-gray-200">
                <div class="flex justify-between items-center text-sm font-medium">
                  <span class="text-gray-700">
                    共 {{ selectedDishOrder.items.length }} 种菜品，{{ getTotalQuantity(selectedDishOrder.items) }} 件商品
                  </span>
                  <span class="text-lg font-bold text-orange-600">
                    合计：¥{{ getSubtotal(selectedDishOrder.items).toFixed(2) }}
                  </span>
                </div>
              </div>
            </div>
          </div>
          <div v-else class="text-center py-8 text-gray-400">
            <div class="text-lg mb-2">暂无菜品信息</div>
            <div class="text-sm">订单ID: {{ selectedDishOrder?.orderId }}</div>
            </div>
          </div>

        <div class="p-4 border-t border-gray-200 flex justify-end">
          <button @click="closeDishDetailsDialog" class="btn-outline btn-medium">关闭</button>
                </div>
                </div>
              </div>

    <!-- 配送信息对话框 -->
    <div v-if="showDeliveryInfoDialog"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
      <div
        class="bg-white rounded-2xl w-[600px] max-h-[80vh] flex flex-col overflow-hidden shadow-2xl border border-gray-100 transform transition-all duration-300 scale-100">
        <div
          class="flex items-center justify-between p-6 border-b border-gray-200 bg-gradient-to-r from-blue-50 to-purple-50">
                <div>
            <div class="text-xl font-bold text-gray-900">配送信息</div>
            <div class="text-sm text-blue-600 font-medium">订单ID: {{ deliveryInfo?.order?.orderId }}</div>
          </div>
          <button @click="closeDeliveryInfoDialog" class="btn-icon text-gray-400 hover:text-gray-600">
            <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path>
            </svg>
          </button>
                </div>

        <div class="flex-1 p-6 overflow-y-auto">
          <!-- 配送状态 -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-3">配送状态</h3>
            <div class="bg-gray-50 rounded-lg p-4">
              <span
                :class="[deliveryStatusMap[String(deliveryInfo?.deliveryTask?.status ?? -1)]?.colorClass || 'bg-gray-100 text-gray-600', 'px-4 py-2 rounded-full text-sm font-medium']">
                {{ deliveryStatusMap[String(deliveryInfo?.deliveryTask?.status ?? -1)]?.label || '未知状态' }}
              </span>
                </div>
              </div>

          <!-- 收货地址 -->
          <div class="mb-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-3">收货信息</h3>
            <div class="bg-gray-50 rounded-lg p-4 space-y-2">
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">收货人：</span>
                <span class="text-left">{{ deliveryInfo?.order?.deliveryName || '未提供' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">联系电话：</span>
                <span class="text-left">{{ deliveryInfo?.order?.deliveryPhone || '-' }}</span>
              </div>
              <div class="flex items-start text-gray-700">
                <span class="font-medium mr-2">收货地址：</span>
                <span class="flex-1 text-left">{{ deliveryInfo?.order?.deliveryAddress || '未提供' }}</span>
                  </div>
                </div>
              </div>

          <!-- 骑手信息 -->
          <div>
            <h3 class="text-lg font-semibold text-gray-900 mb-3">骑手信息</h3>
            <div v-if="deliveryInfo?.courier" class="bg-gray-50 rounded-lg p-4 space-y-3">
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">姓名：</span>
                <span>{{ deliveryInfo.courier.fullName || '未知' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">电话：</span>
                <span>{{ deliveryInfo.courier.phoneNumber || '-' }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">交通工具：</span>
                <span>{{ deliveryInfo.courier.vehicleType }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <span class="font-medium mr-2">评分：</span>
                <span>{{ deliveryInfo.courier.averageRating?.toFixed(1) || '0.0' }} 分</span>
              </div>
              
              <!-- 骑手位置 -->
              <div class="border-t border-gray-200 pt-3">
                <div class="flex items-start text-gray-700">
                  <span class="font-medium mr-2">实时位置：</span>
                  <div class="flex-1">
                    <div v-if="deliveryInfo.courier.longitude && deliveryInfo.courier.latitude">
                      经度：{{ deliveryInfo.courier.longitude }}，纬度：{{ deliveryInfo.courier.latitude }}
                    </div>
                    <div v-else class="text-gray-400">
                      位置信息未提供
                    </div>
                  </div>
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

        <div class="p-4 border-t border-gray-200 flex justify-end">
          <button @click="closeDeliveryInfoDialog" class="btn-outline btn-medium">关闭</button>
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
  markAsReady as markAsReadyApi,
  publishDeliveryTask,
  getOrderDeliveryInfo,
  type FoodOrder,
  type OrderItem,
  type OrderCouponInfo
} from '@/api/merchant/orders';

import {
  getMerchantInfo,
  getShopOverview,
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

// 菜品详情对话框
const showDishDetailsDialog = ref(false);
const selectedDishOrder = ref<FoodOrder | null>(null);

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
    } else {
      orders.value = [];
    }
    
    // 无论是否有订单，都需要计算统计（月销量从API获取）
    await calculateOrderStats();
  } catch (error) {
    console.error('加载订单失败:', error);
    errorMessage.value = '加载订单失败，请重试';
    orders.value = [];
  } finally {
    loading.value.orders = false;
  }
};

// 计算订单统计
const calculateOrderStats = async () => {
  const now = new Date();
  const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());

  // 从订单列表计算待处理、今日订单和总收入
  orderStats.value.pending = orders.value.filter(order => order.orderState === 0).length;
  orderStats.value.today = orders.value.filter(order => order.paymentTime ? new Date(order.paymentTime) >= today : false).length;
  orderStats.value.revenue = orders.value.reduce((sum, order) => sum + (order.items ? getTotalAmount(order.items, order.usedCoupon) : 0), 0);
  
  // 本月订单：使用后端API获取的月销量（与店铺概况保持一致）
  try {
    const overview = await getShopOverview();
    if (overview?.data?.monthlySales !== undefined) {
      orderStats.value.monthly = overview.data.monthlySales;
    }
  } catch (error) {
    console.error('获取月销量失败:', error);
    // 如果API失败，回退到从订单列表计算（但应该尽量避免这种情况）
  const thisMonth = new Date(now.getFullYear(), now.getMonth(), 1);
    const COMPLETED_STATUS = 3;
    orderStats.value.monthly = orders.value.filter(order => 
      order.orderState === COMPLETED_STATUS && 
      order.paymentTime ? new Date(order.paymentTime) >= thisMonth : false
    ).length;
  }
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

// 显示菜品详情
const showDishDetails = (order: FoodOrder) => {
  selectedDishOrder.value = order;
  showDishDetailsDialog.value = true;
};

// 关闭菜品详情对话框
const closeDishDetailsDialog = () => {
  showDishDetailsDialog.value = false;
  selectedDishOrder.value = null;
};

// 出餐功能
const markAsReady = async (orderId: number) => {
  try {
    await ElMessageBox.confirm('确定订单已出餐吗？', '确认出餐', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await markAsReadyApi(orderId);
    ElMessage.success('订单已出餐');
    await loadOrders(); // 重新加载订单列表
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('出餐失败，请重试');
      console.error('出餐失败:', error);
    }
  }
};

// 发布配送任务
const openPublishDialog = async (order: FoodOrder) => {
  try {
    const estimatedArrivalTime = new Date(Date.now() + 20 * 60 * 1000); // 20分钟后
    const estimatedDeliveryTime = new Date(Date.now() + 40 * 60 * 1000); // 40分钟后
    
    await ElMessageBox.confirm(
      `确定发布配送任务吗？<br>骑手预计到达：${estimatedArrivalTime.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })} ，预计送达：${estimatedDeliveryTime.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit' })}`,
      '发布配送任务',
      {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'info',
        dangerouslyUseHTMLString: true
      }
    );

    await publishDeliveryTask(
      order.orderId,
      estimatedArrivalTime.toISOString(),
      estimatedDeliveryTime.toISOString()
    );
    
    ElMessage.success('配送任务已发布');
    await loadOrders(); // 重新加载订单列表
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('发布配送任务失败：' + (error?.message || '未知错误'));
    }
  }
};

// 配送信息对话框相关状态
const showDeliveryInfoDialog = ref(false);
const deliveryInfo = ref<any>(null);

// 查看配送信息
const openDeliveryInfo = async (order: FoodOrder) => {
  try {
    const info = await getOrderDeliveryInfo(order.orderId);
    console.log('配送信息：', info);
    deliveryInfo.value = {
      ...info,
      order: order
    };
    showDeliveryInfoDialog.value = true;
  } catch (error: any) {
    ElMessage.error('获取配送信息失败：' + (error?.message || '未知错误'));
  }
};

// 关闭配送信息对话框
const closeDeliveryInfoDialog = () => {
  showDeliveryInfoDialog.value = false;
  deliveryInfo.value = null;
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

// 计算订单总数量
const getTotalQuantity = (items: OrderItem[]) => {
  return items.reduce((total, item) => total + item.quantity, 0);
};

// 计算商品原始总价（不含优惠券，不含配送费）
const getSubtotal = (items: OrderItem[]) => {
  return items.reduce((total, item) => total + (item.totalPrice || 0), 0);
};

// 计算订单总金额（含优惠券折扣，不包含配送费）
// 用于总收入统计
const getTotalAmount = (items: OrderItem[], usedCoupon?: OrderCouponInfo) => {
  const subtotal = getSubtotal(items);
  
  // 如果没有优惠券，直接返回商品总价
  if (!usedCoupon) {
    return subtotal;
  }
  
  // 计算优惠金额（只针对商品总价，不包括配送费）
  let discountAmount = 0;
  
  if (usedCoupon.discountType === 'fixed') {
    // 满减券：discountValue 就是优惠金额
    discountAmount = usedCoupon.discountValue;
  } else if (usedCoupon.discountType === 'discount') {
    // 折扣券：discountValue 是折扣比例（0-1），计算优惠金额
    discountAmount = subtotal * (1 - usedCoupon.discountValue);
  }
  
  // 确保优惠金额不超过商品总价
  discountAmount = Math.min(discountAmount, subtotal);
  
  // 商品总价（含优惠券折扣，不含配送费）
  return Math.max(0, subtotal - discountAmount);
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
};

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  // 使用北京时间格式，精确到分钟
  return date.toLocaleString('zh-CN', {
    timeZone: 'Asia/Shanghai',
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false // 使用24小时制
  });
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
   background-color: #f59e0b;
   color: white;
   padding: 0.5rem 1rem;
   border-radius: 0.5rem;
   transition: all 0.2s;
 }
 .btn-primary:hover {
   background-color: #d97706;
 }

.btn-success {
  background-color: #f59e0b;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-success:hover {
  background-color: #d97706;
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
  background-color: #f59e0b;
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  transition: all 0.2s;
}
.btn-info:hover {
  background-color: #d97706;
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