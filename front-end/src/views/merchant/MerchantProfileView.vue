<!-- eslint-disable -->
<!-- The exported code uses Tailwind CSS. Install Tailwind CSS in your dev environment to ensure all styles work. -->

<template>
  <MerchantLayout>
    <!-- 商家信息 -->
    <div>
          <h2 class="text-2xl font-bold text-gray-800 mb-6">个人信息</h2>
          <div class="bg-white rounded-lg shadow-sm p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">基本信息</h3>
            <div class="grid grid-cols-2 gap-6">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">ID</label>
                <p class="text-gray-900 py-2">{{ merchantInfo.id }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">姓名</label>
                <p class="text-gray-900 py-2">{{ merchantInfo.name }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">手机号</label>
                <div class="flex items-center">
                  <input v-model="merchantInfo.phone" :disabled="!isEditingPhone" :class="{
                      'w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-[#F9771C] text-sm': true,
                      'bg-gray-100 cursor-not-allowed': !isEditingPhone,
                      'bg-white': isEditingPhone
                    }" />
                  <button @click="toggleEdit('phone')"
                    class="ml-2 text-[#F9771C] hover:text-[#E16A0E] transition-colors">
                    <el-icon v-if="!isEditingPhone">
                      <Edit />
                    </el-icon>
                    <el-icon v-else>
                      <Check />
                    </el-icon>
                  </button>
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">电子邮箱</label>
                <div class="flex items-center">
                  <input v-model="merchantInfo.email" type="email" :disabled="!isEditingEmail" :class="{
                      'w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-[#F9771C] text-sm': true,
                      'bg-gray-100 cursor-not-allowed': !isEditingEmail,
                      'bg-white': isEditingEmail
                    }" />
                  <button @click="toggleEdit('email')"
                    class="ml-2 text-[#F9771C] hover:text-[#E16A0E] transition-colors">
                    <el-icon v-if="!isEditingEmail">
                      <Edit />
                    </el-icon>
                    <el-icon v-else>
                      <Check />
                    </el-icon>
                  </button>
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">注册时间</label>
                <p class="text-gray-900 py-2">{{ merchantInfo.registerTime }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">账号状态</label>
                <p :class="{'text-green-600': merchantInfo.status === '正常营业', 'text-red-600': merchantInfo.status === '封禁中'}"
                  class="font-medium py-2">
                  {{ merchantInfo.status }}
                </p>
              </div>
            </div>
          </div>
    </div>
  </MerchantLayout>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { Edit, Check } from '@element-plus/icons-vue';
import apiClient from '@/api/client';

// 布局组件
import MerchantLayout from '@/components/merchant/MerchantLayout.vue';

const isSaving = ref(false);
const isEditingPhone = ref(false);
const isEditingEmail = ref(false);

const merchantInfo = ref({
  id: '加载中...',
  name: '加载中...',
  phone: '加载中...',
  email: '加载中...',
  registerTime: '加载中...',
  status: '正常营业'
});

const toggleEdit = (field: 'phone' | 'email') => {
  if (field === 'phone') {
    isEditingPhone.value = !isEditingPhone.value;
    if (!isEditingPhone.value) {
      saveShopInfo();
    }
  } else if (field === 'email') {
    isEditingEmail.value = !isEditingEmail.value;
    if (!isEditingEmail.value) {
      saveShopInfo();
    }
  }
};

// 获取商家信息
const fetchMerchantInfo = async () => {
  try {
    const response = await apiClient.get('/merchant/profile');
    merchantInfo.value = response.data.data;
  } catch (error) {
    ElMessage.error('获取商家信息失败');
    console.error('Error fetching merchant info:', error);
  }
};

// 保存商家信息
const saveShopInfo = async () => {
  if (isSaving.value) return;
  isSaving.value = true;
  
  try {
    const { phone, email } = merchantInfo.value;
    const response = await apiClient.put('/merchant/profile', {
      phone,
      email
    });
    
    if (response.data.success) {
      ElMessage.success('商家信息更新成功');
      fetchMerchantInfo(); // 重新获取最新数据
    } else {
      ElMessage.error(response.data.message || '更新失败');
    }
  } catch (error) {
    ElMessage.error('保存失败，请重试');
    console.error('Error saving merchant info:', error);
  } finally {
    isSaving.value = false;
  }
};

// 组件挂载时获取商家信息
onMounted(() => {
  fetchMerchantInfo();
});


</script>

<style scoped>
.\!rounded-button {
  border-radius: 8px;
}
input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
input[type="number"] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>