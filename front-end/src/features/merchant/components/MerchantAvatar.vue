<template>
  <img
    :src="currentSrc"
    :alt="alt || '商家头像'"
    :style="sizeStyle"
    class="object-cover rounded-full"
    @error="onError"
  />
</template>

<script lang="ts" setup>
/* eslint-env vue/setup-compiler-macros */
import { ref, watch, computed } from 'vue';
import { API_CONFIG } from '@/config';

interface Props {
  src?: string;
  size?: number; // px
  alt?: string;
}

const props = defineProps<Props>();

const normalizeAssetUrl = (u?: string): string => {
  if (!u) return `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;
  if (u.startsWith('http://') || u.startsWith('https://')) return u;
  if (u.startsWith('/')) return `${API_CONFIG.BASE_URL}${u}`;
  return `${API_CONFIG.BASE_URL}/${u}`;
};

const currentSrc = ref<string>(normalizeAssetUrl(props.src));

watch(
  () => props.src,
  (val) => {
    currentSrc.value = normalizeAssetUrl(val);
  },
  { immediate: true }
);

const sizeStyle = computed(() => {
  const px = props.size ?? 40;
  return { width: `${px}px`, height: `${px}px` } as Record<string, string>;
});

const onError = () => {
  currentSrc.value = `${API_CONFIG.BASE_URL}${API_CONFIG.DEFAULT_AVATAR}`;
};
</script>

<style scoped>
</style>


