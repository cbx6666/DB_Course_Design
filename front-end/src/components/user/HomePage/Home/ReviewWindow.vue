<template>
    <div v-if="visible"
        class="fixed border-2 top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-full max-w-md p-6 bg-white rounded-xl shadow-xl z-50">
        <!-- 标题 -->
        <h2 class="text-xl text-left font-semibold text-gray-800 mb-4">提交评价</h2>

        <!-- 评价输入 -->
        <div class="mb-6">
            <!-- 五星评分 -->
            <div class="flex items-center mb-2">
                <template v-for="star in 5" :key="star">
                    <i class="fas fa-star cursor-pointer text-xl mr-1 transition-colors"
                        :class="star <= storeRating ? 'text-yellow-400' : 'text-gray-300'"
                        @click="storeRating = star"></i>
                </template>
            </div>

            <!-- 文本评价 -->
            <textarea v-model="storeComment" placeholder="写下您对店铺的评价..." rows="3"
                class="w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-orange-500 focus:ring-1 focus:ring-orange-500 mb-4" />

            <!-- 图片上传 -->
            <div class="mb-4">
                <label class="block text-sm font-medium text-gray-700 mb-2">上传图片（可选）</label>
                <div class="flex flex-wrap gap-2 mb-2">
                    <div v-for="(image, index) in uploadedImages" :key="index" class="relative">
                        <img :src="image.startsWith('http') ? image : `${API_CONFIG.BASE_URL}${image}`" alt="上传的图片" class="w-20 h-20 object-cover rounded border border-gray-300" />
                        <button @click="removeImage(index)" class="absolute -top-2 -right-2 bg-red-500 text-white rounded-full w-5 h-5 flex items-center justify-center text-xs hover:bg-red-600">
                            ×
                        </button>
                    </div>
                    <label v-if="uploadedImages.length < 5" class="w-20 h-20 border-2 border-dashed border-gray-300 rounded flex items-center justify-center cursor-pointer hover:border-orange-500 transition-colors">
                        <input type="file" accept="image/*" class="hidden" @change="handleImageUpload" />
                        <i class="fas fa-plus text-gray-400 text-xl"></i>
                    </label>
                </div>
                <p class="text-xs text-gray-500">最多上传5张图片，支持 JPG/PNG 格式，单张不超过 5MB</p>
            </div>
        </div>

        <!-- 错误提示 -->
        <p v-if="errorMsg" class="text-sm text-red-500 mb-4">{{ errorMsg }}</p>

        <!-- 按钮 -->
        <div class="flex justify-end gap-3">
            <button class="px-4 py-2 rounded-lg border border-gray-300 text-gray-700 text-sm hover:bg-gray-100"
                @click="close">
                取消
            </button>
            <button
                class="px-4 py-2 rounded-lg bg-orange-500 text-white text-sm hover:bg-orange-600 disabled:opacity-50"
                :disabled="submitting" @click="submit">
                {{ submitting ? "提交中..." : "提交" }}
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from "vue";
import { ElMessage } from "element-plus";
import { useUserStore } from "@/stores/user";

import type { OrderInfo } from "@/api/user";
import { postStoreComment, uploadImage } from "@/api/user";
import { API_CONFIG } from "@/config/index";

const userStore = useUserStore();
const userId = userStore.getUserID();

const props = defineProps<{
    visible: boolean;
    order: OrderInfo;
}>();

const emit = defineEmits(["close"]);

// 输入框内容
const storeComment = ref("");

// 店铺评分
const storeRating = ref(0);

// 上传的图片URL列表
const uploadedImages = ref<string[]>([]);

// 错误提示 & 提交状态
const errorMsg = ref("");
const submitting = ref(false);
const uploadingImage = ref(false);

function close() {
    emit("close");
    // 重置数据
    storeComment.value = "";
    storeRating.value = 0;
    uploadedImages.value = [];
    errorMsg.value = "";
}

// 处理图片上传
async function handleImageUpload(event: Event) {
    const target = event.target as HTMLInputElement;
    const file = target.files?.[0];
    if (!file) return;

    // 验证文件类型
    if (!file.type.startsWith('image/')) {
        ElMessage.error('请选择图片文件');
        return;
    }

    // 验证文件大小 (5MB)
    if (file.size > 5 * 1024 * 1024) {
        ElMessage.error('图片大小不能超过 5MB');
        return;
    }

    uploadingImage.value = true;
    try {
        const imageUrl = await uploadImage(file);
        // 保存相对路径用于提交，但显示时使用完整路径
        uploadedImages.value.push(imageUrl);
    } catch (error: any) {
        ElMessage.error(error?.response?.data?.message || '图片上传失败，请重试');
    } finally {
        uploadingImage.value = false;
        // 清空input，允许重复选择同一文件
        target.value = '';
    }
}

// 移除图片
function removeImage(index: number) {
    uploadedImages.value.splice(index, 1);
}

async function submit() {
    errorMsg.value = "";

    if (!storeComment.value.trim()) {
        errorMsg.value = "请填写店铺评价内容";
        return;
    }
    if (storeRating.value === 0) {
        errorMsg.value = "请为店铺打分";
        return;
    }

    submitting.value = true;

    try {
        const storeId = props.order?.storeId;
        if (!storeId || storeId === 0) {
            errorMsg.value = "无法获取店铺信息，请刷新页面后重试";
            ElMessage.error("无法获取店铺信息，请刷新页面后重试");
            submitting.value = false;
            return;
        }
        const rating = storeRating.value;
        const content = storeComment.value.trim();
        const orderId = props.order?.orderId;
        // 将图片URL列表转换为逗号分隔的字符串
        const imagesString = uploadedImages.value.length > 0 ? uploadedImages.value.join(',') : undefined;
        await postStoreComment(userId, storeId, rating, content, imagesString, orderId);
        ElMessage.success('评价提交成功');
        close();
    } catch (error: any) {
        console.error('提交评价失败 - 完整错误对象:', error);
        console.error('error.response:', error?.response);
        console.error('error.response.data:', error?.response?.data);
        
        // 尝试从多个可能的路径提取错误信息
        let errorMessage = '提交失败，请稍后重试';
        if (error?.response?.data) {
            errorMessage = error.response.data.message || error.response.data.Message || errorMessage;
        } else if (error?.message) {
            errorMessage = error.message;
        }
        
        errorMsg.value = errorMessage;
        ElMessage.error(errorMessage);
    } finally {
        submitting.value = false;
    }
}
</script>
