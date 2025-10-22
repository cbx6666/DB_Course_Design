<template>
  <Layout>
    <!-- 菜品管理 -->
    <div>
      <div class="flex justify-between items-center mb-6">
        <h2 class="text-2xl font-bold text-gray-800">菜品管理</h2>
        <div class="flex items-center space-x-3">
          <el-button @click="openCreateMenuDialog" type="primary" :icon="Plus">
            新建菜单
          </el-button>
        </div>
      </div>

      <!-- 菜单选择区域 -->
      <div class="bg-white rounded-lg shadow-sm p-6 mb-6">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-lg font-semibold text-gray-800">选择菜单</h3>
          <el-button @click="refreshMenus" :icon="Refresh" size="small">
            刷新
          </el-button>
        </div>

        <!-- 菜单列表 -->
        <div v-if="loading" class="text-center py-8">
          <el-icon class="text-4xl text-gray-400 animate-spin">
            <Loading />
          </el-icon>
          <p class="text-gray-500 mt-2">加载菜单中...</p>
        </div>

        <div v-else-if="menus.length === 0" class="text-center py-12">
          <el-icon class="text-6xl text-gray-300 mb-4">
            <Document />
          </el-icon>
          <h4 class="text-lg font-medium text-gray-600 mb-2">暂无菜单</h4>
          <p class="text-gray-500 mb-4">请先创建一个菜单来管理菜品</p>
          <el-button @click="openCreateMenuDialog" type="primary" :icon="Plus">
            创建第一个菜单
          </el-button>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="menu in menus"
            :key="menu.id"
            @click="selectMenu(menu)"
            :class="[
              'border-2 rounded-lg p-4 cursor-pointer transition-all hover:shadow-md',
              selectedMenu?.id === menu.id 
                ? 'border-[#F9771C] bg-orange-50' 
                : 'border-gray-200 hover:border-gray-300'
            ]"
          >
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-semibold text-gray-800">{{ menu.name }}</h4>
              <div class="flex items-center space-x-2">
                <el-tag 
                  :type="menu.isActive ? 'success' : 'info'" 
                  size="small"
                >
                  {{ menu.isActive ? '使用中' : '未使用' }}
                </el-tag>
                <el-button 
                  v-if="!menu.isActive"
                  @click.stop="setActiveMenu(menu)"
                  type="primary" 
                  size="small"
                  :loading="settingActive"
                >
                  使用
                </el-button>
                <el-button 
                  @click.stop="deleteMenuHandler(menu)"
                  type="danger" 
                  size="small"
                  :loading="deletingMenu"
                >
                  删除
                </el-button>
              </div>
            </div>
            <p class="text-sm text-gray-600 mb-2">{{ menu.description }}</p>
            <div class="text-xs text-gray-500">
              <p>创建时间: {{ formatDate(menu.createdAt) }}</p>
              <p>菜品数量: {{ menu.dishCount || 0 }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 菜品种类管理区域 -->
      <div v-if="selectedMenu" class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-800">{{ selectedMenu.name }}</h3>
            <p class="text-sm text-gray-600">{{ selectedMenu.description }}</p>
          </div>
          <div class="flex items-center space-x-2">
            <el-button @click="loadDishCategories" :icon="Refresh" size="small">
              刷新分类
            </el-button>
            <el-button @click="openCreateCategoryDialog" type="primary" :icon="Plus">
              添加菜品种类
            </el-button>
          </div>
        </div>

        <!-- 菜品种类列表 -->
        <div v-if="categoriesLoading" class="text-center py-8">
          <el-icon class="text-4xl text-gray-400 animate-spin">
            <Loading />
          </el-icon>
          <p class="text-gray-500 mt-2">加载菜品种类中...</p>
        </div>

        <div v-else-if="dishCategories.length === 0" class="text-center py-12">
          <el-icon class="text-6xl text-gray-300 mb-4">
            <Document />
          </el-icon>
          <h4 class="text-lg font-medium text-gray-600 mb-2">暂无菜品种类</h4>
          <p class="text-gray-500 mb-4">请先创建菜品种类来组织您的菜品</p>
          <el-button @click="openCreateCategoryDialog" type="primary" :icon="Plus">
            创建第一个菜品种类
          </el-button>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
          <div
            v-for="category in dishCategories"
            :key="category.categoryID"
            @click="selectCategory(category)"
            :class="[
              'border-2 rounded-lg p-4 cursor-pointer transition-all hover:shadow-md',
              selectedCategory?.categoryID === category.categoryID 
                ? 'border-[#F9771C] bg-orange-50' 
                : 'border-gray-200 hover:border-gray-300'
            ]"
          >
            <div class="flex items-center justify-between mb-2">
              <h4 class="font-semibold text-gray-800">{{ category.categoryName }}</h4>
              <div class="flex items-center space-x-2">
                <el-tag type="info" size="small">
                  {{ category.dishCount }} 个菜品
                </el-tag>
                <el-button 
                  @click.stop="deleteCategoryHandler(category)"
                  type="danger" 
                  size="small"
                  :loading="deletingCategory"
                >
                  删除
                </el-button>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- 菜品管理区域 -->
      <div v-if="selectedMenu && selectedCategory" class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-800">{{ selectedCategory.categoryName }}</h3>
            <p class="text-sm text-gray-600">在 {{ selectedMenu.name }} 菜单中</p>
          </div>
          <div class="flex items-center space-x-2">
            <el-button @click="loadDishes" :icon="Refresh" size="small">
              刷新菜品
            </el-button>
            <el-button @click="openCreateDishDialog" type="primary" :icon="Plus">
              添加菜品
            </el-button>
          </div>
        </div>

        <!-- 菜品列表 -->
        <div v-if="dishesLoading" class="text-center py-8">
          <el-icon class="text-4xl text-gray-400 animate-spin">
            <Loading />
          </el-icon>
          <p class="text-gray-500 mt-2">加载菜品中...</p>
        </div>

        <div v-else-if="dishes.length === 0" class="text-center py-12">
          <el-icon class="text-6xl text-gray-300 mb-4">
            <Food />
          </el-icon>
          <h4 class="text-lg font-medium text-gray-600 mb-2">暂无菜品</h4>
          <p class="text-gray-500 mb-4">在此菜单中添加您的第一个菜品</p>
          <el-button @click="openCreateDishDialog" type="primary" :icon="Plus">
            添加菜品
          </el-button>
        </div>

        <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <div
            v-for="dish in dishes"
            :key="dish.dishId"
            class="border border-gray-200 rounded-lg p-4 hover:shadow-md transition-shadow"
          >
            <div class="aspect-w-16 aspect-h-9 mb-3">
              <div class="w-full h-32 bg-gray-50 rounded-lg flex items-center justify-center overflow-hidden border border-gray-200">
                <img 
                  v-if="dish.image && dish.image.trim() !== ''" 
                  :src="getFullImageUrl(dish.image)" 
                  :alt="dish.dishName"
                  class="w-full h-full object-contain bg-white dish-image"
                  @error="handleDishImageError"
                />
                <el-icon v-else class="text-4xl text-gray-400">
                  <Picture />
                </el-icon>
              </div>
            </div>
            
            <h4 class="font-semibold text-gray-800 mb-1">{{ dish.dishName }}</h4>
            <p class="text-sm text-gray-600 mb-2 line-clamp-2">{{ dish.description }}</p>
            
            <div class="flex items-center justify-between mb-3">
              <span class="text-lg font-bold text-[#F9771C]">¥{{ dish.price }}</span>
              <el-tag 
                :type="dish.isSoldOut === 0 ? 'danger' : 'success'" 
                size="small"
              >
                {{ dish.isSoldOut === 0 ? '售罄' : '在售' }}
              </el-tag>
            </div>

            <div class="flex items-center justify-end space-x-2">
              <el-button @click="editDish(dish)" size="small">
                编辑
              </el-button>
              <el-button 
                @click="toggleDishStatusHandler(dish)" 
                :type="dish.isSoldOut === 0 ? 'success' : 'warning'"
                size="small"
              >
                {{ dish.isSoldOut === 0 ? '上架' : '下架' }}
              </el-button>
              <el-button @click="deleteDishHandler(dish)" type="danger" size="small">
                删除
              </el-button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 新建菜单弹窗 -->
    <el-dialog
      v-model="showCreateMenuDialog"
      title="新建菜单"
      width="500px"
      @close="closeCreateMenuDialog"
    >
      <el-form :model="menuForm" :rules="menuRules" ref="menuFormRef" label-width="80px">
        <el-form-item label="菜单名称" prop="name">
          <el-input v-model="menuForm.name" placeholder="请输入菜单名称" />
        </el-form-item>
        
        <el-form-item label="菜单描述" prop="description">
          <el-input 
            v-model="menuForm.description" 
            type="textarea" 
            :rows="3"
            placeholder="请输入菜单描述" 
          />
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeCreateMenuDialog">取消</el-button>
          <el-button type="primary" @click="createMenuHandler" :loading="creatingMenu">
            创建
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 新建菜品种类弹窗 -->
    <el-dialog
      v-model="showCreateCategoryDialog"
      title="新建菜品种类"
      width="500px"
      @close="closeCreateCategoryDialog"
    >
      <el-form :model="categoryForm" :rules="categoryRules" ref="categoryFormRef" label-width="80px">
        <el-form-item label="类别名称" prop="categoryName">
          <el-input v-model="categoryForm.categoryName" placeholder="请输入菜品种类名称" />
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeCreateCategoryDialog">取消</el-button>
          <el-button type="primary" @click="createCategoryHandler" :loading="creatingCategory">
            创建
          </el-button>
        </div>
      </template>
    </el-dialog>

    <!-- 新建菜品弹窗 -->
    <el-dialog
      v-model="showCreateDishDialog"
      title="添加菜品"
      width="600px"
      @close="closeCreateDishDialog"
    >
      <el-form :model="dishForm" :rules="dishRules" ref="dishFormRef" label-width="80px">
        <el-form-item label="菜品名称" prop="name">
          <el-input v-model="dishForm.name" placeholder="请输入菜品名称" />
        </el-form-item>
        
        <el-form-item label="菜品描述" prop="description">
          <el-input 
            v-model="dishForm.description" 
            type="textarea" 
            :rows="3"
            placeholder="请输入菜品描述" 
          />
        </el-form-item>

        <el-form-item label="价格" prop="price">
          <el-input-number 
            v-model="dishForm.price" 
            :min="0" 
            :precision="2"
            suffix="元"
            class="w-full"
          />
        </el-form-item>

        <el-form-item label="菜品图片">
          <el-upload
            class="avatar-uploader"
            :show-file-list="false"
            :before-upload="beforeImageUpload"
            :http-request="uploadImage"
            :on-success="handleUploadSuccess"
            :on-error="handleUploadError"
          >
            <img v-if="dishForm.image && dishForm.image.trim() !== ''" :src="getFullImageUrl(dishForm.image)" class="avatar" @error="handleImageError" />
            <el-icon v-else class="avatar-uploader-icon"><Plus /></el-icon>
          </el-upload>
        </el-form-item>
      </el-form>

      <template #footer>
        <div class="dialog-footer">
          <el-button @click="closeCreateDishDialog">取消</el-button>
          <el-button type="primary" @click="createDishHandler" :loading="creatingDish">
            添加
          </el-button>
        </div>
      </template>
    </el-dialog>
  </Layout>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { 
  Plus, 
  Refresh, 
  Loading, 
  Document, 
  Food, 
  Picture 
} from '@element-plus/icons-vue';

import Layout from '@/features/merchant/components/Layout.vue';
import { devLog } from '@/utils/logger';
import { useMerchantLayout } from '@/composables/useMerchantLayout';
import { 
  getMenus, 
  createMenu,
  deleteMenu,
  getDishes, 
  createDish, 
  updateDish, 
  deleteDish,
  toggleDishSoldOut,
  setActiveMenu as setActiveMenuAPI,
  getDishCategories,
  createDishCategory,
  updateDishCategory,
  deleteDishCategory,
  uploadDishImage,
  type MenuInfo, 
  type Dish,
  type CreateMenuRequest,
  type NewDishData,
  type DishCategoryInfo,
  type CreateDishCategoryRequest
} from '@/api/merchant';

// 类型定义
interface Menu {
  id: number;
  name: string;
  description: string;
  isActive: boolean;
  createdAt: string;
  dishCount?: number;
}

// 使用从 merchant_api 引入的 Dish 类型（避免本地定义冲突）

// 使用商家布局组合式函数
const { merchantInfo } = useMerchantLayout();

// 响应式数据
const menus = ref<Menu[]>([]);
const selectedMenu = ref<Menu | null>(null);
const dishCategories = ref<DishCategoryInfo[]>([]);
const selectedCategory = ref<DishCategoryInfo | null>(null);
const dishes = ref<Dish[]>([]);
const loading = ref(false);
const categoriesLoading = ref(false);
const dishesLoading = ref(false);
const creatingMenu = ref(false);
const creatingCategory = ref(false);
const creatingDish = ref(false);
const deletingMenu = ref(false);
const deletingCategory = ref(false);

// 弹窗状态
const showCreateMenuDialog = ref(false);
const showCreateCategoryDialog = ref(false);
const showCreateDishDialog = ref(false);

// 菜单表单
const menuForm = ref({
  name: '',
  description: ''
});

// 菜品种类表单
const categoryForm = ref({
  categoryName: ''
});

// 菜品表单
const dishForm = ref({
  name: '',
  description: '',
  price: 0,
  image: ''
});

// 表单验证规则
const menuRules = {
  name: [{ required: true, message: '请输入菜单名称', trigger: 'blur' }],
  description: [{ required: true, message: '请输入菜单描述', trigger: 'blur' }]
};

const categoryRules = {
  categoryName: [{ required: true, message: '请输入菜品种类名称', trigger: 'blur' }]
};

const dishRules = {
  name: [{ required: true, message: '请输入菜品名称', trigger: 'blur' }],
  description: [{ required: true, message: '请输入菜品描述', trigger: 'blur' }],
  price: [{ required: true, message: '请输入价格', trigger: 'blur' }]
};

// 加载菜单列表
const loadMenus = async () => {
  try {
    loading.value = true;
    devLog.component('MerchantMenu', '开始加载菜单列表');
    
    // 获取当前商家ID
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const sellerId = merchantInfo.value.sellerId;
    
    const response = await getMenus(sellerId);
    menus.value = response.list || [];
    
    devLog.component('MerchantMenu', '菜单列表加载成功');
  } catch (error) {
    devLog.error('加载菜单列表失败:', error);
    ElMessage.error('加载菜单列表失败，请重试');
    menus.value = [];
  } finally {
    loading.value = false;
  }
};

// 刷新菜单
const refreshMenus = async () => {
  await loadMenus();
};

// 选择菜单
const selectMenu = async (menu: Menu) => {
  selectedMenu.value = menu;
  selectedCategory.value = null;
  devLog.component('MerchantMenu', `选择菜单: ${menu.name}`);
  await loadDishCategories();
};

// 选择菜品种类
const selectCategory = async (category: DishCategoryInfo) => {
  selectedCategory.value = category;
  devLog.component('MerchantMenu', `选择菜品种类: ${category.categoryName}`);
  await loadDishes();
};

// 加载菜品种类列表
const loadDishCategories = async () => {
  if (!selectedMenu.value) return;
  
  try {
    categoriesLoading.value = true;
    devLog.component('MerchantMenu', `开始加载菜品种类列表，菜单ID: ${selectedMenu.value.id}`);
    
    const response = await getDishCategories(selectedMenu.value.id);
    dishCategories.value = response.list;
    
    devLog.component('MerchantMenu', `菜品种类列表加载成功，共${response.total}个类别`);
  } catch (error) {
    devLog.error('加载菜品种类列表失败:', error);
    ElMessage.error('加载菜品种类失败，请重试');
  } finally {
    categoriesLoading.value = false;
  }
};

// 加载菜品列表
const loadDishes = async () => {
  if (!selectedMenu.value || !selectedCategory.value) return;
  
  try {
    dishesLoading.value = true;
    
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const categoryId = selectedCategory.value!.categoryID;
    const dishesList = await getDishes(categoryId);
    dishes.value = dishesList || [];
    
  } catch (error) {
    ElMessage.error('加载菜品列表失败，请重试');
    dishes.value = [];
  } finally {
    dishesLoading.value = false;
  }
};

// 打开新建菜单弹窗
const openCreateMenuDialog = () => {
  menuForm.value = {
    name: '',
    description: ''
  };
  showCreateMenuDialog.value = true;
};

// 关闭新建菜单弹窗
const closeCreateMenuDialog = () => {
  showCreateMenuDialog.value = false;
};

// 创建菜单
const createMenuHandler = async () => {
  try {
    creatingMenu.value = true;
    
    // 获取当前商家ID
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const sellerId = merchantInfo.value.sellerId;
    
    const menuData: CreateMenuRequest = {
      name: menuForm.value.name,
      description: menuForm.value.description
    };
    
    await createMenu(menuData, sellerId);
    
    ElMessage.success('菜单创建成功');
    closeCreateMenuDialog();
    await loadMenus();
  } catch (error) {
    ElMessage.error('创建菜单失败，请重试');
  } finally {
    creatingMenu.value = false;
  }
};

// 打开新建菜品种类弹窗
const openCreateCategoryDialog = () => {
  if (!selectedMenu.value) {
    ElMessage.warning('请先选择一个菜单');
    return;
  }
  
  categoryForm.value = {
    categoryName: ''
  };
  showCreateCategoryDialog.value = true;
};

// 关闭新建菜品种类弹窗
const closeCreateCategoryDialog = () => {
  showCreateCategoryDialog.value = false;
};

// 创建菜品种类
const createCategoryHandler = async () => {
  try {
    creatingCategory.value = true;
    
    if (!selectedMenu.value) {
      ElMessage.error('请先选择一个菜单');
      return;
    }
    
    const categoryData: CreateDishCategoryRequest = {
      categoryName: categoryForm.value.categoryName,
      menuId: selectedMenu.value.id
    };
    
    await createDishCategory(categoryData);
    
    ElMessage.success('菜品种类创建成功');
    closeCreateCategoryDialog();
    await loadDishCategories();
  } catch (error) {
    ElMessage.error('创建菜品种类失败，请重试');
  } finally {
    creatingCategory.value = false;
  }
};

// 打开新建菜品弹窗
const openCreateDishDialog = () => {
  if (!selectedMenu.value) {
    ElMessage.warning('请先选择一个菜单');
    return;
  }
  
  if (!selectedCategory.value) {
    ElMessage.warning('请先选择一个菜品种类');
    return;
  }
  
  dishForm.value = {
    name: '',
    description: '',
    price: 0,
    image: ''
  };
  showCreateDishDialog.value = true;
};

// 关闭新建菜品弹窗
const closeCreateDishDialog = () => {
  showCreateDishDialog.value = false;
};

// 创建菜品
const createDishHandler = async () => {
  try {
    creatingDish.value = true;
    
    if (!selectedMenu.value || !selectedCategory.value) {
      ElMessage.error('请先选择一个菜单和菜品种类');
      return;
    }
    
    // 获取当前商家ID
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const sellerId = merchantInfo.value.sellerId;
    
    const dishData: NewDishData = {
      dishName: dishForm.value.name,
      description: dishForm.value.description,
      price: dishForm.value.price,
      isSoldOut: 2, // 默认在售
      categoryId: selectedCategory.value.categoryID,
      image: dishForm.value.image
    };
    
    await createDish(dishData, sellerId);
    ElMessage.success('菜品添加成功');
    closeCreateDishDialog();
    await loadDishes();
  } catch (error) {
    ElMessage.error('添加菜品失败，请重试');
  } finally {
    creatingDish.value = false;
  }
};

// 编辑菜品
const editDish = (dish: Dish) => {
  ElMessage.info('编辑菜品功能开发中...');
};

// 设置激活菜单
const settingActive = ref(false);
const setActiveMenu = async (menu: Menu) => {
  try {
    settingActive.value = true;
    
    // 获取当前商家ID
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const sellerId = merchantInfo.value.sellerId;
    
    await setActiveMenuAPI(menu.id, sellerId);
    
    // 更新菜单状态
    menu.isActive = true;
    
    // 将其他菜单设置为未激活
    menus.value.forEach(m => {
      if (m.id !== menu.id) {
        m.isActive = false;
      }
    });
    
    ElMessage.success(`菜单"${menu.name}"已设为使用`);
  } catch (error) {
    ElMessage.error('设置失败，请重试');
  } finally {
    settingActive.value = false;
  }
};

// 切换菜品状态
const toggleDishStatusHandler = async (dish: Dish) => {
  try {
    // 获取当前商家ID
    if (!merchantInfo.value?.sellerId) {
      ElMessage.error('商家信息未加载，请刷新页面重试');
      return;
    }
    
    const sellerId = merchantInfo.value.sellerId;
    
    // 切换状态：0=售罄, 2=在售
    const newStatus = dish.isSoldOut === 0 ? 2 : 0;
    
    await toggleDishSoldOut(dish.dishId, newStatus, sellerId);
    dish.isSoldOut = newStatus;
    
    ElMessage.success(dish.isSoldOut === 0 ? '菜品已下架' : '菜品已上架');
  } catch (error) {
    ElMessage.error('操作失败，请重试');
  }
};

// 删除菜单
const deleteMenuHandler = async (menu: Menu) => {
  try {
    await ElMessageBox.confirm(`确定要删除菜单"${menu.name}"吗？删除后不可恢复！`, '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning',
    });

    deletingMenu.value = true;
    await deleteMenu(menu.id);
    
    ElMessage.success('菜单删除成功');
    
    // 重新加载菜单列表
    await loadMenus();
    
    // 如果删除的是当前选中的菜单，清空选择
    if (selectedMenu.value?.id === menu.id) {
      selectedMenu.value = null;
      selectedCategory.value = null;
      dishCategories.value = [];
      dishes.value = [];
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除菜单失败，请重试');
    }
  } finally {
    deletingMenu.value = false;
  }
};

// 删除菜品种类
const deleteCategoryHandler = async (category: DishCategoryInfo) => {
  try {
    await ElMessageBox.confirm(`确定要删除菜品种类"${category.categoryName}"吗？删除后不可恢复！`, '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning',
    });

    deletingCategory.value = true;
    await deleteDishCategory(category.categoryID);
    
    ElMessage.success('菜品种类删除成功');
    
    // 重新加载菜品种类列表
    if (selectedMenu.value) {
      await loadDishCategories();
    }
    
    // 如果删除的是当前选中的菜品种类，清空选择
    if (selectedCategory.value?.categoryID === category.categoryID) {
      selectedCategory.value = null;
      dishes.value = [];
    }
  } catch (error: any) {
    if (error !== 'cancel') {
      ElMessage.error('删除菜品种类失败，请重试');
    }
  } finally {
    deletingCategory.value = false;
  }
};

// 删除菜品
const deleteDishHandler = async (dish: Dish) => {
  try {
    await ElMessageBox.confirm('确定要删除这个菜品吗？删除后不可恢复！', '确认删除', {
      confirmButtonText: '确定',
      cancelButtonText: '取消',
      type: 'warning'
    });

    await deleteDish(dish.dishId);
    ElMessage.success('菜品已删除');
    await loadDishes();
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败，请重试');
    }
  }
};

// 图片上传前验证
const beforeImageUpload = (file: File) => {
  const isJPG = file.type === 'image/jpeg' || file.type === 'image/png';
  const isLt2M = file.size / 1024 / 1024 < 2;

  if (!isJPG) {
    ElMessage.error('图片只能是 JPG/PNG 格式!');
  }
  if (!isLt2M) {
    ElMessage.error('图片大小不能超过 2MB!');
  }
  return isJPG && isLt2M;
};

// 上传图片
const uploadImage = async (options: any) => {
  try {
    const file = options.file;
    if (!file) {
      throw new Error('没有选择文件');
    }
    
    const imageUrl = await uploadDishImage(file);
    dishForm.value.image = imageUrl;
  } catch (error: any) {
    ElMessage.error(`图片上传失败: ${error.message}`);
    throw error;
  }
};

// 处理图片加载错误
const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement;
  console.error('图片加载失败:', img.src);
  img.src = 'https://via.placeholder.com/300x200?text=Image+Error';
};

// 处理菜品图片加载错误
const handleDishImageError = (event: Event) => {
  const img = event.target as HTMLImageElement;
  console.error('菜品图片加载失败:', img.src);
  // 隐藏图片，显示占位符
  img.style.display = 'none';
};

// 处理上传成功
const handleUploadSuccess = (response: any, file: any) => {
  console.log('上传成功回调:', response, file);
  ElMessage.success('图片上传成功');
};

// 处理上传失败
const handleUploadError = (error: any, file: any) => {
  console.error('上传失败回调:', error, file);
  ElMessage.error('图片上传失败，请重试');
};

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN');
};

// 获取完整的图片URL
const getFullImageUrl = (imageUrl: string) => {
  if (!imageUrl) return '';
  
  // 如果URL已经是完整的（包含协议），直接返回
  if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
    return imageUrl;
  }
  
  // 如果URL以/开头，拼接后端地址
  if (imageUrl.startsWith('/')) {
    return `http://localhost:5250${imageUrl}`;
  }
  
  // 否则直接返回原URL
  return imageUrl;
};

// 组件挂载时加载数据
onMounted(() => {
  // 监听商家信息变化，当商家信息加载完成后自动加载菜单
  watch(merchantInfo, (newMerchantInfo) => {
    if (newMerchantInfo?.sellerId) {
      loadMenus();
    }
  }, { immediate: true });
});
</script>

<style scoped>
.avatar-uploader .avatar {
  width: 178px;
  height: 178px;
  display: block;
  object-fit: contain;
  background-color: white;
  border-radius: 6px;
}

.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  text-align: center;
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: 0.2s;
}

.avatar-uploader-icon:hover {
  border-color: #409eff;
}

.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

/* 菜品图片样式优化 */
.dish-image {
  transition: transform 0.2s ease;
}

.dish-image:hover {
  transform: scale(1.02);
}

/* 上传图片预览样式 */
.avatar-uploader .avatar {
  transition: opacity 0.2s ease;
}

.avatar-uploader .avatar:hover {
  opacity: 0.9;
}
</style>

