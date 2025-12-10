<template>
   <div class="store-order-page bg-gray-100 min-h-screen pt-4 pb-8 relative overflow-hidden">
     <div class="store-order-bg pointer-events-none" aria-hidden="true"></div>
    <div class="store-order-layout relative z-10 max-w-[1200px] mx-auto px-4 gap-5">
    <!-- 左侧分类菜单 -->
      <div class="category-panel w-full lg:w-[210px] bg-white border border-gray-200 rounded-2xl overflow-y-auto shadow-sm" style="max-height: calc(100vh - 200px);">
        <div class="p-3">
        <div 
          v-for="category in categories" 
          :key="category.id"
          @click="selectCategory(category.id)"
          :class="[
              'px-3 py-2.5 mb-2 rounded-xl cursor-pointer text-sm transition-all',
            selectedCategoryId === category.id 
                ? 'bg-[#F9771C] text-white shadow'
              : 'bg-gray-50 hover:bg-gray-100 text-gray-700'
          ]"
        >
            <span class="font-medium truncate block">{{ category.name }}</span>
        </div>
      </div>
    </div>

    <!-- 右侧菜品展示 -->
      <div class="order-content flex flex-col bg-white rounded-3xl border border-gray-100 shadow-sm px-4 py-3">
      <DishIntro 
        :cart="cart" 
          :menuItems="currentMenuItems" 
        @increase="increaseQuantity"
        @decrease="decreaseQuantity"
      />
        <div
          v-if="currentCategoryLoading && currentMenuItems.length === 0"
          class="py-6 text-center text-gray-400 text-sm"
        >
          <i class="fas fa-spinner fa-spin mr-2"></i> 菜品加载中...
        </div>
        <div v-else-if="currentMenuItems.length === 0" class="py-6 text-center text-gray-400 text-sm">
          暂无菜品，稍后再来看看~
        </div>
        <div v-else-if="currentHasMore" class="py-6 text-center">
          <button
            @click="loadMoreForCurrentCategory"
            :disabled="currentCategoryLoading"
            class="px-6 py-2 bg-orange-500 text-white rounded-full text-sm font-medium hover:bg-orange-600 transition-colors disabled:opacity-60 disabled:cursor-not-allowed"
          >
            <template v-if="currentCategoryLoading">
              <i class="fas fa-spinner fa-spin mr-2"></i> 加载中...
            </template>
            <template v-else>
              加载更多菜品
            </template>
          </button>
        </div>
      </div>

      <!-- 右侧辅助信息 -->
      <aside class="order-aside flex flex-col gap-4 w-full lg:w-[250px] shrink-0">
        <div class="bg-white rounded-3xl border border-gray-100 shadow-sm p-4 space-y-3">
          <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-store text-orange-500"></i>
            门店速览
          </h4>
          <div class="grid grid-cols-2 gap-3 text-xs text-gray-500">
            <div class="info-card gradient-orange">
              <p class="text-[11px] text-gray-400">评分</p>
              <p class="text-lg font-semibold text-gray-900">{{ storeInfo?.rating ?? '--' }}</p>
            </div>
            <div class="info-card gradient-blue">
              <p class="text-[11px] text-gray-400">月售</p>
              <p class="text-lg font-semibold text-gray-900">{{ storeInfo?.monthlySales ?? 0 }} 单</p>
            </div>
            <div class="info-card gradient-green">
              <p class="text-[11px] text-gray-400">菜品分类</p>
              <p class="text-lg font-semibold text-gray-900">{{ categories.length }} 类</p>
            </div>
            <div class="info-card gradient-purple">
              <p class="text-[11px] text-gray-400">菜品总数</p>
              <p class="text-lg font-semibold text-gray-900">{{ flattenedMenuItems.length }}</p>
            </div>
            <div class="info-card col-span-2 gradient-amber">
              <p class="text-[11px] text-gray-400">营业时间</p>
              <p class="text-lg font-semibold text-gray-900">{{ businessHours }}</p>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-3xl border border-gray-100 shadow-sm p-4 space-y-3">
          <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-receipt text-blue-500"></i>
            点单进度
          </h4>
          <div class="text-xs text-gray-500 space-y-2">
            <div class="flex items-center justify-between">
              <span>当前分类</span>
              <span class="font-semibold text-gray-900">{{ currentCategoryName || '全部' }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>已选菜品</span>
              <span class="font-semibold text-gray-900">{{ cartItemCount }} 件</span>
            </div>
            <div class="flex items-center justify-between">
              <span>购物车金额</span>
              <span class="font-semibold text-orange-500">¥{{ cartTotalPrice }}</span>
            </div>
          </div>
    </div>

        <div class="bg-gradient-to-br from-orange-50 via-white to-white rounded-3xl border border-orange-100 shadow-sm p-4 space-y-3">
          <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
            <i class="fas fa-lightbulb text-yellow-500"></i>
            点单助手
          </h4>
          <ul class="text-xs text-gray-500 space-y-2 text-left">
            <li v-for="tip in quickTips" :key="tip" class="flex items-start gap-2">
              <i class="fas fa-check-circle text-green-400 mt-0.5"></i>
              <span>{{ tip }}</span>
            </li>
          </ul>
        </div>
      </aside>

    <!-- 购物车 -->
    <ItemCart 
      v-if="storeID"
      :cart="cart" 
      :storeID="storeID"
        :menuItems="flattenedMenuItems" 
      @increase="increaseQuantity"
      @decrease="decreaseQuantity"
    />
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ElMessage } from 'element-plus'

import type { StoreInfo, MenuItem, ShoppingCart, ShoppingCartItem, Category } from '@/api/user'
import { getStoreInfo, getMenuItemPage, getShoppingCart, addOrUpdateCartItem, removeCartItem, getStoreCategories } from '@/api/user'

import DishIntro from '@/components/user/StoreDetail/OrderView/DishIntro.vue'
import ItemCart from '@/components/user/StoreDetail/OrderView/ItemCart.vue'

// 路由
const route = useRoute()
const storeID = ref<string>('')

// 数据
const storeInfo = ref<StoreInfo>()
const categories = ref<Category[]>([])
const selectedCategoryId = ref<number | null>(null)
const allMenuItems = ref<Record<number, MenuItem[]>>({})
const pageByCategory = ref<Record<number, number>>({})
const hasMoreByCategory = ref<Record<number, boolean>>({})
const itemsPerPage = 20
const menuLoading = ref<Record<number, boolean>>({})

const cart = ref<ShoppingCart>({
  cartId: 0,
  totalPrice: 0,
  items: []
});  // 防止未定义
const cartLoading = ref(false)
let cartRefreshTimer: ReturnType<typeof setTimeout> | null = null
let cartReloadRequested = false
let cartLoadPromise: Promise<void> | null = null

// 请求锁：防止快速点击导致重复添加
const pendingRequests = ref<Set<number>>(new Set());

// 过滤后的菜品列表
const currentCategoryId = computed(() => selectedCategoryId.value ?? categories.value[0]?.id ?? null)

const currentMenuItems = computed(() => {
  const target = currentCategoryId.value
  if (!target) return []
  return allMenuItems.value[target] ?? []
})

const flattenedMenuItems = computed(() =>
  Object.values(allMenuItems.value).reduce<MenuItem[]>((acc, cur) => acc.concat(cur ?? []), [])
)

const currentCategoryLoading = computed(() => {
  const target = currentCategoryId.value
  if (!target) return false
  return !!menuLoading.value[target]
})

// 选择分类
function selectCategory(categoryId: number) {
  selectedCategoryId.value = categoryId
  if (!allMenuItems.value[categoryId]) {
    fetchCategoryPage(categoryId, 1, true)
  }
}

const currentHasMore = computed(() => {
  const target = currentCategoryId.value
  if (!target) return false
  return hasMoreByCategory.value[target] ?? false
})

const cartItemCount = computed(() =>
  cart.value.items.reduce((sum, item) => sum + item.quantity, 0)
)

// 购物车金额，优先使用菜品单价 * 数量，若缺失则回退 totalPrice
const cartTotalPrice = computed(() => {
  const priceMap: Record<number, number> = {}
  flattenedMenuItems.value.forEach(mi => { priceMap[mi.id] = mi.price ?? 0 })

  const total = cart.value.items.reduce((sum, item) => {
    const unit = priceMap[item.dishId] ?? (
      item.quantity > 0 && item.totalPrice != null
        ? item.totalPrice / item.quantity
        : 0
    )
    return sum + unit * item.quantity
  }, 0)

  return total.toFixed(2)
})

const currentCategoryName = computed(() => {
  const target = currentCategoryId.value
  if (!target) return ''
  return categories.value.find(cat => cat.id === target)?.name ?? ''
})

const businessHours = computed(() => {
  if (!storeInfo.value) return '未提供'
  return storeInfo.value.businessHours || '未提供'
})

const quickTips = [
  '高峰期提前 20 分钟下单更容易有骑手接单',
  '收藏店铺可享受专属优惠',
  '到店自取可享更快出餐体验'
]

function scheduleCartRefresh(delay = 400) {
  if (typeof window === 'undefined') return
  if (cartRefreshTimer) {
    clearTimeout(cartRefreshTimer)
  }
  cartRefreshTimer = setTimeout(() => {
    cartRefreshTimer = null
    loadCart()
  }, delay)
}

function applyCartSnapshot(dish: MenuItem, newQty: number) {
  const items = cart.value.items
  const index = items.findIndex(i => i.dishId === dish.id)
  if (newQty <= 0) {
    if (index !== -1) {
      items.splice(index, 1)
    }
  } else if (index === -1) {
    items.push({
      itemId: -Date.now(),
      dishId: dish.id,
      quantity: newQty,
      totalPrice: Number((dish.price * newQty).toFixed(2))
    })
  } else {
    const basePrice = items[index].quantity > 0
      ? items[index].totalPrice / items[index].quantity
      : dish.price
    items[index].quantity = newQty
    items[index].totalPrice = Number((basePrice * newQty).toFixed(2))
  }
  cart.value.totalPrice = items.reduce((sum, item) => sum + (item.totalPrice ?? 0), 0)
}

async function ensureCartReady() {
  if (cart.value.cartId) {
    return true
  }
  await loadCart()
  return !!cart.value.cartId
}

async function fetchCategoryPage(categoryId: number, page: number, reset = false) {
  if (!storeID.value) return
  if (menuLoading.value[categoryId]) return

  menuLoading.value = { ...menuLoading.value, [categoryId]: true }
  try {
    const response = await getMenuItemPage(storeID.value, {
      categoryId,
      page,
      pageSize: itemsPerPage
    })

    const incoming = response?.items ?? []
    const existing = reset ? [] : (allMenuItems.value[categoryId] ?? [])
    allMenuItems.value = {
      ...allMenuItems.value,
      [categoryId]: [...existing, ...incoming]
    }
    pageByCategory.value = { ...pageByCategory.value, [categoryId]: page }
    hasMoreByCategory.value = { ...hasMoreByCategory.value, [categoryId]: !!response?.hasMore }
  } catch (error) {
    hasMoreByCategory.value = { ...hasMoreByCategory.value, [categoryId]: false }
    if (reset) {
      allMenuItems.value = { ...allMenuItems.value, [categoryId]: [] }
    }
    console.warn('加载菜品失败:', error)
  } finally {
    menuLoading.value = { ...menuLoading.value, [categoryId]: false }
  }
}

function loadMoreForCurrentCategory() {
  const target = currentCategoryId.value
  if (!target) return
  const nextPage = (pageByCategory.value[target] ?? 1) + 1
  fetchCategoryPage(target, nextPage)
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

  const cartReady = await ensureCartReady()
  if (!cartReady || !cart.value.cartId) {
    ElMessage.error('购物车初始化失败，请稍后重试')
    return
  }
  
  try {
    // 添加到待处理集合
    pendingRequests.value.add(dish.id);
    
    const item = cart.value.items.find(i => i.dishId === dish.id)
    const newQty = (item?.quantity ?? 0) + 1
    applyCartSnapshot(dish, newQty)

    await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
  } catch (error) {
    console.warn('添加菜品到购物车失败:', error)
    ElMessage.error('添加失败，请稍后重试')
    await loadCart()
  } finally {
    // 请求完成后从待处理集合中移除
    pendingRequests.value.delete(dish.id);
    scheduleCartRefresh()
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

  const cartReady = await ensureCartReady()
  if (!cartReady || !cart.value.cartId) {
    ElMessage.error('购物车初始化失败，请稍后重试')
    return
  }
  
  try {
    // 添加到待处理集合
    pendingRequests.value.add(dish.id);
    
    const item = cart.value.items.find(i => i.dishId === dish.id)
    if (!item) return

    const newQty = item.quantity - 1
    applyCartSnapshot(dish, newQty)
    if (newQty > 0) {
      await addOrUpdateCartItem(cart.value.cartId, dish.id, newQty)
    } else {
      await removeCartItem(cart.value.cartId, dish.id)
    }
  } catch (error) {
    console.warn('减少菜品数量失败:', error)
    ElMessage.error('操作失败，请稍后重试')
    await loadCart()
  } finally {
    // 请求完成后从待处理集合中移除
    pendingRequests.value.delete(dish.id);
    scheduleCartRefresh()
  }
}

// 读取购物车
async function loadCart(): Promise<void> {
  if (!storeID.value) return
  if (cartLoading.value) {
    cartReloadRequested = true
    return cartLoadPromise ?? Promise.resolve()
  }

  cartLoading.value = true
  cartLoadPromise = (async () => {
  try {
    // 检查用户是否已登录
    const token = localStorage.getItem('authToken');
    if (!token) {
      cart.value = { cartId: 0, totalPrice: 0, items: [] };
      return;
    }
    
    const data = await getShoppingCart(storeID.value);
    cart.value = data ?? { cartId: 0, totalPrice: 0, items: [] };
  } catch (error) {
    console.warn('加载购物车失败，可能是用户未登录:', error);
    cart.value = { cartId: 0, totalPrice: 0, items: [] };
    } finally {
      cartLoading.value = false
    }
  })()

  await cartLoadPromise

  if (cartReloadRequested) {
    cartReloadRequested = false
    await loadCart()
  }
}

// 获取数据
async function loadData(storeId: string) {
    // 先重置状态，避免显示旧数据
  allMenuItems.value = {}
    categories.value = []
    selectedCategoryId.value = null
  pageByCategory.value = {}
  hasMoreByCategory.value = {}
  menuLoading.value = {}
  cart.value = { cartId: 0, totalPrice: 0, items: [] }
  pendingRequests.value.clear()
  
  // 购物车异步加载，不阻塞UI
  loadCart()
    
  // 并行加载店铺信息和分类
  const [storeInfoData, categoriesData] = await Promise.all([
      getStoreInfo(storeId),
      getStoreCategories(storeId)
    ])
    
    storeInfo.value = storeInfoData
    categories.value = categoriesData
    
  // 如果有分类，默认选中第一个并加载其菜品
    if (categories.value.length > 0) {
      selectedCategoryId.value = categories.value[0].id
    // 并行预加载前两个分类的菜品，提升切换体验
    const preloadPromises = []
    preloadPromises.push(fetchCategoryPage(categories.value[0].id, 1, true))
    if (categories.value.length > 1) {
      preloadPromises.push(fetchCategoryPage(categories.value[1].id, 1, true))
    }
    await Promise.all(preloadPromises)
  }
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

<style scoped>
.store-order-layout {
  display: grid;
  grid-template-columns: 1fr;
  gap: 20px;
}

@media (min-width: 1024px) {
  .store-order-layout {
    grid-template-columns: 210px minmax(0, 1fr) 250px;
    align-items: flex-start;
  }
}

.store-order-page {
  position: relative;
}

.store-order-bg {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(circle at 15% 20%, rgba(249, 119, 28, 0.12), transparent 45%),
    radial-gradient(circle at 85% 15%, rgba(249, 119, 28, 0.08), transparent 40%),
    radial-gradient(circle at 80% 80%, rgba(59, 130, 246, 0.08), transparent 45%),
    linear-gradient(135deg, rgba(249, 250, 251, 0.9), rgba(243, 244, 246, 0.9));
  filter: blur(0.5px);
}

.info-card {
  background: linear-gradient(135deg, rgba(249, 250, 251, 0.9), rgba(255, 255, 255, 0.95));
  border-radius: 18px;
  padding: 10px 12px;
  border: 1px solid rgba(228, 233, 242, 0.9);
  box-shadow: inset 0 1px 1px rgba(255, 255, 255, 0.8), 0 10px 20px rgba(15, 23, 42, 0.08);
}

.info-card p:last-child {
  margin-top: 4px;
}

.gradient-orange {
  background: linear-gradient(135deg, rgba(253, 242, 233, 0.9), rgba(255, 255, 255, 0.95));
  border: 1px solid rgba(251, 146, 60, 0.2);
}

.gradient-blue {
  background: linear-gradient(135deg, rgba(219, 234, 254, 0.9), rgba(255, 255, 255, 0.95));
  border: 1px solid rgba(59, 130, 246, 0.2);
}

.gradient-green {
  background: linear-gradient(135deg, rgba(220, 252, 231, 0.9), rgba(255, 255, 255, 0.95));
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.gradient-purple {
  background: linear-gradient(135deg, rgba(237, 233, 254, 0.9), rgba(255, 255, 255, 0.95));
  border: 1px solid rgba(139, 92, 246, 0.2);
}
.gradient-amber {
  background: linear-gradient(135deg, rgba(255, 247, 237, 0.9), rgba(255, 255, 255, 0.95));
  border: 1px solid rgba(251, 191, 36, 0.2);
}
</style>