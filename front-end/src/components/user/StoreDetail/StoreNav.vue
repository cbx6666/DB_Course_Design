<template>
    <div class="relative bg-gradient-to-r from-orange-50 to-orange-100 shadow-sm overflow-hidden">
        <!--返回按钮-->
        <button @click="goBack"
            class="fixed left-6 top-6 flex items-center bg-white shadow-lg px-3 py-2 rounded-xl z-10 hover:bg-gray-100">
            <i class="fas fa-arrow-left mr-2"></i>
            返回
        </button>

        <!--背景图标-->
        <div>
            <div class="absolute top-0 right-0 w-1/3 h-full opacity-10">
                <i
                    class="fas fa-utensils text-orange-500 text-[300px] transform rotate-12 translate-x-1/4 -translate-y-1/4"></i>
            </div>
            <div class="absolute bottom-0 left-0 w-1/4 h-full opacity-10">
                <i
                    class="fas fa-pepper-hot text-orange-500 text-[200px] transform -rotate-12 -translate-x-1/4 translate-y-1/4"></i>
            </div>
        </div>

        <!--简要介绍-->
        <div class="max-w-7xl mx-auto px-4 py-8 relative">
            <div class="flex items-center space-x-8">
                <div class="relative w-28 h-28 rounded-2xl overflow-hidden bg-gray-100 flex items-center justify-center shadow-lg">
                    <img :src="normalizeImageUrl(storeInfo.image)" alt="商家头像" class="max-w-full max-h-full w-auto h-auto object-contain" @error="handleImageError" />
                </div>
                <div class="flex-1">
            <div class="flex items-center space-x-4 mb-3 flex-wrap gap-3">
                        <h1 class="text-3xl font-bold text-gray-900">
                            {{ storeInfo.name }}
                        </h1>
                        <div class="flex items-center px-3 py-1 bg-orange-500 bg-opacity-10 rounded-full">
                            <i class="fas fa-crown text-orange-500 mr-2"></i>
                            <span class="text-orange-600 font-medium">优质商家</span>
                        </div>
                <!-- 收藏按钮 -->
                <button
                  class="flex items-center space-x-2 px-4 py-2 rounded-full bg-white border border-orange-200 text-orange-600 hover:bg-orange-50 hover:border-orange-400 transition-colors shadow-sm"
                  @click.stop="openFavoriteModal"
                >
                  <i class="fas fa-heart"></i>
                  <span>收藏</span>
                </button>
                    </div>
                                                              <div class="flex flex-col space-y-3">
                         <div class="flex items-center space-x-6 text-sm">
                             <div class="flex items-center bg-white px-3 py-1.5 rounded-full shadow-sm">
                                 <i class="fas fa-star text-yellow-400 mr-2"></i>
                                 <span class="font-medium text-gray-900">{{ storeInfo.rating }}</span>
                             </div>
                             <div class="flex items-center">
                                 <i class="fas fa-shopping-bag text-gray-500 mr-2"></i>
                                 <span>月售 {{ storeInfo.monthlySales }} 单</span>
                             </div>
                             <div class="flex items-center">
                                 <i class="fas fa-truck text-gray-500 mr-2"></i>
                                 <span>配送费 ¥ {{ deliveryTask.deliveryFee }}</span>
                             </div>
                             <div class="flex items-center">
                                 <i class="fas fa-clock text-gray-500 mr-2"></i>
                                 <span>配送时间 {{ deliveryTask.deliveryTime }} 分钟</span>
                             </div>
                         </div>
                         <div class="flex flex-wrap gap-2">
                             <p class="text-white bg-[#F9771C] px-4 py-2 rounded-lg text-left w-fit">{{ storeInfo.category }}</p>
                             <p class="text-white bg-[#F9771C] px-4 py-2 rounded-lg text-left w-fit">{{ storeInfo.description }}</p>
                         </div>
                     </div>
                </div>
            </div>
        </div>

        <!-- Tab导航栏 -->
        <div class="bg-white border-b">
            <div class="max-w-7xl mx-auto px-4">
                <div class="flex space-x-8">
                    <button v-for="tab in tabs" :key="tab.label"
                    @click="goToPage(tab.path)" 
                    :class="{
                        'border-b-2 border-[#F9771C] text-[#F9771C]': route.path === tab.path,
                        'text-gray-600 hover:text-gray-900': route.path !== tab.path
                    }" class="py-4 px-2 font-medium transition-colors cursor-pointer z-10">
                    {{ tab.label }}
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- 收藏弹窗 -->
    <transition name="fade">
      <div v-if="showFavModal" class="fixed inset-0 bg-black/40 z-50 flex items-center justify-center px-4">
        <div class="bg-white w-full max-w-md rounded-2xl shadow-2xl p-6 relative">
          <div class="flex justify-between items-center mb-4">
            <h3 class="text-lg font-bold text-gray-800">收藏到</h3>
            <button class="text-gray-400 hover:text-gray-600" @click="closeFavoriteModal">
              <i class="fas fa-times"></i>
            </button>
          </div>

          <div v-if="loadingFav" class="py-10 flex flex-col items-center text-gray-400">
            <i class="fas fa-spinner fa-spin text-2xl mb-2"></i>
            <span>加载收藏夹...</span>
          </div>

          <div v-else class="space-y-4">
            <div class="space-y-2 max-h-56 overflow-y-auto pr-1">
              <template v-if="folders.length > 0">
                <label
                  v-for="folder in folders"
                  :key="folder.folderID"
                  class="flex items-center justify-between px-3 py-2 border rounded-lg cursor-pointer hover:bg-orange-50"
                >
              <div class="flex items-center space-x-2">
                    <input
                      type="radio"
                      :value="folder.folderID"
                      v-model="selectedFolderId"
                    />
                    <span class="text-sm text-gray-800">{{ folder.folderName }}</span>
                    <span class="text-xs text-gray-400">({{ folder.favoriteItems.length }})</span>
                  </div>
                  <i class="fas fa-folder text-orange-400"></i>
                </label>
              </template>
              <div v-else class="text-sm text-gray-500 px-3 py-4 border rounded-lg bg-orange-50/40">
                还没有收藏夹，先创建一个吧
              </div>
            </div>

            <div>
              <p class="text-xs text-gray-500 mb-1">备注（可选）</p>
              <textarea
                v-model="favoriteReason"
                rows="2"
                class="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-orange-400"
                placeholder="为何收藏这个店铺？"
              ></textarea>
            </div>

            <p v-if="errorMsg" class="text-red-500 text-xs">{{ errorMsg }}</p>

            <div class="flex justify-end space-x-3 pt-2">
              <button class="px-4 py-2 text-sm text-gray-500 hover:text-gray-700" @click="closeFavoriteModal">取消</button>
              <button
                class="px-4 py-2 text-sm bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors disabled:opacity-60 flex items-center space-x-2"
                @click="submitFavorite"
                :disabled="submittingFav || !selectedFolderId"
              >
                <i v-if="submittingFav" class="fas fa-spinner fa-spin text-xs"></i>
                <span>{{ submittingFav ? '提交中...' : '确定' }}</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    </transition>

</template>

<script setup lang="ts">
import { computed, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils'

import type { DeliveryTask, StoreInfo } from '@/api/user'
import type { FavoritesFolder } from '@/api/user/favorites'
import { getFavoritesFolders, addFavoriteToFolder } from '@/api/user/favorites'

const route = useRoute();
const router = useRouter();

// 从父组件获得信息
// eslint-disable-next-line no-undef
const props = defineProps<{
    storeInfo: StoreInfo;
    deliveryTask: DeliveryTask;
    storeID: string;
}>()

const tabs = computed( () => [
    { path: `/store/${props.storeID}/order`, label: "点餐" },
    { path: `/store/${props.storeID}/comment`, label: "评价" },
    { path: `/store/${props.storeID}/info`, label: "商家" },
]);

function goBack() {
    goToPage('/home/restaurants');
}

function goToPage(path: string) {
    router.push(path);
}

// 收藏弹窗相关
const showFavModal = ref(false);
const folders = ref<FavoritesFolder[]>([]);
const loadingFav = ref(false);
const submittingFav = ref(false);
const selectedFolderId = ref<number | null>(null);
const favoriteReason = ref('');
const errorMsg = ref('');

async function loadFolders() {
    try {
        loadingFav.value = true;
        const data = await getFavoritesFolders();
        folders.value = (data && data.length > 0) ? data : [];
        if (!selectedFolderId.value && folders.value.length > 0) {
            selectedFolderId.value = folders.value[0].folderID;
        }
    } catch (e) {
        errorMsg.value = '加载收藏夹失败，请稍后重试';
    } finally {
        loadingFav.value = false;
    }
}

function openFavoriteModal() {
    errorMsg.value = '';
    favoriteReason.value = '';
    selectedFolderId.value = null;
    showFavModal.value = true;
    loadFolders();
}

function closeFavoriteModal() {
    showFavModal.value = false;
}

async function submitFavorite() {
    if (!selectedFolderId.value || folders.value.length === 0) {
        errorMsg.value = '请选择收藏夹（请先在个人中心创建收藏夹）';
        return;
    }
    errorMsg.value = '';
    try {
        submittingFav.value = true;
        await addFavoriteToFolder(selectedFolderId.value, Number(props.storeID), favoriteReason.value.trim() || undefined);
        closeFavoriteModal();
        alert('收藏成功');
    } catch (e) {
        console.error(e);
        // 尝试读取后端返回的提示
        const msg = (e as any)?.response?.data?.message || '收藏失败，请稍后重试或避免重复收藏';
        errorMsg.value = msg;
    } finally {
        submittingFav.value = false;
    }
}


</script>