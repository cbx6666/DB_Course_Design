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
              <el-tag 
                :type="menu.isActive ? 'success' : 'info'" 
                size="small"
              >
                {{ menu.isActive ? '当前菜单' : '历史菜单' }}
              </el-tag>
            </div>
            <p class="text-sm text-gray-600 mb-2">{{ menu.description }}</p>
            <div class="text-xs text-gray-500">
              <p>版本: {{ menu.version }}</p>
              <p>创建时间: {{ formatDate(menu.createdAt) }}</p>
              <p>菜品数量: {{ menu.dishCount || 0 }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- 菜品管理区域 -->
      <div v-if="selectedMenu" class="bg-white rounded-lg shadow-sm p-6">
        <div class="flex items-center justify-between mb-4">
          <div>
            <h3 class="text-lg font-semibold text-gray-800">{{ selectedMenu.name }}</h3>
            <p class="text-sm text-gray-600">{{ selectedMenu.description }}</p>
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
              <div class="w-full h-32 bg-gray-100 rounded-lg flex items-center justify-center">
                <el-icon class="text-4xl text-gray-400">
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

        <el-form-item label="版本号" prop="version">
          <el-input v-model="menuForm.version" placeholder="如：v1.0" />
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
          >
            <img v-if="dishForm.image" :src="dishForm.image" class="avatar" />
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
import { ref, onMounted } from 'vue';
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
import { 
  getMenus, 
  createMenu, 
  getDishes, 
  createDish, 
  updateDish, 
  deleteDish,
  toggleDishSoldOut,
  type MenuInfo, 
  type Dish,
  type CreateMenuRequest,
  type NewDishData
} from '@/api/merchant';

// 类型定义
interface Menu {
  id: number;
  name: string;
  description: string;
  version: string;
  isActive: boolean;
  createdAt: string;
  dishCount?: number;
}

// 使用从 merchant_api 引入的 Dish 类型（避免本地定义冲突）

// 响应式数据
const menus = ref<Menu[]>([]);
const selectedMenu = ref<Menu | null>(null);
const dishes = ref<Dish[]>([]);
const loading = ref(false);
const dishesLoading = ref(false);
const creatingMenu = ref(false);
const creatingDish = ref(false);

// 弹窗状态
const showCreateMenuDialog = ref(false);
const showCreateDishDialog = ref(false);

// 菜单表单
const menuForm = ref({
  name: '',
  description: '',
  version: ''
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
  description: [{ required: true, message: '请输入菜单描述', trigger: 'blur' }],
  version: [{ required: true, message: '请输入版本号', trigger: 'blur' }]
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
    
    // 获取当前商家ID（从token或store中获取）
    const sellerId = 3; // TODO: 从实际登录状态获取
    
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
  devLog.component('MerchantMenu', `选择菜单: ${menu.name}`);
  await loadDishes();
};

// 加载菜品列表
const loadDishes = async () => {
  if (!selectedMenu.value) return;
  
  try {
    dishesLoading.value = true;
    devLog.component('MerchantMenu', `开始加载菜单 ${selectedMenu.value.name} 的菜品`);
    
    // 获取当前商家ID
    const sellerId = 3; // TODO: 从实际登录状态获取
    
    const dishesList = await getDishes(sellerId);
    dishes.value = dishesList || [];
    
    devLog.component('MerchantMenu', '菜品列表加载成功');
  } catch (error) {
    devLog.error('加载菜品列表失败:', error);
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
    description: '',
    version: ''
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
    const sellerId = 3; // TODO: 从实际登录状态获取
    
    const menuData: CreateMenuRequest = {
      name: menuForm.value.name,
      description: menuForm.value.description,
      version: menuForm.value.version
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

// 打开新建菜品弹窗
const openCreateDishDialog = () => {
  if (!selectedMenu.value) {
    ElMessage.warning('请先选择一个菜单');
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
    
    if (!selectedMenu.value) {
      ElMessage.error('请先选择一个菜单');
      return;
    }
    
    // 获取当前商家ID
    const sellerId = 3; // TODO: 从实际登录状态获取
    
    const dishData: NewDishData = {
      dishName: dishForm.value.name,
      description: dishForm.value.description,
      price: dishForm.value.price,
      isSoldOut: 2, // 默认在售
      categoryId: 1 // TODO: 从分类选择获取
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

// 切换菜品状态
const toggleDishStatusHandler = async (dish: Dish) => {
  try {
    // 获取当前商家ID
    const sellerId = 3; // TODO: 从实际登录状态获取
    
    // 切换状态：0=售罄, 2=在售
    const newStatus = dish.isSoldOut === 0 ? 2 : 0;
    
    await toggleDishSoldOut(dish.dishId, newStatus, sellerId);
    dish.isSoldOut = newStatus;
    
    ElMessage.success(dish.isSoldOut === 0 ? '菜品已下架' : '菜品已上架');
  } catch (error) {
    ElMessage.error('操作失败，请重试');
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
    // TODO: 实现图片上传
    await new Promise(resolve => setTimeout(resolve, 1000));
    
    // 模拟上传成功，返回图片URL
    dishForm.value.image = 'https://via.placeholder.com/300x200?text=Dish+Image';
    ElMessage.success('图片上传成功');
  } catch (error) {
    ElMessage.error('图片上传失败，请重试');
  }
};

// 处理图片加载错误
const handleImageError = (event: Event) => {
  const img = event.target as HTMLImageElement;
  img.src = 'https://via.placeholder.com/300x200?text=Image+Error';
};

// 格式化日期
const formatDate = (dateString: string) => {
  return new Date(dateString).toLocaleDateString('zh-CN');
};

// 组件挂载时加载数据
onMounted(() => {
  loadMenus();
});
</script>

<style scoped>
.avatar-uploader .avatar {
  width: 178px;
  height: 178px;
  display: block;
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
</style>

