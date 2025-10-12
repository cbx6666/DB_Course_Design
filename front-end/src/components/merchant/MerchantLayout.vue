<!-- src/components/merchant/MerchantLayout.vue -->
<template>
  <div class="min-h-screen bg-gray-50">
    <!-- 顶部导航栏 -->
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
            <img
              :src="merchantInfo.avatar || defaultAvatar"
              alt="商家头像" 
              class="w-10 h-10 rounded-full object-cover"
              @error="handleImageError"
            />
            <span class="text-gray-700 font-medium">{{ merchantInfo.username || '加载中...' }}</span>
          </div>
        </div>
      </div>
    </header>

    <div class="flex pt-16">
      <!-- 左侧导航菜单 -->
      <aside class="fixed left-0 top-16 bottom-0 w-52 bg-white shadow-sm overflow-y-auto">
        <nav class="p-4">
          <div class="space-y-2">
            <div 
              v-for="(item, index) in menuItems" 
              :key="index" 
              @click="handleMenuClick(item)" 
              :class="{
                'bg-orange-50 text-[#F9771C] border-r-3 border-[#F9771C]': $route.name === item.routeName,
                'text-gray-700 hover:bg-gray-50': $route.name !== item.routeName
              }"
              class="flex items-center px-4 py-3 rounded-l-lg cursor-pointer transition-colors whitespace-nowrap !rounded-button"
            >
              <el-icon class="mr-3 text-lg">
                <component :is="item.icon" />
              </el-icon>
              <span class="font-medium">{{ item.label }}</span>
            </div>
          </div>
        </nav>

        <div class="p-4 border-t border-gray-100">
          <div 
            @click="handleLogout"
            class="flex items-center px-4 py-3 rounded-lg cursor-pointer transition-colors text-red-500 hover:bg-red-50"
          >
            <el-icon class="mr-3 text-lg">
              <SwitchButton />
            </el-icon>
            <span class="font-medium">退出登录</span>
          </div>
        </div>
      </aside>

      <!-- 主内容区 -->
      <main class="ml-52 flex-1 p-6">
        <slot />
      </main>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { ElMessage, ElMessageBox } from 'element-plus';
import { 
  Bell, 
  House, 
  List, 
  Ticket, 
  Warning, 
  User, 
  SwitchButton 
} from '@element-plus/icons-vue';

import { getProjectName } from '@/stores/name';
import loginApi from '@/api/login_api';
import { removeToken } from '@/utils/jwt';
import { handleImageError } from '@/utils/errorHandler';
import { API_CONFIG } from '@/config';
import { useMerchantLayout } from '@/composables/useMerchantLayout';

// 定义菜单项类型
interface MenuItem {
  key: string;
  label: string;
  icon: any;
  routeName: string;
}

// 定义商家信息类型
interface MerchantInfo {
  username?: string;
  avatar?: string;
}

// 使用商家布局组合式函数
const { merchantInfo } = useMerchantLayout();

// 响应式数据
const router = useRouter();
const $route = useRoute();
const useProjectName = getProjectName();
const projectName = useProjectName.projectName;

// 默认头像
const defaultAvatar = `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;

// 菜单配置
const menuItems: MenuItem[] = [
  { key: 'overview', label: '店铺概况', icon: House, routeName: 'MerchantHome' },
  { key: 'orders', label: '订单中心', icon: List, routeName: 'MerchantOrders' },
  { key: 'coupons', label: '配券中心', icon: Ticket, routeName: 'MerchantCoupons' },
  { key: 'aftersale', label: '订单售后', icon: Warning, routeName: 'MerchantAftersale' },
  { key: 'profile', label: '商家信息', icon: User, routeName: 'MerchantProfile' }
];

// 菜单点击处理
const handleMenuClick = (menuItem: MenuItem) => {
  router.push({ name: menuItem.routeName });
};

// 退出登录
const handleLogout = async () => {
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
};
</script>
