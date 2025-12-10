<!-- eslint-disable -->
<!-- The exported code uses Tailwind CSS. Install Tailwind CSS in your dev environment to ensure all styles work. -->

<template>
  <Layout>
    <!-- 订单售后 -->
    <div class="min-h-screen bg-gray-100 pb-12">
      <div class="max-w-7xl mx-auto px-4 py-6">
        <h2 class="text-xl font-bold text-gray-900 mb-6 text-center">订单售后管理</h2>

        <!-- 统计卡片 -->
        <div class="grid grid-cols-4 gap-4 mb-6">
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 hover:shadow-md transition-all">
            <div class="flex items-center justify-between">
    <div>
                <p class="text-xs text-gray-500 mb-1">待处理售后</p>
                <p class="text-2xl font-bold text-orange-600">{{ aftersaleList.filter(a => a.status === '待处理').length }}</p>
              </div>
              <div class="w-12 h-12 bg-orange-100 rounded-full flex items-center justify-center">
                <i class="fas fa-exclamation-circle text-orange-500 text-xl"></i>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 hover:shadow-md transition-all">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-xs text-gray-500 mb-1">本月评论</p>
                <p class="text-2xl font-bold text-blue-600">{{ reviewTotal }}</p>
              </div>
              <div class="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center">
                <i class="fas fa-comment text-blue-500 text-xl"></i>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 hover:shadow-md transition-all">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-xs text-gray-500 mb-1">处罚记录</p>
                <p class="text-2xl font-bold text-red-600">{{ penaltyList.length }}</p>
              </div>
              <div class="w-12 h-12 bg-red-100 rounded-full flex items-center justify-center">
                <i class="fas fa-gavel text-red-500 text-xl"></i>
              </div>
            </div>
          </div>
          <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 hover:shadow-md transition-all">
            <div class="flex items-center justify-between">
              <div>
                <p class="text-xs text-gray-500 mb-1">已完成售后</p>
                <p class="text-2xl font-bold text-green-600">{{ aftersaleList.filter(a => a.status === '已完成').length }}</p>
              </div>
              <div class="w-12 h-12 bg-green-100 rounded-full flex items-center justify-center">
                <i class="fas fa-check-circle text-green-500 text-xl"></i>
              </div>
            </div>
          </div>
        </div>

        <!-- 主要内容区域 -->
        <div class="flex gap-4">
          <!-- 左侧主内容 -->
          <div class="flex-1 min-w-0">
          <!-- 切换标签 -->
            <div class="flex overflow-x-auto space-x-2 mb-6 scrollbar-hide sticky top-0 z-10 py-2 bg-gray-100">
              <button
                v-for="tab in aftersaleTabs"
                :key="tab.value"
                @click="activeAftersaleTab = tab.value"
                :class="{
                  'bg-orange-500 text-white font-bold shadow-md transform scale-105': activeAftersaleTab === tab.value,
                  'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50': activeAftersaleTab !== tab.value
                }"
                class="flex-1 px-4 py-2.5 rounded-full text-sm transition-all duration-200 whitespace-nowrap text-center min-w-[120px]"
              >
                {{ tab.label }}
              </button>
          </div>

          <!-- 处罚记录 -->
          <div v-if="activeAftersaleTab === 'penalties'">
          <!-- 筛选条件 -->
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 mb-4">
            <div class="flex items-center gap-3">
              <el-select v-model="penaltyKeywordType" placeholder="筛选字段" size="small" teleported style="position: relative; z-index: 1; width: 100px; flex-shrink: 0;">
                    <el-option label="全部" value="" />
                    <el-option label="处罚编号" value="id" />
                    <el-option label="处罚原因" value="reason" />
                  </el-select>
              <el-input v-model="penaltyFilters.keyword" placeholder="处罚编号/原因关键词" size="small" clearable style="position: relative; z-index: 2; flex: 1; min-width: 0;" />
              <el-button type="warning" size="small" @click="loadPenalties()">筛选</el-button>
                </div>
              </div>

          <!-- 记录列表 -->
          <div class="space-y-4">
            <div v-if="penaltyList.length === 0" class="text-center py-8 text-gray-400">
              <i class="fas fa-flag text-3xl mb-3"></i>
              <p class="text-sm">暂无处罚记录</p>
            </div>
            <div
              v-for="item in penaltyList"
              :key="item.id"
              class="bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm hover:shadow-md cursor-pointer"
              @click="openPenaltyDetail(item)"
            >
              <div class="flex justify-between items-start mb-3 pb-3 border-b border-dashed border-gray-200">
                <div class="flex-1">
                  <div class="flex items-center gap-2 mb-1">
                    <h3 class="font-bold text-base text-gray-900">处罚编号：{{ item.id }}</h3>
                  </div>
                  <div class="space-y-0.5">
                    <p class="text-xs text-gray-500">处罚时间：<span class="text-gray-700">{{ item.time }}</span></p>
                  </div>
                </div>
              </div>
              <div class="mb-3">
                <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.reason }}</p>
              </div>
              <div class="mt-3 p-3 bg-orange-50 rounded-lg space-y-1.5 border border-orange-100">
                <div class="flex gap-2 text-xs">
                  <span class="font-medium text-orange-700 shrink-0">商家处罚:</span>
                  <span class="text-gray-600">{{ punishmentDict[item.merchantAction] || item.merchantAction }}</span>
                </div>
                <div class="flex gap-2 text-xs">
                  <span class="font-medium text-orange-700 shrink-0">店铺处罚:</span>
                  <span class="text-gray-600">{{ punishmentDict[item.platformAction] || item.platformAction }}</span>
                </div>
              </div>
            </div>
            </div>
            <!-- 处罚详情抽屉 -->
            <el-drawer v-model="penaltyDetailVisible" title="处罚详情" size="520px" direction="rtl" class="modern-drawer">
              <div v-if="penaltyDetail" class="p-6">
                <div class="space-y-4">
                  <div class="bg-gray-50 rounded-xl p-4">
                    <div class="space-y-2 text-sm">
                      <div><b class="text-gray-600">处罚编号：</b>{{ penaltyDetail.id }}</div>
                      <div><b class="text-gray-600">处罚时间：</b>{{ penaltyDetail.time }}</div>
                      <div><b class="text-gray-600">处罚原因：</b>{{ penaltyDetail.reason }}</div>
                      <div><b class="text-gray-600">平台措施：</b>{{ punishmentDict[penaltyDetail.platformAction] ||
                        penaltyDetail.platformAction }}</div>
                      <div><b class="text-gray-600">商家措施：</b>{{ punishmentDict[penaltyDetail.merchantAction] ||
                        penaltyDetail.merchantAction }}</div>
                    </div>
                  </div>
                </div>
              </div>
            </el-drawer>
          </div>

          <!-- 售后申请列表 -->
          <div v-if="activeAftersaleTab === 'aftersale'">
            <!-- 筛选条件 -->
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 mb-4">
              <div class="flex items-center gap-3 mb-3">
                <el-select v-model="asKeywordType" placeholder="筛选字段" size="small" teleported style="position: relative; z-index: 1; width: 100px; flex-shrink: 0;">
                    <el-option label="全部" value="" />
                    <el-option label="内容" value="content" />
                    <el-option label="订单号" value="orderNo" />
                    <el-option label="用户名" value="user.name" />
                  </el-select>
                <el-input v-model="asFilters.keyword" placeholder="内容/用户名/订单号" size="small" clearable style="position: relative; z-index: 2; flex: 1; min-width: 0;" />
                <el-button type="warning" size="small" @click="loadAfterSales(1)">查询</el-button>
                <el-button size="small" @click="resetAsFilters">重置</el-button>
                </div>
              <div class="flex items-center gap-2 flex-wrap">
                  <button
                    v-for="btn in asStatusButtons"
                    :key="btn.value"
                    @click="asSelectedStatus = btn.value"
                  :class="[
                    asSelectedStatus === btn.value
                      ? 'bg-orange-500 text-white font-bold shadow-sm'
                      : 'bg-white text-gray-600 border border-gray-200 hover:bg-gray-50'
                  ]"
                  class="px-3 py-1.5 rounded-full text-xs transition-all"
                  >
                    {{ btn.label }}
                  </button>
                </div>
              </div>

            <!-- 记录列表 -->
            <div class="space-y-4">
              <div v-if="aftersaleList.length === 0" class="text-center py-8 text-gray-400">
                <i class="fas fa-clipboard-list text-3xl mb-3"></i>
                <p class="text-sm">暂无售后申请</p>
                </div>
                <div
                  v-for="item in filteredAftersaleList"
                  :key="item.id"
                class="bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm hover:shadow-md"
                >
                  <!-- 用户信息和订单信息 -->
                <div class="flex items-start justify-between mb-3 pb-3 border-b border-dashed border-gray-200">
                    <div class="flex items-center gap-3 flex-1">
                      <img
                        v-if="item.user?.avatar"
                        :src="normalizeImageUrl(item.user.avatar)"
                        :alt="item.user.name"
                      class="w-10 h-10 rounded-full object-cover border border-gray-100"
                        @error="handleImageError"
                      />
                      <div class="flex-1">
                        <div class="flex items-center gap-2 mb-1">
                        <span class="font-bold text-base text-gray-900">{{ item.accountUserName || item.user?.name || '未知用户' }}</span>
                        </div>
                      <div class="space-y-0.5">
                        <p v-if="item.orderNo" class="text-xs text-gray-500">订单号：<span class="font-mono text-gray-700">{{ item.orderNo }}</span></p>
                        <p class="text-xs text-gray-500">申请时间：<span class="text-gray-700">{{ item.createdAt }}</span></p>
                        </div>
                      </div>
                    </div>
                    <div class="flex flex-col items-end gap-2">
                      <span
                      class="px-2 py-1 rounded text-xs font-medium"
                        :class="statusClass(item.status)"
                      >{{ item.status }}</span>
                    <button v-if="item.status === '待处理'" @click="openAsDetail(item.id)" class="bg-orange-500 hover:bg-orange-600 text-white px-3 py-1.5 rounded-full text-xs transition-colors cursor-pointer shadow-sm">处理</button>
                    </div>
                  </div>
                  <!-- 申请原因 -->
                <div class="mb-3">
                  <div class="p-3 bg-orange-50 rounded-lg border border-orange-100">
                    <div class="flex gap-2 text-xs">
                      <span class="font-medium text-orange-700 shrink-0">售后理由:</span>
                      <span class="text-gray-600">{{ item.reason }}</span>
                    </div>
                  </div>
                  </div>
                  <!-- 商家回复（仅待审核/已完成显示） -->
                <div v-if="(item.status === '待审核' || item.status === '已完成') && item.merchantReply" class="mb-3">
                  <div class="p-3 bg-orange-50 rounded-lg border border-orange-100">
                    <div class="flex gap-2 text-xs">
                      <span class="font-medium text-orange-700 shrink-0">商家回复:</span>
                      <span class="text-gray-600">{{ item.merchantReply }}</span>
                    </div>
                  </div>
                  </div>
                  <!-- 管理员处理结果（仅已完成显示） -->
                <div v-if="item.status === '已完成'" class="mb-3 p-3 bg-orange-50 rounded-lg space-y-1.5 border border-orange-100">
                  <div class="flex gap-2 text-xs">
                    <span class="font-medium text-orange-700 shrink-0">处理措施:</span>
                    <span class="text-gray-600">{{ getPunishmentLabel(item.punishment) }}</span>
                  </div>
                  <div class="flex gap-2 text-xs">
                    <span class="font-medium text-orange-700 shrink-0">处理原因:</span>
                    <span class="text-gray-600">{{ (item as any).punishmentReason || '-' }}</span>
                  </div>
                  </div>
                  <!-- 申请图片 -->
                <div v-if="item.images && item.images.length > 0" class="mt-4 pt-3 border-t border-gray-100">
                  <h4 class="text-xs font-semibold text-gray-700 mb-2">申请图片</h4>
                  <div class="flex flex-wrap gap-2">
                    <img
                      v-for="(image, idx) in item.images"
                      :key="idx"
                      :src="normalizeImageUrl(image)"
                      alt="申请图片"
                      class="w-20 h-20 object-cover rounded-lg border border-gray-200 cursor-pointer hover:opacity-80 hover:scale-105 transition-all"
                      @error="handleImageError"
                      @click="previewImage(image)"
                    />
                  </div>
                  </div>
                  <!-- 订单菜品列表 -->
                <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mt-4 pt-3 border-t border-gray-100">
                  <h4 class="text-xs font-semibold text-gray-700 mb-2">订单菜品</h4>
                  <div class="bg-gray-50 rounded-lg overflow-hidden border border-gray-200">
                    <div class="divide-y divide-gray-200">
                        <div
                          v-for="(dish, dishIdx) in item.dishDetails"
                          :key="dishIdx"
                        class="flex items-center gap-3 p-2 hover:bg-gray-100 transition-colors"
                        >
                          <img
                            :src="normalizeImageUrl(dish.dishImage)"
                            :alt="dish.dishName"
                          class="w-12 h-12 object-cover rounded border border-gray-200 shrink-0"
                            @error="handleImageError"
                          />
                        <div class="flex-1 min-w-0">
                          <p class="font-medium text-gray-900 text-xs truncate">{{ dish.dishName }}</p>
                          </div>
                        <div class="flex items-center gap-4 text-xs text-gray-600 shrink-0">
                          <div class="text-right">
                            <span class="text-gray-500">单价</span>
                            <p class="font-medium text-gray-900">¥{{ Number.isInteger(dish.price) ? dish.price : dish.price.toFixed(2) }}</p>
                          </div>
                          <div class="text-center w-12">
                            <span class="text-gray-500">数量</span>
                            <p class="font-medium text-gray-900">×{{ dish.quantity }}</p>
                        </div>
                          <div class="text-right w-16">
                            <span class="text-gray-500">小计</span>
                            <p class="font-medium text-orange-600">¥{{ Number.isInteger(dish.price * dish.quantity) ? (dish.price * dish.quantity) : (dish.price * dish.quantity).toFixed(2) }}</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              </div>
              </div>
            </div>
            
            <!-- 分页控制 -->
            <div v-if="asTotal > asPageSize" class="flex justify-center items-center gap-2 mt-6 mb-4">
              <button 
                @click="loadAfterSales(asPage - 1)"
                :disabled="asPage === 1"
                :class="[
                  asPage === 1
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                ]"
                class="px-3 py-1.5 rounded-full text-xs transition-colors">
                <i class="fas fa-chevron-left"></i>
              </button>
              
              <div class="flex gap-1">
                <button 
                  v-for="page in Math.ceil(asTotal / asPageSize)" 
                  :key="page"
                  @click="loadAfterSales(page)"
                  :class="[
                    asPage === page
                      ? 'bg-orange-500 text-white font-bold shadow-sm'
                      : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                  class="w-8 h-8 rounded-full text-xs transition-colors cursor-pointer flex items-center justify-center">
                  {{ page }}
                </button>
              </div>
              
              <button 
                @click="loadAfterSales(asPage + 1)"
                :disabled="asPage === Math.ceil(asTotal / asPageSize)"
                :class="[
                  asPage === Math.ceil(asTotal / asPageSize)
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                ]"
                class="px-3 py-1.5 rounded-full text-xs transition-colors">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
            <!-- 详情抽屉 -->
            <el-drawer v-model="asDetailVisible" title="售后详情" size="600px" direction="rtl" class="modern-drawer">
              <div v-if="asDetail" class="p-6">
                <div class="space-y-4">
                  <div class="bg-gray-50 rounded-xl p-4">
                    <div class="grid grid-cols-2 gap-4 text-sm">
                      <div class="flex items-center">
                        <span class="text-gray-600 inline-block w-20">订单号：</span>
                        <span>{{ asDetail.orderNo }}</span>
                      </div>
                      <div class="flex items-center">
                        <span class="text-gray-600 inline-block w-20">申请时间：</span>
                        <span>{{ asDetail.createdAt }}</span>
                      </div>
                      <div class="flex items-center">
                        <span class="text-gray-600 inline-block w-20">电话号：</span>
                        <span>{{ formatUserPhone(asDetail?.user) }}</span>
                      </div>
                      <div class="flex items-center">
                        <span class="text-gray-600 inline-block w-20">收货人：</span>
                        <span>{{ formatUserDisplayName(asDetail?.user) }}</span>
                      </div>
                    </div>
                  </div>
                  <div class="bg-gray-50 rounded-xl p-4">
                    <b class="text-gray-600 block mb-2">申请原因：</b>
                    <p class="text-sm">{{ asDetail.reason }}</p>
                  </div>
                  <div class="bg-orange-50 rounded-xl p-4 border border-orange-200">
                    <b class="text-gray-600 block mb-3">商家回复：</b>
                    <el-input
                      v-model="decision.remark"
                      placeholder="请输入商家回复（必填）"
                      class="modern-input"
                      :disabled="asDetail && (asDetail as any).status !== '待处理'"
                    />
                    <el-button
                      class="modern-btn-primary mt-3 mb-4"
                      :disabled="!decision.remark || (asDetail && (asDetail as any).status !== '待处理')"
                      @click="submitDecision"
                    >
                      {{ (asDetail && (asDetail as any).status !== '待处理') || (asDetail && asDetail.merchantReply) ? '已提交回复' : '提交回复' }}
                    </el-button>
                    <!-- 管理员处理结果（仅已完成显示） -->
                    <div v-if="asDetail && (asDetail as any).status === '已完成'" class="bg-gray-50 rounded-xl p-4 border border-gray-200 mt-2">
                      <b class="text-gray-600 block mb-2">管理员处理结果：</b>
                      <p class="text-gray-700"><b class="text-gray-600">处理措施：</b>{{ (asDetail as any).punishment || '-' }}</p>
                      <p class="text-gray-700 mt-2"><b class="text-gray-600">处理原因：</b>{{ (asDetail as any).punishmentReason || '-' }}</p>
                    </div>
                  </div>
                </div>
              </div>
            </el-drawer>
          </div>

          <!-- 评论查看 -->
          <div v-if="activeAftersaleTab === 'reviews'">
            <!-- 筛选条件 -->
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4 mb-4">
              <div class="flex items-center gap-3">
                <el-select v-model="reviewKeywordType" placeholder="筛选字段" size="small" teleported style="position: relative; z-index: 1; width: 100px; flex-shrink: 0;">
                    <el-option label="全部" value="" />
                    <el-option label="内容" value="content" />
                    <el-option label="订单号" value="orderNo" />
                    <el-option label="用户名" value="user.name" />
                  </el-select>
                <el-input v-model="reviewFilters.keyword" placeholder="内容/订单号" size="small" clearable style="position: relative; z-index: 2; flex: 1; min-width: 0;" />
                <el-button type="warning" size="small" @click="fetchReviews(1)">筛选</el-button>
                <el-button size="small" @click="resetReviewFilters">重置</el-button>
                </div>
              </div>

            <!-- 记录列表 -->
            <div class="space-y-4">
              <div v-if="reviews.length === 0" class="text-center py-8 text-gray-400">
                <i class="fas fa-comment text-3xl mb-3"></i>
                <p class="text-sm">暂无评论</p>
                </div>
                <div
                  v-for="item in reviews"
                  :key="item.id"
                class="bg-white rounded-xl p-4 text-left transition-all duration-200 mb-4 border border-gray-100 shadow-sm hover:shadow-md"
                >
                  <!-- 用户信息和评分 -->
                <div class="flex items-start justify-between mb-3 pb-3 border-b border-dashed border-gray-200">
                    <div class="flex items-center gap-3 flex-1">
                      <img
                        v-if="item.user?.avatar"
                        :src="normalizeImageUrl(item.user.avatar)"
                        :alt="item.user.name"
                      class="w-10 h-10 rounded-full object-cover border border-gray-100"
                        @error="handleImageError"
                      />
                      <div class="flex-1">
                        <div class="flex items-center gap-2 mb-1">
                        <span class="font-bold text-base text-gray-900">{{ item.user?.name || '未知用户' }}</span>
                          <div class="flex items-center">
                          <span v-for="i in 5" :key="i" class="text-xs">
                              <i
                                :class="i <= (item.rating || 0) ? 'fas fa-star text-yellow-400' : 'far fa-star text-gray-300'"
                              ></i>
                            </span>
                          </div>
                        </div>
                      <div class="space-y-0.5">
                        <p v-if="item.orderNo" class="text-xs text-gray-500">订单号：<span class="font-mono text-gray-700">{{ item.orderNo }}</span></p>
                        <p class="text-xs text-gray-500">评论时间：<span class="text-gray-700">{{ item.createdAt }}</span></p>
                        </div>
                      </div>
                    </div>
                  <button @click="openReplyDialog(item)" class="bg-orange-500 hover:bg-orange-600 text-white px-3 py-1.5 rounded-full text-xs transition-colors cursor-pointer shadow-sm">回复</button>
                  </div>
                  <!-- 评论内容 -->
                <div class="mb-3">
                  <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-2 rounded">{{ item.content }}</p>
                  </div>
                  <!-- 商家回复 -->
                <div v-if="item.merchantReply" class="mb-3 pt-3 border-t border-orange-200">
                  <div class="bg-gradient-to-r from-orange-50 to-orange-100 rounded-lg p-3 border border-orange-200">
                    <div class="flex items-center gap-2 mb-2">
                      <i class="fas fa-store text-orange-600 text-xs"></i>
                      <span class="font-semibold text-orange-700 text-sm">商家回复</span>
                      <span 
                        v-if="item.merchantReplyStatus"
                        :class="[
                          'px-2 py-0.5 rounded-full text-xs font-medium',
                          item.merchantReplyStatus === '已通过' 
                            ? 'bg-green-100 text-green-700' 
                            : item.merchantReplyStatus === '待审核'
                            ? 'bg-yellow-100 text-yellow-700'
                            : 'bg-red-100 text-red-700'
                        ]"
                      >
                        {{ item.merchantReplyStatus }}
                      </span>
                      <span v-if="item.merchantReplyTime" class="text-xs text-orange-600 ml-auto">{{ item.merchantReplyTime }}</span>
                    </div>
                    <p class="text-sm text-gray-800 leading-relaxed">{{ item.merchantReply }}</p>
                  </div>
                  </div>
                  <!-- 评论图片 -->
                <div v-if="item.images && item.images.length > 0" class="mt-4 pt-3 border-t border-gray-100">
                  <h4 class="text-xs font-semibold text-gray-700 mb-2">评论图片</h4>
                  <div class="flex flex-wrap gap-2">
                    <img
                      v-for="(image, idx) in item.images"
                      :key="idx"
                      :src="normalizeImageUrl(image)"
                      alt="评论图片"
                      class="w-20 h-20 object-cover rounded-lg border border-gray-200 cursor-pointer hover:opacity-80 hover:scale-105 transition-all"
                      @error="handleImageError"
                      @click="previewImage(image)"
                    />
                  </div>
                  </div>
                  <!-- 订单菜品列表 -->
                <div v-if="item.dishDetails && item.dishDetails.length > 0" class="mt-4 pt-3 border-t border-gray-100">
                  <h4 class="text-xs font-semibold text-gray-700 mb-2">订单菜品</h4>
                  <div class="bg-gray-50 rounded-lg overflow-hidden border border-gray-200">
                    <div class="divide-y divide-gray-200">
                        <div
                          v-for="(dish, dishIdx) in item.dishDetails"
                          :key="dishIdx"
                        class="flex items-center gap-3 p-2 hover:bg-gray-100 transition-colors"
                        >
                          <img
                            :src="normalizeImageUrl(dish.dishImage)"
                            :alt="dish.dishName"
                          class="w-12 h-12 object-cover rounded border border-gray-200 shrink-0"
                            @error="handleImageError"
                          />
                        <div class="flex-1 min-w-0">
                          <p class="font-medium text-gray-900 text-xs truncate">{{ dish.dishName }}</p>
                          </div>
                        <div class="flex items-center gap-4 text-xs text-gray-600 shrink-0">
                          <div class="text-right">
                            <span class="text-gray-500">单价</span>
                            <p class="font-medium text-gray-900">¥{{ Number.isInteger(dish.price) ? dish.price : dish.price.toFixed(2) }}</p>
                          </div>
                          <div class="text-center w-12">
                            <span class="text-gray-500">数量</span>
                            <p class="font-medium text-gray-900">×{{ dish.quantity }}</p>
                        </div>
                          <div class="text-right w-16">
                            <span class="text-gray-500">小计</span>
                            <p class="font-medium text-orange-600">¥{{ Number.isInteger(dish.price * dish.quantity) ? (dish.price * dish.quantity) : (dish.price * dish.quantity).toFixed(2) }}</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              </div>
              </div>
            </div>
            
            <!-- 分页控制 -->
            <div v-if="reviewTotal > reviewPageSize" class="flex justify-center items-center gap-2 mt-6 mb-4">
              <button 
                @click="fetchReviews(reviewPage - 1)"
                :disabled="reviewPage === 1"
                :class="[
                  reviewPage === 1
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                ]"
                class="px-3 py-1.5 rounded-full text-xs transition-colors">
                <i class="fas fa-chevron-left"></i>
              </button>
              
              <div class="flex gap-1">
                <button 
                  v-for="page in Math.ceil(reviewTotal / reviewPageSize)" 
                  :key="page"
                  @click="fetchReviews(page)"
                  :class="[
                    reviewPage === page
                      ? 'bg-orange-500 text-white font-bold shadow-sm'
                      : 'bg-white border border-gray-200 text-gray-600 hover:bg-gray-50'
                  ]"
                  class="w-8 h-8 rounded-full text-xs transition-colors cursor-pointer flex items-center justify-center">
                  {{ page }}
                </button>
              </div>
              
              <button 
                @click="fetchReviews(reviewPage + 1)"
                :disabled="reviewPage === Math.ceil(reviewTotal / reviewPageSize)"
                :class="[
                  reviewPage === Math.ceil(reviewTotal / reviewPageSize)
                    ? 'bg-gray-100 text-gray-400 cursor-not-allowed'
                    : 'bg-white border border-orange-200 hover:bg-orange-50 text-orange-600 cursor-pointer shadow-sm hover:shadow'
                ]"
                class="px-3 py-1.5 rounded-full text-xs transition-colors">
                <i class="fas fa-chevron-right"></i>
              </button>
            </div>
            <!-- 回复弹窗 -->
            <el-dialog 
              v-model="replyDialogVisible" 
              width="600px" 
              class="modern-dialog"
              :close-on-click-modal="false"
            >
              <template #header>
                <div class="flex items-center pb-3 border-b border-gray-100">
                  <!-- 用户信息（左上角） -->
                  <div class="flex items-center gap-3">
                    <img
                      v-if="currentReview?.user?.avatar"
                      :src="normalizeImageUrl(currentReview.user.avatar)"
                      :alt="currentReview.user.name"
                      class="w-12 h-12 rounded-full object-cover border-2 border-orange-200 shadow-md"
                      @error="handleImageError"
                    />
                    <div 
                      v-else
                      class="w-12 h-12 rounded-full bg-gradient-to-br from-orange-400 to-orange-600 flex items-center justify-center border-2 border-orange-200 shadow-md"
                    >
                      <i class="fas fa-user text-white text-lg"></i>
                    </div>
                <div>
                      <h3 class="text-lg font-bold text-gray-900">{{ currentReview?.user?.name || '未知用户' }}</h3>
                      <p class="text-xs text-gray-500 mt-0.5">回复评论</p>
                    </div>
                  </div>
                </div>
              </template>
              
              <div class="space-y-5 pt-2">
                <!-- 用户评论 -->
                <div class="bg-gradient-to-br from-gray-50 to-gray-100 rounded-xl p-4 border border-gray-200">
                  <div class="flex items-center gap-2 mb-3">
                    <i class="fas fa-comment text-orange-500 text-sm"></i>
                    <span class="font-semibold text-gray-700 text-sm">用户评论</span>
                  </div>
                  <div 
                    class="bg-white rounded-lg border border-gray-200 p-4 space-y-3 shadow-inner"
                  >
                    <div v-if="!currentReview" class="text-center text-gray-400 text-sm py-8">
                      <i class="fas fa-spinner fa-spin text-orange-500 text-xl mb-2"></i>
                      <p>加载中...</p>
                    </div>
                    <template v-else>
                      <!-- 评论信息 -->
                      <div class="flex items-center justify-between pb-3 border-b border-gray-100">
                        <div class="flex items-center gap-2">
                          <span class="text-xs text-gray-500">评论时间：</span>
                          <span class="text-xs text-gray-700 font-medium">{{ currentReview.createdAt }}</span>
                        </div>
                        <div v-if="currentReview.rating" class="flex items-center gap-1">
                          <span v-for="i in 5" :key="i" class="text-sm">
                            <i
                              :class="i <= (currentReview.rating || 0) ? 'fas fa-star text-yellow-400' : 'far fa-star text-gray-300'"
                            ></i>
                          </span>
                        </div>
                      </div>
                      
                      <!-- 评论内容 -->
                      <div class="bg-gray-50 rounded-lg p-3 border border-gray-100">
                        <p class="text-sm text-gray-800 leading-relaxed whitespace-pre-wrap">{{ currentReview.content }}</p>
                      </div>
                      
                      <!-- 评论图片 -->
                      <div v-if="currentReview.images && currentReview.images.length > 0" class="pt-3 border-t border-gray-100">
                        <div class="flex items-center gap-2 mb-2">
                          <i class="fas fa-images text-orange-500 text-xs"></i>
                          <span class="text-xs font-semibold text-gray-700">评论图片</span>
                        </div>
                        <div class="grid grid-cols-3 gap-2">
                          <div
                            v-for="(image, idx) in currentReview.images"
                            :key="idx"
                            class="relative group cursor-pointer rounded-lg overflow-hidden border border-gray-200 hover:border-orange-400 transition-all"
                            @click="previewImage(image)"
                          >
                            <img
                              :src="normalizeImageUrl(image)"
                              :alt="`评论图片${idx + 1}`"
                              class="w-full h-24 object-cover group-hover:scale-105 transition-transform duration-200"
                              @error="handleImageError"
                            />
                            <div class="absolute inset-0 bg-black bg-opacity-0 group-hover:bg-opacity-20 transition-all flex items-center justify-center">
                              <i class="fas fa-search-plus text-white opacity-0 group-hover:opacity-100 transition-opacity text-lg"></i>
                            </div>
                          </div>
                        </div>
                      </div>
                    </template>
                  </div>
                </div>

                <!-- 快捷工具 -->
                <div class="grid grid-cols-2 gap-4">
                  <!-- 常用语 -->
                  <div class="bg-white rounded-xl p-4 border border-gray-200 shadow-sm">
                    <div class="flex items-center gap-2 mb-3">
                      <i class="fas fa-lightbulb text-yellow-500 text-sm"></i>
                      <span class="font-semibold text-gray-700 text-sm">常用语</span>
                  </div>
                    <div class="flex flex-wrap gap-2">
                      <button
                        v-for="(phrase, idx) in quickPhrases"
                        :key="idx"
                        @click="insertToReply(phrase)"
                        class="px-3 py-1.5 bg-orange-50 hover:bg-orange-100 text-orange-700 rounded-lg text-xs transition-all duration-200 border border-orange-200 hover:border-orange-300 hover:shadow-sm"
                      >
                        {{ phrase }}
                      </button>
                </div>
                  </div>

                  <!-- 表情 -->
                  <div class="bg-white rounded-xl p-4 border border-gray-200 shadow-sm">
                    <div class="flex items-center gap-2 mb-3">
                      <i class="fas fa-smile text-yellow-500 text-sm"></i>
                      <span class="font-semibold text-gray-700 text-sm">表情</span>
                </div>
                    <div class="flex flex-wrap gap-2 max-h-24 overflow-y-auto">
                      <button
                        v-for="(emoji, idx) in emojis"
                        :key="idx"
                        @click="insertToReply(emoji)"
                        class="text-xl w-8 h-8 flex items-center justify-center rounded-lg hover:bg-orange-50 transition-all duration-200 hover:scale-110 cursor-pointer"
                      >
                        {{ emoji }}
                      </button>
              </div>
                  </div>
                </div>

                <!-- 输入框 -->
                <div class="bg-white rounded-xl p-4 border border-gray-200 shadow-sm">
                  <div class="flex items-center gap-2 mb-3">
                    <i class="fas fa-edit text-orange-500 text-sm"></i>
                    <span class="font-semibold text-gray-700 text-sm">回复内容</span>
                  </div>
                  <el-input 
                    id="reply-content-textarea" 
                    v-model="replyContent" 
                    type="textarea" 
                    placeholder="请输入您的回复内容..."
                    :rows="5"
                    :maxlength="500"
                    show-word-limit
                    class="modern-textarea"
                  />
                </div>
              </div>

              <template #footer>
                <div class="flex items-center justify-end gap-3 pt-4 border-t border-gray-100">
                  <el-button 
                    @click="replyDialogVisible = false" 
                    class="px-6 py-2.5 border border-gray-300 text-gray-700 hover:bg-gray-50 rounded-lg transition-all"
                  >
                    <i class="fas fa-times mr-2"></i>取消
                  </el-button>
                  <el-button 
                    @click="submitReply"
                    :disabled="!replyContent.trim()"
                    class="px-6 py-2.5 bg-gradient-to-r from-orange-500 to-orange-600 hover:from-orange-600 hover:to-orange-700 text-white rounded-lg shadow-md hover:shadow-lg transition-all disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <i class="fas fa-paper-plane mr-2"></i>发送回复
                  </el-button>
                </div>
              </template>
            </el-dialog>
          </div>

          <!-- 左侧主内容结束 -->
        </div>
        
          <!-- 右侧辅助面板 -->
          <div class="w-72 shrink-0 space-y-4 lg:ml-4 xl:ml-10 lg:sticky lg:top-24 self-start">
            <!-- 快捷操作 -->
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-bolt text-orange-500"></i>
                快捷操作
              </h3>
              <div class="space-y-2">
                <button 
                  @click="activeAftersaleTab = 'aftersale'; asSelectedStatus = '待处理'"
                  class="w-full bg-orange-50 hover:bg-orange-100 text-orange-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between">
                  <span>查看待处理售后</span>
                  <i class="fas fa-chevron-right text-orange-400"></i>
                </button>
                <button 
                  @click="activeAftersaleTab = 'reviews'"
                  class="w-full bg-blue-50 hover:bg-blue-100 text-blue-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between">
                  <span>查看最新评论</span>
                  <i class="fas fa-chevron-right text-blue-400"></i>
                </button>
                <button 
                  @click="activeAftersaleTab = 'penalties'"
                  class="w-full bg-red-50 hover:bg-red-100 text-red-700 px-3 py-2 rounded-lg text-xs transition-colors text-left flex items-center justify-between">
                  <span>查看处罚记录</span>
                  <i class="fas fa-chevron-right text-red-400"></i>
                </button>
              </div>
            </div>

            <!-- 温馨提示 -->
            <div class="bg-gradient-to-br from-orange-50 to-yellow-50 rounded-xl shadow-sm border border-orange-200 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-lightbulb text-yellow-500"></i>
                温馨提示
              </h3>
              <div class="space-y-2 text-xs text-gray-700">
                <div class="flex items-start gap-2">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 shrink-0"></i>
                  <p>及时处理售后申请可提升用户满意度</p>
                </div>
                <div class="flex items-start gap-2">
                  <i class="fas fa-check-circle text-green-500 mt-0.5 shrink-0"></i>
                  <p>积极回复评论有助于改善店铺形象</p>
                </div>
              </div>
            </div>

            <!-- 数据统计 -->
            <div class="bg-white rounded-xl shadow-sm border border-gray-100 p-4">
              <h3 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <i class="fas fa-chart-bar text-blue-500"></i>
                本月数据
              </h3>
              <div class="space-y-3">
                <div class="flex items-center justify-between text-xs">
                  <span class="text-gray-600">售后申请</span>
                  <span class="font-bold text-gray-900">{{ asTotal }} 条</span>
                </div>
                <div class="h-px bg-gray-100"></div>
                <div class="flex items-center justify-between text-xs">
                  <span class="text-gray-600">评论总数</span>
                  <span class="font-bold text-gray-900">{{ reviewTotal }} 条</span>
                </div>
                <div class="h-px bg-gray-100"></div>
                <div class="flex items-center justify-between text-xs">
                  <span class="text-gray-600">处罚记录</span>
                  <span class="font-bold text-gray-900">{{ penaltyList.length }} 条</span>
                </div>
              </div>
            </div>
          </div>
        </div>
          </div>
    </div>
  </Layout>
</template>

<script lang="ts" setup>
import { ref, reactive, onMounted, nextTick, computed } from 'vue';
import { ElMessage } from 'element-plus';

// API 导入
import { replyReview, getReviewList, getPenaltyList, getPenaltyDetail, type Review } from '@/api/merchant';
import type { AfterSaleApplication, AfterSaleListParams, AfterSaleUserInfo } from '@/api/merchant';
import { getAfterSaleList, getAfterSaleDetail, replyAfterSale } from '@/api/merchant';
import { type PenaltyRecord } from '@/api/merchant';
import { getMerchantInfo, type MerchantInfo } from '@/api/merchant';

// 布局组件
import Layout from '@/features/merchant/components/Layout.vue';

// 图片工具函数
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { API_CONFIG } from '@/config';

// 本地聊天消息类型
interface LocalChatMessage {
  sender: 'user' | 'merchant';
  content: string;
  time: string;
}

// 处罚选项映射
const punishmentLabels: Record<string, string> = {
  // 售后处理
  'full_refund': '全额退款',
  'partial_refund': '部分退款',
  'redelivery': '重新配送',
  'apology': '商家道歉',
  'compensation': '赔偿用户',
};

// 辅助函数：将处罚 value 转换为 label
const getPunishmentLabel = (value: string | undefined): string => {
  if (!value) return '-';
  return punishmentLabels[value] || value;
};

// 统一关键字规范化：支持按字段类型规范化（ORD/PEN 编号、手机号等）
function normalizeKeyword(raw: string, type?: string): string | undefined {
  const value = (raw ?? '').trim();
  if (!value) return undefined;

  // 按字段类型优先处理
  if (type === 'orderNo' || type === 'id') {
    return value.replace(/\D/g, '');
  }
  if (type === 'user.phone' || type === 'phone') {
    return value.replace(/\D/g, '');
  }

  const upper = value.toUpperCase();
  // 处理以 ORD / PEN 开头的编号：提取纯数字
  if (/^(ORD|PEN)\d+$/.test(upper)) {
    return value.replace(/\D/g, '');
  }

  // 对看起来像号码的内容（包含空格或短横线）保留数字以增强匹配
  const digitsOnly = value.replace(/\D/g, '');
  if (digitsOnly.length >= 4 && /[\d\-\s]+$/.test(value)) {
    return digitsOnly;
  }

  return value;
}

function formatUserDisplayName(user?: AfterSaleUserInfo): string {
  if (!user) return '未知';
  const fullName = (user as any).fullName as (string | undefined);
  const baseName = fullName && fullName.trim().length > 0 ? fullName.trim() : (user.name || '').trim();
  const hasChinese = /[\u4e00-\u9fa5]/.test(baseName);
  const surname = baseName ? (hasChinese ? baseName.slice(0, 1) : baseName) : '未知';
  const rawGender = ((user as any).gender || '').toString().trim().toLowerCase();
  let honorific = '';
  if (rawGender === 'm' || rawGender === 'male' || rawGender === '男' || rawGender === '先生' || rawGender === 'mr' || rawGender === '1') honorific = '先生';
  else if (rawGender === 'f' || rawGender === 'female' || rawGender === '女' || rawGender === '女士' || rawGender === 'ms' || rawGender === '0') honorific = '女士';
  return honorific ? `${surname}${honorific}` : surname;
}

function formatUserPhone(user?: AfterSaleUserInfo): string {
  if (!user) return '-';
  const raw = (user as any).phoneNumber ?? (user as any).phone;
  if (typeof raw === 'number') return raw > 0 ? String(raw) : '-';
  if (typeof raw === 'string') return raw.trim() || '-';
  return '-';
}

const fetchAllData = async () => {
  try {
    console.log('开始获取商家信息...');
    const merchant = await getMerchantInfo();
    console.log('获取到的商家信息:', merchant);
    
    if (merchant) {
      merchantInfo.value = { ...defaultMerchantInfo, ...merchant };
      console.log('设置后的商家信息:', merchantInfo.value);
    }
  } catch (error) {
    console.error('获取商家信息失败:', error);
    merchantInfo.value = { ...defaultMerchantInfo };
  }
};

const defaultMerchantInfo = {
  username: '',
  sellerId: 0
};

const merchantInfo = ref({ ...defaultMerchantInfo });

const handleMenuClick = (menuItem: any) => {
  // 菜单点击处理已由MerchantLayout组件处理
};

// 处罚记录
const penaltyList = ref<PenaltyRecord[]>([]);
const penaltyFilters = reactive<{ keyword: string }>({ keyword: '' });
const penaltyDetailVisible = ref(false);
const penaltyDetail = ref<PenaltyRecord | null>(null);

async function loadPenalties() {
  const params: { keyword?: string } = {};
  const k = normalizeKeyword(penaltyFilters.keyword, penaltyKeywordType.value || undefined);
  if (k) params.keyword = k;
  // 传入筛选字段（id | reason）
  const list = await getPenaltyList({ keyword: params.keyword, field: penaltyKeywordType.value || undefined });
  try {
    penaltyList.value = list || [];
  } catch (error) {
    console.error('加载处罚记录失败:', error);
    penaltyList.value = [];
  }
}

async function openPenaltyDetail(row: PenaltyRecord) {
  try {
    penaltyDetail.value = await getPenaltyDetail(row.id);
    penaltyDetailVisible.value = true;
  } catch (error) {
    console.error('获取处罚详情失败:', error);
  }
}

// 评论管理
const reviews = ref<Review[]>([]);
const reviewPage = ref(1);
const reviewPageSize = ref(10);
const reviewTotal = ref(0);
const reviewFilters = reactive({
  keyword: ''
});

async function fetchReviews(page = 1) {
  reviewPage.value = page;
  
  // 检查是否有商家ID
  console.log('当前商家信息:', merchantInfo.value);
  console.log('商家ID:', merchantInfo.value.sellerId);
  
  if (!merchantInfo.value.sellerId) {
    console.warn('商家ID未获取到，无法加载评论');
    reviews.value = [];
    reviewTotal.value = 0;
    return;
  }
  
  try {
    const params = {
      page: reviewPage.value,
      pageSize: reviewPageSize.value,
      keyword: normalizeKeyword(reviewFilters.keyword, reviewKeywordType.value || undefined),
      field: reviewKeywordType.value || undefined,
      sellerId: merchantInfo.value.sellerId
    };
    
    console.log('发送评论请求参数:', params);
    const res = await getReviewList(params);
    console.log('评论API响应:', res);
    
    reviews.value = res.list || [];
    reviewTotal.value = res.total || 0;
  } catch (error) {
    console.error('获取评论列表失败:', error);
    reviews.value = [];
    reviewTotal.value = 0;
  }
}

function resetReviewFilters() {
  reviewFilters.keyword = '';
  fetchReviews(1);
}
// 回复
const replyDialogVisible = ref(false);
const replyContent = ref('');
const replyReviewId = ref<number | null>(null);
const currentReview = ref<Review | null>(null);
const chatMessages = ref<LocalChatMessage[]>([]);
const chatLoading = ref(false);
const activeChatOrderNo = ref<string | null>(null);

async function loadChatHistory() {
  chatLoading.value = true;
  try {
    // 聊天记录，实际项目中应该调用后端API
    const synthetic: LocalChatMessage[] = [];
    if (currentReview.value) {
      // 用户最开始的评论
      synthetic.push({ sender: 'user', content: currentReview.value.content, time: currentReview.value.createdAt });
    }
    chatMessages.value = [...synthetic];
  } catch (err) {
    const fallback: LocalChatMessage[] = [];
    if (currentReview.value) {
      fallback.push({ sender: 'user', content: currentReview.value.content, time: currentReview.value.createdAt });
    }
    chatMessages.value = fallback;
  } finally {
    chatLoading.value = false;
    nextTick(() => {
      const el = document.getElementById('reply-chat-container');
      if (el) el.scrollTop = el.scrollHeight;
    });
  }
}

function openReplyDialog(review: Review) {
  replyReviewId.value = review.id;
  currentReview.value = review;
  replyContent.value = '';
  replyDialogVisible.value = true;
  activeChatOrderNo.value = review.orderNo;
  loadChatHistory();
}

async function submitReply() {
  if (!replyReviewId.value || !replyContent.value.trim()) {
    ElMessage.warning('请输入回复内容');
    return;
  }
  
  try {
  await replyReview(replyReviewId.value, replyContent.value);
    ElMessage.success('回复成功，等待管理员审核');
    
    // 关闭对话框
    replyDialogVisible.value = false;
  replyContent.value = '';
    
    // 刷新评论列表
    await fetchReviews(reviewPage.value);
  } catch (error) {
    console.error('回复评论失败:', error);
    ElMessage.error('回复失败，请稍后重试');
  }
}
onMounted(async () => {
  await fetchAllData();
  fetchReviews();
  loadPenalties();
  loadAfterSales(1);
});

const aftersaleTabs = [
  { value: 'aftersale', label: '售后申请' },
  { value: 'reviews', label: '评论查看' },
  { value: 'penalties', label: '处罚记录' }
];

const activeAftersaleTab = ref('aftersale');
// 售后筛选字段
const asKeywordType = ref('');
// 评论与处罚筛选字段
const reviewKeywordType = ref('');
const penaltyKeywordType = ref('');

// 售后状态筛选按钮与选择
const asStatusButtons = [
  { label: '全部', value: '' },
  { label: '待处理', value: '待处理' },
  { label: '待审核', value: '待审核' },
  { label: '已完成', value: '已完成' }
];
const asSelectedStatus = ref<string>(''); // 空为不过滤

// 常用语和表情
const quickPhrases = [
  '感谢您的反馈！',
  '欢迎再次光临！',
  '我们会尽快改进',
  '祝您生活愉快！',
  '很抱歉给您带来不便'
];
const emojis = [
  '😀','😂','🥰','😎','🤔','😱','😴','🤗','😤','😇','😜','😅','😆','😏','😬','😳','😢','😭','😡','😋',
  '👍','🙏','��','🎉','🌟','🍽️','🍔','🍟','🍕','🍜','🍣','🍦','🍰','🥤','🥟','🥗','🥩','🥚','🥛'
];

function insertToReply(text: string) {
  // 插入到光标处
  const textarea = document.getElementById('reply-content-textarea') as HTMLTextAreaElement | null;
  if (textarea) {
    const start = textarea.selectionStart;
    const end = textarea.selectionEnd;
    const value = replyContent.value;
    replyContent.value = value.slice(0, start) + text + value.slice(end);
    // 重新聚焦并设置光标
    nextTick(() => {
      textarea.focus();
      textarea.selectionStart = textarea.selectionEnd = start + text.length;
    });
  } else {
    replyContent.value += text;
  }
}

// 2. 售后申请相关数据与方法
const aftersaleList = ref<AfterSaleApplication[]>([]);
const asPage = ref(1);
const asPageSize = ref(10);
const asTotal = ref(0);
const asFilters = reactive({
  keyword: ''
});


async function loadAfterSales(page = 1) {
  asPage.value = page;
  const params: AfterSaleListParams = {
    page: asPage.value,
    pageSize: asPageSize.value,
    keyword: normalizeKeyword(asFilters.keyword, asKeywordType.value || undefined),
    field: asKeywordType.value || undefined,
    sellerId: merchantInfo.value.sellerId || 0
  };
  try {
    const res = await getAfterSaleList(params);
    aftersaleList.value = res.list || [];
    asTotal.value = res.total || 0;
  } catch (error) {
    console.error('获取售后申请列表失败:', error);
    aftersaleList.value = [];
    asTotal.value = 0;
  }
}

function resetAsFilters() {
  asFilters.keyword = '';
  loadAfterSales(1);
}

// 详情与处理
const asDetailVisible = ref(false);
const asDetail = ref<AfterSaleApplication | null>(null);
// 计算：按状态筛选后的售后列表
const filteredAftersaleList = computed(() => {
  const list = aftersaleList.value || [];
  const s = (asSelectedStatus.value || '').trim();
  if (!s) return list;
  return list.filter(x => (x.status || '').trim() === s);
});

async function openAsDetail(id: number) {
  asDetailVisible.value = true;
  try {
    const detail = await getAfterSaleDetail(id);
    asDetail.value = detail;
    // 打开详情后，将输入框内容与已存在的商家回复同步（若有）
    decision.remark = (asDetail.value && asDetail.value.merchantReply) ? asDetail.value.merchantReply : '';
  } catch (error) {
    console.error('获取售后申请详情失败:', error);
    asDetail.value = null;
  }
}

function clearDecision() {
  decision.remark = '';
}

const decision = reactive<{ remark: string }>({ remark: '' });

async function submitDecision() {
  if (!asDetail.value || !decision.remark) return;
  try {
    await replyAfterSale(asDetail.value.id, { remark: decision.remark });
    await loadAfterSales(asPage.value);
    asDetail.value = await getAfterSaleDetail(asDetail.value.id);
    // 提交成功后，保持输入框内为已提交的商家回复
    decision.remark = (asDetail.value && asDetail.value.merchantReply) ? asDetail.value.merchantReply : decision.remark;
  } catch (error) {
    console.error('提交商家回复失败:', error);
  }
}

const punishmentDict: Record<string, string> = {
  verbal_warning: '口头警告',
  written_warning: '书面警告',
  fine_500: '罚款500元',
  fine_1000: '罚款1000元',
  correction: '限期整改',
  suspend_3days: '暂停营业3天',
  suspend_7days: '暂停营业7天',
  permanent_removal: '永久下架',
};

function statusClass(status?: string): string {
  const s = (status || '').trim();
  if (s === '待处理') return 'text-amber-700 bg-amber-100 border-amber-200';
  if (s === '待审核') return 'text-blue-700 bg-blue-100 border-blue-200';
  if (s === '已完成') return 'text-green-700 bg-green-100 border-green-200';
  return 'text-gray-700 bg-gray-100 border-gray-200';
}

// 图片预览
const previewImage = (imageUrl: string) => {
  const fullUrl = imageUrl.startsWith('http') ? imageUrl : `${API_CONFIG.BASE_URL}${imageUrl}`;
  window.open(fullUrl, '_blank');
};

</script>

<style scoped>
/* 隐藏滚动条 */
.scrollbar-hide::-webkit-scrollbar {
    display: none;
}

.scrollbar-hide {
    -ms-overflow-style: none;
    scrollbar-width: none;
}

/* 苹果风格设计 */
.modern-select :deep(.el-input__wrapper) {
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.modern-input :deep(.el-input__wrapper) {
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.modern-textarea :deep(.el-textarea__inner) {
  border-radius: 12px;
  border: 1px solid #e5e7eb;
  background: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
}

.modern-btn-primary {
  background: linear-gradient(135deg, #F9771C 0%, #ff8c42 100%);
  border: none;
  border-radius: 12px;
  color: white;
  font-weight: 500;
  box-shadow: 0 4px 12px rgba(249, 119, 28, 0.3);
  transition: all 0.3s ease;
}

.modern-btn-primary:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(249, 119, 28, 0.4);
}

.modern-btn-secondary {
  background: rgba(255, 255, 255, 0.8);
  border: 1px solid #F9771C;
  border-radius: 12px;
  color: #F9771C;
  font-weight: 500;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.modern-btn-secondary:hover {
  background: rgba(249, 119, 28, 0.1);
  transform: translateY(-1px);
}

.modern-table :deep(.el-table__header) {
  background: rgba(249, 119, 28, 0.05);
}

.modern-table :deep(.el-table__row:hover) {
  background: rgba(249, 119, 28, 0.02);
}

.modern-tag {
  background: rgba(249, 119, 28, 0.1);
  border: 1px solid rgba(249, 119, 28, 0.2);
  color: #F9771C;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.modern-tag:hover {
  background: rgba(249, 119, 28, 0.2);
  transform: scale(1.05);
}

.modern-radio :deep(.el-radio__input.is-checked .el-radio__inner) {
  background: #F9771C;
  border-color: #F9771C;
}

.modern-pagination :deep(.el-pager li.is-active) {
  background: #F9771C;
  color: white;
}

.modern-drawer :deep(.el-drawer__header) {
  background: rgba(249, 119, 28, 0.05);
  border-bottom: 1px solid #e5e7eb;
}

.modern-dialog :deep(.el-dialog__header) {
  background: rgba(249, 119, 28, 0.05);
  border-bottom: 1px solid #e5e7eb;
}

.modern-timeline :deep(.el-timeline-item__node) {
  background: #F9771C;
}

.modern-timeline :deep(.el-timeline-item__tail) {
  border-left-color: rgba(249, 119, 28, 0.2);
}

.modern-file-input {
  border: 2px dashed #e5e7eb;
  border-radius: 12px;
  padding: 20px;
  text-align: center;
  background: rgba(255, 255, 255, 0.5);
  cursor: pointer;
  transition: all 0.3s ease;
}

.modern-file-input:hover {
  border-color: #F9771C;
  background: rgba(249, 119, 28, 0.05);
}

.\!rounded-button {
  border-radius: 12px;
}

input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
  appearance: textfield;
}
</style>

