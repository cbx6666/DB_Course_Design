<!-- eslint-disable -->
<!-- The exported code uses Tailwind CSS. Install Tailwind CSS in your dev environment to ensure all styles work. -->

<template>
  <Layout>
    <!-- 商家信息 -->
    <div>
          <h2 class="text-2xl font-bold text-gray-800 mb-6">个人信息</h2>
  <div class="bg-white rounded-lg shadow-sm p-6">
            <h3 class="text-lg font-semibold text-gray-800 mb-4">基本信息</h3>
            <div class="grid grid-cols-2 gap-6">
              <div class="col-span-2">
                <label class="block text-sm font-medium text-gray-700 mb-2">头像</label>
                <div class="flex flex-col items-center justify-center space-y-3">
                  <div class="w-20 h-20 rounded-full overflow-hidden bg-gray-100 flex items-center justify-center">
                    <MerchantAvatar :src="merchantInfo.avatar" :size="80" />
                  </div>
                  <el-upload :show-file-list="false" :before-upload="beforeAvatarUpload" :http-request="uploadAvatar">
                    <el-button type="primary">上传/更换头像</el-button>
                  </el-upload>
                </div>
                <p class="text-xs text-gray-500 mt-2">支持 JPG/PNG，大小不超过 2MB。</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">真实姓名</label>
                <p class="text-gray-900 py-2">{{ merchantInfo.fullName }}</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">用户昵称</label>
                <div class="flex items-center">
                  <input v-model="merchantInfo.username" :disabled="!isEditingUsername" :class="{
                      'w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-[#F9771C] text-sm': true,
                      'bg-gray-100 cursor-not-allowed': !isEditingUsername,
                      'bg-white': isEditingUsername
                    }" />
                  <button @click="toggleEdit('username')"
                    class="ml-2 text-[#F9771C] hover:text-[#E16A0E] transition-colors">
                    <el-icon v-if="!isEditingUsername">
                      <Edit />
                    </el-icon>
                    <el-icon v-else>
                      <Check />
                    </el-icon>
                  </button>
                </div>
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
  </Layout>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import { Edit, Check } from '@element-plus/icons-vue';
import apiClient from '@/api/client';
import { API_CONFIG } from '@/config';
import { useMerchantLayout } from '@/composables/useMerchantLayout';

// 布局组件
import Layout from '@/features/merchant/components/Layout.vue';
import MerchantAvatar from '@/features/merchant/components/MerchantAvatar.vue';

const isSaving = ref(false);
const isEditingUsername = ref(false);
const isEditingPhone = ref(false);
const isEditingEmail = ref(false);

const merchantInfo = ref({
  username: '加载中...',
  fullName: '加载中...',
  phone: '加载中...',
  email: '加载中...',
  registerTime: '加载中...',
  status: '正常营业',
  avatar: ''
});

// 引入布局共享的商家信息刷新方法，用于更新右上角头像
const { merchantInfo: layoutMerchantInfo, fetchMerchantInfo: refreshLayoutMerchantInfo } = useMerchantLayout();

const toggleEdit = (field: 'username' | 'phone' | 'email') => {
  if (field === 'username') {
    isEditingUsername.value = !isEditingUsername.value;
    if (!isEditingUsername.value) {
      saveShopInfo();
    }
  } else if (field === 'phone') {
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
    // 规范化头像 URL（将后端返回的相对路径转换为可访问的绝对地址）
    if (merchantInfo.value && (merchantInfo.value as any).avatar) {
      const u = (merchantInfo.value as any).avatar as string;
      (merchantInfo.value as any).avatar = normalizeAssetUrl(u);
    }
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
    const { username, phone, email } = merchantInfo.value;
    const response = await apiClient.put('/merchant/profile', {
      name: username,  // 后端期望的字段名是 name，对应 Username
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

// 头像上传
const beforeAvatarUpload = (file: File) => {
  const isAllowedType = file.type === 'image/jpeg' || file.type === 'image/png';
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isAllowedType) {
    ElMessage.error('图片格式仅支持 JPG/PNG');
  }
  if (!isLt2M) {
    ElMessage.error('图片大小不能超过 2MB');
  }
  return isAllowedType && isLt2M;
};

const uploadAvatar = async (options: any) => {
  try {
    const file: File = options.file as File;
    const formData = new FormData();
    formData.append('AvatarFile', file);

    const resp = await apiClient.put('/merchant/profile/avatar', formData, {
      headers: { 'Content-Type': 'multipart/form-data' }
    });

    const newUrl: string | undefined = resp?.data?.avatar;
    if (!newUrl) throw new Error('上传成功但未返回图片地址');

    merchantInfo.value.avatar = normalizeAssetUrl(newUrl);
    // 立即同步到布局共享状态，保证右上角头像即时更新
    if (layoutMerchantInfo?.value) {
      (layoutMerchantInfo.value as any).avatar = merchantInfo.value.avatar;
    }
    ElMessage.success('头像已更新');

    // 同步刷新头部布局的商家信息（右上角头像）
    try {
      await refreshLayoutMerchantInfo();
    } catch (e) {
      // 忽略刷新失败（不阻塞头像更新提示）
      console.warn('Failed to refresh merchant info after avatar upload', e);
    }
  } catch (error) {
    ElMessage.error('头像上传失败，请重试');
  }
};

// 将后端返回的资源相对路径转换为绝对 URL（指向后端 BASE_URL）
function normalizeAssetUrl(u?: string): string {
  if (!u) return '';
  if (u.startsWith('http://') || u.startsWith('https://')) return u;
  if (u.startsWith('/')) return `${API_CONFIG.BASE_URL}${u}`;
  return `${API_CONFIG.BASE_URL}/${u}`;
}

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

