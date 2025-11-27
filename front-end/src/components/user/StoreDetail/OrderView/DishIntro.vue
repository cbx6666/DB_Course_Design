<template>
    <div class="mt-6 w-full bg-white border border-gray-100 rounded-[38px] shadow-[0_25px_60px_rgba(15,23,42,0.06)] px-6 py-8 relative overflow-hidden">
        <div class="absolute inset-0 bg-[radial-gradient(circle_at_top,_rgba(249,119,28,0.08),_transparent_60%)] pointer-events-none"></div>
        <div class="relative flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900 flex items-center gap-2">
                <i class="fas fa-utensils text-orange-500"></i>
                菜品推荐
            </h3>
            <div class="flex items-center gap-2 text-xs text-gray-500">
                <span class="inline-flex items-center gap-1 px-2.5 py-1 bg-orange-50 text-orange-600 rounded-full border border-orange-100">
                    <i class="fas fa-filter text-gray-400 text-[10px]"></i> 精选优先
                </span>
                <span class="inline-flex items-center gap-1 px-2.5 py-1 bg-green-50 text-green-600 rounded-full border border-green-100">
                    <i class="fas fa-leaf text-gray-400 text-[10px]"></i> 健康搭配
                </span>
            </div>
        </div>
        <div v-if="menuItems.length > 0" class="relative">
            <div class="grid gap-5 grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 xl:grid-cols-5">
            <div v-for="item in menuItems" :key="item.id">
                    <div v-if="item" class="dish-card bg-white border border-gray-100 rounded-[30px] overflow-hidden hover:shadow-2xl transition-all duration-200 mx-auto relative">
                        <div class="absolute top-4 left-4 text-[11px] font-semibold text-orange-500 bg-white/80 backdrop-blur px-2.5 py-1 rounded-full border border-orange-100 shadow-sm flex items-center gap-1">
                            <i class="fas fa-fire text-[10px]"></i>
                            精选
                        </div>
                        <div class="w-full h-40 overflow-hidden bg-gradient-to-b from-orange-50/80 to-white flex items-center justify-center border-b border-gray-100">
                            <img :src="normalizeImageUrl(item.image)" class="w-full h-full object-cover transition-transform duration-200 hover:scale-105" />
                    </div>
                        <div class="p-4 space-y-3">
                            <h4 class="font-semibold text-gray-900 text-[15px] mb-1 truncate">{{ item.name }}</h4>
                            <p class="text-xs text-gray-500 line-clamp-2 min-h-[32px] leading-relaxed">{{ item.description }}</p>
                            <div class="flex items-center justify-between mt-2">
                                <div class="flex flex-col">
                                    <span class="text-xl font-extrabold text-orange-600">¥{{ Number.isInteger(item.price) ? item.price : item.price.toFixed(2) }}</span>
                                    <span class="text-[11px] text-orange-500 bg-orange-50 inline-flex items-center gap-1 px-2 py-0.5 rounded-full w-max border border-orange-100">
                                        <i class="far fa-clock text-[10px]"></i> 现做现卖
                                    </span>
                            </div>
                                <div v-if="!isItemSoldOut(item.isSoldOut)" class="flex items-center space-x-1.5 bg-gray-50 rounded-full px-2 py-1 border border-gray-100 shadow-sm">
                                <button @click="emit('decrease', item)" :disabled="!getItemQuantity(item.id)"
                                        class="w-6 h-6 rounded-full bg-white flex items-center justify-center text-gray-600 hover:bg-orange-50 disabled:opacity-50 cursor-pointer border border-gray-200 transition-colors">
                                        <i class="fas fa-minus text-[10px]"></i>
                                </button>
                                    <span class="w-6 text-center text-[12px] font-semibold text-gray-700">{{ getItemQuantity(item.id) || 0 }}</span>
                                <button @click="emit('increase', item)"
                                        class="w-6 h-6 rounded-full bg-orange-500 text-white flex items-center justify-center hover:bg-orange-600 cursor-pointer shadow-md transition-colors">
                                        <i class="fas fa-plus text-[10px]"></i>
                                </button>
                            </div>
                            <div v-else>
                                    <span class="text-xs text-red-400 font-semibold">售罄</span>
                            </div>
                        </div>
                    </div>

                </div>
            </div>
        </div>
        </div>
        <div v-else class="flex flex-col items-center justify-center py-16 text-gray-400 text-sm gap-3 relative">
            <div class="w-16 h-16 rounded-full bg-orange-50 flex items-center justify-center text-orange-400 mb-2 shadow-inner border border-orange-100">
                <i class="fas fa-utensils text-2xl"></i>
            </div>
            抱歉，暂时没有菜品~
            <span class="text-xs text-gray-400">刷新试试或联系商家</span>
        </div>
    </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue'
import type { MenuItem, ShoppingCart } from '@/api/user';
import { normalizeImageUrl } from '@/utils/imageUtils';

const props = defineProps<{
    cart: ShoppingCart;
    menuItems: MenuItem[];
}>();

const isItemSoldOut = (isSoldOut?: number) => isSoldOut == 0; // 等于0返回True

const getItemQuantity = (dishId: number) => {
    const item = props.cart.items.find(i => i.dishId === dishId);
    return item ? item.quantity : 0;
}

const emit = defineEmits<{
    (e: 'increase', item: MenuItem): void;
    (e: 'decrease', item: MenuItem): void;
}>();
</script>

<style scoped>
.dish-card {
    max-width: 210px;
    transition: transform 0.2s ease;
}

@media (min-width: 1536px) {
    .dish-card {
        max-width: 220px;
    }
}

.dish-card:hover {
    transform: translateY(-2px);
}
</style>