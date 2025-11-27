<template>
  <!-- 购物车按钮 -->
  <div class="fixed right-10 bottom-10 z-10">
    <button @click="toggleCart"
      class="bg-[#F9771C] text-white w-14 h-14 rounded-xl shadow-lg hover:bg-orange-600 transition-colors flex items-center justify-center cursor-pointer !rounded-button whitespace-nowrap">
      <i class="fas fa-shopping-cart text-lg"></i>
      <span v-if="totalItems > 0"
        class="absolute -top-2 -right-2 bg-red-500 text-white text-xs w-6 h-6 rounded-full flex items-center justify-center">
        {{ totalItems }}
      </span>
    </button>
  </div>

  <!-- 购物车侧边栏 -->
  <div v-if="showCart" class="fixed inset-0 z-50 overflow-hidden" @click="showCart = false">
    <div class="absolute right-0 top-0 h-full w-96 bg-white shadow-xl" @click.stop>
      <div class="flex flex-col h-full">
        <div class="flex items-center justify-between p-4 border-b">
          <h3 class="text-lg font-semibold text-gray-900">购物车</h3>
          <button @click="showCart = false" class="text-gray-500 hover:text-gray-700 cursor-pointer">
            <i class="fas fa-times text-lg"></i>
          </button>
        </div>

        <div class="flex-1 overflow-y-auto p-4">
          <div v-if="cartItems.length === 0" class="text-center text-gray-500 mt-8">
            <i class="fas fa-shopping-cart text-4xl mb-4"></i>
            <p>购物车是空的</p>
          </div>
          <div v-else class="space-y-4">
            <div v-for="item in cartItems" :key="item.id" class="flex items-center space-x-3 bg-gray-50 p-3 rounded-lg">
              <img :src="normalizeImageUrl(item.image)" 
                   :alt="item.name"
                   class="w-12 h-12 object-cover rounded"
                   @error="(e) => (e.target as HTMLImageElement).src = fallbackImage" />
              <div class="flex-1 min-w-0">
                <h4 class="font-medium text-gray-900 text-sm truncate">{{ item.name }}</h4>
                <p class="text-[#F9771C] font-semibold text-sm">
                  ¥{{ item.price > 0 ? item.price.toFixed(2) : '-.--' }}
                </p>
              </div>
              <div class="flex items-center space-x-2 shrink-0">
                <button @click="emit('decrease', item)"
                  class="w-6 h-6 rounded-full bg-gray-200 flex items-center justify-center text-gray-600 hover:bg-gray-300 cursor-pointer">
                  <i class="fas fa-minus text-xs"></i>
                </button>
                <span class="w-6 text-center text-sm font-medium">{{ item.quantity }}</span>
                <button @click="emit('increase', item)"
                  class="w-6 h-6 rounded-full bg-[#F9771C] text-white flex items-center justify-center hover:bg-orange-600 cursor-pointer">
                  <i class="fas fa-plus text-xs"></i>
                </button>
              </div>
            </div>
            <div class="border-t p-4">
              <div class="flex items-center justify-between mb-4">
                <span class="text-lg font-semibold text-gray-900">总计</span>
                <span class="text-xl font-bold text-[#F9771C]">¥{{ totalPrice.toFixed(2) }}</span>
              </div>
              <button
                class="w-full bg-[#F9771C] text-white py-3 rounded-lg font-semibold hover:bg-orange-600 transition-colors cursor-pointer !rounded-button whitespace-nowrap"
                @click="goToChekout">
                去结算
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, defineProps, defineEmits } from 'vue'
import { useRouter } from 'vue-router'
import { normalizeImageUrl } from '@/utils/imageUtils';

import type { MenuItem, ShoppingCart } from '@/api/user'
import { getMenuItemPage } from '@/api/user'

const router = useRouter();
const props = defineProps<{
  cart: ShoppingCart;
  storeID: string;
  menuItems: MenuItem[];
}>();

const emit = defineEmits<{
  (e: 'increase', item: MenuItem): void;
  (e: 'decrease', item: MenuItem): void;
}>();

const showCart = ref(false);
const cartDishDetails = ref<Record<number, MenuItem>>({})
const loadingDishes = ref(false)

// 购物车里显示的菜品
const fallbackImage = 'https://via.placeholder.com/48x48?text=Dish'

// 当购物车打开时，加载缺失的菜品信息
watch(showCart, async (isOpen) => {
  if (!isOpen || !props.cart?.items?.length) return
  
  const missingDishIds = props.cart.items
    .filter(ci => !props.menuItems?.find(m => m.id === ci.dishId))
    .map(ci => ci.dishId)
  
  if (missingDishIds.length === 0) return
  
  // 加载缺失的菜品信息
  loadingDishes.value = true
  try {
    const response = await getMenuItemPage(props.storeID, { page: 1, pageSize: 1000 })
    const dishes = response?.items ?? []
    
    const dishMap: Record<number, MenuItem> = {}
    dishes.forEach(dish => {
      dishMap[dish.id] = dish
    })
    cartDishDetails.value = dishMap
  } catch (error) {
    console.warn('加载购物车菜品详情失败:', error)
  } finally {
    loadingDishes.value = false
  }
})

const cartItems = computed(() => {
  if (!props.cart || !props.cart.items) return []
  
  return props.cart.items.map(ci => {
    // 先从 menuItems 查找，再从 cartDishDetails 查找
    const dish = props.menuItems?.find(d => d.id === ci.dishId) ?? cartDishDetails.value[ci.dishId]
    
    // 计算单价：优先使用菜品价格，否则从购物车总价计算
    let price = 0
    if (dish?.price != null) {
      price = dish.price
    } else if (ci.totalPrice != null && ci.quantity > 0) {
      price = ci.totalPrice / ci.quantity
    }
    
    return {
      id: ci.dishId,
      name: dish?.name ?? `菜品 #${ci.dishId}`,
      image: dish?.image ?? fallbackImage,
      price,
      quantity: ci.quantity
    }
  })
})

// 购物车总价
const totalPrice = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.price * item.quantity, 0)
})

// 购物车总数量
const totalItems = computed(() => {
  return cartItems.value.reduce((sum, item) => sum + item.quantity, 0)
})

const toggleCart = () => {
  showCart.value = !showCart.value
}

function goToChekout() {
  router.push({
    name: 'Checkout', // 路由 name 对应 /checkout/:id
    params: { id: props.storeID },
  })
}
</script>