<template>
    <div v-if="visible"
        class="fixed border-2 top-1/2 left-1/2 transform -translate-x-1/2 -translate-y-1/2 w-full max-w-md p-6 bg-white rounded-xl shadow-xl z-50">
        <!-- 标题 -->
        <h2 class="text-xl text-left font-semibold text-gray-800 mb-4">骑手打分</h2>

        <!-- 评分输入 -->
        <div class="mb-6">
            <!-- 五星评分 -->
            <div class="flex items-center justify-center mb-4">
                <template v-for="star in 5" :key="star">
                    <i class="fas fa-star cursor-pointer text-3xl mr-2 transition-colors"
                        :class="star <= courierRating ? 'text-yellow-400' : 'text-gray-300'"
                        @click="courierRating = star"></i>
                </template>
            </div>
            <p class="text-center text-sm text-gray-600">请为骑手的服务打分</p>
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
                class="px-4 py-2 rounded-lg bg-yellow-500 text-white text-sm hover:bg-yellow-600 disabled:opacity-50"
                :disabled="submitting || courierRating === 0" @click="submit">
                {{ submitting ? "提交中..." : "提交" }}
            </button>
        </div>
    </div>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits } from "vue";
import { ElMessage } from "element-plus";
import { useUserStore } from "@/stores/user";
import { rateCourier } from "@/api/user/courier";

const userStore = useUserStore();
const userId = userStore.getUserID();

const props = defineProps<{
    visible: boolean;
    courierId: number;
    orderId?: number;
    taskId?: number;
}>();

const emit = defineEmits(["close", "rated"]);

// 骑手评分
const courierRating = ref(0);

// 错误提示 & 提交状态
const errorMsg = ref("");
const submitting = ref(false);

function close() {
    emit("close");
    // 重置数据
    courierRating.value = 0;
    errorMsg.value = "";
}

async function submit() {
    errorMsg.value = "";

    if (courierRating.value === 0) {
        errorMsg.value = "请为骑手打分";
        return;
    }

    submitting.value = true;
    try {
        await rateCourier(userId, props.courierId, courierRating.value, props.orderId, props.taskId);
        ElMessage.success('评分提交成功');
        emit("rated");
        close();
    } catch (error: any) {
        const errorMessage = error?.response?.data?.message || error?.response?.data?.Message || error?.message || '提交失败，请稍后重试';
        errorMsg.value = errorMessage;
        ElMessage.error(errorMessage);
    } finally {
        submitting.value = false;
    }
}
</script>

