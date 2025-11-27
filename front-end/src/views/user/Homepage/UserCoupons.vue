<template>
  <div class="coupons-page min-h-screen bg-gray-100 pt-20 pb-12">
    <div class="max-w-6xl mx-auto px-4">
      <div class="coupons-layout">
        <section class="coupon-main space-y-6">
          <div class="text-center md:text-left">
            <h1 class="text-3xl font-bold text-gray-800 mb-2">优惠券中心</h1>
            <p class="text-sm text-gray-500">精选商家限时福利，及时领取不错过</p>
          </div>

          <div class="stats-grid">
            <div class="stat-card">
              <p class="stat-label">可领取</p>
              <p class="stat-value text-orange-500">{{ availableCoupons }}</p>
              <p class="stat-desc">尚有库存的优惠券</p>
            </div>
            <div class="stat-card">
              <p class="stat-label">已领取</p>
              <p class="stat-value text-green-500">{{ claimedCoupons }}</p>
              <p class="stat-desc">我的账户中优惠券</p>
            </div>
            <div class="stat-card">
              <p class="stat-label">即将过期</p>
              <p class="stat-value text-red-500">{{ expiringSoon }}</p>
              <p class="stat-desc"> 即将请尽快使用</p>
            </div>
            <div class="stat-card hidden lg:block">
              <p class="stat-label">优惠总面值</p>
              <p class="stat-value text-purple-500">¥{{ totalCouponValue }}</p>
              <p class="stat-desc">可用优惠券总金额</p>
            </div>
          </div>

          <div v-if="loading" class="flex justify-center items-center h-64 bg-white rounded-2xl shadow-sm">
      <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
    </div>

    <div v-else class="space-y-4">
            <div v-if="coupons.length === 0" class="text-center py-12 text-gray-500 bg-white rounded-2xl border border-dashed border-gray-200">
              <i class="fas fa-ticket-alt text-4xl mb-4 text-orange-400"></i>
        <p>暂无可用优惠券</p>
      </div>

      <div
        v-for="coupon in coupons"
        :key="coupon.couponManagerID"
              class="coupon-card"
      >
              <div class="coupon-card__main">
                <div class="flex items-start justify-between">
                  <div class="flex items-center gap-3">
                    <div class="coupon-store-logo">
                  <img
                    v-if="coupon.storeImage"
                    :src="normalizeImageUrl(coupon.storeImage)"
                    :alt="coupon.storeName"
                        class="max-w-full max-h-full object-contain"
                    @error="handleImageError"
                  />
                  <i v-else class="fas fa-store text-2xl text-gray-400"></i>
                </div>
                <div>
                      <p class="text-xs uppercase tracking-widest text-gray-400">合作商家</p>
                      <h3 class="font-bold text-lg text-gray-900">{{ coupon.storeName }}</h3>
                  <p class="text-sm text-gray-500">{{ coupon.couponName }}</p>
                </div>
              </div>
              <div class="text-right">
                    <div class="coupon-value">
                  <span v-if="coupon.type === 'fixed'">¥{{ coupon.value.toFixed(0) }}</span>
                  <span v-else>{{ (coupon.value * 10).toFixed(1) }}折</span>
                </div>
                <p class="text-xs text-gray-500 mt-1">
                  <span v-if="coupon.minimumSpend === 0">无门槛</span>
                  <span v-else>满{{ coupon.minimumSpend.toFixed(0) }}元可用</span>
                </p>
              </div>
            </div>

                <div v-if="coupon.description" class="text-sm text-gray-600 bg-gray-50 rounded-lg px-3 py-2">
              {{ coupon.description }}
            </div>

                <div class="flex flex-wrap items-center gap-2 text-xs text-gray-600">
                  <span class="tag-chip bg-orange-50 text-orange-600">有效期 {{ formatDate(coupon.validFrom) }} - {{ formatDate(coupon.validTo) }}</span>
                  <span v-if="coupon.remainingQuantity > 0" class="tag-chip bg-green-50 text-green-600">
                剩余 {{ coupon.remainingQuantity }} 张
              </span>
                  <span v-else class="tag-chip bg-red-50 text-red-600">已领完</span>
            </div>
          </div>

              <div class="coupon-card__action">
            <button
              v-if="!coupon.isClaimed && coupon.remainingQuantity > 0"
              @click="claimCoupon(coupon.couponManagerID)"
              :disabled="claimingCouponId === coupon.couponManagerID"
                  class="coupon-btn"
            >
              <span v-if="claimingCouponId === coupon.couponManagerID">
                <i class="fas fa-spinner fa-spin mr-2"></i>领取中
              </span>
              <span v-else>立即领取</span>
            </button>
                <div v-else-if="coupon.isClaimed" class="text-center">
                  <div class="text-green-500 font-semibold mb-1">
                    <i class="fas fa-check-circle text-xl"></i>
                  </div>
                  <p class="text-sm text-gray-600">已领取</p>
                </div>
                <div v-else class="text-center text-sm text-gray-500">
                  已领完
                </div>
              </div>
            </div>
          </div>
        </section>

        <aside class="coupon-aside space-y-4">
          <div class="aside-card">
            <h3 class="aside-title">
              <i class="fas fa-wallet text-orange-500"></i>
              我的优惠权益
            </h3>
            <div class="space-y-3 text-sm text-gray-600">
              <div class="flex items-center justify-between">
                <span>总优惠券</span>
                <span class="font-semibold text-gray-900">{{ coupons.length }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span>未领取</span>
                <span class="font-semibold text-gray-900">{{ availableCoupons }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span>已领取</span>
                <span class="font-semibold text-gray-900">{{ claimedCoupons }}</span>
              </div>
              <div class="flex items-center justify-between">
                <span>即将过期</span>
                <span class="font-semibold text-red-500">{{ expiringSoon }}</span>
              </div>
            </div>
          </div>

          <div class="aside-card">
            <h3 class="aside-title">
              <i class="fas fa-lightbulb text-yellow-500"></i>
              使用提示
            </h3>
            <ul class="space-y-2 text-xs text-gray-600 list-disc pl-4 text-left">
              <li>领取后前往下单页面付款时可选择使用。</li>
              <li>同一订单仅可使用一张优惠券，部分活动不可叠加。</li>
              <li>过期自动失效，系统不会自动续期。</li>
            </ul>
          </div>

          <div class="aside-card">
            <h3 class="aside-title">
              <i class="fas fa-bullhorn text-blue-500"></i>
              热门活动
            </h3>
            <div class="space-y-3 text-sm text-gray-600 text-left">
              <div class="flex">
                <span class="shrink-0 mr-1">1.</span>
                <span>周三会员日店铺满减券，多店共享，数量有限。</span>
              </div>
              <div class="flex">
                <span class="shrink-0 mr-1">2.</span>
                <span>夜宵时段大额券 22:00-24:00 限时领取。</span>
              </div>
              <div class="flex">
                <span class="shrink-0 mr-1">3.</span>
                <span>连续签到 7 天可额外获得隐藏大额优惠券。</span>
            </div>
          </div>
        </div>
        </aside>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, computed } from 'vue';
import { ElMessage } from 'element-plus';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { getAvailableCoupons, claimCoupon as claimCouponApi, type AvailableCoupon } from '@/api/user/coupon';

const loading = ref(true);
const coupons = ref<AvailableCoupon[]>([]);
const claimingCouponId = ref<number | null>(null);

onMounted(() => {
  loadCoupons();
});

const loadCoupons = async () => {
  try {
    loading.value = true;
    coupons.value = await getAvailableCoupons();
  } catch (error) {
    console.error('加载优惠券失败:', error);
    ElMessage.error('加载优惠券失败，请重试');
  } finally {
    loading.value = false;
  }
};

const claimCoupon = async (couponManagerId: number) => {
  try {
    claimingCouponId.value = couponManagerId;
    const result = await claimCouponApi(couponManagerId);
    
    if (result.success) {
      ElMessage.success('优惠券领取成功！');
      // 刷新列表
      await loadCoupons();
    } else {
      ElMessage.error(result.message || '优惠券领取失败');
    }
  } catch (error: any) {
    console.error('领取优惠券失败:', error);
    ElMessage.error(error.response?.data?.message || '优惠券领取失败，请重试');
  } finally {
    claimingCouponId.value = null;
  }
};

const availableCoupons = computed(() => {
  return coupons.value.filter(coupon => !coupon.isClaimed && coupon.remainingQuantity > 0).length;
});

const claimedCoupons = computed(() => {
  return coupons.value.filter(coupon => coupon.isClaimed).length;
});

const expiringSoon = computed(() => {
  const threeDays = 3 * 24 * 60 * 60 * 1000;
  const now = Date.now();
  return coupons.value.filter(coupon => {
    const end = new Date(coupon.validTo).getTime();
    return end > now && end - now <= threeDays;
  }).length;
});

const totalCouponValue = computed(() => {
  const total = coupons.value.reduce((sum, coupon) => {
    if (coupon.type === 'fixed') {
      return sum + coupon.value;
    }
    return sum;
  }, 0);
  return total.toFixed(0);
});

// 格式化日期显示（去掉秒数）
function formatDate(dateStr: string) {
  const date = new Date(dateStr);
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}`;
}
</script>

<style scoped>
.coupons-layout {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.coupon-main,
.coupon-aside {
  width: 100%;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
  gap: 16px;
}

.stat-card {
  background: white;
  border-radius: 18px;
  padding: 16px;
  border: 1px solid rgba(249, 119, 28, 0.08);
  box-shadow: 0 8px 20px rgba(15, 23, 42, 0.06);
}

.stat-label {
  font-size: 12px;
  color: #9ca3af;
  letter-spacing: 0.08em;
  text-transform: uppercase;
  margin-bottom: 6px;
}

.stat-value {
  font-size: 30px;
  font-weight: 700;
  line-height: 1.1;
}

.stat-desc {
  font-size: 12px;
  color: #6b7280;
  margin-top: 4px;
}

.coupon-card {
  display: flex;
  flex-direction: column;
  background: white;
  border-radius: 20px;
  border: 1px solid #fef3c7;
  box-shadow: 0 12px 35px rgba(249, 119, 28, 0.08);
  overflow: hidden;
}

.coupon-card__main {
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.coupon-card__action {
  background: linear-gradient(135deg, #fff7ed, #ffedd5);
  padding: 20px;
  text-align: center;
}

.coupon-store-logo {
  width: 56px;
  height: 56px;
  border-radius: 16px;
  background: #f3f4f6;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid #e5e7eb;
}

.coupon-value {
  font-size: 32px;
  font-weight: 800;
  color: #f97316;
  line-height: 1;
}

.coupon-btn {
  width: 100%;
  background: #f97316;
  color: white;
  padding: 12px;
  border-radius: 999px;
  font-weight: 600;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.coupon-btn:disabled {
  background: #d1d5db;
  cursor: not-allowed;
}

.coupon-btn:not(:disabled):hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 25px rgba(249, 115, 22, 0.3);
}

.aside-card {
  background: white;
  border-radius: 18px;
  border: 1px solid #e5e7eb;
  padding: 16px;
  box-shadow: 0 8px 20px rgba(15, 23, 42, 0.04);
}

.aside-title {
  font-size: 14px;
  font-weight: 700;
  color: #111827;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.tag-chip {
  padding: 4px 10px;
  border-radius: 999px;
  font-size: 11px;
  font-weight: 600;
}

@media (min-width: 1024px) {
  .coupons-layout {
    flex-direction: row;
    align-items: flex-start;
  }

  .coupon-main {
    flex: 0 0 820px;
    max-width: 820px;
  }

  .coupon-aside {
    flex: 0 0 260px;
    max-width: 260px;
    position: sticky;
    top: 112px;
  }

  .coupon-card {
    flex-direction: row;
  }

  .coupon-card__main {
    flex: 1;
  }

  .coupon-card__action {
    width: 220px;
    border-left: 1px dashed rgba(249, 115, 22, 0.4);
    min-height: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
  }
}
</style>

