<template>
  <div class="min-h-screen bg-gray-100 pt-20 pb-12">
    <main class="after-layout max-w-6xl mx-auto px-4">
      <section class="after-main space-y-6">
        <h1 class="text-xl font-bold text-gray-900 mb-4 text-center">售后中心</h1>

    <!-- 标签页 -->
      <div class="flex overflow-x-auto space-x-2 mb-6 scrollbar-hide sticky top-20 z-10 py-3 px-1">
      <button
        v-for="(tab, index) in tabs"
        :key="index"
        @click="activeTab = tab.key"
        :class="{
            'bg-orange-500 text-white font-bold shadow-md': activeTab === tab.key,
            'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50': activeTab !== tab.key
        }"
          class="flex-1 px-4 py-2.5 rounded-full text-sm transition-all duration-200 whitespace-nowrap text-center min-w-[100px]"
      >
        {{ tab.label }}
      </button>
    </div>

    <!-- 加载中 -->
    <div v-if="loading" class="flex justify-center items-center h-64">
      <i class="fas fa-spinner fa-spin text-3xl text-[#F9771C]"></i>
    </div>

    <!-- 内容区域 -->
    <div v-else class="space-y-6">
      <!-- 售后申请 -->
      <div v-if="activeTab === 'afterSale'">
          <div v-if="afterSales.length === 0" class="text-center py-8 text-gray-400">
            <i class="fas fa-clipboard-list text-3xl mb-3"></i>
            <p class="text-sm">暂无售后申请</p>
        </div>
        <div
            v-for="item in paginatedData as AfterSaleListItem[]"
          :key="item.applicationId"
          :class="[
              'bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm',
              'hover:shadow-md'
          ]"
        >
            <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
            <div class="flex-1">
                <div class="flex items-center gap-2 mb-1">
                  <h3 class="font-bold text-base text-gray-900">{{ item.storeName }}</h3>
                  <i class="fas fa-chevron-right text-xs text-gray-400"></i>
                </div>
                <div class="space-y-0.5">
                  <p class="text-xs text-gray-500">订单号：<span class="text-gray-700 font-mono">{{ item.orderId }}</span></p>
                  <p class="text-xs text-gray-500">申请时间：<span class="text-gray-700">{{ formatDateTime(item.applicationTime) }}</span></p>
                </div>
              </div>
              <span :class="getStatusClass(item.status)" class="px-2 py-1 rounded text-xs font-medium ml-2 whitespace-nowrap">
                {{
                  item.status === 'Pending'
                    ? '商家未反馈'
                    : item.status === 'MerchantFeedback'
                      ? '商家已反馈'
                      : '已完成'
                }}
              </span>
            </div>
            <div class="mb-3">
              <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.description }}</p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="售后图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 订单菜品列表 -->
            <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mb-3">
              <div class="mb-2 flex items-center gap-2">
                <span class="text-xs font-semibold text-gray-700">菜品信息</span>
                <div class="flex-1 h-px bg-gradient-to-r from-gray-200 to-transparent"></div>
              </div>
              <div class="flex justify-between items-start p-3 bg-gradient-to-br from-gray-50 to-white rounded-lg border border-gray-100">
                <!-- 左边：菜品 -->
                <div class="flex gap-2 overflow-x-auto scrollbar-hide pb-1 flex-1 min-w-0">
                  <div v-for="(dish, idx) in item.dishDetails.slice(0, 8)" :key="idx" class="flex flex-col items-center min-w-[4.5rem]">
                    <div class="relative w-16 h-16 rounded-lg bg-white flex items-center justify-center overflow-hidden border border-gray-200 shadow-sm">
                      <img :src="normalizeImageUrl(dish.dishImage)" :alt="dish.dishName"
                        class="w-full h-full object-cover" @error="handleImageError" />
                      <span v-if="dish.quantity > 1" class="absolute top-0 right-0 bg-red-500 text-white text-[10px] px-1 rounded-bl-lg font-bold">x{{ dish.quantity }}</span>
                    </div>
                    <div class="w-16 mt-1.5 text-center">
                      <p class="text-xs text-gray-800 truncate w-full font-medium" :title="dish.dishName">{{ dish.dishName }}</p>
                      <p class="text-[10px] text-gray-500 mt-0.5 font-mono">
                        ¥{{ Number.isInteger(dish.price) ? dish.price : dish.price.toFixed(2) }}
                      </p>
                    </div>
                  </div>
                  <!-- 超过 8 个时显示省略 -->
                  <div v-if="item.dishDetails.length > 8"
                    class="w-16 h-16 flex flex-col items-center justify-center rounded-lg bg-white text-gray-500 text-xs border border-gray-200 min-w-[4.5rem] shadow-sm">
                    <span class="text-lg font-bold">+{{ item.dishDetails.length - 8 }}</span>
                    <span class="text-[10px]">更多</span>
                  </div>
                </div>
                <!-- 右边：共X件 -->
                <div class="ml-3 flex h-16 items-center shrink-0">
                  <span class="text-xs text-gray-600 bg-white px-2.5 py-1.5 rounded-lg border border-gray-200 shadow-sm font-medium">共 {{ item.dishDetails.reduce((acc, d) => acc + d.quantity, 0) }} 件</span>
                </div>
              </div>
            </div>
            <div class="mt-3 p-3 bg-orange-50 rounded-lg space-y-1.5 border border-orange-100">
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">商家回复:</span>
                <span class="text-gray-600">{{ item.merchantReply || '-' }}</span>
              </div>
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处理结果:</span>
                <span class="text-gray-600">{{ getPunishmentLabel(item.processingResult) }}</span>
              </div>
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处理原因:</span>
                <span class="text-gray-600">{{ item.processingReason || '-' }}</span>
          </div>
          </div>
          <!-- 消费者评分（仅已完成时显示） -->
          <div v-if="item.status === 'Completed'" class="mt-3 flex items-center">
            <span class="text-sm text-gray-600 mr-3">我的评分：</span>
            <el-rate
              v-model="item.consumerRating"
              :max="5"
              :allow-half="false"
              :disabled="item.consumerRating != null || ratingSubmitting[item.applicationId]"
              @change="(val:number) => handleRateAfterSale(item, val)"
            />
            <span v-if="item.consumerRating != null" class="ml-2 text-sm text-gray-500">{{ item.consumerRating }} 分</span>
          </div>
        </div>
      </div>

      <!-- 配送投诉 -->
      <div v-if="activeTab === 'complaint'">
          <div v-if="complaints.length === 0" class="text-center py-8 text-gray-400">
            <i class="fas fa-exclamation-triangle text-3xl mb-3"></i>
            <p class="text-sm">暂无配送投诉</p>
        </div>
        <div
            v-for="item in paginatedData as DeliveryComplaintListItem[]"
          :key="item.complaintId"
          :class="[
              'bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm',
              'hover:shadow-md'
          ]"
        >
            <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
            <div class="flex-1">
                <h3 class="font-bold text-base text-gray-900 mb-1">配送投诉</h3>
                <div class="space-y-0.5">
                  <p class="text-xs text-gray-500">订单号：<span class="text-gray-700 font-mono">{{ item.orderId }}</span></p>
                  <p class="text-xs text-gray-500">配送任务ID：<span class="text-gray-700 font-mono">{{ item.deliveryTaskId }}</span></p>
                  <p class="text-xs text-gray-500">投诉时间：<span class="text-gray-700">{{ formatDateTime(item.complaintTime) }}</span></p>
                </div>
              </div>
              <span :class="getStatusClass(item.status)" class="px-2 py-1 rounded text-xs font-medium ml-2 whitespace-nowrap">
              {{ getStatusText(item.status) }}
            </span>
          </div>
            <div class="mb-3">
              <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.complaintReason }}</p>
          </div>
          <!-- 配送信息板块 -->
            <div class="mb-3 p-3 bg-orange-50 rounded-lg space-y-2 border border-orange-100">
              <div class="text-xs">
                <div class="text-gray-600 mb-0.5">骑手</div>
                <div class="font-medium text-gray-900">{{ item.courierName || '-' }}</div>
              </div>
              <div class="text-xs">
                <div class="text-gray-600 mb-0.5">联系电话</div>
                <div class="font-mono text-gray-900">{{ item.courierPhone || '-' }}</div>
              </div>
              <div class="text-xs">
                <div class="text-gray-600 mb-0.5">接单时间</div>
                <div class="text-gray-900">{{ item.acceptTime ? formatDateTime(item.acceptTime) : '-' }}</div>
              </div>
              <div class="text-xs">
                <div class="text-gray-600 mb-0.5">送达时间</div>
                <div class="text-gray-900">{{ item.completionTime ? formatDateTime(item.completionTime) : '-' }}</div>
              </div>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="投诉图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
            <div v-if="item.processingResult || item.processingReason || item.status === 'Completed'" class="mt-3 p-3 bg-orange-50 rounded-lg space-y-1.5 border border-orange-100">
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处理结果:</span>
                <span class="text-gray-600">{{ getPunishmentLabel(item.processingResult) }}</span>
              </div>
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处理原因:</span>
                <span class="text-gray-600">{{ item.processingReason || '-' }}</span>
              </div>
          </div>
        </div>
      </div>

      <!-- 店铺举报 -->
      <div v-if="activeTab === 'report'">
          <div v-if="reports.length === 0" class="text-center py-8 text-gray-400">
            <i class="fas fa-flag text-3xl mb-3"></i>
            <p class="text-sm">暂无店铺举报</p>
        </div>
        <div
            v-for="item in paginatedData as StoreReportListItem[]"
          :key="item.penaltyId"
          :class="[
              'bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm',
              'hover:shadow-md'
          ]"
        >
            <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
            <div class="flex-1">
                <h3 class="font-bold text-base text-gray-900 mb-1">{{ item.storeName }}</h3>
                <div class="space-y-0.5">
                  <p class="text-xs text-gray-500">店铺ID：<span class="text-gray-700 font-mono">{{ item.storeId }}</span></p>
                  <p class="text-xs text-gray-500">举报时间：<span class="text-gray-700">{{ formatDateTime(item.reportTime) }}</span></p>
                </div>
              </div>
              <span :class="getStatusClass(item.status)" class="px-2 py-1 rounded text-xs font-medium ml-2 whitespace-nowrap">
              {{ getStatusText(item.status) }}
            </span>
          </div>
            <div class="mb-3">
              <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.content }}</p>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="举报图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 处罚信息 -->
            <div class="mt-3 p-3 bg-orange-50 rounded-lg space-y-1.5 border border-orange-100">
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">店铺处罚:</span>
                <span class="text-gray-600">{{ getPunishmentLabel(item.storePunishment) }}</span>
              </div>
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">商家处罚:</span>
                <span class="text-gray-600">{{ getPunishmentLabel(item.merchantPunishment) }}</span>
              </div>
              <div class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处罚时间:</span>
                <span class="text-gray-600">{{ item.penaltyTime ? formatDateTime(item.penaltyTime) : '-' }}</span>
              </div>
              <div v-if="item.processingReason" class="flex gap-2 text-xs">
                <span class="font-medium text-orange-700 shrink-0">处理原因:</span>
                <span class="text-gray-600">{{ item.processingReason }}</span>
              </div>
          </div>
        </div>
      </div>

      <!-- 评论 -->
      <div v-if="activeTab === 'comment'">
          <div v-if="comments.length === 0" class="text-center py-8 text-gray-400">
            <i class="fas fa-comment text-3xl mb-3"></i>
            <p class="text-sm">暂无评论</p>
        </div>
        <div
            v-for="item in paginatedData as CommentListItem[]"
          :key="item.commentId"
          :class="[
              'bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm',
              'hover:shadow-md'
          ]"
        >
            <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
            <div class="flex-1">
                <h3 class="font-bold text-base text-gray-900 mb-1">{{ item.storeName }}</h3>
                <div class="space-y-0.5">
                  <p v-if="item.orderId" class="text-xs text-gray-500">订单号：<span class="text-gray-700 font-mono">{{ item.orderId }}</span></p>
                  <p class="text-xs text-gray-500">店铺ID：<span class="text-gray-700 font-mono">{{ item.storeId }}</span></p>
                  <p class="text-xs text-gray-500">评论时间：<span class="text-gray-700">{{ formatDateTime(item.postedAt) }}</span></p>
                </div>
              </div>
              <div class="flex flex-col items-end gap-1">
              <div class="flex items-center">
                  <span v-for="i in 5" :key="i" class="text-xs">
                  <i
                    :class="i <= item.rating ? 'fas fa-star text-yellow-400' : 'far fa-star text-gray-300'"
                  ></i>
                </span>
              </div>
                <span :class="getStatusClass(item.status)" class="px-2 py-1 rounded text-xs font-medium whitespace-nowrap">
                  {{ item.status === 'Completed' ? '审核通过' : getStatusText(item.status) }}
              </span>
            </div>
          </div>
            <div class="mb-3">
              <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.content }}</p>
          </div>
          <!-- 商家回复（只显示已通过的） -->
          <div v-if="item.merchantReply" class="mb-3 pt-3 border-t border-orange-200">
            <div class="bg-gradient-to-r from-orange-50 to-orange-100 rounded-lg p-3 border border-orange-200">
              <div class="flex items-center gap-2 mb-2">
                <i class="fas fa-store text-orange-600 text-xs"></i>
                <span class="font-semibold text-orange-700 text-sm">商家回复</span>
                <span v-if="item.merchantReplyTime" class="text-xs text-orange-600 ml-auto">{{ formatDateTime(item.merchantReplyTime) }}</span>
              </div>
              <p class="text-sm text-gray-800 leading-relaxed">{{ item.merchantReply }}</p>
            </div>
          </div>
          <div v-if="item.images && item.images.length > 0" class="mb-4 flex flex-wrap gap-2">
            <img
              v-for="(image, idx) in item.images"
              :key="idx"
              :src="normalizeImageUrl(image)"
              alt="评论图片"
              class="w-24 h-24 object-cover rounded border border-gray-300 cursor-pointer hover:opacity-80"
              @error="handleImageError"
              @click="previewImage(image)"
            />
          </div>
          <!-- 订单菜品列表 -->
            <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mb-3">
              <div class="mb-2 flex items-center gap-2">
                <span class="text-xs font-semibold text-gray-700">菜品信息</span>
                <div class="flex-1 h-px bg-gradient-to-r from-gray-200 to-transparent"></div>
              </div>
              <div class="flex justify-between items-start p-3 bg-gradient-to-br from-gray-50 to-white rounded-lg border border-gray-100">
                <!-- 左边：菜品 -->
                <div class="flex gap-2 overflow-x-auto scrollbar-hide pb-1 flex-1 min-w-0">
                  <div v-for="(dish, idx) in item.dishDetails.slice(0, 8)" :key="idx" class="flex flex-col items-center min-w-[4.5rem]">
                    <div class="relative w-16 h-16 rounded-lg bg-white flex items-center justify-center overflow-hidden border border-gray-200 shadow-sm">
                      <img :src="normalizeImageUrl(dish.dishImage)" :alt="dish.dishName"
                        class="w-full h-full object-cover" @error="handleImageError" />
                      <span v-if="dish.quantity > 1" class="absolute top-0 right-0 bg-red-500 text-white text-[10px] px-1 rounded-bl-lg font-bold">x{{ dish.quantity }}</span>
                    </div>
                    <div class="w-16 mt-1.5 text-center">
                      <p class="text-xs text-gray-800 truncate w-full font-medium" :title="dish.dishName">{{ dish.dishName }}</p>
                      <p class="text-[10px] text-gray-500 mt-0.5 font-mono">
                        ¥{{ Number.isInteger(dish.price) ? dish.price : dish.price.toFixed(2) }}
                      </p>
                    </div>
                  </div>
                  <!-- 超过 8 个时显示省略 -->
                  <div v-if="item.dishDetails.length > 8"
                    class="w-16 h-16 flex flex-col items-center justify-center rounded-lg bg-white text-gray-500 text-xs border border-gray-200 min-w-[4.5rem] shadow-sm">
                    <span class="text-lg font-bold">+{{ item.dishDetails.length - 8 }}</span>
                    <span class="text-[10px]">更多</span>
                  </div>
                </div>
                <!-- 右边：共X件 -->
                <div class="ml-3 flex h-16 items-center shrink-0">
                  <span class="text-xs text-gray-600 bg-white px-2.5 py-1.5 rounded-lg border border-gray-200 shadow-sm font-medium">共 {{ item.dishDetails.reduce((acc, d) => acc + d.quantity, 0) }} 件</span>
                </div>
              </div>
            </div>
          </div>
        </div>
        
        <!-- 分页控制 -->
        <div v-if="totalPages > 1" class="flex justify-center items-center gap-2 mt-8 mb-4">
          <button 
            @click="currentPage--"
            :disabled="currentPage === 1"
            :class="[
              currentPage === 1
                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
            ]"
            class="px-3 py-1.5 rounded-full text-xs transition-colors">
            <i class="fas fa-chevron-left"></i>
          </button>
          
          <div class="flex gap-1">
            <button 
              v-for="page in totalPages" 
              :key="page"
              @click="currentPage = page"
              :class="[
                currentPage === page
                  ? 'bg-orange-500 text-white font-bold shadow-sm'
                  : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
              ]"
              class="w-8 h-8 rounded-full text-xs transition-colors cursor-pointer flex items-center justify-center">
              {{ page }}
            </button>
          </div>
          
          <button 
            @click="currentPage++"
            :disabled="currentPage === totalPages"
            :class="[
              currentPage === totalPages
                ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
            ]"
            class="px-3 py-1.5 rounded-full text-xs transition-colors">
            <i class="fas fa-chevron-right"></i>
          </button>
        </div>
        </div>
      </section>

      <aside class="after-aside space-y-4">
        <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
          <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
            <i class="fas fa-clipboard-check text-orange-500"></i>
            最近售后概况
          </h3>
          <div class="space-y-3 text-xs text-gray-600">
            <div class="flex items-center justify-between">
              <span>售后总数</span>
              <span class="font-semibold text-gray-900">{{ afterSales.length }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>配送投诉</span>
              <span class="font-semibold text-gray-900">{{ complaints.length }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>店铺举报</span>
              <span class="font-semibold text-gray-900">{{ reports.length }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>评论</span>
              <span class="font-semibold text-gray-900">{{ comments.length }}</span>
            </div>
          </div>
        </div>

        <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
          <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
            <i class="fas fa-lightbulb text-yellow-500"></i>
            温馨提示
          </h3>
          <ul class="space-y-2 text-xs text-gray-600 list-disc pl-4 text-left">
            <li>可以对管理员的处理结果进行打分</li>
            <li>每个订单只能进行一次售后申请，若不满意请及时反馈</li>
            <li>违规评论会被系统自动屏蔽，并标记账号异常</li>
            <li>投诉处理后系统会短信通知，请留意</li>
          </ul>
        </div>

        <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
          <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center justify-between">
            <span class="flex items-center gap-2">
              <i class="fas fa-brain text-purple-500"></i>
              智能分析
            </span>
            <span class="text-xs text-gray-400">本月</span>
          </h3>
          <div class="space-y-3 text-xs text-gray-600">
            <div class="flex items-center justify-between">
              <span>售后申请</span>
              <span class="font-semibold text-gray-900">{{ afterSales.length }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>商家已回复</span>
              <span class="font-semibold text-gray-900">{{ merchantReplyCount }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>管理员已处理</span>
              <span class="font-semibold text-gray-900">{{ adminProcessedCount }}</span>
            </div>
            <div class="flex items-center justify-between">
              <span>评价满意度</span>
              <span class="font-semibold text-gray-900">{{ satisfactionRate }} 分</span>
            </div>
            <div class="pt-2 border-t border-dashed border-gray-100 space-y-2">
              <div class="flex items-center justify-between text-xs">
                <span>评论总数</span>
                <span class="font-semibold text-gray-900">{{ comments.length }}</span>
              </div>
              <div class="flex items-center justify-between text-xs">
                <span>审核通过</span>
                <span class="font-semibold text-gray-900">{{ approvedCommentsCount }}</span>
              </div>
              <div class="flex items-center justify-between text-xs">
                <span>平均评分</span>
                <span class="font-semibold text-gray-900">{{ averageCommentRating }} 分</span>
          </div>
        </div>
      </div>
    </div>
      </aside>
  </main>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';
import { ElMessage } from 'element-plus';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { API_CONFIG } from '@/config/index';
import {
  getMyAfterSales,
  getMyDeliveryComplaints,
  getMyStoreReports,
  getMyComments,
  rateAfterSale,
  type AfterSaleListItem,
  type DeliveryComplaintListItem,
  type StoreReportListItem,
  type CommentListItem
} from '@/api/user/afterSale';

const loading = ref(true);
const route = useRoute();
const activeTab = ref('afterSale');
const afterSales = ref<AfterSaleListItem[]>([]);
const complaints = ref<DeliveryComplaintListItem[]>([]);
const reports = ref<StoreReportListItem[]>([]);
const comments = ref<CommentListItem[]>([]);

// 新增：评分提交中状态，防重入
const ratingSubmitting = ref<Record<number, boolean>>({});

// 分页相关
const currentPage = ref(1);
const itemsPerPage = ref(5);

const tabs = [
  { key: 'afterSale', label: '售后申请' },
  { key: 'complaint', label: '配送投诉' },
  { key: 'report', label: '店铺举报' },
  { key: 'comment', label: '评论' }
];

// 处罚选项映射
const punishmentLabels: Record<string, string> = {
  // 售后处理
  'full_refund': '全额退款',
  'partial_refund': '部分退款',
  'redelivery': '重新配送',
  'apology': '商家道歉',
  'compensation': '赔偿用户',
  // 配送投诉
  'warning': '警告处分',
  'suspend_3days': '暂停接单3天',
  'suspend_7days': '暂停接单7天',
  'fine': '罚款处理',
  'terminate': '终止合作',
  // 违规举报 - 商家处罚
  'verbal_warning': '口头警告',
  'written_warning': '书面警告',
  'fine_500': '罚款500元',
  'fine_1000': '罚款1000元',
  // 违规举报 - 店铺处罚
  'correction': '限期整改',
  'permanent_removal': '永久下架',
};

// 辅助函数：将处罚 value 转换为 label
const getPunishmentLabel = (value: string | undefined): string => {
  if (!value) return '-';
  return punishmentLabels[value] || value;
};

onMounted(() => {
  const tab = (route.query.tab as string) || '';
  const allowed = ['afterSale', 'complaint', 'report', 'comment'];
  if (allowed.includes(tab)) {
    activeTab.value = tab as any;
  }
  loadData();
});

watch(activeTab, () => {
  currentPage.value = 1;
});

// 获取当前标签对应的数据
const currentTabData = computed(() => {
  switch (activeTab.value) {
    case 'afterSale':
      return afterSales.value;
    case 'complaint':
      return complaints.value;
    case 'report':
      return reports.value;
    case 'comment':
      return comments.value;
    default:
      return [];
  }
});

// 分页后的数据
const paginatedData = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return currentTabData.value.slice(start, end);
});

// 总页数
const totalPages = computed(() => {
  return Math.ceil(currentTabData.value.length / itemsPerPage.value);
});

const merchantReplyCount = computed(() => {
  return afterSales.value.filter(item => !!item.merchantReply).length;
});

const adminProcessedCount = computed(() => {
  const adminHandledStatuses: AfterSaleListItem['status'][] = ['Completed'];
  return afterSales.value.filter(item => adminHandledStatuses.includes(item.status)).length;
});

const satisfactionRate = computed(() => {
  const rated = afterSales.value.filter(item => item.consumerRating != null);
  if (!rated.length) return '0.0';
  const total = rated.reduce((sum, item) => sum + (item.consumerRating ?? 0), 0);
  return (total / rated.length).toFixed(1);
});

const approvedCommentsCount = computed(() => {
  return comments.value.filter(item => item.status === 'Completed').length;
});

const averageCommentRating = computed(() => {
  if (!comments.value.length) return '0.0';
  const total = comments.value.reduce((sum, item) => sum + (item.rating || 0), 0);
  return (total / comments.value.length).toFixed(1);
});

const loadData = async () => {
  try {
    loading.value = true;
    const [afterSaleRes, complaintRes, reportRes, commentRes] = await Promise.all([
      getMyAfterSales(),
      getMyDeliveryComplaints(),
      getMyStoreReports(),
      getMyComments()
    ]);
    afterSales.value = afterSaleRes;
    complaints.value = complaintRes;
    reports.value = reportRes;
    comments.value = commentRes;
  } catch (error) {
    console.error('加载数据失败:', error);
    ElMessage.error('加载数据失败，请重试');
  } finally {
    loading.value = false;
  }
};

const handleRateAfterSale = async (item: AfterSaleListItem, val: number) => {
  // 已评分则直接禁止再次评分
  if (item.consumerRating != null) {
    ElMessage.info('已评分，不能重复评分');
    return;
  }
  if (ratingSubmitting.value[item.applicationId]) return;
  ratingSubmitting.value[item.applicationId] = true;
  try {
    const ok = await rateAfterSale(item.applicationId, val);
    if (ok) {
      item.consumerRating = val;
      ElMessage.success('评分已提交');
    } else {
      // 回滚 UI
      item.consumerRating = undefined;
      ElMessage.error('评分提交失败');
    }
  } catch (e) {
    item.consumerRating = undefined;
    ElMessage.error('评分提交失败');
  } finally {
    ratingSubmitting.value[item.applicationId] = false;
  }
};

const formatDateTime = (dateStr: string) => {
  const date = new Date(dateStr);
  const year = date.getFullYear();
  const month = (date.getMonth() + 1).toString().padStart(2, '0');
  const day = date.getDate().toString().padStart(2, '0');
  const hours = date.getHours().toString().padStart(2, '0');
  const minutes = date.getMinutes().toString().padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}`;
};

const getStatusText = (status: string) => {
  const statusMap: Record<string, string> = {
    'Pending': '待处理',
    'MerchantFeedback': '商家已反馈',
    'Processing': '处理中',
    'Approved': '已通过',
    'Rejected': '已拒绝',
    'Completed': '已完成',
    'Illegal': '违规'
  };
  return statusMap[status] || status;
};

const getStatusClass = (status: string) => {
  const classMap: Record<string, string> = {
    'Pending': 'bg-yellow-100 text-yellow-700',
    'MerchantFeedback': 'bg-blue-100 text-blue-700',
    'Processing': 'bg-blue-100 text-blue-700',
    'Approved': 'bg-green-100 text-green-700',
    'Rejected': 'bg-red-100 text-red-700',
    'Completed': 'bg-green-100 text-green-700',
    'Illegal': 'bg-red-100 text-red-700'
  };
  return classMap[status] || 'bg-gray-100 text-gray-700';
};

const previewImage = (imageUrl: string) => {
  const fullUrl = imageUrl.startsWith('http') ? imageUrl : `${API_CONFIG.BASE_URL}${imageUrl}`;
  window.open(fullUrl, '_blank');
};
</script>

<style scoped>
/* Hide scrollbar for Chrome, Safari and Opera */
.scrollbar-hide::-webkit-scrollbar {
    display: none;
}

/* Hide scrollbar for IE, Edge and Firefox */
.scrollbar-hide {
    -ms-overflow-style: none;  /* IE and Edge */
    scrollbar-width: none;  /* Firefox */
}

.after-layout {
  display: flex;
  flex-direction: column;
  gap: 24px;
  align-items: flex-start;
}

.after-main {
  flex: 1;
  min-width: 0;
  width: 100%;
}

.after-aside {
  width: 100%;
  flex-shrink: 0;
}

@media (min-width: 1024px) {
  .after-layout {
    flex-direction: row;
    flex-wrap: nowrap;
    justify-content: center;
    align-items: flex-start;
  }

  .after-main {
    flex: 0 0 820px;
    max-width: 820px;
  }

  .after-aside {
    flex: 0 0 300px;
    max-width: 300px;
    position: sticky;
    top: 120px;
  }
}
</style>
