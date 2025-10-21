<template>
  <div>
    <div class="flex">
      <!-- 菜品展示 -->
      <DishIntro 
        :cart="cart" 
        :menuItems="menuItems" 
        @increase="increaseQuantity"
        @decrease="decreaseQuantity"
      />
    </div>

    <!-- 购物车 -->
    <ItemCart 
      :cart="cart" 
      :storeID="storeID"
      :menuItems="menuItems" 
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

import type { StoreInfo, MenuItem, ShoppingCart, ShoppingCartItem } from '@/api/user'
import { getStoreInfo, getMenuItem, getShoppingCart, addOrUpdateCartItem, removeCartItem } from '@/api/user'

import DishIntro from '@/components/user/StoreDetail/OrderView/DishIntro.vue'
import ItemCart from '@/components/user/StoreDetail/OrderView/ItemCart.vue'

// 路由
const route = useRoute()
const userStore = useUserStore();
const userID = userStore.getUserID();
const storeID = computed(() => route.params.id as string)

// 数据
const storeInfo = ref<StoreInfo>()
const menuItems = ref<MenuItem[]>([])
const cart = ref<ShoppingCart>({
  cartId: 3,
  totalPrice: 0,
  items: []
});  // 防止未定义

// 增加数量
async function increaseQuantity(dish: MenuItem) {
  const token = localStorage.getItem('authToken');
  if (!token) {
    // 用户未登录，提示登录
    ElMessage.warning('请先登录后再添加商品到购物车');
    return;
  }
  
  if (!cart.value) return;
  
  const item = cart.value.items.find(i => i.dishId === dish.id)
  const newQty = (item?.quantity ?? 0) + 1

  await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
  await loadCart()
}

// 减少数量
async function decreaseQuantity(dish: MenuItem) {
  const token = localStorage.getItem('authToken');
  if (!token) {
    ElMessage.warning('请先登录后再操作购物车');
    return;
  }
  
  if (!cart.value) return;
  
  const item = cart.value.items.find(i => i.dishId === dish.id)
  if (!item) return

  const newQty = item.quantity - 1
  if (newQty > 0) {
    await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
  } else {
    await removeCartItem(cart.value.cartId, dish.id)
  }
  await loadCart()
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
    
    const data = await getShoppingCart(storeID.value);
    cart.value = data ?? { cartId: 0, totalPrice: 0, items: [] };
  } catch (error) {
    console.warn('加载购物车失败，可能是用户未登录:', error);
    // 购物车加载失败时使用空购物车
    cart.value = { cartId: 0, totalPrice: 0, items: [] };
  }
}

// 获取数据
async function loadData(storeID: string) {
  storeInfo.value = await getStoreInfo(storeID)
  menuItems.value = await getMenuItem(storeID)
  await loadCart()
}

// 生命周期
onMounted(() => loadData(storeID.value))
watch(
  storeID, 
  (newID) => {
    if (newID) {
      loadData(newID);
    }
  },
  {
    immediate: true
  }
);
</script>