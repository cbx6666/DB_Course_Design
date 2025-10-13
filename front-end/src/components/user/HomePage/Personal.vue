<template>
    <button @click="showUserPanel = !showUserPanel"
        class="w-10 h-10 bg-orange-500 rounded-lg flex items-center justify-center text-white hover:bg-orange-600 transition-colors cursor-pointer">
        <i class="fas fa-user"></i>
    </button>

    <div
      v-if="showUserPanel"
      class="fixed inset-0"
      @click="showUserPanel = false"
    >
      <div
        class="absolute right-0 top-0 h-full w-80 bg-white shadow-xl"
        @click.stop
      >
        <div class="p-6">
          <!-- 用户信息 -->
          <div class="flex items-center space-x-4 mb-8">
            <!-- 头像显示 -->
            <div class="w-16 h-16 rounded-full overflow-hidden bg-gray-200 flex items-center justify-center">
              <img 
                v-if="userInfo.image && userInfo.image !== ''" 
                :src="userInfo.image" 
                alt="用户头像" 
                class="w-full h-full object-cover"
                @error="handleImageError"
              />
              <div 
                v-else
                class="w-full h-full bg-orange-500 flex items-center justify-center text-white text-xl font-bold"
              >
                {{ userInfo.name ? userInfo.name.charAt(0) : '?' }}
              </div>
            </div>
            <div>
              <h3 class="font-bold text-lg">{{ userInfo.name || '未设置姓名' }}</h3>
              <p class="text-gray-600 text-sm">{{ userInfo.phoneNumber ? userInfo.phoneNumber : '未设置手机号' }}</p>
            </div>
          </div>
          <!-- 功能菜单 -->
          <div class="space-y-2">
            <button
              v-for="(menu, index) in userMenus"
              :key="index"
              class="w-full flex items-center justify-between p-3 hover:bg-gray-50 rounded-lg transition-colors cursor-pointer"
              @click="menu.action"
              >
              <div class="flex items-center space-x-3">
                <i :class="menu.icon" class="text-gray-600"></i>
                <span>{{ menu.label }}</span>
              </div>
              <i class="fas fa-chevron-right text-gray-400"></i>
            </button>
          </div>
          <!-- 退出登录 -->
          <button
            class="w-full mt-8 bg-red-500 hover:bg-red-600 text-white py-3 rounded-lg font-medium transition-colors cursor-pointer whitespace-nowrap"
            @click="showExitForm=true"
          >
            退出登录
          </button>

          <AddrSetting v-model:showAddressForm="showAddressForm" />
          <CouponSetting v-model:showCouponForm="showCouponForm" />
          <AccountSetting 
            v-model:showAccountForm="showAccountForm" 
            @update:account="handleAccountUpdate"
          />
          <ExitAccount v-model:showExitForm="showExitForm" />
        </div>
      </div>
    </div>

</template>

<script setup lang="ts">
import { onMounted, reactive, ref } from "vue";

import { useUserStore } from "@/stores/user";
import { getUserInfo } from "@/api/user_home";
import { restoreUserState, getCurrentUserId } from "@/utils/auth";
import { handleImageError } from "@/utils/errorHandler";
import { devLog } from "@/utils/logger";

import AddrSetting from "./PersonalTransition/AddrSetting.vue";
import CouponSetting from "./PersonalTransition/CouponSetting.vue";
import AccountSetting from "./PersonalTransition/AccountSetting.vue";
import ExitAccount from "./PersonalTransition/ExitAccount.vue";

const userStore = useUserStore();

const userInfo = ref({
  name: "",
  phoneNumber: 0,
  image: '',
  defaultAddress: ""
});

onMounted(async () => {
  try {
    // 恢复用户状态
    restoreUserState();
    
    // 获取当前用户ID
    const currentUserID = getCurrentUserId();
    
    if (currentUserID > 0) {
      await loadUserInfo();
    }
  } catch (error) {
    console.error('获取用户信息失败:', error);
  }
});

// 加载用户信息
async function loadUserInfo() {
  try {
    const result = await getUserInfo();
    if (result) {
      // 确保头像URL是完整的URL
      if (result.image && !result.image.startsWith('http') && !result.image.startsWith('data:')) {
        result.image = `http://localhost:5250${result.image}`;
      }
      userInfo.value = result;
    }
  } catch (error) {
    console.error('加载用户信息失败:', error);
  }
}

// 处理账户更新
function handleAccountUpdate(updatedAccount: any) {
  // 更新本地用户信息
  userInfo.value.name = updatedAccount.name;
  userInfo.value.image = updatedAccount.image;
  // 重新加载用户信息以确保数据同步
  loadUserInfo();
}

const showUserPanel = ref(false);
const showAddressForm = ref(false);
const showCouponForm = ref(false);
const showAccountForm = ref(false);
const showExitForm = ref(false);

// 用户菜单
const userMenus = [
  { icon: "fas fa-map-marker-alt", label: "默认地址管理", action: ()=>openForm(showAddressForm) },
  { icon: "fas fa-ticket-alt", label: "我的优惠券", action: ()=>openForm(showCouponForm) },
  { icon: "fas fa-cog", label: "账户设置", action: ()=>openForm(showAccountForm) }
];

function openForm(f: { value: Boolean }) {
  f.value = true;
}

// 使用统一的图片错误处理函数

</script>