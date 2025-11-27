<template>
  <div class="min-h-screen bg-gray-100">
    <!-- 顶部导航栏 -->
    <div class="fixed top-0 left-0 right-0 bg-white z-20 overflow-hidden border-b-2 border-orange-200">
      <!-- 背景渐变层 -->
      <div class="absolute inset-0 bg-gradient-to-r from-orange-50 to-orange-100"></div>
      
      <!-- 背景装饰图标 -->
      <div class="absolute top-0 right-0 w-1/3 h-full opacity-10">
        <i class="fas fa-utensils text-orange-500 text-[200px] transform rotate-12 translate-x-1/4 -translate-y-1/4"></i>
      </div>

      <!-- 店铺信息 -->
      <div v-if="storeInfo" class="max-w-6xl mx-auto px-4 py-5 relative">
        <div class="flex items-center space-x-6">
          <div class="relative w-20 h-20 rounded-xl overflow-hidden bg-gray-100 flex items-center justify-center shadow-md border-2 border-white">
            <img :src="normalizeImageUrl(storeInfo.image)" alt="店铺图片" class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
          </div>
          <div class="flex-1">
            <div class="flex items-center space-x-3 mb-2">
              <h1 class="text-2xl font-bold text-gray-900">{{ storeInfo.name }}</h1>
              <div class="flex items-center px-2.5 py-1 bg-orange-500 bg-opacity-10 rounded-full">
                <i class="fas fa-crown text-orange-500 mr-1.5 text-xs"></i>
                <span class="text-orange-600 font-medium text-xs">优质商家</span>
              </div>
            </div>
            <div class="flex items-center space-x-5 text-xs">
              <div class="flex items-center bg-white px-2.5 py-1 rounded-full shadow-sm">
                <i class="fas fa-star text-yellow-400 mr-1.5"></i>
                <span class="font-medium text-gray-900">{{ storeInfo.rating }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <i class="fas fa-shopping-bag text-gray-500 mr-1.5"></i>
                <span>月售 {{ storeInfo.monthlySales }} 单</span>
              </div>
              <div class="flex items-center text-gray-700">
                <i class="fas fa-truck text-gray-500 mr-1.5"></i>
                <span>配送费 ¥{{ deliveryTask.deliveryFee }}</span>
              </div>
              <div class="flex items-center text-gray-700">
                <i class="fas fa-clock text-gray-500 mr-1.5"></i>
                <span>{{ deliveryTask.deliveryTime }} 分钟</span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <main class="max-w-6xl mx-auto px-4 pt-44 pb-12">
      <div class="flex gap-4">
        <!-- 左侧菜品列表 -->
        <div class="flex-1 bg-white rounded-xl shadow-sm border border-gray-100 flex flex-col" style="height: calc(100vh - 200px);">
          <div class="flex items-center justify-between mb-4 pb-3 border-b border-dashed border-gray-200 px-4 pt-4">
            <h2 class="text-base font-bold text-gray-900">已选菜品</h2>
            <span class="text-xs text-gray-500 bg-gray-50 px-2 py-1 rounded">共 {{ cartItems.reduce((acc, item) => acc + item.quantity, 0) }} 件</span>
          </div>

          <!-- 菜品列表 - 可滚动区域 -->
          <div class="flex-1 overflow-y-auto scrollbar-hide px-4">
            <div v-if="cartItems.length > 0" class="grid grid-cols-2 gap-3 pb-4">
            <DishCard
              v-for="item in cartItems"
              :key="item.id"
              :item="item"
              @updateQuantity="updateQuantity"
              @onRemove="removeItem"
            />
          </div>
            <div v-else class="flex flex-col items-center justify-center h-full">
              <i class="fas fa-shopping-cart text-4xl text-gray-300 mb-3"></i>
              <h3 class="text-sm font-semibold text-gray-400 mb-1">购物车是空的</h3>
              <p class="text-xs text-gray-400">快去选择您喜欢的菜品吧~</p>
            </div>
          </div>

          <!-- 底部操作按钮 -->
          <div v-if="cartItems.length > 0" class="border-t border-gray-100 px-4 py-3 flex gap-2">
            <button 
              @click="goBack"
              class="flex-1 bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer flex items-center justify-center gap-2 shadow-sm hover:shadow">
              <i class="fas fa-arrow-left"></i>
              <span>继续点餐</span>
            </button>
            <button 
              @click="clearCart"
              class="flex-1 bg-white border border-gray-200 hover:bg-gray-50 text-gray-600 px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer flex items-center justify-center gap-2 shadow-sm hover:shadow">
              <i class="fas fa-trash-alt"></i>
              <span>一键清空</span>
            </button>
          </div>
        </div>

        <!-- 右侧订单/支付区 -->
        <div class="space-y-4 w-80 shrink-0">
          <AddressSelector
            :selectedAddress="selectedAddress"
            @onAddressChange="onAddressChange"
          />
          <CouponSelector
            :totalAmount="subtotal"
            v-model:selectedCoupon="selectedCoupon"
            @onCouponChange="selectedCoupon = $event"
          />
          <PaymentSelector v-model:selectedMethod="paymentMethod" />
          <OrderSummary
            v-model:subtotal="subtotal"
            :selectedCoupon="selectedCoupon"
            :deliveryFee="deliveryFee"
            @checkout="checkout"
          />
        </div>
      </div>
    </main>

    <!-- 清空购物车确认弹窗 -->
    <div v-if="showClearConfirm" class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50" @click.self="showClearConfirm = false">
      <div class="bg-white rounded-2xl shadow-2xl p-6 max-w-sm mx-4 transform transition-all">
        <div class="text-center mb-6">
          <div class="w-16 h-16 bg-orange-100 rounded-full flex items-center justify-center mx-auto mb-4">
            <i class="fas fa-exclamation-triangle text-orange-500 text-2xl"></i>
          </div>
          <h3 class="text-lg font-bold text-gray-900 mb-2">清空购物车</h3>
          <p class="text-sm text-gray-600">确定要清空购物车中的所有菜品吗？</p>
        </div>
        <div class="flex gap-3">
          <button 
            @click="showClearConfirm = false"
            class="flex-1 bg-white border border-gray-200 hover:bg-gray-50 text-gray-700 px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer">
            取消
          </button>
          <button 
            @click="confirmClearCart"
            class="flex-1 bg-orange-500 hover:bg-orange-600 text-white px-4 py-2.5 rounded-full text-sm transition-colors cursor-pointer shadow-sm">
            确定清空
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useUserStore } from '@/stores/user';

import type { UserAddress as Address, MenuItem, ShoppingCart, CouponInfo, StoreInfo, DeliveryTask } from '@/api/user';
import { getMenuItem, getShoppingCart, addOrUpdateCartItem, removeCartItem, submitOrder, getDeliveryTasks, getStoreInfo } from '@/api/user';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';

import DishCard from '@/components/user/Checkout/DishCard.vue';
import AddressSelector from '@/components/user/Checkout/AddressSelector.vue';
import CouponSelector from '@/components/user/Checkout/CouponSelector.vue';
import PaymentSelector from '@/components/user/Checkout/PaymentSelector.vue';
import OrderSummary from '@/components/user/Checkout/OrderSummary.vue';

// 路由参数
const route = useRoute();
const router = useRouter();
const userStore = useUserStore();
const userID = userStore.getUserID();
const storeID = computed(() => route.params.id as string);
const deliveryTask = computed(() => getDeliveryTasks(storeID.value));
const deliveryFee = computed(() => deliveryTask.value.deliveryFee);

// 数据
const menuItems = ref<MenuItem[]>([]);
const storeInfo = ref<StoreInfo | null>(null);
const cart = ref<ShoppingCart>({
  cartId: 0,
  totalPrice: 0,
  items: [],
});

const selectedAddress = ref<Address>();
const selectedCoupon = ref < CouponInfo | null>(null);
const paymentMethod = ref('wechat');
const showClearConfirm = ref(false);

// 计算已选菜品
const cartItems = computed(() => {
  if (!cart.value || !cart.value.items) return [];
  return cart.value.items
    .map(ci => {
      const dish = menuItems.value.find(d => d.id === ci.dishId);
      return dish ? { ...dish, quantity: ci.quantity } : null;
    })
    .filter((item): item is MenuItem & { quantity: number } => item !== null);
});

const subtotal = computed(() =>
  cartItems.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
);

watch(
  storeID,
  (newID) => {
    // 只有当 newID 有一个有效值时才加载数据
    if (newID) {
      loadData(newID);
    }
  },
  { immediate: true } // 立即执行以处理首次加载
);

async function loadData(currentStoreID: string) {
  try {
    // 并行加载，速度更快
    const [menuItemsData, cartData, storeInfoData] = await Promise.all([
      getMenuItem(currentStoreID),
      getShoppingCart(currentStoreID, userID),
      getStoreInfo(currentStoreID)
    ]);
    
    menuItems.value = menuItemsData;
    cart.value = cartData;
    storeInfo.value = storeInfoData;

  } catch (error) {
    console.error("加载结算页面数据失败:", error);
  }
}

// 增减菜品数量
async function updateQuantity(dish: MenuItem, quantity: number) {
  if (!cart.value.cartId) return;
  if (quantity > 0) {
    await addOrUpdateCartItem(cart.value.cartId, dish.id, quantity);
  } else {
    await removeCartItem(cart.value.cartId, dish.id);
  }
  await refreshCart();
}

// 移除菜品
async function removeItem(dish: MenuItem) {
  if (!cart.value.cartId) return;
  await removeCartItem(cart.value.cartId, dish.id);
  await refreshCart();
}

// 支付结算
async function checkout() {
  if (!selectedAddress.value) {
    alert('请先选择收货地址');
    return;
  }
  if (cartItems.value.length === 0) {
    alert('购物车为空');
    return;
  }

  try {
    await submitOrder(
      userID, 
      cart.value.cartId, 
      Number(storeID.value), 
      selectedAddress.value.id, 
      deliveryFee.value,
      undefined,
      selectedCoupon.value?.couponID ?? null
    );
    // 重新加载购物车数据以获取清空后的状态
    cart.value = await getShoppingCart(storeID.value, userID);
    alert('下单成功！');
    goBack();
  } catch (error: any) {
    console.error('下单失败:', error);
    const errorMessage = error.response?.data?.message || error.message || '下单失败，请重试';
    alert(errorMessage);
  }
}

// 处理地址变更
function onAddressChange(address: Address) {
  selectedAddress.value = address;
}

console.log(deliveryFee);

function goBack() {
  router.back();
}

// 清空购物车
async function clearCart() {
  if (!cart.value.cartId || cartItems.value.length === 0) return;
  showClearConfirm.value = true;
}

// 确认清空购物车
async function confirmClearCart() {
  showClearConfirm.value = false;
  
  try {
    // 逐个删除购物车中的菜品
    for (const item of cartItems.value) {
      await removeCartItem(cart.value.cartId, item.id);
    }
    await refreshCart();
  } catch (error) {
    console.error('清空购物车失败:', error);
    alert('清空购物车失败，请重试');
  }
}

async function refreshCart() {
  if (!storeID.value) return;
  try {
    cart.value = await getShoppingCart(storeID.value, userID);
  } catch (error) {
    console.error('刷新购物车失败:', error);
  }
}


</script>

<style scoped>
/* Hide scrollbar for Chrome, Safari and Opera */
.scrollbar-hide::-webkit-scrollbar {
    display: none;
}

/* Hide scrollbar for IE, Edge and Firefox */
.scrollbar-hide {
    -ms-overflow-style: none;  /* IE and Edge */
    scrollbar-width: none;  /* Firefox */
}
</style>
