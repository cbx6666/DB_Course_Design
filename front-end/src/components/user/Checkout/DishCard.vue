<template>
  <div v-if="visible" class="dish-card group">
    <div class="dish-card__image">
        <img
          :src="normalizeImageUrl(item.image)"
        class="w-full h-full object-cover transition-transform duration-200 group-hover:scale-105"
        @error="handleImageError"
        />
      <button class="dish-card__remove" @click="handleRemove">
        <i class="fas fa-times"></i>
      </button>
      </div>

    <div class="dish-card__body">
      <div class="flex items-start justify-between gap-2">
        <div class="flex-1">
          <h3 class="text-base font-semibold text-gray-900 truncate">{{ item.name }}</h3>
          <p class="text-[11px] text-gray-400 mt-1">口味新鲜 · 热卖菜品</p>
        </div>
        <span class="text-lg font-bold text-orange-600">
          ¥{{ Number.isInteger(item.price) ? item.price : item.price.toFixed(2) }}
        </span>
        </div>

      <p class="dish-card__desc text-xs text-gray-500 mt-2 line-clamp-2 min-h-[32px] leading-relaxed">
        {{ item.description || '暂无描述' }}
      </p>

      <div class="dish-card__footer mt-3">
        <div class="dish-card__subtotal">
          <span class="text-xs text-gray-400 block mb-1">小计</span>
          <span class="text-base font-semibold text-gray-900">¥{{ itemSubtotal }}</span>
        </div>
      </div>

      <div class="dish-card__actions mt-3">
        <div class="quantity-controls">
          <button @click="decreaseQuantity" class="quantity-btn text-gray-600">
            <i class="fas fa-minus"></i>
          </button>
          <span class="quantity-value">{{ item.quantity }}</span>
          <button @click="increaseQuantity" class="quantity-btn bg-orange-500 text-white hover:bg-orange-600">
            <i class="fas fa-plus"></i>
            </button>
        </div>
        <button class="remove-link" @click="handleRemove">
          <i class="fas fa-trash-alt"></i>
          移除
            </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, defineProps, defineEmits, computed } from 'vue'
import { normalizeImageUrl } from '@/utils/imageUtils';
import type { MenuItem } from '@/api/user'

interface DishWithQty extends MenuItem {
  quantity: number
}

const props = defineProps<{
  item: DishWithQty
}>()

const emit = defineEmits<{
  (e: 'updateQuantity', dish: MenuItem, quantity: number): void
  (e: 'remove', dish: MenuItem): void
}>()

const visible = ref(true)
const itemSubtotal = computed(() =>
  Number.isInteger(props.item.price * props.item.quantity)
    ? props.item.price * props.item.quantity
    : (props.item.price * props.item.quantity).toFixed(2)
)

// 增加数量
const increaseQuantity = () => {
  emit('updateQuantity', props.item, props.item.quantity + 1)
}

// 减少数量
const decreaseQuantity = () => {
  const newQty = props.item.quantity - 1
  if (newQty <= 0) {
    handleRemove()
  } else {
    emit('updateQuantity', props.item, newQty)
  }
}

// 删除
const handleRemove = () => {
  visible.value = false
  emit('remove', props.item)
}

const handleImageError = (event: Event) => {
  const target = event.target as HTMLImageElement
  target.src = 'https://via.placeholder.com/300x200?text=No+Image'
}
</script>

<style scoped>
.dish-card {
  display: flex;
  gap: 16px;
  padding: 14px;
  border-radius: 24px;
  border: 1px solid rgba(148, 163, 184, 0.15);
  background: linear-gradient(145deg, rgba(255, 255, 255, 0.95), rgba(248, 250, 252, 0.95));
  box-shadow: 0 18px 35px rgba(15, 23, 42, 0.08);
  position: relative;
  overflow: hidden;
}

.dish-card__image {
  width: 120px;
  height: 120px;
  border-radius: 20px;
  overflow: hidden;
  position: relative;
  flex-shrink: 0;
  background: linear-gradient(180deg, rgba(249, 250, 251, 0.9), rgba(243, 244, 246, 0.9));
  border: 1px solid rgba(226, 232, 240, 0.8);
}

.dish-card__remove {
  position: absolute;
  top: 6px;
  right: 6px;
  width: 26px;
  height: 26px;
  border-radius: 999px;
  background: rgba(15, 23, 42, 0.5);
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  opacity: 0;
  transform: translateY(-4px);
  transition: all 0.2s ease;
}

.dish-card:hover .dish-card__remove {
  opacity: 1;
  transform: translateY(0);
}

.dish-card__body {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.dish-card__desc {
  background: rgba(248, 250, 252, 0.7);
  border-radius: 14px;
  padding: 8px 10px;
  border: 1px dashed rgba(203, 213, 225, 0.7);
}

.dish-card__footer {
  display: flex;
  justify-content: center;
  padding: 8px 12px;
  border-radius: 14px;
  background: rgba(249, 250, 251, 0.9);
  border: 1px solid rgba(226, 232, 240, 0.8);
}

.dish-card__subtotal {
  text-align: center;
}

.dish-card__actions {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
  margin-top: 10px;
  padding-top: 10px;
  border-top: 1px dashed rgba(226, 232, 240, 0.9);
}

.quantity-controls {
  display: inline-flex;
  align-items: center;
  gap: 8px;
  background: rgba(248, 250, 252, 0.9);
  border-radius: 999px;
  padding: 4px;
  border: 1px solid rgba(226, 232, 240, 0.9);
  box-shadow: inset 0 1px 3px rgba(15, 23, 42, 0.08);
}

.quantity-btn {
  width: 30px;
  height: 30px;
  border-radius: 999px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 12px;
  border: none;
  cursor: pointer;
  transition: background 0.2s ease;
}

.quantity-btn.text-gray-600 {
  background: white;
  border: 1px solid rgba(148, 163, 184, 0.2);
}

.quantity-value {
  min-width: 30px;
  text-align: center;
  font-weight: 600;
  color: #0f172a;
}

.remove-link {
  border: 1px solid rgba(249, 115, 22, 0.3);
  background: rgba(249, 115, 22, 0.08);
  font-size: 12px;
  color: #f97316;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
  cursor: pointer;
  border-radius: 999px;
  padding: 6px 12px;
  transition: all 0.2s ease;
}

.remove-link:hover {
  color: #fff;
  background: #f97316;
  border-color: #f97316;
  box-shadow: 0 8px 18px rgba(249, 115, 22, 0.3);
}
</style>
