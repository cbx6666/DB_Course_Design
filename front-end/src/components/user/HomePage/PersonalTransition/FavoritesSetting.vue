<template>
  <transition name="fade">
    <div v-if="props.showFavoritesForm" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div class="bg-white w-full max-w-5xl h-[85vh] rounded-xl shadow-2xl flex flex-col overflow-hidden relative font-sans">
        <!-- 头部 -->
        <div class="px-6 py-4 border-b border-gray-100 flex justify-between items-center bg-white z-20">
          <div class="flex items-center space-x-3">
            <div class="w-9 h-9 bg-orange-50 rounded-full flex items-center justify-center">
              <i class="fas fa-star text-orange-500 text-lg"></i>
            </div>
            <h3 class="text-lg font-bold text-gray-800 tracking-wide">我的收藏</h3>
          </div>
          <div class="flex items-center space-x-3">
            <button
              class="px-3 py-1.5 bg-orange-500 hover:bg-orange-600 text-white text-xs font-semibold rounded-full shadow-sm flex items-center space-x-1 transition-colors"
              @click.stop="createFolder"
            >
              <i class="fas fa-plus text-xs"></i>
              <span>新建收藏夹</span>
            </button>
            <button 
              class="w-8 h-8 rounded-full hover:bg-gray-100 flex items-center justify-center text-gray-400 hover:text-gray-600 transition-colors" 
              @click="closeForm"
            >
              <i class="fas fa-times text-lg"></i>
            </button>
          </div>
        </div>

        <!-- 加载状态 -->
        <div v-if="loading" class="flex-1 flex flex-col justify-center items-center bg-[#F6F7F8]">
          <div class="w-10 h-10 border-3 border-orange-100 border-t-orange-500 rounded-full animate-spin mb-3"></div>
          <p class="text-gray-400 text-sm">加载中...</p>
        </div>

        <div v-else class="flex-1 flex flex-col min-h-0 bg-[#F6F7F8]">
          <!-- 顶部横向导航栏 -->
          <div class="bg-white px-6 shadow-sm z-10 sticky top-0">
            <div class="flex items-center space-x-8 overflow-x-auto no-scrollbar">
              <button
                v-for="folder in folders"
                :key="folder.folderID"
                class="relative py-3 text-sm font-medium transition-all duration-300 whitespace-nowrap group flex items-center"
                :class="selectedFolderId === folder.folderID ? 'text-orange-500' : 'text-gray-600 hover:text-gray-900'"
                @click="selectFolder(folder.folderID)"
              >
                <span class="text-base">{{ folder.folderName }}</span>
                <span 
                  class="ml-1.5 text-xs px-1.5 py-0.5 rounded-full transition-colors"
                  :class="selectedFolderId === folder.folderID ? 'bg-orange-100 text-orange-600' : 'bg-gray-100 text-gray-400 group-hover:text-gray-500'"
                >
                  {{ folder.favoriteItems.length }}
                </span>
                
                <!-- 底部指示条 -->
                <div 
                  class="absolute bottom-0 left-1/2 -translate-x-1/2 w-8 h-0.5 bg-orange-500 rounded-full transform transition-all duration-300"
                  :class="selectedFolderId === folder.folderID ? 'opacity-100 scale-x-100' : 'opacity-0 scale-x-0 group-hover:scale-x-50 group-hover:opacity-50'"
                ></div>
              </button>
              
              <!-- 右侧占位或操作按钮 -->
              <div class="flex-1"></div>
              
              <!-- 搜索框 (装饰用) -->
              <div class="hidden md:flex items-center bg-gray-100 rounded-full px-3 py-1.5 w-48 my-2">
                <input type="text" placeholder="搜索收藏" class="bg-transparent border-none text-xs w-full focus:outline-none text-gray-600 placeholder-gray-400">
                <i class="fas fa-search text-gray-400 text-xs"></i>
              </div>
            </div>
          </div>

          <!-- 内容区域 -->
          <div class="flex-1 overflow-y-auto p-6 scrollbar-thin">
            <!-- 空状态 -->
            <div v-if="!currentFolder || currentFolder.favoriteItems.length === 0" class="h-full flex flex-col justify-center items-center text-gray-400 min-h-[300px]">
              <div class="w-40 h-40 bg-gray-200 rounded-full flex items-center justify-center mb-6 opacity-50">
                 <i class="fas fa-folder-open text-6xl text-gray-400"></i>
              </div>
              <p class="text-base font-medium text-gray-500 mb-2">这个收藏夹是空的</p>
              <p class="text-xs text-gray-400 mb-6">快去添加喜欢的店铺吧~</p>
              <button class="px-6 py-2 bg-orange-500 text-white text-sm rounded-lg hover:bg-orange-600 transition-colors shadow-lg shadow-orange-200" @click="closeForm">
                去首页逛逛
              </button>
            </div>

            <!-- 收藏列表 -->
            <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-5 pb-8">
              <div
                v-for="item in currentFolder.favoriteItems"
                :key="item.itemID"
                class="bg-white rounded-xl overflow-hidden group cursor-pointer hover:shadow-xl transition-all duration-300 transform hover:-translate-y-1 border border-gray-100"
                @click="goToStore(item.storeID)"
              >
                <!-- 封面图容器 -->
                <div class="relative aspect-[16/10] overflow-hidden bg-gray-100">
                  <img
                    v-if="item.storeImage"
                    :src="normalizeImageUrl(item.storeImage)"
                    :alt="item.storeName"
                    class="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                    @error="handleImageError"
                  />
                  <div v-else class="w-full h-full flex items-center justify-center text-gray-300 bg-gray-50">
                    <i class="fas fa-store text-4xl opacity-50"></i>
                  </div>
                  
                  <!-- 遮罩层 -->
                  <div class="absolute inset-0 bg-gradient-to-t from-black/60 via-transparent to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                  
                  <!-- 收藏时间标签 -->
                  <div class="absolute top-2 right-2 px-2 py-1 bg-black/60 backdrop-blur-md rounded text-white text-[10px] opacity-0 group-hover:opacity-100 transition-all duration-300 transform translate-y-2 group-hover:translate-y-0">
                    {{ formatDate(item.favoritedAt) }}
                  </div>
                  
                  <!-- 播放/进入图标 -->
                  <div class="absolute bottom-3 right-3 w-8 h-8 bg-orange-500 rounded-full flex items-center justify-center text-white opacity-0 group-hover:opacity-100 transition-all duration-300 transform scale-50 group-hover:scale-100 shadow-lg">
                    <i class="fas fa-arrow-right text-xs"></i>
                  </div>
                </div>

                <!-- 信息区域 -->
                <div class="p-3.5">
                  <h4 class="font-bold text-gray-800 mb-1.5 truncate group-hover:text-orange-500 transition-colors text-[15px]">
                    {{ item.storeName }}
                  </h4>
                  
                  <!-- 备注信息，如果没有则显示默认文案 -->
                  <div class="bg-gray-50 rounded px-2 py-1.5 mb-3">
                    <p class="text-xs text-gray-500 line-clamp-2 h-8 leading-4">
                      {{ item.favoriteReason || '这个人很懒，没有写备注~' }}
                    </p>
                  </div>
                  
                  <div class="flex justify-between items-center pt-1">
                    <div class="flex items-center text-xs text-gray-400 space-x-4">
                      <button class="flex items-center hover:text-orange-500 transition-colors group/btn" @click.stop>
                        <i class="fas fa-share-alt mr-1 group-hover/btn:scale-110 transition-transform"></i> 
                      </button>
                      <button class="flex items-center hover:text-orange-500 transition-colors group/btn" @click.stop="deleteItem(item)">
                        <i class="fas fa-trash-alt mr-1 group-hover/btn:scale-110 transition-transform"></i>
                      </button>
                    </div>
                    <button class="text-gray-300 hover:text-gray-600" @click.stop>
                        <i class="fas fa-ellipsis-h text-xs"></i>
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script setup lang="ts">
import { defineProps, defineEmits, ref, watch, computed } from 'vue'
import { useRouter } from 'vue-router'

import type { FavoritesFolder, FavoriteItem } from '@/api/user/favorites';
import { getFavoritesFolders } from '@/api/user/favorites';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';

const router = useRouter();

const folders = ref<FavoritesFolder[]>([]);
const loading = ref(false);
const selectedFolderId = ref<number | null>(null);

// 计算当前选中的文件夹
const currentFolder = computed(() => {
  if (!selectedFolderId.value) return null;
  return folders.value.find(f => f.folderID === selectedFolderId.value) || null;
});

const props = defineProps<{
    showFavoritesForm: Boolean;
}>();

const emit = defineEmits<{
    (e: "update:showFavoritesForm", value: Boolean): void;
}>();

// 关闭弹窗
function closeForm() {
    emit("update:showFavoritesForm", false);
}

// 选择文件夹
function selectFolder(id: number) {
  selectedFolderId.value = id;
}

// 加载收藏夹数据
async function loadFavorites() {
  try {
    loading.value = true;
    const data = await getFavoritesFolders();
    folders.value = data;
    
    // 默认选中第一个收藏夹
    if (folders.value.length > 0 && !selectedFolderId.value) {
      selectedFolderId.value = folders.value[0].folderID;
    } else if (folders.value.length > 0 && selectedFolderId.value) {
      // 检查当前选中的ID是否还在列表中
      const exists = folders.value.some(f => f.folderID === selectedFolderId.value);
      if (!exists) {
        selectedFolderId.value = folders.value[0].folderID;
      }
    }
  } catch (error) {
    console.error('获取收藏夹信息失败:', error);
    folders.value = [];
  } finally {
    loading.value = false;
  }
}

// 监听弹窗打开，每次打开时重新加载数据
watch(() => props.showFavoritesForm, async (newVal) => {
  if (newVal) {
    await loadFavorites();
  }
})

// 格式化日期显示
function formatDate(dateStr: string) {
  const date = new Date(dateStr);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const days = Math.floor(diff / (1000 * 60 * 60 * 24));
  
  if (days === 0) return '今天';
  if (days === 1) return '昨天';
  if (days < 30) return `${days}天前`;
  
  const year = date.getFullYear();
  const month = date.getMonth() + 1;
  const day = date.getDate();
  return `${year}-${month}-${day}`;
}

// 跳转到店铺页面
function goToStore(storeId: number) {
  closeForm();
  router.push(`/home/stores/${storeId}`);
}

// 删除收藏项 (预留功能)
function deleteItem(item: FavoriteItem) {
  // TODO: 实现删除功能 API
  console.log('Delete item', item);
  alert('删除功能开发中...');
}

// 新建收藏夹 (预留功能)
function createFolder() {
  // TODO: 接入后端创建收藏夹接口
  alert('新建收藏夹功能待接入后端');
}

</script>

<style scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity 0.3s;
}
.fade-enter-from, .fade-leave-to {
  opacity: 0;
}

/* 自定义滚动条 */
.scrollbar-thin::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}
.scrollbar-thin::-webkit-scrollbar-track {
  background: transparent;
}
.scrollbar-thin::-webkit-scrollbar-thumb {
  background-color: #E5E7EB;
  border-radius: 20px;
}
.scrollbar-thin::-webkit-scrollbar-thumb:hover {
  background-color: #D1D5DB;
}

/* 隐藏横向滚动条 */
.no-scrollbar::-webkit-scrollbar {
  display: none;
}
.no-scrollbar {
  -ms-overflow-style: none;
  scrollbar-width: none;
}
</style>
