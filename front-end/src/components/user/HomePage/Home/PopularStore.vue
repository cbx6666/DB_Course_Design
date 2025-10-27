<template>
  <section class="max-w-screen-xl mx-auto px-6 pb-12">
    <h2 class="text-2xl font-bold text-gray-800 mb-8 text-left">人气商家</h2>

    <!-- 占位 / 缓冲图标 -->
    <div v-if="showLoading" class="flex justify-center items-center h-64">
      <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
    </div>

    <!-- 商家列表 -->
    <div v-else class="grid grid-cols-4 gap-6">
      <div v-for="(restaurant, index) in popularRestaurants?.recomStore" :key="index"
        class="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow cursor-pointer text-left">
        <div class="w-full h-40 bg-gray-100 flex items-center justify-center overflow-hidden">
          <img :src="normalizeImageUrl(restaurant.image)" :alt="restaurant.name" class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
        </div>
        <div class="p-4">
          <div class="mb-3">
            <h3 class="font-bold text-lg mb-2">{{ restaurant.name }}</h3>
            <div class="flex items-center text-sm text-gray-600 mb-2">
              <span class="flex items-center">
                <i class="fas fa-star text-yellow-400 mr-1"></i>
                {{ restaurant.averageRating > 0 ? restaurant.averageRating.toFixed(1) : '暂无评分' }}
              </span>
            </div>
            <!-- 店铺种类和特色 -->
            <div v-if="restaurant.category || restaurant.description" class="flex flex-wrap gap-2">
              <span v-if="restaurant.category" class="text-white bg-[#F9771C] px-2 py-1 rounded text-xs">{{ restaurant.category }}</span>
              <span v-if="restaurant.description" class="text-white bg-[#F9771C] px-2 py-1 rounded text-xs">{{ restaurant.description }}</span>
            </div>
          </div>
          <!-- 底部：按钮 -->
          <button
            class="w-full bg-orange-500 hover:bg-orange-600 text-white px-4 py-2 rounded-lg text-sm transition-colors cursor-pointer"
            @click="goToPage(`${restaurant.id}`)">
            进入店铺
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router'
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils'

import type { RecomStore } from '@/api/user';
import { getRecomStore } from '@/api/user';

const router = useRouter();

const popularRestaurants = ref<RecomStore>({ recomStore: [] });
const showLoading = ref(true); // 控制缓冲图标显示

onMounted(async () => {
  try {
    popularRestaurants.value = (await getRecomStore());
    showLoading.value = false;
  } catch (error) {
    alert('获取推荐商家失败');
    console.error('获取推荐商家失败', error);
  }
});

function goToPage(id: number | string) {
  router.push({ name: 'InStore', params: { id } });
}
</script>