<template>
  <Layout>
    <!-- 配券中心 -->
    <div>
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-800">配券中心</h2>
        <div class="flex items-center space-x-3">
          <el-button @click="openCreateForm" type="primary" :icon="Plus">
            新建优惠券
          </el-button>
        </div>
      </div>

      <!-- 统计卡片 -->
      <div class="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div class="bg白e rounded-lg shadow-sm p-6 border-l-4 border-[#F9771C]">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-500">总优惠券</p>
              <p class="text-2xl font-bold">{{ stats.total || 0 }}</p>
            </div>
            <el-icon class="text-[#F9771C] text-3xl">
              <Collection />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border-l-4 border-green-500">
          <div class="flex items中心 justify-between">
            <div>
              <p class="text-sm text-gray-500">有效优惠券</p>
              <p class="text-2xl font-bold">{{ stats.active || 0 }}</p>
            </div>
            <el-icon class="text-green-500 text-3xl">
              <Check />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border-l-4 border-yellow-500">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-500">未开始</p>
              <p class="text-2xl font-bold">{{ stats.upcoming || 0 }}</p>
            </div>
            <el-icon class="text-yellow-500 text-3xl">
              <Clock />
            </el-icon>
          </div>
        </div>

        <div class="bg-white rounded-lg shadow-sm p-6 border-l-4 border-red-500">
          <div class="flex items-center justify-between">
            <div>
              <p class="text-sm text-gray-500">已过期</p>
              <p class="text-2xl font-bold">{{ stats.expired || 0 }}</p>
            </div>
            <el-icon class="text-red-500 text-3xl">
              <Close />
            </el-icon>
          </div>
        </div>
      </div>

      <!-- 优惠券列表 -->
      <div class="bg-white rounded-lg shadow-sm">
        <div class="p-6 border-b border-gray-200">
          <div class="flex items-center justify-between">
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
          <div v-if="!loading && coupons.length > 0" class="space-y-4">
            <div
              v-for="coupon in filteredCoupons"
              :key="coupon.id"
              class="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow"
            >
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center space-x-4">
                  <div class="bg-[#F9771C] text-white px-4 py-2 rounded-lg">
                    <p class="text-lg font-bold">
                      {{ coupon.type === 'discount' ? `${coupon.value.toFixed(1)}折` : `¥${coupon.value}` }}
                    </p>
                    <p class="text-xs">{{ coupon.type === 'discount' ? '折扣券' : '满减券' }}</p>
                  </div>
                  <div>
                    <h4 class="font-semibold text-gray-800">{{ coupon.name }}</h4>
                    <p class="text-sm text-gray-500">{{ coupon.description }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <el-tag :type="getStatusType(coupon.status)" size="small">
                    {{ getStatusText(coupon.status) }}
                  </el-tag>
                  <p class="text-sm text-gray-500 mt-1">{{ formatTime(coupon.startTime) }}</p>
                </div>
              </div>

              <!-- 优惠券详情 -->
              <div class="grid grid-cols-2 gap-4 mb-4">
                <div>
                  <p class="text-sm text-gray-600">使用条件</p>
                  <p class="font-medium">满¥{{ coupon.minAmount || 0 }}可用</p>
                </div>
                <div>
                  <p class="text-sm text-gray-600">有效期</p>
                  <p class="font-medium">{{ formatDateRange(coupon.startTime, coupon.endTime) }}</p>
                </div>
              </div>

              <!-- 使用统计 -->
              <div class="grid grid-cols-3 gap-4 mb-4">
                <div class="text-center">
                  <p class="text-sm text-gray-600">发放数量</p>
                  <p class="font-semibold">{{ coupon.totalQuantity || 0 }}</p>
                </div>
                <div class="text-center">
                  <p class="text-sm text-gray-600">已使用</p>
                  <p class="font-semibold">{{ coupon.usedQuantity || 0 }}</p>
                </div>
                <div class="text-center">
                  <p class="text-sm text-gray-600">剩余</p>
                  <p class="font-semibold">{{ (coupon.totalQuantity || 0) - (coupon.usedQuantity || 0) }}</p>
                </div>
              </div>

              <!-- 操作按钮 -->
              <div class="flex items-center justify-end space-x-2">
                <el-button 
                  v-if="coupon.status === 'upcoming'"
                  @click="activateCoupon(coupon.id)"
                  type="primary"
                  size="small"
                >
                  启用
                </el-button>
                <el-button 
                  v-if="coupon.status === 'active'"
                  @click="deactivateCoupon(coupon.id)"
                  type="warning"
                  size="small"
                >
                  停用
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

          <!-- 空状态 -->
          <div v-else-if="!loading && coupons.length === 0" class="text-center py-12">
            <el-icon class="text-gray-400 text-6xl mb-4">
              <Collection />
            </el-icon>
            <p class="text-gray-500 text-lg">暂无优惠券</p>
            <p class="text-gray-400 text-sm">点击"新建优惠券"创建您的第一张优惠券</p>
          </div>

          <!-- 加载状态 -->
          <div v-else-if="loading" class="text-center py-12">
            <el-icon class="text-gray-400 text-6xl mb-4 animate-spin">
              <Loading />
            </el-icon>
            <p class="text-gray-500">加载优惠券中...</p>
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

// 停用优惠券
const deactivateCoupon = async (couponId: number) => {
  try {
    await ElMessageBox.confirm('确定要停用这个优惠券吗？', '确认停用', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    // TODO: 调用真实API停用优惠券
    ElMessage.success('优惠券已停用');
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
</style>

