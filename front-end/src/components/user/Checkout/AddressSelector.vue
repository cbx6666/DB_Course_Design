<template>
  <div>
    <!-- 地址卡片 -->
    <div class="bg-white shadow-lg rounded-lg border-0">
      <div class="flex items-center justify-between px-4 py-3 border-b">
        <div class="flex items-center gap-2 text-[#F9771C]">
          <i class="fas fa-map-pin"></i>
          收货地址
        </div>
        <button
          class="text-[#F9771C] text-sm flex items-center gap-1 hover:bg-[#F9771C]/10 px-2 py-1 rounded"
          @click="showAddressManager = true"
        >
          {{ selectedAddress ? '更换' : '选择' }} <i class="fas fa-chevron-right text-xs"></i>
        </button>
      </div>

      <div class="p-4">
        <div v-if="selectedAddress">
          <div class="flex items-center gap-2 mb-1">
            <span class="font-semibold text-gray-900">{{ selectedAddress.name }}</span>
            <span class="text-sm text-gray-600">{{ selectedAddress.phoneNumber }}</span>
          </div>
          <p class="text-sm text-gray-600 leading-relaxed">{{ selectedAddress.address }}</p>
        </div>
        <div v-else class="text-gray-500 text-center py-4">
          请选择收货地址
        </div>
      </div>
    </div>

    <!-- 地址管理弹窗 -->
    <transition name="fade">
      <div v-if="showAddressManager" class="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
        <div class="bg-white w-full max-w-md rounded-lg shadow-xl p-6 max-h-[80vh] overflow-y-auto">
          <div class="flex justify-between items-center mb-4">
            <h3 class="font-medium text-gray-900">选择收货地址</h3>
            <button class="text-gray-500 hover:text-gray-700" @click="closeAddressManager">
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
                <div v-for="(item, idx) in deliveryInfos" :key="idx" 
                     :class="[
                       'border rounded-lg p-4 cursor-pointer transition-colors',
                       selectedAddress?.id === item.id 
                         ? 'border-[#F9771C] bg-[#F9771C]/5' 
                         : 'hover:bg-gray-50'
                     ]"
                     @click="selectAddress(item)">
                  <div class="flex items-start gap-3">
                    <!-- 选择状态圆圈 -->
                    <div class="flex items-center mt-1">
                      <div 
                        :class="[
                          'w-4 h-4 rounded-full border-2 flex items-center justify-center transition-colors',
                          selectedAddress?.id === item.id 
                            ? 'border-[#F9771C] bg-[#F9771C]' 
                            : 'border-gray-300'
                        ]"
                      >
                        <div v-if="selectedAddress?.id === item.id" class="w-2 h-2 bg-white rounded-full"></div>
                      </div>
                    </div>
                    
                    <!-- 地址信息 -->
                    <div class="flex-1">
                      <div class="text-base font-semibold text-gray-900 mb-1 break-words">
                        {{ item.address || '未填写详细地址' }}
                      </div>
                      <div class="text-sm text-gray-600">
                        <span class="mr-3">{{ item.phoneNumber || '未填写手机号' }}</span>
                        <span>{{ formatShortName(item.name) }}</span>
                        <span v-if="item.isDefault" class="ml-2 text-xs bg-[#F9771C] text-white px-2 py-0.5 rounded">默认</span>
                      </div>
                    </div>
                    
                    <!-- 操作按钮 -->
                    <div class="flex gap-2" @click.stop>
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

                <div class="flex gap-3 mt-2">
                  <button type="button" class="flex-1 border rounded px-4 py-2 hover:bg-gray-50" @click="closeAddressManager">
                    取消
                  </button>
                  <button type="button" 
                          :disabled="!selectedAddress"
                          class="flex-1 bg-[#F9771C] text-white rounded px-4 py-2 hover:bg-[#F9771C]/90 disabled:opacity-50 disabled:cursor-not-allowed"
                          @click="confirmSelection">
                    确认选择
                  </button>
                </div>
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
  </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted, defineEmits, defineProps } from 'vue'

import type { UserAddress as Address } from '@/api/user'
import { useUserStore } from '@/stores/user'
import API from '@/api/index'
import { createUserAddress as createAddress, type UserAddress as CreateAddressPayload } from '@/api/user'

const userStore = useUserStore()
const userID = userStore.getUserID()

const props = defineProps<{
  selectedAddress?: Address
}>()

const emit = defineEmits<{
  (e: 'onAddressChange', addr: Address): void
}>()

// 状态管理
const deliveryInfos = ref<Array<{ id?: number; name: string; phoneNumber: number; address: string; gender?: string; isDefault?: boolean }>>([])
const selectedAddress = ref<Address | undefined>(props.selectedAddress)
const showAddressManager = ref(false)
const creating = ref(false)
const editing = ref(false)
const editingIndex = ref(-1)
const isSubmitting = ref(false)
const showDeleteModal = ref(false)
const deletingIndex = ref(-1)

const createForm = reactive<{ name: string; phoneNumber: string; address: string; gender: string }>({
  name: '', phoneNumber: '', address: '', gender: '先生'
})

onMounted(async () => {
  await loadAddresses()
  // 如果没有选中地址，自动选择默认地址
  if (!selectedAddress.value && deliveryInfos.value.length > 0) {
    const defaultAddr = deliveryInfos.value.find(addr => addr.isDefault) || deliveryInfos.value[0]
    if (defaultAddr) {
      selectedAddress.value = defaultAddr as Address
      emit('onAddressChange', selectedAddress.value)
    }
  }
})

async function loadAddresses() {
  try {
    const resp = await API.get('/user/profile/addresses')
    const list = resp?.data as Array<{ deliveryInfoID: number; name: string; phoneNumber: string; address: string; gender?: string; isDefault: boolean }> | undefined
    if (Array.isArray(list)) {
      deliveryInfos.value = list.map(x => ({
        id: x.deliveryInfoID,
        name: x.name,
        phoneNumber: Number(x.phoneNumber) || 0,
        address: x.address,
        gender: x.gender,
        isDefault: x.isDefault
      }))
    }
  } catch (error) {
    console.warn('获取地址信息失败:', error)
    deliveryInfos.value = []
  }
}

function closeAddressManager() {
  creating.value = false
  editing.value = false
  editingIndex.value = -1
  showAddressManager.value = false
}

function selectAddress(address: Address) {
  selectedAddress.value = address
}

function confirmSelection() {
  if (selectedAddress.value) {
    emit('onAddressChange', selectedAddress.value)
    closeAddressManager()
  }
}

function resetForm() {
  createForm.name = ''
  createForm.phoneNumber = ''
  createForm.address = ''
  createForm.gender = '先生'
}

function startEdit(index: number) {
  editingIndex.value = index
  editing.value = true
  const address = deliveryInfos.value[index]
  createForm.name = address.name
  createForm.phoneNumber = address.phoneNumber.toString()
  createForm.address = address.address
  createForm.gender = address.gender || '先生'
}

function cancelEdit() {
  editing.value = false
  editingIndex.value = -1
  resetForm()
}

function showDeleteConfirm(index: number) {
  deletingIndex.value = index
  showDeleteModal.value = true
}

function cancelDelete() {
  showDeleteModal.value = false
  deletingIndex.value = -1
}

function formatShortName(name: string) {
  if (!name || name.length === 0) return '未知用户'
  const lastName = name.charAt(0)
  if (name.includes('先生') || name.includes('女士')) return name
  return `${lastName}先生`
}

async function onSubmit() {
  if (!createForm.address || !createForm.phoneNumber || !createForm.name) return
  
  try {
    isSubmitting.value = true
    const payload: CreateAddressPayload = {
      id: 0,
      address: createForm.address,
      phoneNumber: Number(createForm.phoneNumber),
      name: createForm.name,
      gender: createForm.gender
    } as any
    
    if (editing.value) {
      // 更新地址
      await updateAddress(editingIndex.value, payload)
    } else {
      // 创建地址
      await createAddress(payload)
    }
    
    await loadAddresses()
    resetForm()
    creating.value = false
    editing.value = false
    editingIndex.value = -1
  } catch (error) {
    console.error('操作失败:', error)
    alert(editing.value ? '更新地址失败，请重试' : '创建地址失败，请重试')
  } finally {
    isSubmitting.value = false
  }
}

async function updateAddress(index: number, payload: CreateAddressPayload) {
  const addressId = deliveryInfos.value[index].id
  if (!addressId) throw new Error('地址ID不存在')
  
  await API.put(`/user/profile/account/address/update/${addressId}`, payload)
}

async function deleteAddress() {
  if (deletingIndex.value === -1) return
  
  try {
    const addressId = deliveryInfos.value[deletingIndex.value].id
    if (!addressId) throw new Error('地址ID不存在')
    
    await API.delete(`/user/profile/account/address/delete/${addressId}`)
    
    // 如果删除的是当前选中的地址，清空选择
    if (selectedAddress.value?.id === addressId) {
      selectedAddress.value = undefined
      emit('onAddressChange', undefined as any)
    }
    
    await loadAddresses()
    showDeleteModal.value = false
    deletingIndex.value = -1
  } catch (error) {
    console.error('删除地址失败:', error)
    alert('删除地址失败，请重试')
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
</style>
