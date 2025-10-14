<template>
    <transition name="fade">
        <div v-if="props.showAddressForm" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
            <div class="bg-white w-full max-w-md rounded-lg shadow-xl p-6">
                <div class="flex justify-between items-center mb-4">
                    <h3 class="font-medium text-gray-900">
                        收货地址管理
                    </h3>
                    <button class="text-gray-500 hover:text-gray-700" @click="closeForm">
                        <i class="fas fa-times"></i>
                    </button>
                </div>

                <div class="space-y-3">
                    <!-- 空态：先提示+按钮；点击后再显示表单 -->
                    <div v-if="deliveryInfos.length === 0 && !creating" class="py-8 text-center">
                        <div class="text-gray-500 mb-4">请创建收货地址</div>
                        <button type="button" class="w-full bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90"
                                @click="creating = true">
                            新建收货地址
                        </button>
                    </div>

                    <!-- 空态：创建表单 -->
                    <div v-else-if="deliveryInfos.length === 0 && creating" class="space-y-3">
                        <div class="text-lg font-medium mb-4">新建收货地址</div>
                        <input v-model="createForm.address" placeholder="详细地址" class="w-full border rounded px-3 py-2" />
                        <div class="grid grid-cols-2 gap-3">
                            <input v-model="createForm.phoneNumber" placeholder="手机号" class="border rounded px-3 py-2" />
                            <input v-model="createForm.name" placeholder="姓名" class="border rounded px-3 py-2" />
                        </div>
                        <select v-model="createForm.gender" class="w-full border rounded px-3 py-2">
                            <option value="先生">先生</option>
                            <option value="女士">女士</option>
                        </select>
                        <div class="flex gap-3">
                            <button type="button" :disabled="isSubmitting"
                                    class="flex-1 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90 disabled:opacity-60"
                                    @click="onSubmit">
                                {{ isSubmitting ? '创建中...' : '提交创建' }}
                            </button>
                            <button type="button" class="flex-1 border rounded px-4 py-2 hover:bg-gray-50"
                                    @click="creating = false">
                                取消
                            </button>
                        </div>
                    </div>

                    <!-- 非空：展示列表与操作按钮，或创建表单 -->
                    <template v-else>
                        <template v-if="!creating && !editing">
                            <div v-for="(item, idx) in deliveryInfos" :key="idx" class="border rounded-lg p-4 hover:bg-gray-50">
                                <div class="flex items-start gap-3">
                                    <!-- 默认地址选择圆圈 -->
                                    <div class="flex items-center mt-1">
                                        <button 
                                            @click="setDefaultAddress(idx)"
                                            :class="[
                                                'w-4 h-4 rounded-full border-2 flex items-center justify-center transition-colors',
                                                item.isDefault 
                                                    ? 'border-[#F9771C] bg-[#F9771C]' 
                                                    : 'border-gray-300 hover:border-[#F9771C]'
                                            ]"
                                        >
                                            <div v-if="item.isDefault" class="w-2 h-2 bg-white rounded-full"></div>
                                        </button>
                                    </div>
                                    
                                    <!-- 地址信息 -->
                                    <div class="flex-1">
                                        <div class="text-base font-semibold text-gray-900 mb-1 break-words">
                                            {{ item.address || '未填写详细地址' }}
                                        </div>
                                        <div class="text-sm text-gray-600">
                                            <span class="mr-3">{{ item.phoneNumber || '未填写手机号' }}</span>
                                            <span>{{ formatShortName(item.name) }}</span>
                                        </div>
                                    </div>
                                    
                                    <!-- 操作按钮 -->
                                    <div class="flex gap-2">
                                        <button 
                                            @click="startEdit(idx)"
                                            class="px-3 py-1 text-sm text-[#F9771C] border border-[#F9771C] rounded hover:bg-[#F9771C] hover:text-white transition-colors"
                                        >
                                            修改
                                        </button>
                                        <button 
                                            @click="showDeleteConfirm(idx)"
                                            class="px-3 py-1 text-sm text-red-600 border border-red-600 rounded hover:bg-red-600 hover:text-white transition-colors"
                                        >
                                            删除
                                        </button>
                                    </div>
                                </div>
                            </div>

                            <button type="button" class="w-full mt-2 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90"
                                    @click="creating = true">
                                新建收货地址
                            </button>

                            <button type="button" class="w-full border rounded px-4 py-2 hover:bg-gray-50 mt-2" @click="closeForm">
                                关闭
                            </button>
                        </template>
                        <template v-else-if="editing">
                            <div class="space-y-3">
                                <div class="text-lg font-medium mb-4">修改收货地址</div>
                                <input v-model="createForm.address" placeholder="详细地址" class="w-full border rounded px-3 py-2" />
                                <div class="grid grid-cols-2 gap-3">
                                    <input v-model="createForm.phoneNumber" placeholder="手机号" class="border rounded px-3 py-2" />
                                    <input v-model="createForm.name" placeholder="姓名" class="border rounded px-3 py-2" />
                                </div>
                                <select v-model="createForm.gender" class="w-full border rounded px-3 py-2">
                                    <option value="先生">先生</option>
                                    <option value="女士">女士</option>
                                </select>
                                <div class="flex gap-3">
                                    <button type="button" :disabled="isSubmitting"
                                            class="flex-1 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90 disabled:opacity-60"
                                            @click="onSubmit">
                                        {{ isSubmitting ? '更新中...' : '提交更新' }}
                                    </button>
                                    <button type="button" class="flex-1 border rounded px-4 py-2 hover:bg-gray-50"
                                            @click="cancelEdit">
                                        取消
                                    </button>
                                </div>
                            </div>
                        </template>
                        <template v-else-if="creating">
                            <div class="space-y-3">
                                <div class="text-lg font-medium mb-4">新建收货地址</div>
                                <input v-model="createForm.address" placeholder="详细地址" class="w-full border rounded px-3 py-2" />
                                <div class="grid grid-cols-2 gap-3">
                                    <input v-model="createForm.phoneNumber" placeholder="手机号" class="border rounded px-3 py-2" />
                                    <input v-model="createForm.name" placeholder="姓名" class="border rounded px-3 py-2" />
                                </div>
                                <select v-model="createForm.gender" class="w-full border rounded px-3 py-2">
                                    <option value="先生">先生</option>
                                    <option value="女士">女士</option>
                                </select>
                                <div class="flex gap-3">
                                    <button type="button" :disabled="isSubmitting"
                                            class="flex-1 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90 disabled:opacity-60"
                                            @click="onSubmit">
                                        {{ isSubmitting ? '创建中...' : '提交创建' }}
                                    </button>
                                    <button type="button" class="flex-1 border rounded px-4 py-2 hover:bg-gray-50"
                                            @click="creating = false">
                                        取消
                                    </button>
                                </div>
                            </div>
                        </template>
                    </template>
                </div>
            </div>
        </div>
    </transition>

    <!-- 删除确认弹窗 -->
    <transition name="fade">
        <div v-if="showDeleteModal" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
            <div class="bg-white w-full max-w-sm rounded-lg shadow-xl p-6">
                <div class="text-center">
                    <div class="text-lg font-medium text-gray-900 mb-2">确认删除</div>
                    <div class="text-gray-600 mb-6">是否删除此收货地址？</div>
                    <div class="flex gap-3">
                        <button 
                            @click="deleteAddress"
                            class="flex-1 bg-red-600 text-white rounded px-4 py-2 hover:bg-red-700 transition-colors"
                        >
                            删除
                        </button>
                        <button 
                            @click="cancelDelete"
                            class="flex-1 border border-gray-300 rounded px-4 py-2 hover:bg-gray-50 transition-colors"
                        >
                            取消
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </transition>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits, reactive, watch, onMounted } from 'vue'

import type { UserAddress as Address } from '@/api/user';
import { useUserStore } from '@/stores/user';
import API from '@/api/index';
import { createUserAddress as createAddress, type UserAddress as CreateAddressPayload } from '@/api/user';

const userStore = useUserStore();
const userID = userStore.getUserID();
const deliveryInfos = ref<Array<{ id?: number; name: string; phoneNumber: number; address: string; gender?: string; isDefault?: boolean }>>([])
const creating = ref(false)
const editing = ref(false)
const editingIndex = ref(-1)
const isSubmitting = ref(false)
const showDeleteModal = ref(false)
const deletingIndex = ref(-1)
const createForm = reactive<{ name: string; phoneNumber: string; address: string; gender: string }>({
    name: '', phoneNumber: '', address: '', gender: '先生'
})

const props = defineProps<{
    showAddressForm: Boolean;
}>();

const emit = defineEmits<{
    (e: "update:showAddressForm", value: Boolean): void;
    (e: "update:address", value: Address): void;
}>();


onMounted(async () => {
    await loadAddresses();
})

async function loadAddresses() {
    try {
        const resp = await API.get('/user/profile/addresses');
        const list = resp?.data as Array<{ deliveryInfoID: number; name: string; phoneNumber: string; address: string; gender?: string; isDefault: boolean }> | undefined;
        if (Array.isArray(list)) {
            deliveryInfos.value = list.map(x => ({
                id: x.deliveryInfoID,
                name: x.name,
                phoneNumber: Number(x.phoneNumber) || 0,
                address: x.address,
                gender: x.gender,
                isDefault: x.isDefault
            }));
        }
    } catch (error) {
        console.warn('获取地址信息失败:', error);
        deliveryInfos.value = [];
    }
}

watch(() => props.showAddressForm, () => {});

function closeForm() {
    creating.value = false;
    editing.value = false;
    editingIndex.value = -1;
    emit("update:showAddressForm", false);
}

function resetForm() {
    createForm.name = '';
    createForm.phoneNumber = '';
    createForm.address = '';
    createForm.gender = '先生';
}

function startEdit(index: number) {
    editingIndex.value = index;
    editing.value = true;
    const address = deliveryInfos.value[index];
    createForm.name = address.name;
    createForm.phoneNumber = address.phoneNumber.toString();
    createForm.address = address.address;
    createForm.gender = address.gender || '先生';
}

function cancelEdit() {
    editing.value = false;
    editingIndex.value = -1;
    resetForm();
}

function showDeleteConfirm(index: number) {
    deletingIndex.value = index;
    showDeleteModal.value = true;
}

function cancelDelete() {
    showDeleteModal.value = false;
    deletingIndex.value = -1;
}

function formatShortName(name: string) {
    if (!name || name.length === 0) return '未知用户';
    const lastName = name.charAt(0);
    if (name.includes('先生') || name.includes('女士')) return name;
    return `${lastName}先生`;
}

async function onSubmit() {
    if (!createForm.address || !createForm.phoneNumber || !createForm.name) return;
    
    try {
        isSubmitting.value = true;
        const payload: CreateAddressPayload = {
            id: 0,
            address: createForm.address,
            phoneNumber: Number(createForm.phoneNumber),
            name: createForm.name,
            gender: createForm.gender
        } as any;
        
        if (editing.value) {
            // 更新地址
            await updateAddress(editingIndex.value, payload);
        } else {
            // 创建地址
            await createAddress(payload);
        }
        
        await loadAddresses();
        resetForm();
        creating.value = false;
        editing.value = false;
        editingIndex.value = -1;
    } catch (error) {
        console.error('操作失败:', error);
        alert(editing.value ? '更新地址失败，请重试' : '创建地址失败，请重试');
    } finally {
        isSubmitting.value = false;
    }
}

async function updateAddress(index: number, payload: CreateAddressPayload) {
    const addressId = deliveryInfos.value[index].id;
    if (!addressId) throw new Error('地址ID不存在');
    
    await API.put(`/user/profile/account/address/update/${addressId}`, payload);
}

async function deleteAddress() {
    if (deletingIndex.value === -1) return;
    
    try {
        const addressId = deliveryInfos.value[deletingIndex.value].id;
        if (!addressId) throw new Error('地址ID不存在');
        
        await API.delete(`/user/profile/account/address/delete/${addressId}`);
        await loadAddresses();
        showDeleteModal.value = false;
        deletingIndex.value = -1;
    } catch (error) {
        console.error('删除地址失败:', error);
        alert('删除地址失败，请重试');
    }
}

async function setDefaultAddress(index: number) {
    try {
        const addressId = deliveryInfos.value[index].id;
        if (!addressId) throw new Error('地址ID不存在');
        
        await API.put(`/user/profile/account/address/set-default/${addressId}`);
        await loadAddresses();
    } catch (error) {
        console.error('设置默认地址失败:', error);
        alert('设置默认地址失败，请重试');
    }
}

</script>