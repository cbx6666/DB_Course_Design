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
                                    v-if="formData.image && formData.image !== ''" 
                                    :src="formData.image" 
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

import type { AccountInfo, AccountUpdateData } from '@/api/user';
import { saveAccountInfo } from '@/api/user';
import { useUserStore } from '@/stores/user';
import { getAccountInfo } from '@/api/user';
import { handleImageError } from '@/utils/errorHandler';

const userStore = useUserStore();
const userID = userStore.getUserID();
// 用户信息
const accountInfo = ref({
    name: "",
    image: ""
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
    image: ''
})

// 添加文件引用
const selectedFile = ref<File | null>(null)

onMounted(async () => {
    try {
        const result = await getAccountInfo();
        accountInfo.value = result;
        // 确保头像URL是完整的URL
        if (result.image && !result.image.startsWith('http') && !result.image.startsWith('data:')) {
            result.image = `http://localhost:5250${result.image}`;
        }
    } catch (error) {
        console.error('获取账户信息失败:', error);
    }
});

watch(
    () => props.showAccountForm,
    (visible) => {
        if (visible && accountInfo.value) {
            formData.name = accountInfo.value.name;
            // 确保头像URL是完整的URL
            formData.image = accountInfo.value.image && !accountInfo.value.image.startsWith('http') && !accountInfo.value.image.startsWith('data:')
                ? `http://localhost:5250${accountInfo.value.image}`
                : accountInfo.value.image;
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
        if (!selectedFile.value) {
            alert('请选择头像文件');
            return;
        }

        const updateData: AccountUpdateData = {
            id: formData.id,
            name: formData.name,
            avatarFile: selectedFile.value
        };

        const result = await saveAccountInfo(updateData);
        
        if (result) {
            // 重新获取用户信息来获取正确的头像URL
            try {
                const updatedInfo = await getAccountInfo();
                accountInfo.value = updatedInfo;
                // 确保头像URL是完整的URL
                formData.image = updatedInfo.image.startsWith('http') 
                    ? updatedInfo.image 
                    : `http://localhost:5250${updatedInfo.image}`;
            } catch (error) {
                console.error('获取更新后的用户信息失败:', error);
            }

            emit('update:account', { ...formData })
            closeForm()
        }
        else {
            alert('保存失败')
        }
    } catch (err) {
        console.error(err)
        alert('更新账户信息时出错')
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
        formData.image = previewUrl;
    }
}

// 使用统一的图片错误处理函数
</script>
