<template>
  <Layout>
    <div class="min-h-screen bg-gray-100 py-10">
      <div class="max-w-[1400px] mx-auto px-8 space-y-6">
        <div class="coupon-top flex flex-col gap-4">
          <div class="flex justify-between items-center">
            <h2 class="text-2xl font-bold text-gray-800">配券中心</h2>
            <div class="flex items-center space-x-3">
              <el-button @click="openCreateForm" type="primary" :icon="Plus">
                新建优惠券
              </el-button>
            </div>
          </div>

          <!-- 统计卡片 -->
          <div class="coupon-stats overflow-x-auto mb-2 pb-1 scrollbar-hide">
            <div class="flex gap-5 min-w-[1050px]">
              <div class="stat-card border border-orange-200">
                <div class="stat-card__accent bg-orange-100"></div>
                <div class="flex items-center justify-between relative">
                  <div>
                    <p class="text-xs text-gray-500">总优惠券</p>
                    <p class="text-3xl font-extrabold text-gray-900">{{ stats.total || 0 }}</p>
                    <p class="text-[11px] text-gray-400 mt-1">累计创建数量</p>
                  </div>
                  <div class="stat-icon bg-orange-50 text-[#F9771C]">
                    <Collection />
                  </div>
                </div>
              </div>

              <div class="stat-card border border-green-200">
                <div class="stat-card__accent bg-green-100"></div>
                <div class="flex items-center justify-between relative">
                  <div>
                    <p class="text-xs text-gray-500">有效优惠券</p>
                    <p class="text-3xl font-extrabold text-gray-900">{{ stats.active || 0 }}</p>
                    <p class="text-[11px] text-gray-400 mt-1">正在投放中</p>
                  </div>
                  <div class="stat-icon bg-green-50 text-green-500">
                    <Check />
                  </div>
                </div>
              </div>

              <div class="stat-card border border-yellow-200">
                <div class="stat-card__accent bg-yellow-100"></div>
                <div class="flex items-center justify-between relative">
                  <div>
                    <p class="text-xs text-gray-500">未开始</p>
                    <p class="text-3xl font-extrabold text-gray-900">{{ stats.upcoming || 0 }}</p>
                    <p class="text-[11px] text-gray-400 mt-1">等待生效</p>
                  </div>
                  <div class="stat-icon bg-yellow-50 text-yellow-500">
                    <Clock />
                  </div>
                </div>
              </div>

              <div class="stat-card border border-red-200">
                <div class="stat-card__accent bg-red-100"></div>
                <div class="flex items-center justify-between relative">
                  <div>
                    <p class="text-xs text-gray-500">已过期</p>
                    <p class="text-3xl font-extrabold text-gray-900">{{ stats.expired || 0 }}</p>
                    <p class="text-[11px] text-gray-400 mt-1">自动下线数量</p>
                  </div>
                  <div class="stat-icon bg-red-50 text-red-500">
                    <Close />
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>

        <div class="coupon-layout">
          <div class="coupon-main space-y-4">
            <!-- 优惠券列表 -->
            <div class="bg-white rounded-lg shadow-sm border border-gray-100">
              <div class="p-6 border-b border-gray-200">
                <div class="flex items-center justify-between flex-wrap gap-4">
                  <h3 class="text-lg font-semibold text-gray-800">优惠券列表</h3>
                  <div class="flex items-center space-x-4">
                    <el-select v-model="selectedStatus" placeholder="筛选状态" @change="filterCoupons">
                      <el-option label="全部" value="all" />
                      <el-option label="有效" value="active" />
                      <el-option label="未开始" value="upcoming" />
                      <el-option label="已过期" value="expired" />
                    </el-select>
                    <el-button @click="refreshCoupons" :loading="loading" icon="Refresh" />
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

                <!-- 优惠券列表 -->
                <template v-if="!loading">
                  <template v-if="coupons.length > 0">
                    <div class="space-y-5">
                      <div
                        v-for="coupon in filteredCoupons"
                        :key="coupon.id"
                        class="coupon-card"
                      >
                        <div class="relative">
                          <div class="flex flex-col md:flex-row md:items-start md:justify-between gap-4">
                            <div class="flex items-start gap-4 flex-1">
                              <div class="coupon-badge-wrapper">
                                <div class="coupon-badge">
                                  <p class="text-2xl font-extrabold leading-tight">
                                    {{ coupon.type === 'discount' ? `${coupon.value.toFixed(1)}折` : `¥${formatAmount(coupon.value)}` }}
                                  </p>
                                  <p class="text-xs opacity-80 mt-1">{{ coupon.type === 'discount' ? '折扣券' : '满减券' }}</p>
                                </div>
                              </div>
                              <div class="space-y-2 flex-1 min-w-0">
                              <h4 class="coupon-title" :title="coupon.name">
                                {{ coupon.name }}
                              </h4>
                                <p class="text-sm text-gray-500 leading-relaxed text-left w-full" :title="coupon.description || '暂无描述'">
                                  {{ coupon.description || '暂无描述' }}
                                </p>
                                <div class="flex flex-wrap gap-2 text-xs text-gray-600">
                                  <span class="tag-chip bg-orange-50 text-orange-600">满¥{{ coupon.minAmount || 0 }}可用</span>
                                  <span class="tag-chip bg-gray-100 text-gray-600">{{ coupon.type === 'discount' ? '折扣券' : '满减券' }}</span>
                                  <span class="tag-chip bg-blue-50 text-blue-600">共 {{ coupon.totalQuantity || 0 }} 张</span>
                                </div>
                              </div>
                            </div>
                            <div class="text-right space-y-2 pt-8 md:pt-0">
                              <div class="text-xs text-gray-500 leading-5">
                                <p>生效：{{ formatTime(coupon.startTime) }}</p>
                                <p>结束：{{ formatTime(coupon.endTime) }}</p>
                              </div>
                            </div>
                          </div>
                          <div class="coupon-status-tag">
                            <el-tag :type="getStatusType(coupon.status)" size="small" effect="dark">
                              {{ getStatusText(coupon.status) }}
                            </el-tag>
                          </div>
                        </div>

                        <div class="mt-5 grid grid-cols-1 md:grid-cols-3 gap-4">
                          <div class="info-box">
                            <p class="info-label">使用条件</p>
                            <p class="info-value">订单满 ¥{{ formatAmount(coupon.minAmount || 0) }} 可用</p>
                            <p class="info-sub">店内堂食/外卖均可使用</p>
                          </div>
                          <div class="info-box">
                            <p class="info-label">有效期</p>
                            <p class="info-value">{{ formatDateRange(coupon.startTime, coupon.endTime) }}</p>
                            <p class="info-sub">可配合店内其他活动</p>
                          </div>
                          <div class="info-box">
                            <p class="info-label">领取限制</p>
                            <p class="info-value">单用户最多 1 张</p>
                            <p class="info-sub">领取后过期自动退回名额</p>
                          </div>
                        </div>

                        <div class="mt-5 grid grid-cols-1 md:grid-cols-2 gap-6">
                          <div>
                            <div class="flex items-center justify-between text-sm text-gray-600">
                              <span>发放数量</span>
                              <span class="font-semibold text-gray-900">{{ coupon.totalQuantity || 0 }}</span>
                            </div>
                            <div class="flex items-center justify-between text-sm text-gray-600 mt-2">
                              <span>已使用</span>
                              <span class="font-semibold text-gray-900">{{ coupon.usedQuantity || 0 }}</span>
                            </div>
                            <div class="flex items-center justify-between text-sm text-gray-600 mt-2">
                              <span>剩余</span>
                              <span class="font-semibold text-gray-900">{{ Math.max((coupon.totalQuantity || 0) - (coupon.usedQuantity || 0), 0) }}</span>
                            </div>
                          </div>
                          <div>
                            <div class="flex items-center justify-between text-sm text-gray-600 mb-2">
                              <span>使用率</span>
                              <span class="font-semibold text-gray-900">{{ getUsagePercent(coupon) }}%</span>
                            </div>
                            <div class="progress-track">
                              <div class="progress-bar" :style="{ width: `${getUsagePercent(coupon)}%` }"></div>
                            </div>
                            <p class="text-xs text-gray-400 mt-2">实时统计领取/核销情况，帮助及时调整投放策略</p>
                          </div>
                        </div>

                        <div class="flex flex-wrap items-center justify-end gap-2 pt-4 mt-4 border-t border-dashed border-gray-200">
                          <el-button 
                            v-if="coupon.status === 'upcoming'"
                            @click="activateCoupon(coupon.id)"
                            type="primary"
                            size="small"
                          >
                            启用
                          </el-button>
                          <el-button @click="editCoupon(coupon)" size="small">
                            编辑
                          </el-button>
                          <el-button @click="deleteCouponItem(coupon.id)" type="danger" size="small">
                            删除
                          </el-button>
                        </div>
                      </div>
                    </div>
                  </template>
                  <template v-else>
                    <div class="text-center py-12">
                      <el-icon class="text-gray-400 text-6xl mb-4">
                        <Collection />
                      </el-icon>
                      <p class="text-gray-500 text-lg">暂无优惠券</p>
                      <p class="text-gray-400 text-sm">点击"新建优惠券"创建您的第一张优惠券</p>
                    </div>
                  </template>
                </template>
                <template v-else>
                  <div class="text-center py-12">
                    <el-icon class="text-gray-400 text-6xl mb-4 animate-spin">
                      <Loading />
                    </el-icon>
                    <p class="text-gray-500">加载优惠券中...</p>
                  </div>
                </template>
              </div>
            </div>
          </div>

          <!-- 右侧辅助信息 -->
          <div class="coupon-aside space-y-4">
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-bolt text-orange-500"></i>
                快捷操作
              </h3>
              <div class="space-y-2">
                <button
                  @click="openCreateForm"
                  class="w-full bg-orange-50 hover:bg-orange-100 text-orange-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between"
                >
                  <span>创建新优惠券</span>
                  <i class="fas fa-chevron-right text-orange-400"></i>
                </button>
                <button
                  @click="selectedStatus = 'active'; filterCoupons()"
                  class="w-full bg-green-50 hover:bg-green-100 text-green-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between"
                >
                  <span>查看正在投放</span>
                  <i class="fas fa-chevron-right text-green-400"></i>
                </button>
                <button
                  @click="selectedStatus = 'expired'; filterCoupons()"
                  class="w-full bg-red-50 hover:bg-red-100 text-red-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between"
                >
                  <span>清理已过期</span>
                  <i class="fas fa-chevron-right text-red-400"></i>
                </button>
              </div>
            </div>

            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-lightbulb text-yellow-500"></i>
                投放建议
              </h3>
              <div class="space-y-3 text-xs text-gray-600">
                <div class="flex items-start gap-2">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 shrink-0"></i>
                  <p>保持至少 2 张有效优惠券，满足不同消费层级</p>
                </div>
                <div class="flex items-start gap-2">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 shrink-0"></i>
                  <p>针对午晚高峰设置短期折扣券，提升转化率</p>
                </div>
                <div class="flex items-start gap-2">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 shrink-0"></i>
                  <p>监控核销率，及时调整优惠力度与发放数量</p>
                </div>
              </div>
            </div>

            <div class="monthly-card rounded-xl shadow-sm border border-gray-100 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-4 flex items-center gap-2">
                <i class="fas fa-chart-line text-blue-500"></i>
                本月投放概况
              </h3>
              <div class="space-y-3 text-xs text-gray-600">
                <div class="flex items-center justify-between">
                  <span>在投优惠券</span>
                  <span class="font-semibold text-gray-900">{{ stats.active || 0 }} 张</span>
                </div>
                <div class="flex items-center justify-between">
                  <span>待上线</span>
                  <span class="font-semibold text-gray-900">{{ stats.upcoming || 0 }} 张</span>
                </div>
                <div class="flex items-center justify-between">
                  <span>已下线</span>
                  <span class="font-semibold text-gray-900">{{ stats.expired || 0 }} 张</span>
                </div>
                <div class="h-px bg-gray-100"></div>
                <p class="monthly-tip text-[11px] text-gray-400 leading-relaxed">
                  建议每周至少复盘一次核销数据，结合商家活动及时调整优惠方案。
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 创建/编辑优惠券弹窗 -->
    <el-dialog
      v-model="showCreateForm"
      :title="editingCoupon ? '编辑优惠券' : '新建优惠券'"
      width="600px"
      @close="closeCreateForm"
    >
      <el-form :model="couponForm" :rules="couponRules" ref="couponFormRef" label-width="100px">
        <el-form-item label="优惠券名称" prop="name">
          <el-input v-model="couponForm.name" placeholder="请输入优惠券名称" />
        </el-form-item>
        
        <el-form-item label="优惠券描述" prop="description">
          <el-input 
            v-model="couponForm.description" 
            type="textarea" 
            :rows="3"
            placeholder="请输入优惠券描述" 
          />
        </el-form-item>

        <el-form-item label="优惠类型" prop="couponType">
          <el-radio-group v-model="couponForm.couponType">
            <el-radio value="fixed">满减券</el-radio>
            <el-radio value="percentage">折扣券</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item :label="couponForm.couponType === 'percentage' ? '优惠折扣' : '优惠金额'" prop="discountAmount">
          <el-input-number 
            v-model="couponForm.discountAmount" 
            :min="couponForm.couponType === 'percentage' ? 0.1 : 1" 
            :max="couponForm.couponType === 'percentage' ? 10 : 9999"
            :precision="couponForm.couponType === 'percentage' ? 1 : 2"
            :suffix="couponForm.couponType === 'percentage' ? '折' : '元'"
          />
        </el-form-item>

        <el-form-item label="最低消费" prop="minimumSpend">
          <el-input-number 
            v-model="couponForm.minimumSpend" 
            :min="0" 
            :precision="2"
            suffix="元"
          />
        </el-form-item>

        <el-form-item label="发放数量" prop="totalQuantity">
          <el-input-number 
            v-model="couponForm.totalQuantity" 
            :min="1" 
            :max="10000"
          />
        </el-form-item>

        <el-form-item label="有效期" prop="dateRange">
          <div class="date-range-container">
            <el-date-picker
              v-model="startDate"
              type="datetime"
              placeholder="开始时间"
              format="YYYY-MM-DD HH:mm"
              value-format="YYYY-MM-DD HH:mm:ss"
              :disabled-date="disabledDate"
              style="width: 48%; margin-right: 4%"
            />
            <el-date-picker
              v-model="endDate"
              type="datetime"
              placeholder="结束时间"
              format="YYYY-MM-DD HH:mm"
              value-format="YYYY-MM-DD HH:mm:ss"
              :disabled-date="disabledEndDate"
              style="width: 48%"
            />
          </div>
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeCreateForm">取消</el-button>
          <el-button type="primary" @click="saveCoupon" :loading="saving">
            {{ editingCoupon ? '更新' : '创建' }}
          </el-button>
        </div>
      </template>
    </el-dialog>
  </Layout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { 
  Plus, 
  Collection, 
  Check, 
  Clock, 
  Close, 
  Refresh, 
  Loading 
} from '@element-plus/icons-vue';

import Layout from '@/features/merchant/components/Layout.vue';
import { devLog } from '@/utils/logger';
import { getCoupons, createCoupon, updateCoupon, deleteCoupon, getCouponStats, type CouponInfo, type CreateCouponRequest } from '@/api/merchant';

// 响应式数据
const coupons = ref<CouponInfo[]>([]);
const selectedStatus = ref('all');
const loading = ref(false);
const saving = ref(false);
const errorMessage = ref('');
const showCreateForm = ref(false);
const editingCoupon = ref<CouponInfo | null>(null);

// 分离的日期选择器
const startDate = ref('');
const endDate = ref('');

// 优惠券表单
const couponForm = ref({
  name: '',
  description: '',
  couponType: 'fixed' as 'fixed' | 'percentage',
  discountAmount: 0,
  minimumSpend: 0,
  totalQuantity: 100,
  dateRange: [] as string[]
});

// 表单验证规则
const couponRules = {
  name: [{ required: true, message: '请输入优惠券名称', trigger: 'blur' }],
  description: [{ required: true, message: '请输入优惠券描述', trigger: 'blur' }],
  discountAmount: [{ 
    required: true, 
    message: computed(() => couponForm.value.couponType === 'percentage' ? '请输入优惠折扣' : '请输入优惠金额'), 
    trigger: 'blur' 
  }],
  minimumSpend: [{ required: true, message: '请输入最低消费', trigger: 'blur' }],
  totalQuantity: [{ required: true, message: '请输入发放数量', trigger: 'blur' }],
  dateRange: [{ required: true, message: '请选择有效期', trigger: 'change' }]
};

// 统计信息
const stats = ref({
  total: 0,
  active: 0,
  upcoming: 0,
  expired: 0
});

// 计算属性
const filteredCoupons = computed(() => {
  if (selectedStatus.value === 'all') {
    return coupons.value;
  }
  return coupons.value.filter(coupon => {
    return coupon.status === selectedStatus.value;
  });
});

// 加载优惠券数据
const loadCoupons = async () => {
  try {
    loading.value = true;
    errorMessage.value = '';
    
    const response = await getCoupons(1, 100);
    console.log('API响应:', response);
    
    // 确保响应结构正确
    if (response && Array.isArray(response.list)) {
      coupons.value = response.list;
    } else if (Array.isArray(response)) {
      // 如果API直接返回数组
      coupons.value = response;
    } else {
      console.warn('API响应格式不正确:', response);
      coupons.value = [];
    }
    
    calculateStats();
    devLog.component('MerchantCoupons', '优惠券数据加载成功');
  } catch (error) {
    devLog.error('加载优惠券失败:', error);
    errorMessage.value = '加载优惠券失败，请重试';
    coupons.value = [];
    calculateStats(); // 确保统计信息被重置
  } finally {
    loading.value = false;
  }
};

// 计算统计信息
const calculateStats = () => {
  const couponsList = coupons.value || [];
  
  stats.value = {
    total: couponsList.length,
    active: couponsList.filter(coupon => coupon.status === 'active').length,
    upcoming: couponsList.filter(coupon => coupon.status === 'upcoming').length,
    expired: couponsList.filter(coupon => coupon.status === 'expired').length
  };
};

// 筛选优惠券
const filterCoupons = () => {
  devLog.component('MerchantCoupons', `筛选优惠券状态: ${selectedStatus.value}`);
};

// 刷新优惠券
const refreshCoupons = async () => {
  await loadCoupons();
};

// 重试加载
const retryLoad = async () => {
  errorMessage.value = '';
  await loadCoupons();
};

// 打开创建表单
const openCreateForm = () => {
  editingCoupon.value = null;
  couponForm.value = {
    name: '',
    description: '',
    couponType: 'fixed',
    discountAmount: 0,
    minimumSpend: 0,
    totalQuantity: 100,
    dateRange: []
  };
  // 清空日期选择器
  startDate.value = '';
  endDate.value = '';
  showCreateForm.value = true;
};

// 关闭创建表单
const closeCreateForm = () => {
  showCreateForm.value = false;
  editingCoupon.value = null;
};


// 禁用日期（允许选择任何日期）
const disabledDate = (time: Date) => {
  return false; // 不禁用任何日期
};

// 禁用结束日期（不能早于开始日期）
const disabledEndDate = (time: Date) => {
  if (startDate.value) {
    const start = new Date(startDate.value);
    return time.getTime() < start.getTime();
  }
  return false; // 如果没有开始日期，不禁用任何日期
};

// 监听日期变化，同步到表单
watch([startDate, endDate], ([newStart, newEnd]) => {
  if (newStart && newEnd) {
    couponForm.value.dateRange = [newStart, newEnd];
  } else {
    couponForm.value.dateRange = [];
  }
});

// 编辑优惠券
const editCoupon = (coupon: CouponInfo) => {
  editingCoupon.value = coupon;
  
  couponForm.value = {
    name: coupon.name,
    description: coupon.description || '',
    couponType: coupon.type === 'discount' ? 'percentage' : 'fixed',
    discountAmount: coupon.value,
    minimumSpend: coupon.minAmount || 0,
    totalQuantity: coupon.totalQuantity,
    dateRange: [coupon.startTime, coupon.endTime]
  };
  
  // 设置日期选择器的值
  startDate.value = coupon.startTime;
  endDate.value = coupon.endTime;
  
  showCreateForm.value = true;
};

// 保存优惠券
const saveCoupon = async () => {
  try {
    saving.value = true;
    
    const couponData: CreateCouponRequest = {
      name: couponForm.value.name,
      description: couponForm.value.description,
      couponType: couponForm.value.couponType,
      minimumSpend: couponForm.value.minimumSpend,
      discountAmount: couponForm.value.discountAmount,
      totalQuantity: couponForm.value.totalQuantity,
      validFrom: couponForm.value.dateRange[0],
      validTo: couponForm.value.dateRange[1]
    };
    
    
    if (editingCoupon.value) {
      await updateCoupon(editingCoupon.value.id, couponData);
      ElMessage.success('优惠券更新成功');
    } else {
      await createCoupon(couponData);
      ElMessage.success('优惠券创建成功');
    }
    
    closeCreateForm();
    await loadCoupons();
  } catch (error) {
    ElMessage.error('保存失败，请重试');
  } finally {
    saving.value = false;
  }
};

// 启用优惠券
const activateCoupon = async (couponId: number) => {
  try {
    await ElMessageBox.confirm('确定要启用这个优惠券吗？', '确认启用', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    // TODO: 调用真实API启用优惠券
    ElMessage.success('优惠券已启用');
    await loadCoupons();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('操作失败，请重试');
    }
  }
};

// 删除优惠券
const deleteCouponItem = async (couponId: number) => {
  try {
    await ElMessageBox.confirm('确定要删除这个优惠券吗？删除后不可恢复！', '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await deleteCoupon(couponId);
    ElMessage.success('优惠券已删除');
    await loadCoupons();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败，请重试');
    }
  }
};

// 获取状态类型
const getStatusType = (status: string) => {
  const statusMap: Record<string, string> = {
    active: 'success',
    upcoming: 'warning',
    expired: 'danger',
    inactive: 'info'
  };
  return statusMap[status] || 'info';
};

// 获取状态文本
const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    active: '有效',
    upcoming: '未开始',
    expired: '已过期',
    inactive: '已停用'
  };
  return statusMap[status] || status;
};

// 格式化时间
const formatTime = (time: string) => {
  return new Date(time).toLocaleString('zh-CN');
};

// 格式化日期范围
const formatDateRange = (startTime: string, endTime: string) => {
  const start = new Date(startTime).toLocaleDateString('zh-CN');
  const end = new Date(endTime).toLocaleDateString('zh-CN');
  return `${start} - ${end}`;
};

const formatAmount = (value?: number) => {
  if (value === undefined || value === null) return '0';
  return Number.isInteger(value) ? `${value}` : Number(value).toFixed(2);
};

const getUsagePercent = (coupon: CouponInfo) => {
  const total = coupon.totalQuantity || 0;
  if (!total) return 0;
  const used = coupon.usedQuantity || 0;
  return Math.min(100, Math.max(0, Math.round((used / total) * 100)));
};

// 初始化数据
onMounted(() => {
  loadCoupons();
});
</script>

<style scoped>
/* 日期范围容器样式 */
.date-range-container {
  display: flex;
  width: 100%;
  gap: 4%;
}

/* 确保日期选择器面板在弹窗中正确显示 */
:deep(.el-picker-panel) {
  z-index: 9999 !important;
}

/* 确保日期选择器面板的确定按钮可以正常点击 */
:deep(.el-picker-panel__footer) {
  z-index: 10000 !important;
}

/* 确保日期选择器面板的按钮区域可以正常点击 */
:deep(.el-picker-panel__footer .el-button) {
  z-index: 10001 !important;
  position: relative;
}

.scrollbar-hide {
  -ms-overflow-style: none;
  scrollbar-width: none;
}

.scrollbar-hide::-webkit-scrollbar {
  display: none;
}

.stat-card {
  position: relative;
  min-width: 250px;
  background: #fff;
  border-radius: 1.25rem;
  padding: 1.6rem;
  box-shadow: 0 12px 28px rgba(15, 23, 42, 0.05);
  overflow: hidden;
}

.stat-card__accent {
  position: absolute;
  inset: auto -1rem -1rem auto;
  width: 120px;
  height: 120px;
  border-radius: 9999px;
  filter: blur(20px);
  opacity: 0.45;
}

.stat-icon {
  width: 52px;
  height: 52px;
  border-radius: 9999px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.5rem;
  box-shadow: inset 0 0 0 1px rgba(255, 255, 255, 0.4);
}

.coupon-card {
  background: #fff;
  border-radius: 1.25rem;
  padding: 1.75rem;
  border: 1px solid #f1f5f9;
  box-shadow: 0 20px 35px rgba(15, 23, 42, 0.05);
  transition: box-shadow 0.2s ease, transform 0.2s ease;
}

.coupon-card:hover {
  box-shadow: 0 25px 45px rgba(15, 23, 42, 0.08);
  transform: translateY(-2px);
}

.coupon-badge {
  min-width: 120px;
  background: linear-gradient(135deg, #f9771c, #ffb347);
  color: #fff;
  padding: 1rem;
  border-radius: 1rem;
  text-align: center;
  box-shadow: 0 10px 25px rgba(249, 119, 28, 0.35);
}

.coupon-badge-wrapper {
  width: 130px;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: flex-start;
}

.coupon-status-tag {
  position: absolute;
  top: 0;
  right: 0;
}

.coupon-title {
  width: 100%;
  text-align: left;
  font-size: 1.25rem;
  font-weight: 800;
  letter-spacing: 0.02em;
  line-height: 1.5;
  color: #f97316;
  background: linear-gradient(120deg, rgba(249, 119, 28, 0.18), rgba(255, 255, 255, 0));
  padding: 0.35rem 0.75rem;
  border-radius: 0.85rem;
  box-shadow: inset 0 -1px 0 rgba(249, 119, 28, 0.25);
  display: inline-block;
  text-shadow: 0 1px 1px rgba(249, 119, 28, 0.2);
}

.monthly-card {
  background: linear-gradient(135deg, rgba(59, 130, 246, 0.1), rgba(249, 115, 22, 0.08));
  border-color: rgba(59, 130, 246, 0.25);
}

.monthly-tip {
  text-align: left;
  color: #475569;
}

.tag-chip {
  padding: 0.3rem 0.8rem;
  border-radius: 999px;
  font-weight: 500;
}

.info-box {
  border: 1px solid #f1f5f9;
  border-radius: 1rem;
  padding: 1rem;
  background: #f8fafc;
}

.info-label {
  font-size: 0.8rem;
  color: #64748b;
  margin-bottom: 0.25rem;
}

.info-value {
  font-size: 1rem;
  font-weight: 600;
  color: #0f172a;
}

.info-sub {
  font-size: 0.75rem;
  color: #94a3b8;
  margin-top: 0.2rem;
}

.progress-track {
  width: 100%;
  height: 8px;
  border-radius: 999px;
  background: #e2e8f0;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, #f9771c, #facc15);
  border-radius: 999px;
  transition: width 0.3s ease;
}

.coupon-top {
  max-width: 1100px;
  width: 100%;
  margin: 0 auto;
}

.coupon-top > div:first-child {
  flex-wrap: wrap;
  gap: 1rem;
}

.coupon-stats {
  padding-left: 1rem;
  padding-right: 1rem;
}

.coupon-layout {
  display: flex;
  gap: 28px;
  align-items: flex-start;
  flex-wrap: wrap;
  justify-content: center;
  width: 100%;
}

.coupon-main {
  flex: 0 0 760px;
  max-width: 100%;
}

.coupon-aside {
  flex: 0 0 300px;
  position: sticky;
  top: 96px;
}

@media (max-width: 1023px) {
  .coupon-layout {
    flex-direction: column;
  }

  .coupon-main,
  .coupon-aside {
    flex: 0 0 auto;
    width: 100%;
    position: static;
  }
}
</style>

