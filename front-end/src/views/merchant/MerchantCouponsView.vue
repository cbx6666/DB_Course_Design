<template>
  <MerchantLayout>
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
        <div class="bg-white rounded-lg shadow-sm p-6 border-l-4 border-[#F9771C]">
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
          <div class="flex items-center justify-between">
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
              :key="coupon.couponId"
              class="border border-gray-200 rounded-lg p-6 hover:shadow-md transition-shadow"
            >
              <div class="flex items-center justify-between mb-4">
                <div class="flex items-center space-x-4">
                  <div class="bg-[#F9771C] text-white px-4 py-2 rounded-lg">
                    <p class="text-lg font-bold">¥{{ coupon.discountAmount }}</p>
                    <p class="text-xs">{{ coupon.discountType === 'percentage' ? '折扣券' : '满减券' }}</p>
                  </div>
                  <div>
                    <h4 class="font-semibold text-gray-800">{{ coupon.title }}</h4>
                    <p class="text-sm text-gray-500">{{ coupon.description }}</p>
                  </div>
                </div>
                <div class="text-right">
                  <el-tag :type="getStatusType(coupon.status)" size="small">
                    {{ getStatusText(coupon.status) }}
                  </el-tag>
                  <p class="text-sm text-gray-500 mt-1">{{ formatTime(coupon.createTime) }}</p>
                </div>
              </div>

              <!-- 优惠券详情 -->
              <div class="grid grid-cols-2 gap-4 mb-4">
                <div>
                  <p class="text-sm text-gray-600">使用条件</p>
                  <p class="font-medium">满¥{{ coupon.minOrderAmount }}可用</p>
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
                  @click="activateCoupon(coupon.couponId)"
                  type="primary"
                  size="small"
                >
                  启用
                </el-button>
                <el-button 
                  v-if="coupon.status === 'active'"
                  @click="deactivateCoupon(coupon.couponId)"
                  type="warning"
                  size="small"
                >
                  停用
                </el-button>
                <el-button @click="editCoupon(coupon)" size="small">
                  编辑
                </el-button>
                <el-button @click="deleteCoupon(coupon.couponId)" type="danger" size="small">
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
        <el-form-item label="优惠券标题" prop="title">
          <el-input v-model="couponForm.title" placeholder="请输入优惠券标题" />
        </el-form-item>
        
        <el-form-item label="优惠券描述" prop="description">
          <el-input 
            v-model="couponForm.description" 
            type="textarea" 
            :rows="3"
            placeholder="请输入优惠券描述" 
          />
        </el-form-item>

        <el-form-item label="优惠类型" prop="discountType">
          <el-radio-group v-model="couponForm.discountType">
            <el-radio value="fixed">满减券</el-radio>
            <el-radio value="percentage">折扣券</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item label="优惠金额" prop="discountAmount">
          <el-input-number 
            v-model="couponForm.discountAmount" 
            :min="1" 
            :max="couponForm.discountType === 'percentage' ? 100 : 9999"
            :precision="couponForm.discountType === 'percentage' ? 0 : 2"
            :suffix="couponForm.discountType === 'percentage' ? '%' : '元'"
          />
        </el-form-item>

        <el-form-item label="最低消费" prop="minOrderAmount">
          <el-input-number 
            v-model="couponForm.minOrderAmount" 
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
          <el-date-picker
            v-model="couponForm.dateRange"
            type="datetimerange"
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            format="YYYY-MM-DD HH:mm"
            value-format="YYYY-MM-DD HH:mm:ss"
          />
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
  </MerchantLayout>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
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

import MerchantLayout from '@/components/merchant/MerchantLayout.vue';
import { devLog } from '@/utils/logger'; // Updated logger with error method

// 定义优惠券类型
interface Coupon {
  couponId: string;
  title: string;
  description: string;
  discountType: 'fixed' | 'percentage';
  discountAmount: number;
  minOrderAmount: number;
  totalQuantity: number;
  usedQuantity: number;
  startTime: string;
  endTime: string;
  createTime: string;
  status: 'active' | 'upcoming' | 'expired' | 'inactive';
}

// 响应式数据
const coupons = ref<Coupon[]>([]);
const selectedStatus = ref('all');
const loading = ref(false);
const saving = ref(false);
const errorMessage = ref('');
const showCreateForm = ref(false);
const editingCoupon = ref<Coupon | null>(null);

// 优惠券表单
const couponForm = ref({
  title: '',
  description: '',
  discountType: 'fixed' as 'fixed' | 'percentage',
  discountAmount: 0,
  minOrderAmount: 0,
  totalQuantity: 100,
  dateRange: [] as string[]
});

// 表单验证规则
const couponRules = {
  title: [{ required: true, message: '请输入优惠券标题', trigger: 'blur' }],
  description: [{ required: true, message: '请输入优惠券描述', trigger: 'blur' }],
  discountAmount: [{ required: true, message: '请输入优惠金额', trigger: 'blur' }],
  minOrderAmount: [{ required: true, message: '请输入最低消费', trigger: 'blur' }],
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
  return coupons.value.filter(coupon => coupon.status === selectedStatus.value);
});

// 加载优惠券数据
const loadCoupons = async () => {
  try {
    loading.value = true;
    errorMessage.value = '';
    
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // 模拟数据
    coupons.value = [
      {
        couponId: '1',
        title: '新用户专享',
        description: '新用户首次下单立减10元',
        discountType: 'fixed',
        discountAmount: 10,
        minOrderAmount: 30,
        totalQuantity: 1000,
        usedQuantity: 150,
        startTime: '2024-01-01 00:00:00',
        endTime: '2024-12-31 23:59:59',
        createTime: '2024-01-01 00:00:00',
        status: 'active'
      },
      {
        couponId: '2',
        title: '满减优惠',
        description: '满50减5元',
        discountType: 'fixed',
        discountAmount: 5,
        minOrderAmount: 50,
        totalQuantity: 500,
        usedQuantity: 80,
        startTime: '2024-01-01 00:00:00',
        endTime: '2024-12-31 23:59:59',
        createTime: '2024-01-01 00:00:00',
        status: 'active'
      }
    ];
    
    calculateStats();
    devLog.component('MerchantCoupons', '优惠券数据加载成功');
  } catch (error) {
    devLog.error('加载优惠券失败:', error);
    errorMessage.value = '加载优惠券失败，请重试';
    coupons.value = [];
  } finally {
    loading.value = false;
  }
};

// 计算统计信息
const calculateStats = () => {
  stats.value = {
    total: coupons.value.length,
    active: coupons.value.filter(c => c.status === 'active').length,
    upcoming: coupons.value.filter(c => c.status === 'upcoming').length,
    expired: coupons.value.filter(c => c.status === 'expired').length
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
    title: '',
    description: '',
    discountType: 'fixed',
    discountAmount: 0,
    minOrderAmount: 0,
    totalQuantity: 100,
    dateRange: []
  };
  showCreateForm.value = true;
};

// 关闭创建表单
const closeCreateForm = () => {
  showCreateForm.value = false;
  editingCoupon.value = null;
};

// 编辑优惠券
const editCoupon = (coupon: Coupon) => {
  editingCoupon.value = coupon;
  couponForm.value = {
    title: coupon.title,
    description: coupon.description,
    discountType: coupon.discountType,
    discountAmount: coupon.discountAmount,
    minOrderAmount: coupon.minOrderAmount,
    totalQuantity: coupon.totalQuantity,
    dateRange: [coupon.startTime, coupon.endTime]
  };
  showCreateForm.value = true;
};

// 保存优惠券
const saveCoupon = async () => {
  try {
    saving.value = true;
    
    // 模拟API调用
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    if (editingCoupon.value) {
      ElMessage.success('优惠券更新成功');
    } else {
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
const activateCoupon = async (couponId: string) => {
  try {
    await ElMessageBox.confirm('确定要启用这个优惠券吗？', '确认启用', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    // 模拟API调用
    ElMessage.success('优惠券已启用');
    await loadCoupons();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('操作失败，请重试');
    }
  }
};

// 停用优惠券
const deactivateCoupon = async (couponId: string) => {
  try {
    await ElMessageBox.confirm('确定要停用这个优惠券吗？', '确认停用', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    // 模拟API调用
    ElMessage.success('优惠券已停用');
    await loadCoupons();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('操作失败，请重试');
    }
  }
};

// 删除优惠券
const deleteCoupon = async (couponId: string) => {
  try {
    await ElMessageBox.confirm('确定要删除这个优惠券吗？删除后不可恢复！', '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    // 模拟API调用
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
