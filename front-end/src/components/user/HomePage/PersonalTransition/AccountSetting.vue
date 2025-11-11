<template>
    <transition name="fade">
        <div v-if="props.showAccountForm" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
            <div class="bg-white w-full max-w-md rounded-lg shadow-xl p-6 overflow-y-auto max-h-[80vh]">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="font-medium text-gray-900">
                        账户设置
                    </h3>
                    <button class="text-gray-500 hover:text-gray-700" @click="closeForm">
                        <i class="fas fa-times"></i>
                    </button>
                </div>

                <div>
                    <form @submit.prevent="saveAccount" class="space-y-4">
                        <!-- 头像上传 -->
                        <div class="flex flex-col items-center mb-4">
                            <div class="w-20 h-20 rounded-full overflow-hidden bg-gray-200 flex items-center justify-center text-gray-400 text-2xl font-bold mb-2 cursor-pointer hover:ring-2 hover:ring-[#F9771C]"
                                @click="triggerFileInput">
                                <img 
                                    v-if="formData.avatar && formData.avatar !== ''" 
                                    :src="formData.avatar" 
                                    alt="用户头像" 
                                    class="w-full h-full object-cover"
                                    @error="handleImageError"
                                />
                                <div 
                                    v-else
                                    class="w-full h-full bg-orange-500 flex items-center justify-center text-white text-xl font-bold"
                                >
                                    {{ formData.name ? formData.name.charAt(0) : '?' }}
                                </div>
                            </div>
                            <!-- 隐藏文件输入框 -->
                            <input ref="fileInput" type="file" accept="image/*" class="hidden"
                                @change="onAvatarChange" />
                        </div>

                        <!-- 昵称 -->
                        <div>
                            <label class="block text-sm text-gray-700 mb-1">昵称</label>
                            <input v-model="formData.name" type="text" class="w-full border rounded px-2 py-1"
                                required />
                        </div>

                        <!-- 操作按钮 -->
                        <div class="flex gap-3 pt-2">
                            <button type="button" class="flex-1 border rounded px-4 py-2 hover:bg-gray-50"
                                @click="closeForm">
                                取消
                            </button>
                            <button type="submit"
                                class="flex-1 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90">
                                保存
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import { reactive, defineProps, defineEmits, ref, watch, onMounted } from 'vue'
import { ElMessage } from 'element-plus'

import type { AccountInfo, AccountUpdateData } from '@/api/user';
import { saveAccountInfo, getAccountInfo } from '@/api/user';
import { useUserStore } from '@/stores/user';
import { handleImageError } from '@/utils/imageUtils';
import { API_CONFIG } from '@/config';

const userStore = useUserStore();
const userID = userStore.getUserID();
// 用户信息
const accountInfo = ref({
    name: "",
    avatar: ""
});

const props = defineProps<{
    showAccountForm: Boolean;
}>()

const emit = defineEmits<{
    (e: 'update:showAccountForm', value: Boolean): void;
    (e: 'update:account', value: AccountInfo): void;
}>()

const formData = reactive<AccountInfo>({
    id: userID,
    name: '',
    avatar: ''
})

// 添加文件引用
const selectedFile = ref<File | null>(null)

onMounted(async () => {
    try {
        const result = await getAccountInfo();
        accountInfo.value = result;
        // 确保头像URL是完整的URL
        if (result.avatar && !result.avatar.startsWith('http') && !result.avatar.startsWith('data:')) {
            result.avatar = `${API_CONFIG.BASE_URL}${result.avatar}`;
        }
    } catch (error) {
        // 静默处理错误
    }
});

watch(
    () => props.showAccountForm,
    (visible) => {
        if (visible && accountInfo.value) {
            formData.name = accountInfo.value.name;
            // 确保头像URL是完整的URL
            formData.avatar = accountInfo.value.avatar && !accountInfo.value.avatar.startsWith('http') && !accountInfo.value.avatar.startsWith('data:')
                ? `${API_CONFIG.BASE_URL}${accountInfo.value.avatar}`
                : accountInfo.value.avatar;
        }
    },
);

// ref 用于触发隐藏的文件输入
const fileInput = ref<HTMLInputElement | null>(null)

function triggerFileInput() {
    fileInput.value?.click()
}

// 关闭弹窗
function closeForm() {
    emit('update:showAccountForm', false)
}

// 保存修改
async function saveAccount() {
    try {
        // 验证姓名
        if (!formData.name || formData.name.trim() === '') {
            ElMessage.error('请输入姓名');
            return;
        }

        const updateData: AccountUpdateData = {
            id: formData.id,
            name: formData.name.trim(),
            avatarFile: selectedFile.value! // 后端会检查文件是否存在
        };

        const result = await saveAccountInfo(updateData);
        
        // 检查响应中的 success 字段
        if (result && result.success) {
            ElMessage.success('账户信息更新成功');
            
            // 重新获取用户信息来获取正确的头像URL
            try {
                const updatedInfo = await getAccountInfo();
                accountInfo.value = updatedInfo;
                // 确保头像URL是完整的URL
                formData.avatar = updatedInfo.avatar.startsWith('http') 
                    ? updatedInfo.avatar 
                    : `${API_CONFIG.BASE_URL}${updatedInfo.avatar}`;
            } catch (error) {
                // 静默处理错误
            }

            emit('update:account', { ...formData })
            closeForm()
        }
        else {
            // 显示后端返回的错误信息
            const errorMessage = result?.message || '保存失败，请重试';
            ElMessage.error(errorMessage);
        }
    } catch (err: any) {
        // 提取错误信息
        const errorMessage = err?.response?.data?.message || err?.response?.data?.Message || err?.message || '更新账户信息时出错，请重试';
        ElMessage.error(errorMessage);
    }
}

// 头像选择
function onAvatarChange(event: Event) {
    const target = event.target as HTMLInputElement
    const file = target.files?.[0]
    if (file) {
        // 保存文件引用用于上传
        selectedFile.value = file;
        
        // 生成预览URL用于显示
        const previewUrl = URL.createObjectURL(file);
        formData.avatar = previewUrl;
    }
}

// 使用统一的图片错误处理函数
</script>
