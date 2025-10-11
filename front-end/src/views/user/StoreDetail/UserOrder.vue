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

import type { StoreInfo } from '@/api/user_store_info'
import type { MenuItem, ShoppingCart, ShoppingCartItem } from '@/api/user_checkout'
import { getStoreInfo } from '@/api/user_store_info'
import { getMenuItem, getShoppingCart, addOrUpdateCartItem, removeCartItem } from '@/api/user_checkout'

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
  const item = cart.value.items.find(i => i.dishId === dish.id)
  const newQty = (item?.quantity ?? 0) + 1

  await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
  await loadCart()
}

// 减少数量
async function decreaseQuantity(dish: MenuItem) {
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
  const data = await getShoppingCart(storeID.value)
  cart.value = data ?? { cartId: 0, totalPrice: 0, items: [] }
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