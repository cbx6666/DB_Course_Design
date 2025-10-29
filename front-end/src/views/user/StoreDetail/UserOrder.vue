<template>
  <div class="flex">
    <!-- 左侧分类菜单 -->
    <div class="w-48 bg-white border-r border-gray-200 overflow-y-auto" style="max-height: calc(100vh - 200px);">
      <div class="p-2">
        <div 
          v-for="category in categories" 
          :key="category.id"
          @click="selectCategory(category.id)"
          :class="[
            'px-4 py-3 mb-2 rounded-lg cursor-pointer transition-all',
            selectedCategoryId === category.id 
              ? 'bg-[#F9771C] text-white' 
              : 'bg-gray-50 hover:bg-gray-100 text-gray-700'
          ]"
        >
          <span class="text-sm font-medium">{{ category.name }}</span>
        </div>
      </div>
    </div>

    <!-- 右侧菜品展示 -->
    <div class="flex-1">
      <DishIntro 
        :cart="cart" 
        :menuItems="filteredMenuItems" 
        @increase="increaseQuantity"
        @decrease="decreaseQuantity"
      />
    </div>

    <!-- 购物车 -->
    <ItemCart 
      v-if="storeID"
      :cart="cart" 
      :storeID="storeID"
      :menuItems="allMenuItems" 
      @increase="increaseQuantity"
      @decrease="decreaseQuantity"
    />
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ElMessage } from 'element-plus'

import type { StoreInfo, MenuItem, ShoppingCart, ShoppingCartItem, Category } from '@/api/user'
import { getStoreInfo, getMenuItem, getShoppingCart, addOrUpdateCartItem, removeCartItem, getStoreCategories } from '@/api/user'

import DishIntro from '@/components/user/StoreDetail/OrderView/DishIntro.vue'
import ItemCart from '@/components/user/StoreDetail/OrderView/ItemCart.vue'

// 路由
const route = useRoute()
const userStore = useUserStore();
const userID = userStore.getUserID();
const storeID = ref<string>('')

// 数据
const storeInfo = ref<StoreInfo>()
const categories = ref<Category[]>([])
const selectedCategoryId = ref<number | null>(null)
const allMenuItems = ref<MenuItem[]>([])

const cart = ref<ShoppingCart>({
  cartId: 3,
  totalPrice: 0,
  items: []
});  // 防止未定义

// 请求锁：防止快速点击导致重复添加
const pendingRequests = ref<Set<number>>(new Set());

// 过滤后的菜品列表
const filteredMenuItems = computed(() => {
  // 如果没有分类，显示所有菜品
  if (categories.value.length === 0) {
    return allMenuItems.value
  }
  // 如果没有选中分类但有分类数据，显示第一个分类的菜品
  if (selectedCategoryId.value === null && categories.value.length > 0) {
    return allMenuItems.value.filter(item => item.categoryId === categories.value[0].id)
  }
  // 根据选中的分类ID过滤菜品
  return allMenuItems.value.filter(item => item.categoryId === selectedCategoryId.value)
})

// 选择分类
function selectCategory(categoryId: number) {
  selectedCategoryId.value = categoryId
}

// 增加数量
async function increaseQuantity(dish: MenuItem) {
  const token = localStorage.getItem('authToken');
  if (!token) {
    // 用户未登录，提示登录
    ElMessage.warning('请先登录后再添加商品到购物车');
    return;
  }
  
  if (!cart.value) return;
  
  // 检查是否已有该菜品的待处理请求
  if (pendingRequests.value.has(dish.id)) {
    console.log('请求正在处理中，请稍候...');
    return;
  }
  
  try {
    // 添加到待处理集合
    pendingRequests.value.add(dish.id);
    
    const item = cart.value.items.find(i => i.dishId === dish.id)
    const newQty = (item?.quantity ?? 0) + 1

    await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
    await loadCart()
  } finally {
    // 请求完成后从待处理集合中移除
    pendingRequests.value.delete(dish.id);
  }
}

// 减少数量
async function decreaseQuantity(dish: MenuItem) {
  const token = localStorage.getItem('authToken');
  if (!token) {
    ElMessage.warning('请先登录后再操作购物车');
    return;
  }
  
  if (!cart.value) return;
  
  // 检查是否已有该菜品的待处理请求
  if (pendingRequests.value.has(dish.id)) {
    console.log('请求正在处理中，请稍候...');
    return;
  }
  
  try {
    // 添加到待处理集合
    pendingRequests.value.add(dish.id);
    
    const item = cart.value.items.find(i => i.dishId === dish.id)
    if (!item) return

    const newQty = item.quantity - 1
    if (newQty > 0) {
      await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
    } else {
      await removeCartItem(cart.value.cartId, dish.id)
    }
    await loadCart()
  } finally {
    // 请求完成后从待处理集合中移除
    pendingRequests.value.delete(dish.id);
  }
}

// 读取购物车
async function loadCart() {
  try {
    // 检查用户是否已登录
    const token = localStorage.getItem('authToken');
    if (!token) {
      // 用户未登录，使用空的购物车
      cart.value = { cartId: 0, totalPrice: 0, items: [] };
      return;
    }
    
    if (!storeID.value) return
    
    const data = await getShoppingCart(storeID.value);
    cart.value = data ?? { cartId: 0, totalPrice: 0, items: [] };
  } catch (error) {
    console.warn('加载购物车失败，可能是用户未登录:', error);
    // 购物车加载失败时使用空购物车
    cart.value = { cartId: 0, totalPrice: 0, items: [] };
  }
}

// 获取数据
async function loadData(storeId: string) {
  storeInfo.value = await getStoreInfo(storeId)
  allMenuItems.value = await getMenuItem(storeId)
  categories.value = await getStoreCategories(storeId)
  
  // 如果有分类，默认选中第一个
  if (categories.value.length > 0) {
    selectedCategoryId.value = categories.value[0].id
  }
  
  await loadCart()
}

// 生命周期
onMounted(() => {
  const id = route.params.id as string
  if (id) {
    storeID.value = id
    loadData(id)
  }
})

watch(
  () => route.params.id,
  (newId) => {
    if (newId && typeof newId === 'string') {
      storeID.value = newId
      loadData(newId)
    }
  }
)
</script>