<template>
  <transition name="fade">
    <div v-if="props.showFavoritesForm" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
      <div class="bg-white w-full max-w-5xl h-[85vh] rounded-xl shadow-2xl flex flex-col overflow-hidden relative font-sans">
        <!-- 新建收藏夹弹窗 -->
        <transition name="fade">
          <div v-if="showCreateModal" class="absolute inset-0 bg-black/50 z-30 flex items-center justify-center px-4">
            <div class="bg-white w-full max-w-sm rounded-xl shadow-2xl p-6 relative">
              <h4 class="text-lg font-bold text-gray-800 mb-4">新建收藏夹</h4>
              <input
                v-model="newFolderName"
                type="text"
                placeholder="输入收藏夹名称"
                class="w-full border border-gray-200 rounded-lg px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-orange-400"
                @keyup.enter="submitCreateFolder"
              />
              <div class="flex justify-end space-x-3 mt-6">
                <button
                  class="px-4 py-2 text-sm text-gray-500 hover:text-gray-700"
                  @click="closeCreateModal"
                  :disabled="creating"
                >取消</button>
                <button
                  class="px-4 py-2 text-sm bg-orange-500 hover:bg-orange-600 text-white rounded-lg shadow-sm flex items-center space-x-2 disabled:opacity-60"
                  @click="submitCreateFolder"
                  :disabled="creating || !newFolderName.trim()"
                >
                  <i v-if="creating" class="fas fa-spinner fa-spin text-xs"></i>
                  <span>{{ creating ? '创建中...' : '创建' }}</span>
                </button>
              </div>
            </div>
          </div>
        </transition>

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
              @click.stop="openCreateModal"
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

                <!-- 删除按钮（非默认、非占位才显示） -->
                <button
                  v-if="folder.folderName !== '默认收藏夹' && folder.folderID > 0"
                  class="ml-2 text-gray-300 hover:text-red-500 transition-colors text-xs"
                  @click.stop="removeFolder(folder.folderID, folder.folderName)"
                  :disabled="deleting"
                >
                  <i class="fas fa-times"></i>
                </button>
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

            <!-- 收藏列表（长条券式） -->
            <div v-else class="space-y-4 pb-8">
              <div
                v-for="item in currentFolder.favoriteItems"
                :key="item.itemID"
                class="bg-white rounded-xl border border-orange-100 shadow-sm hover:shadow-lg transition-all duration-300 flex items-center gap-4 p-4 cursor-pointer hover:-translate-y-0.5"
                @click="goToStore(item.storeID)"
              >
                <!-- 左侧封面 -->
                <div class="w-20 h-20 rounded-lg bg-gray-100 overflow-hidden flex items-center justify-center flex-shrink-0">
                  <img
                    v-if="item.storeImage"
                    :src="normalizeImageUrl(item.storeImage)"
                    :alt="item.storeName"
                    class="w-full h-full object-cover"
                    @error="handleImageError"
                  />
                  <i v-else class="fas fa-store text-2xl text-gray-300"></i>
                </div>

                <!-- 中部信息 -->
                <div class="flex-1 min-w-0">
                  <div class="flex items-center justify-between">
                    <h4 class="font-bold text-gray-900 text-base truncate">{{ item.storeName }}</h4>
                  </div>
                  <div class="flex items-center justify-between mt-2">
                    <p class="text-sm text-gray-600 line-clamp-2 pr-2">
                      {{ item.favoriteReason || '这个人很懒，没有写备注~' }}
                    </p>
                    <span class="text-xs text-gray-400 flex-shrink-0">收藏于 {{ formatDate(item.favoritedAt) }}</span>
                  </div>
                </div>

                <!-- 右侧操作 -->
                <div class="flex items-center space-x-3 flex-shrink-0">
                  <button class="w-9 h-9 rounded-full bg-gray-100 text-gray-500 hover:text-red-500 hover:bg-red-50 transition-colors flex items-center justify-center" @click.stop="deleteItem(item)">
                    <i class="fas fa-trash-alt"></i>
                  </button>
                  <button class="w-9 h-9 rounded-full bg-orange-500 text-white flex items-center justify-center hover:bg-orange-600 transition-colors" @click.stop="goToStore(item.storeID)">
                    <i class="fas fa-arrow-right"></i>
                  </button>
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
import { ref, watch, computed } from 'vue'
import { useRouter } from 'vue-router'

import type { FavoritesFolder, FavoriteItem } from '@/api/user/favorites';
import { getFavoritesFolders, createFavoritesFolder, deleteFavoritesFolder, removeFavoriteFromFolder } from '@/api/user/favorites';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';

const router = useRouter();

const folders = ref<FavoritesFolder[]>([]);
const loading = ref(false);
const selectedFolderId = ref<number | null>(null);
const showCreateModal = ref(false);
const creating = ref(false);
const deleting = ref(false);
const newFolderName = ref('');

// 计算当前选中的文件夹
const currentFolder = computed(() => {
  if (!selectedFolderId.value) return null;
  return folders.value.find(f => f.folderID === selectedFolderId.value) || null;
});

// eslint-disable-next-line no-undef
const props = defineProps<{
    showFavoritesForm: Boolean;
}>();

// eslint-disable-next-line no-undef
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
    // 前端兜底：若后端无收藏夹，填充占位“默认收藏夹”以保证上栏显示
    folders.value = (data && data.length > 0) ? data : [{
      folderID: -1,
      folderName: '默认收藏夹',
      favoriteItems: []
    }];
    
    // 默认选中第一个收藏夹
    ensureSelectedFolder();
  } catch (error) {
    console.error('获取收藏夹信息失败:', error);
    // 出错时也提供占位，确保上栏可见
    folders.value = [{
      folderID: -1,
      folderName: '默认收藏夹',
      favoriteItems: []
    }];
    ensureSelectedFolder();
  } finally {
    loading.value = false;
  }
}

// 保证总有一个选中的收藏夹
function ensureSelectedFolder() {
  if (folders.value.length === 0) return;
  if (!selectedFolderId.value) {
    selectedFolderId.value = folders.value[0].folderID;
    return;
  }
  const exists = folders.value.some(f => f.folderID === selectedFolderId.value);
  if (!exists) {
    selectedFolderId.value = folders.value[0].folderID;
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
  // 店铺详情页路径：/store/:id/order
  router.push(`/store/${storeId}/order`);
}

// 删除收藏项
async function deleteItem(item: FavoriteItem) {
  if (!currentFolder.value) return;
  const confirmDelete = window.confirm(`确定要从「${currentFolder.value.folderName}」删除「${item.storeName}」吗？`);
  if (!confirmDelete) return;
  try {
    await removeFavoriteFromFolder(currentFolder.value.folderID, item.storeID);
    await loadFavorites();
  } catch (error) {
    console.error('删除收藏项失败', error);
    alert('删除失败，请稍后重试');
  }
}

// 新建收藏夹 (预留功能)
function openCreateModal() {
  newFolderName.value = '';
  showCreateModal.value = true;
}

function closeCreateModal() {
  showCreateModal.value = false;
}

async function submitCreateFolder() {
  if (!newFolderName.value.trim()) return;
  try {
    creating.value = true;
    await createFavoritesFolder(newFolderName.value.trim());
    showCreateModal.value = false;
    await loadFavorites();
  } catch (error) {
    console.error('新建收藏夹失败', error);
    alert('新建失败，请稍后重试');
  } finally {
    creating.value = false;
  }
}

// 删除收藏夹（不可删除默认/占位）
async function removeFolder(folderId: number, folderName: string) {
  if (folderId <= 0 || folderName === '默认收藏夹') {
    alert('默认收藏夹不可删除');
    return;
  }
  const confirmDelete = window.confirm(`确认删除收藏夹「${folderName}」？此操作不可恢复。`);
  if (!confirmDelete) return;
  try {
    deleting.value = true;
    await deleteFavoritesFolder(folderId);
    // 如果删除的是当前选中夹，重置选中
    if (selectedFolderId.value === folderId) {
      selectedFolderId.value = null;
    }
    await loadFavorites();
  } catch (error) {
    console.error('删除收藏夹失败', error);
    alert('删除失败，请稍后重试');
  } finally {
    deleting.value = false;
  }
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
