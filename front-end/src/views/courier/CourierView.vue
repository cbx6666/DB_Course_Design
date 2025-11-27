<template>
    <div class="min-h-screen bg-gray-50">
        <!-- 顶部导航栏 - 始终显示 -->
        <div
            class="fixed top-0 left-0 right-0 z-50 px-4 py-4"
            style="background: linear-gradient(to right, #f97316, #fb923c);">
            <div class="flex items-center justify-between relative">
                <!-- 左侧：头像和姓名 -->
                <div class="flex items-center space-x-3">
                    <div
                        class="w-10 h-10 bg-white bg-opacity-20 rounded-xl flex items-center justify-center">
                        <el-icon class="text-white text-xl">
                            <User />
                        </el-icon>
                    </div>
                    <div>
                        <div class="text-lg font-semibold text-white">
                            {{ userProfile?.name || '加载中...' }}
                        </div>
                        <div v-if="userProfile?.fullName && userProfile?.gender" class="text-xs text-white opacity-80 mt-0.5">
                            {{ getHonorificName(userProfile) }}
                        </div>
                    </div>
                </div>

                <!-- 中间：今日收入 -->
                <div class="absolute left-1/2 transform -translate-x-1/2 text-center">
                    <div class="text-lg font-bold text-white">
                        今日收入 ¥{{ todayIncome.toFixed(2) }}
                    </div>
                </div>

                <!-- 右侧：只保留通知铃铛图标 -->
                <div class="flex items-center">
                    <div
                        class="w-9 h-9 bg-white bg-opacity-20 rounded-xl flex items-center justify-center">
                        <el-icon class="text-white text-xl cursor-pointer">
                            <Bell />
                        </el-icon>
                    </div>
                </div>
            </div>
        </div>

        <!-- 状态一: 加载中 -->
        <div v-if="isLoading" class="flex flex-col justify-center items-center h-screen pt-16">
            <p class="text-gray-500">正在拼命加载数据中...</p>
        </div>

        <!-- 状态二: 加载失败 -->
        <div v-else-if="errorState" class="flex flex-col justify-center items-center h-screen space-y-4 pt-16">
            <el-icon class="text-red-500 text-5xl">
                <CircleCloseFilled />
            </el-icon>
            <p class="text-red-500">{{ errorState }}</p>
            <button @click="loadDashboardData"
                class="bg-orange-500 text-white px-6 py-2 rounded-full shadow-md hover:bg-orange-600 transition-all">
                点击重试
            </button>
        </div>

        <!-- 状态三: 加载成功 (渲染主要内容) -->
        <div v-else>

            <!-- 主要内容区域 -->
            <div class="min-h-screen bg-gray-50">
                <div class="pt-24 pb-20">
                    <!-- 工作台页面 -->
                    <div v-if="currentTab === 'home'">
                        <!-- 工作状态卡片 -->
                        <div v-if="workStatus" class="bg-white mx-4 mt-6 rounded-2xl shadow-lg p-5">
                            <div class="flex items-center justify-between mb-5">
                                <div class="flex items-center space-x-3">
                                    <div class="text-xl font-semibold text-gray-800">工作状态</div>
                                    <div v-if="workStatus.isOnline"
                                        class="px-3 py-1 bg-gradient-to-r from-green-500 to-green-400 text-white text-xs rounded-full shadow-sm">
                                        在线</div>
                                    <div v-else
                                        class="px-3 py-1 bg-gradient-to-r from-gray-400 to-gray-500 text-white text-xs rounded-full shadow-sm">
                                        离线</div>
                                </div>
                            </div>
                            <div class="flex items-center justify-center mb-10">
                                <div class="relative">
                                    <div class="w-24 h-24 rounded-3xl flex items-center justify-center cursor-pointer shadow-lg transition-all duration-300 transform hover:scale-105"
                                        :style="workStatus.isOnline 
                                            ? 'background: linear-gradient(to bottom right, #f97316, #fb923c);' 
                                            : 'background: linear-gradient(to bottom right, #9ca3af, #6b7280);'"
                                        @click="toggleWorkStatus">
                                        <el-icon class="text-white text-3xl">
                                            <Switch />
                                        </el-icon>
                                    </div>
                                    <div class="absolute -bottom-8 left-1/2 transform -translate-x-1/2 text-base font-medium"
                                        :style="workStatus.isOnline ? 'color: #f97316;' : 'color: #6b7280;'">
                                        {{ workStatus.isOnline ? '开工中' : '已收工' }}
                                    </div>
                                </div>
                            </div>
                            <div class="grid grid-cols-3 gap-4 text-center">
                                <div class="bg-orange-50 rounded-2xl p-3">
                                    <div class="text-xl font-semibold text-orange-500 mb-1">
                                        {{ pendingOrderCount }}
                                    </div>
                                    <div class="text-xs text-gray-500">待取单</div>
                                </div>
                                <div class="bg-blue-50 rounded-2xl p-3">
                                    <div class="text-xl font-semibold text-blue-500 mb-1">
                                        {{ deliveringOrderCount }}
                                    </div>
                                    <div class="text-xs text-gray-500">配送中</div>
                                </div>
                                <div class="bg-green-50 rounded-2xl p-3">
                                    <div class="text-xl font-semibold text-green-500 mb-1">
                                        {{ completedOrderCount }}
                                    </div>
                                    <div class="text-xs text-gray-500">已送达</div>
                                </div>
                            </div>
                        </div>

                        <!-- 地图卡片 -->
                        <!-- <div v-if="locationInfo" class="mx-4 mt-4 bg-white rounded-lg shadow-sm overflow-hidden">
                            <div class="h-64 relative">
                                <img src="https://readdy.ai/api/search-image?query=Urban%20delivery%20map%20interface%20showing%20rider%20location%20with%20orange%20markers%20and%20navigation%20routes%2C%20clean%20modern%20GPS%20interface%20design%2C%20realistic%20mobile%20map%20view%20with%20clear%20street%20layout%2C%20professional%20delivery%20app%20aesthetic%2C%20bright%20daylight%20view&width=343&height=256&seq=map001&orientation=landscape"
                                    alt="配送地图" class="w-full h-full object-cover" />
                                <div class="absolute top-4 left-4 bg-white rounded-lg px-3 py-2 shadow-sm">
                                    <div class="text-xs text-gray-500">当前位置</div>
                                    <div class="text-sm font-medium text-gray-900">{{ locationInfo.area }}</div>
                                </div>
                                <div
                                    class="absolute bottom-4 right-4 bg-orange-500 w-10 h-10 rounded-full flex items-center justify-center cursor-pointer !rounded-button">
                                    <el-icon class="text-white">
                                        <Location />
                                    </el-icon>
                                </div>
                            </div>
                        </div> -->
                        <CourierLocationMap />

                    </div>

                    <!-- 可接订单页面 -->
                    <div v-if="currentTab === 'available'" class="mx-4 mt-4 space-y-3">
                        <!-- 有订单时显示列表 -->
                        <div v-if="availableOrders.length > 0">
                            <div v-for="order in availableOrders" :key="order.id"
                                class="bg-white border rounded-lg p-3 shadow-sm space-y-3">
                                <div class="flex items-center justify-between">
                                    <div class="text-sm font-medium text-gray-900">配送号: {{ order.id }}</div>
                                    <div class="text-sm font-medium text-orange-500">¥{{ getCourierIncome(order.fee) }}</div>
                                </div>
                                <div class="space-y-3">
                                    <div class="flex items-start space-x-3">
                                        <div
                                            class="w-6 h-6 bg-orange-500 rounded-full flex items-center justify-center text-white">
                                            <el-icon>
                                                <Shop />
                                            </el-icon>
                                        </div>
                                        <div class="flex-1">
                                            <div class="font-medium text-gray-900">{{ order.restaurant }}</div>
                                            <div class="text-sm text-gray-500">{{ order.pickupAddress }}</div>
                                        </div>
                                    </div>
                                    <div class="flex items-start space-x-3">
                                        <div
                                            class="w-6 h-6 bg-green-500 rounded-full flex items-center justify-center text-white">
                                            <el-icon>
                                                <User />
                                            </el-icon>
                                        </div>
                                        <div class="flex-1">
                                            <div class="font-medium text-gray-900">{{ order.customer }}</div>
                                            <div class="text-sm text-gray-500">{{ order.deliveryAddress }}</div>
                                            <div v-if="order.publishTime" class="text-xs text-gray-400 text-center mt-1">
                                                发布时间: {{ order.publishTime }}
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="grid grid-cols-2 gap-2 text-center text-xs text-gray-500">
                                    <div class="bg-orange-50 rounded-lg p-2">
                                        <div class="text-orange-500 font-medium text-sm">{{ order.distance }}km</div>
                                        <div>配送距离</div>
                                    </div>
                                    <div class="bg-blue-50 rounded-lg p-2">
                                        <div class="text-blue-500 font-medium text-sm">{{ order.time }}分钟</div>
                                        <div>预计送达</div>
                                    </div>
                                </div>
                                <button @click="acceptAvailableOrder(order)"
                                    class="w-full bg-orange-500 text-white py-2 rounded-lg font-medium hover:bg-orange-600 transition-all">
                                    接单
                                </button>
                            </div>
                        </div>
                        <!-- 没有订单时显示提示 -->
                        <div v-else class="text-center text-gray-400 py-24">
                            <el-icon class="text-5xl mb-2">
                                <Bell />
                            </el-icon>
                            <p>当前没有可接的订单</p>
                        </div>

                        <!-- ▼▼▼ 新增的悬浮刷新按钮 ▼▼▼ -->
                        <button @click="refreshAvailableOrders"
                            class="fixed bottom-24 right-5 w-14 h-14 bg-orange-500 text-white rounded-full shadow-lg flex items-center justify-center transition-transform transform hover:scale-110">
                            <el-icon class="text-2xl" :class="{ 'is-loading': isRefreshing }">
                                <Refresh />
                            </el-icon>
                        </button>
                    </div>

                    <!-- 订单列表页面 -->
                    <div v-if="currentTab === 'orders'" class="mx-4 mt-4 order-list-container">
                        <div class="bg-white rounded-lg shadow-sm">
                            <div class="flex border-b">
                                <div v-for="tab in orderTabs" :key="tab.key"
                                    class="flex-1 py-3 text-center cursor-pointer"
                                    :class="{ 'text-orange-500 border-b-2 border-orange-500': activeOrderTab === tab.key }"
                                    @click="activeOrderTab = tab.key">{{ tab.label }}</div>
                            </div>
                            <div class="p-4 space-y-3">
                                <div v-if="filteredOrders.length === 0" class="text-center text-gray-400 py-12">
                                    <el-icon class="text-4xl mb-2">
                                        <DocumentCopy />
                                    </el-icon>
                                    <p>当前分类下没有订单哦</p>
                                </div>
                                <div v-else v-for="order in filteredOrders" :key="order.id"
                                    class="bg-white border-2 rounded-xl p-4 mb-4 shadow-md hover:shadow-lg transition-shadow"
                                    :class="{
                                        'border-orange-300': order.status === 'pending',
                                        'border-blue-300': order.status === 'delivering',
                                        'border-green-300': order.status === 'completed'
                                    }">
                                    <div class="flex items-center justify-between mb-3 pb-3 border-b border-gray-200">
                                        <div class="flex items-center space-x-2">
                                            <div class="text-base font-bold text-gray-900">配送号: {{ order.id }}</div>
                                            <div class="text-xs px-2 py-1 rounded-full font-medium"
                                                :class="getOrderStatusClass(order.status)">
                                                {{ getOrderStatusText(order.status) }}
                                            </div>
                                        </div>
                                        <div v-if="order.status === 'completed' && order.completionTime" 
                                            class="text-xs text-gray-500">
                                            送达时间: {{ order.completionTime }}
                                        </div>
                                    </div>
                                    <div class="space-y-3 mb-3">
                                        <div class="flex items-start space-x-3 p-2 bg-orange-50 rounded-lg">
                                            <div
                                                class="mt-1 flex-shrink-0 w-6 h-6 flex items-center justify-center bg-orange-500 rounded-full text-white shadow-sm">
                                                <el-icon :size="14">
                                                    <Shop />
                                                </el-icon>
                                            </div>
                                            <div class="flex-1">
                                                <div class="font-semibold text-sm text-gray-900 mb-1">{{ order.restaurant }}
                                                </div>
                                                <div class="text-xs text-gray-600 mb-1">{{ order.pickupAddress }}</div>
                                                <a v-if="order.restaurantPhone" 
                                                    :href="`tel:${order.restaurantPhone}`"
                                                    class="text-xs text-orange-600 hover:text-orange-700 font-medium flex items-center gap-1">
                                                    <el-icon :size="12">
                                                        <Phone />
                                                    </el-icon>
                                                    {{ order.restaurantPhone }}
                                                </a>
                                            </div>
                                        </div>
                                        <div class="flex items-start space-x-3 p-2 bg-green-50 rounded-lg">
                                            <div
                                                class="mt-1 flex-shrink-0 w-6 h-6 flex items-center justify-center bg-green-500 rounded-full text-white shadow-sm">
                                                <el-icon :size="14">
                                                    <User />
                                                </el-icon>
                                            </div>
                                            <div class="flex-1">
                                                <div class="font-semibold text-sm text-gray-900 mb-1">{{ order.customer }}
                                                </div>
                                                <div class="text-xs text-gray-600 mb-1">{{ order.deliveryAddress }}</div>
                                                <a v-if="order.customerPhone" 
                                                    :href="`tel:${order.customerPhone}`"
                                                    class="text-xs text-green-600 hover:text-green-700 font-medium flex items-center gap-1">
                                                    <el-icon :size="12">
                                                        <Phone />
                                                    </el-icon>
                                                    {{ order.customerPhone }}
                                                </a>
                                            </div>
                                        </div>
                                    </div>

                                    <!-- 菜品列表 -->
                                    <div v-if="order.dishDetails && order.dishDetails.length > 0" class="mt-3 pt-3 border-t">
                                        <div class="text-xs font-medium text-gray-700 mb-2">配送菜品</div>
                                        <div class="overflow-x-auto pb-2">
                                            <div class="flex space-x-3 min-w-max">
                                                <div v-for="(dish, index) in order.dishDetails.slice(0, 3)" 
                                                    :key="index"
                                                    class="flex-shrink-0 flex flex-col items-center"
                                                    style="width: 60px;">
                                                    <img :src="normalizeImageUrl(dish.dishImage)"
                                                        :alt="dish.dishName"
                                                        class="w-12 h-12 rounded-lg object-cover mb-1"
                                                        @error="handleImageError" />
                                                    <div class="w-12 text-center">
                                                        <div class="text-xs font-medium text-gray-900 truncate" 
                                                            :title="dish.dishName">
                                                            {{ dish.dishName }}
                                                        </div>
                                                        <div class="text-xs text-gray-500 truncate" 
                                                            :title="`x${dish.quantity}`">
                                                            x{{ dish.quantity }}
                                                        </div>
                                                    </div>
                                                </div>
                                                <div v-if="order.dishDetails.length > 3"
                                                    class="flex-shrink-0 flex flex-col items-center justify-center"
                                                    style="width: 60px;">
                                                    <div class="w-12 h-12 rounded-lg bg-gray-100 flex items-center justify-center mb-1">
                                                        <span class="text-xs text-gray-400">+{{ order.dishDetails.length - 3 }}</span>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="mt-2 text-center">
                                            <button @click="openDishDetailDialog(order)"
                                                class="text-xs text-orange-500 hover:text-orange-600 font-medium">
                                                查看详细信息
                                            </button>
                                        </div>
                                    </div>

                                    <!-- 订单备注 -->
                                    <div v-if="order.remarks" class="mt-3 pt-3 border-t border-gray-200">
                                        <div class="text-xs font-medium text-gray-700 mb-1">订单备注</div>
                                        <div class="text-sm text-gray-600 bg-gray-50 rounded-lg p-2">
                                            {{ order.remarks }}
                                        </div>
                                    </div>
                                    
                                    <!-- 底部操作区 -->
                                    <div class="pt-4 border-t border-gray-200 mt-4">
                                        <!-- 待取单状态 (pending) 的按钮 -->
                                        <template v-if="order.status === 'pending'">
                                            <button 
                                                @click="handlePickupOrder(order.id)"
                                                :disabled="!order.isReadyForPickup"
                                                type="button"
                                                class="w-full py-3 px-4 rounded-lg font-medium text-white transition-all"
                                                :class="order.isReadyForPickup 
                                                    ? 'bg-orange-500 hover:bg-orange-600 active:bg-orange-700' 
                                                    : 'bg-gray-400 cursor-not-allowed'">
                                                <div class="flex items-center justify-center gap-2">
                                                    <el-icon v-if="order.isReadyForPickup">
                                                        <MostlyCloudy />
                                                    </el-icon>
                                                    <el-icon v-else>
                                                        <Timer />
                                                    </el-icon>
                                                    <span>{{ order.isReadyForPickup ? '我已取餐' : '等待商家出餐' }}</span>
                                                </div>
                                            </button>
                                            <p v-if="!order.isReadyForPickup" class="text-xs text-center text-gray-500 mt-2">
                                                商家正在努力备餐中，请稍候...
                                            </p>
                                        </template>

                                        <!-- 配送中状态 (delivering) 的按钮 -->
                                        <template v-else-if="order.status === 'delivering'">
                                            <button 
                                                @click="handleDeliverOrder(order.id)"
                                                type="button"
                                                class="w-full py-3 px-4 rounded-lg font-medium text-white transition-all bg-green-500 hover:bg-green-600 active:bg-green-700">
                                                <div class="flex items-center justify-center gap-2">
                                                    <el-icon>
                                                        <Position />
                                                    </el-icon>
                                                    <span>我已送达</span>
                                                </div>
                                            </button>
                                        </template>

                                        <!-- 费用信息，移到了按钮下方，作为补充信息 -->
                                        <div class="text-center text-xs text-gray-400 mt-2">
                                            配送费: <span class="font-semibold text-gray-600">¥{{ getCourierIncome(order.fee) }}</span>
                                        </div>
                                    </div>
                                    
                                    <!-- 地图区域 -->
                                    <div v-if="order.status === 'pending' || order.status === 'delivering'"
                                        class="mt-3">
                                        <div class="relative">
                                            <img :src="'https://readdy.ai/api/search-image?query=Simple%20delivery%20route%20map%20showing%20pickup%20and%20delivery%20locations%20with%20orange%20markers%2C%20clean%20interface%20design%2C%20mobile%20app%20style%20map%20view%2C%20clear%20navigation%20paths%2C%20professional%20delivery%20service%20aesthetic&width=280&height=160&seq=' + order.id + '&orientation=landscape'"
                                                alt="导航地图" class="w-full h-32 object-cover rounded-lg mb-2" />
                                            <button @click="showNavigation(order)"
                                                class="absolute bottom-3 right-3 bg-orange-500 text-white px-4 py-2 rounded-lg text-sm font-medium shadow-lg !rounded-button flex items-center space-x-2">
                                                <el-icon>
                                                    <Location />
                                                </el-icon>
                                                <span>导航</span>
                                            </button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <!-- 投诉页面 -->
                    <div v-if="currentTab === 'complaints'" class="mx-4 mt-4">
                        <!-- 顶部统计 -->
                        <div class="bg-white rounded-lg shadow-sm p-4 mb-4">
                            <div class="text-center">
                                <div class="text-2xl font-bold text-gray-900">
                                    {{ complaints.length }}
                                </div>
                                <div class="text-sm text-gray-500">总投诉数</div>
                            </div>
                        </div>

                        <!-- 投诉记录列表 -->
                        <div class="bg-white rounded-lg shadow-sm">
                            <!-- 列表内容 -->
                            <div class="p-4 space-y-4">
                                <!-- 显示所有投诉 -->
                                <div v-for="complaint in complaints" :key="complaint.complaintID"
                                    class="bg-white border border-gray-200 rounded-xl overflow-hidden shadow-sm hover:shadow-md transition-all">
                                    <!-- 头部：投诉编号和状态 -->
                                    <div class="bg-gradient-to-r from-red-50 to-orange-50 px-4 py-3 border-b border-gray-200">
                                        <div class="flex items-center justify-between">
                                            <div class="flex items-center gap-3">
                                                <div class="w-8 h-8 bg-red-500 rounded-full flex items-center justify-center text-white text-xs font-bold">
                                                    <i class="fas fa-exclamation"></i>
                                                </div>
                                                <div>
                                                    <div class="text-sm font-semibold text-gray-900 flex items-center gap-2">
                                                        <span class="text-xs text-gray-500 bg-white px-2 py-0.5 rounded">投诉编号</span>
                                                        <span class="font-mono">{{ complaint.complaintID }}</span>
                                                    </div>
                                                    <p class="text-xs text-gray-500 mt-0.5">{{ complaint.complaintTime }}</p>
                                                </div>
                                            </div>
                                            <span v-if="complaint.processingResult" class="px-2.5 py-1 bg-red-500 text-white text-xs rounded-full font-medium">
                                                已处罚
                                            </span>
                                            <span v-else class="px-2.5 py-1 bg-gray-100 text-gray-600 text-xs rounded-full">
                                                待处理
                                            </span>
                                </div>
                            </div>

                                    <!-- 配送信息 -->
                                    <div class="px-4 py-3 bg-blue-50/30">
                                        <h4 class="text-xs font-semibold text-gray-500 mb-3 flex items-center justify-center gap-1">
                                            <i class="fas fa-shipping-fast text-blue-500"></i> 配送任务信息
                                        </h4>
                                        <div class="space-y-2.5 text-xs">
                                            <div class="flex items-center justify-center gap-2">
                                                <span class="text-gray-400 shrink-0">任务编号</span>
                                                <span class="text-gray-900 font-mono bg-white px-2 py-0.5 rounded border border-gray-200">{{ complaint.deliveryTaskID }}</span>
                                        </div>
                                            <div class="text-center">
                                                <p class="text-gray-400 mb-1">取餐地址</p>
                                                <p class="text-gray-700 font-medium">{{ complaint.pickupAddress || '地址信息暂缺' }}</p>
                                        </div>
                                            <div class="text-center">
                                                <p class="text-gray-400 mb-1">送货地址</p>
                                                <p class="text-gray-700 font-medium">{{ complaint.deliveryAddress || '地址信息暂缺' }}</p>
                                    </div>
                                            <div class="flex items-center justify-center gap-2">
                                                <span class="text-gray-400 shrink-0">接单时间</span>
                                                <span class="text-gray-700 font-mono">{{ complaint.acceptTime || '-' }}</span>
                                    </div>
                                            <div class="flex items-center justify-center gap-2">
                                                <span class="text-gray-400 shrink-0">到店时间</span>
                                                <span class="text-gray-700 font-mono">{{ complaint.pickupTime || '-' }}</span>
                                        </div>
                                            <div class="flex items-center justify-center gap-2">
                                                <span class="text-gray-400 shrink-0">送达时间</span>
                                                <span class="text-gray-700 font-mono">{{ complaint.completionTime || '-' }}</span>
                                    </div>
                                        </div>
                                        </div>

                                    <!-- 投诉原因 -->
                                    <div class="px-4 py-3">
                                        <h4 class="text-xs font-semibold text-gray-500 mb-2 flex items-center gap-1">
                                            <i class="fas fa-comment-dots text-red-500"></i> 投诉原因
                                        </h4>
                                        <div class="bg-red-50 rounded-lg p-3 border border-red-100">
                                            <p class="text-sm text-gray-800 leading-relaxed">
                                            {{ complaint.complaintReason }}
                                            </p>
                                        </div>
                                    </div>

                                    <!-- 处罚措施 -->
                                    <div v-if="complaint.processingResult" class="px-4 py-3 bg-orange-50/50 border-t border-gray-200">
                                        <h4 class="text-xs font-semibold text-orange-700 mb-2 flex items-center gap-1">
                                            <i class="fas fa-gavel"></i> 平台处罚
                                        </h4>
                                        <div class="bg-white rounded-lg p-3 border border-orange-200">
                                            <p class="text-sm text-gray-800 font-medium">
                                                {{ complaint.processingResult }}
                                            </p>
                                        </div>
                                    </div>
                                    <div v-else class="px-4 py-3 bg-gray-50 border-t border-gray-200 text-center">
                                        <p class="text-xs text-gray-400">
                                            <i class="fas fa-hourglass-half mr-1"></i>
                                            等待平台处理中...
                                        </p>
                                    </div>
                                </div>

                                <!-- 空状态提示 -->
                                <div v-if="complaints.length === 0" class="text-center text-gray-400 py-12">
                                    <el-icon class="text-4xl mb-2">
                                        <DocumentCopy />
                                    </el-icon>
                                    <p>暂无投诉记录</p>
                                </div>

                            </div>
                        </div>
                    </div>


                    <!-- 个人中心页面 -->
                    <div v-if="currentTab === 'profile' && userProfile" class="mx-4 mt-4">
                        <!-- 个人资料卡片 (仅更新头像显示) -->
                        <div class="bg-white rounded-lg shadow-sm p-4 mb-4">
                            <div class="flex items-center space-x-4 mb-4">

                                <!-- ▼▼▼ 这是唯一需要修改的部分 ▼▼▼ -->
                                <div
                                    class="w-16 h-16 rounded-full flex items-center justify-center bg-gray-200 overflow-hidden">
                                    <!-- 如果 userProfile.avatar 存在 (是一个有效的URL)，就显示图片 -->
                                    <img v-if="userProfile.avatar" :src="normalizeImageUrl(userProfile.avatar)" alt="用户头像"
                                        class="w-full h-full object-cover" @error="(e) => handleImageError(e)" />
                                    <!-- 否则，显示一个默认的 Element Plus 用户图标 -->
                                    <el-icon v-else class="text-gray-500 text-3xl">
                                        <User />
                                    </el-icon>
                                </div>
                                <!-- ▲▲▲ 修改结束 ▲▲▲ -->

                                <!-- 其他部分保持完全不变 -->
                                <div>
                                    <div class="text-xl font-semibold text-gray-900">{{ userProfile.name }}</div>
                                    <div v-if="userProfile.fullName && userProfile.gender" class="text-sm text-gray-500 mt-0.5">
                                        {{ getHonorificName(userProfile) }}
                                    </div>
                                    <div class="text-xs text-gray-400 mt-1">注册时间: {{ userProfile.registerDate }}</div>
                                </div>
                            </div>
                            <div class="grid grid-cols-3 gap-4">
                                <div class="bg-gray-50 rounded-lg p-3 text-center">
                                    <div class="text-lg font-semibold text-gray-900">
                                        <span v-if="userProfile.rating && userProfile.rating > 0">{{ userProfile.rating.toFixed(1) }}</span>
                                        <span v-else class="text-sm font-normal text-gray-500 whitespace-nowrap">暂未获得评价</span>
                                    </div>
                                    <div class="text-xs text-gray-500">获评均分</div>
                                </div>
                                <div class="bg-gray-50 rounded-lg p-3 text-center">
                                    <div class="text-lg font-semibold"
                                        :class="{
                                            'text-green-600': userProfile.creditScore >= 80 && userProfile.creditScore <= 100,
                                            'text-yellow-600': userProfile.creditScore >= 60 && userProfile.creditScore < 80,
                                            'text-red-600': userProfile.creditScore < 60
                                        }">
                                        {{ userProfile.creditScore }}
                                    </div>
                                    <div class="text-xs text-gray-500">信誉积分</div>
                                </div>
                                <div class="bg-gray-50 rounded-lg p-3 text-center">
                                    <div class="text-lg font-semibold text-gray-900">¥{{ income.toFixed(2) }}</div>
                                    <div class="text-xs text-gray-500">本月收入</div>
                                </div>
                            </div>
                        </div>
                        <!-- 设置菜单 -->
                        <div class="bg-white rounded-lg shadow-sm">
                            <div class="p-4 space-y-1 divide-y divide-gray-100">
                                <!-- 账号与资料设置 -->
                                <router-link :to="{ name: 'AccountSettings' }"
                                    class="flex items-center justify-between cursor-pointer py-3 no-underline text-gray-900">
                                    <div class="flex items-center space-x-3">
                                        <el-icon class="text-gray-400">
                                            <Edit />
                                        </el-icon>
                                        <span>账号与资料设置</span>
                                    </div>
                                    <el-icon class="text-gray-400">
                                        <ArrowRight />
                                    </el-icon>
                                </router-link>

                                <!-- 投诉与处罚菜单项 -->
                                <div @click="currentTab = 'complaints'"
                                    class="flex items-center justify-between cursor-pointer py-3 text-gray-900">
                                    <div class="flex items-center space-x-3">
                                        <el-icon class="text-gray-400">
                                            <Warning />
                                        </el-icon>
                                        <span>投诉与处罚</span>
                                    </div>
                                    <el-icon class="text-gray-400">
                                        <ArrowRight />
                                    </el-icon>
                                </div>

                                <div @click="handleLogout"
                                    class="flex items-center justify-between cursor-pointer py-3 text-red-500">
                                    <div class="flex items-center space-x-3">
                                        <el-icon class="text-red-400">
                                            <SwitchButton />
                                        </el-icon>
                                        <span class="font-semibold">退出登录</span>
                                    </div>
                                    <el-icon class="text-red-400">
                                        <ArrowRight />
                                    </el-icon>
                                </div>


                            </div>
                        </div>
                    </div>



                    <!-- 底部导航栏 -->
                    <div
                        class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-200 flex justify-around py-2">
                        <div v-for="tab in tabs" :key="tab.key"
                            class="flex flex-col items-center justify-center py-2 cursor-pointer w-full"
                            :class="{ 'text-orange-500': currentTab === tab.key }" @click="currentTab = tab.key">
                            <el-icon class="text-xl mb-1">
                                <component :is="tab.icon" />
                            </el-icon>
                            <span class="text-xs">{{ tab.label }}</span>
                        </div>
                    </div>
                </div>
                <!-- 菜品详情弹窗 -->
                <div v-if="showDishDetailDialog"
                    class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50"
                    @click.self="closeDishDetailDialog">
                    <div class="bg-white rounded-2xl w-[90%] max-w-md max-h-[80vh] flex flex-col overflow-hidden shadow-2xl">
                        <div class="flex items-center justify-between p-4 border-b border-gray-200 bg-gradient-to-r from-orange-50 to-yellow-50">
                            <div>
                                <div class="text-lg font-bold text-gray-900">菜品详情</div>
                                <div class="text-sm text-orange-600 font-medium">配送号: {{ selectedDishOrder?.id }}</div>
                            </div>
                            <button @click="closeDishDetailDialog" class="text-gray-400 hover:text-gray-600">
                                <el-icon class="text-xl">
                                    <Close />
                                </el-icon>
                            </button>
                        </div>

                        <div class="flex-1 p-4 overflow-y-auto">
                            <div v-if="selectedDishOrder?.dishDetails && selectedDishOrder.dishDetails.length > 0" class="space-y-3">
                                <div v-for="(dish, index) in selectedDishOrder.dishDetails" :key="index"
                                    class="flex items-center space-x-3 p-3 bg-gray-50 rounded-lg">
                                    <img :src="normalizeImageUrl(dish.dishImage)"
                                        :alt="dish.dishName"
                                        class="w-16 h-16 rounded-lg object-cover flex-shrink-0"
                                        @error="handleImageError" />
                                    <div class="flex-1 min-w-0">
                                        <div class="text-sm font-medium text-gray-900 mb-1">
                                            {{ dish.dishName }}
                                        </div>
                                        <div class="text-xs text-gray-500">
                                            数量: x{{ dish.quantity }}
                                        </div>
                                    </div>
                                </div>
                                <div class="mt-4 pt-3 border-t border-gray-200 text-center text-sm text-gray-600">
                                    共 {{ selectedDishOrder.dishDetails.length }} 种菜品
                                </div>
                            </div>
                            <div v-else class="text-center py-8 text-gray-400">
                                <div class="text-lg mb-2">暂无菜品信息</div>
                            </div>
                        </div>

                        <div class="p-4 border-t border-gray-200 flex justify-end">
                            <button @click="closeDishDetailDialog"
                                class="px-4 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors">
                                关闭
                            </button>
                        </div>
                    </div>
                </div>

                <!-- 导航弹窗 -->
                <div v-if="showNavigationModal"
                    class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
                    <div class="bg-white rounded-lg w-full max-w-sm">
                        <div class="p-4 border-b flex items-center justify-between">
                            <div class="text-lg font-semibold text-gray-900">
                                导航路线
                            </div>
                            <el-icon class="text-gray-400 cursor-pointer" @click="closeNavigation">
                                <Close />
                            </el-icon>
                        </div>
                        <div class="p-4">
                            <img :src="'https://readdy.ai/api/search-image?query=Detailed%20navigation%20route%20map%20with%20turn%20by%20turn%20directions%2C%20showing%20current%20location%20and%20destination%20with%20clear%20path%20markers%2C%20real%20time%20traffic%20information%2C%20estimated%20arrival%20time%20display%2C%20professional%20navigation%20interface&width=280&height=400&seq=nav001&orientation=portrait'"
                                alt="导航路线" class="w-full h-64 object-cover rounded-lg mb-4" />
                            <div class="space-y-3 mb-4">
                                <div class="flex items-center space-x-2">
                                    <div class="w-2 h-2 bg-orange-500 rounded-full"></div>
                                    <!-- 数据绑定到 selectedOrder -->
                                    <div class="text-sm text-gray-900">
                                        {{ selectedOrder?.restaurant }}
                                    </div>
                                </div>
                                <div class="flex items-center space-x-2">
                                    <div class="w-2 h-2 bg-green-500 rounded-full"></div>
                                    <!-- 数据绑定到 selectedOrder -->
                                    <div class="text-sm text-gray-900">
                                        {{ selectedOrder?.deliveryAddress }}
                                    </div>
                                </div>
                                <div class="text-sm text-gray-500">
                                    预计送达时间：15分钟
                                </div>
                            </div>
                            <button class="w-full bg-orange-500 text-white py-3 rounded-lg font-medium !rounded-button"
                                @click="startNavigation">
                                开始导航
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script lang="ts" setup>
import { ref, computed, onMounted, watch } from 'vue';
import CourierLocationMap from '@/components/courier/CourierLocationMap.vue';
import { ElMessage, ElLoading ,ElMessageBox} from 'element-plus';
import {
    // 您已有的图标
    User, Bell, Switch, Location, CircleCloseFilled,
    HomeFilled, DocumentCopy, Coin, UserFilled, Close, Shop, List, Refresh, Warning, Edit,

    ArrowRight,
    SwitchButton,
    MostlyCloudy,
    Timer,
    Position,
    Phone
} from '@element-plus/icons-vue';
import { useRouter } from 'vue-router';
import loginApi from '@/api/login_api';      // 导入我们定义好的通用认证API
import { removeToken } from '@/utils/jwt'; 
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';
import { API_CONFIG } from '@/config';
// ===================================================================
//  API导入
// ===================================================================
import * as RealAPI from '@/api/rider_api';

const api = RealAPI;

const router = useRouter();

// --- 接口定义 ---
export interface UserProfile {
    name: string;
    fullName?: string; // 真实姓名
    id: string;
    registerDate: string;
    rating: number;
    creditScore: number;

    gender?: string;
    birthday?: string; // 通常是 ISO 格式的日期字符串，如 '2024-01-15T00:00:00'
    avatar?: string;   // 头像的 URL
    vehicleType?: string;
    // -----------------------
}

// 计算显示名称：用户姓名 + 姓氏+性别（例如"陈先生"）
const getDisplayName = (profile: UserProfile): string => {
    let displayName = profile.name || '';
    
    // 如果有真实姓名，提取姓氏并添加性别称谓
    if (profile.fullName && profile.fullName.length > 0) {
        const surname = profile.fullName.charAt(0); // 取第一个字符作为姓氏
        const gender = profile.gender || '';
        
        // 根据性别添加称谓
        let honorific = '';
        if (gender === '男') {
            honorific = '先生';
        } else if (gender === '女') {
            honorific = '女士';
        } else {
            honorific = ''; // 保密或未知性别不显示称谓
        }
        
        if (honorific) {
            displayName = `${displayName} ${surname}${honorific}`;
        }
    }
    
    return displayName;
};

// 获取姓氏+性别称谓（例如"陈先生"）
const getHonorificName = (profile: UserProfile): string => {
    if (!profile.fullName || profile.fullName.length === 0) {
        return '';
    }
    
    const surname = profile.fullName.charAt(0); // 取第一个字符作为姓氏
    const gender = profile.gender || '';
    
    // 根据性别添加称谓
    if (gender === '男') {
        return `${surname}先生`;
    } else if (gender === '女') {
        return `${surname}女士`;
    } else {
        return ''; // 保密或未知性别不显示称谓
    }
};
interface OrderDish {
    dishName: string;
    dishImage: string;
    quantity: number;
}

interface Order {
    id: string;
    status: OrderStatus; // 使用我们更精确的类型
    restaurant: string;
    pickupAddress: string;   // 取餐地址
    deliveryAddress: string; // 送达地址
    customer: string;        // 顾客姓名
    fee: string;
    distance: string;        // 配送距离
    time: string;            // 预计时间
    publishTime?: string;    // 发布时间
    completionTime?: string; // 完成时间（仅已完成订单）
    customerPhone?: string;  // 客户电话
    restaurantPhone?: string; // 商家电话
    isReadyForPickup: boolean;
    remarks?: string;        // 订单备注
    dishDetails?: OrderDish[]; // 菜品列表
}

interface Complaint {
    complaintID: string;
    deliveryTaskID: string;
    complaintTime: string;
    complaintReason: string;
    processingResult?: string;
    // 配送信息
    deliveryAddress?: string;
    acceptTime?: string;
    pickupTime?: string;
    completionTime?: string;
    // 店铺信息
    pickupAddress?: string;
}

// --- 状态定义 ---
type OrderStatus = 'to_be_taken' | 'pending' | 'delivering' | 'completed';
const userProfile = ref<UserProfile | null>(null);
const locationInfo = ref<any | null>(null);
const orders = ref<Order[]>([]);
const income = ref<number>(0);
const workStatus = ref<{ isOnline: boolean } | null>(null);

const isLoading = ref(true);
const errorState = ref<string | null>(null);

const currentTab = ref('home');
const activeOrderTab = ref<OrderStatus>('pending');
// 在 ref 定义区域添加
const availableOrders = ref<Order[]>([]);

const complaints = ref<Complaint[]>([]);



const pendingOrderCount = ref(0);
const deliveringOrderCount = ref(0);
const completedOrderCount = ref(0);

const showNavigationModal = ref(false);
const selectedOrder = ref<Order | null>(null); // Order 是您已有的订单接口类型

const isRefreshing = ref(false); // 用于控制刷新按钮的加载状态

// 菜品详情弹窗
const showDishDetailDialog = ref(false);
const selectedDishOrder = ref<Order | null>(null);

// --- 静态数据 ---
const tabs = [
    { key: 'home', label: '工作台', icon: HomeFilled },
    { key: 'available', label: '可接订单', icon: List },
    { key: 'orders', label: '订单', icon: DocumentCopy },
    { key: 'complaints', label: '投诉', icon: Warning },
    { key: 'profile', label: '我的', icon: UserFilled }
];
const orderTabs = [
    { key: 'pending', label: '待取单' },
    { key: 'delivering', label: '配送中' },
    { key: 'completed', label: '已送达' }
] as const;

// --- API 调用逻辑 ---


const todayIncome = computed(() => {
    // 直接使用后端返回的今日收入
    return income.value || 0;
});

// 计算骑手实际收入：配送费 + 5元
const getCourierIncome = (fee: string | number): string => {
    const feeNum = typeof fee === 'string' ? parseFloat(fee) || 0 : fee;
    const totalIncome = feeNum + 5;
    return totalIncome.toFixed(2);
};

/** 刷新当前标签页的订单列表 */
const refreshOrderList = async () => {
    const loadingInstance = ElLoading.service({ target: '.order-list-container', text: '刷新中...' });
    try {
        const res = await api.fetchOrders(activeOrderTab.value) as any;
        // 后端返回 camelCase 格式：{ success, code, message, data }
        const rawOrders = res.data?.data ?? [];
        // 映射后端数据到前端期望的格式（后端已统一返回 camelCase）
        // 确保 status 正确映射，后端返回的是小写字符串
        orders.value = rawOrders.map((order: any) => {
            // 后端返回的 status 可能是 "delivering"，需要确保匹配
            let orderStatus: OrderStatus = activeOrderTab.value;
            if (order.status) {
                const statusLower = order.status.toLowerCase();
                if (statusLower === 'pending' || statusLower === 'delivering' || statusLower === 'completed') {
                    orderStatus = statusLower as OrderStatus;
                }
            }
            return {
                id: order.id || '',
                status: orderStatus,
                restaurant: order.restaurant || '未知商家',
                pickupAddress: order.pickupAddress || order.address || '地址未提供',
                deliveryAddress: order.deliveryAddress || order.address || '地址未提供',
                customer: order.customer || '未知客户',
                fee: order.fee || '0.00',
                distance: order.distance || '0',
                time: order.time || '',
                completionTime: order.completionTime || undefined,
                customerPhone: order.customerPhone || undefined,
                restaurantPhone: order.restaurantPhone || undefined,
                isReadyForPickup: order.isReadyForPickup ?? false,
                remarks: order.remarks || '',
                dishDetails: order.dishDetails || []
            };
        });
    } catch (error) {
        ElMessage.error("订单列表刷新失败");
    } finally {
        loadingInstance.close();
    }
};




/** 处理"取单"操作 */
const handlePickupOrder = async (orderId: string) => {
    try {
        await api.pickupOrderAPI(orderId);
        ElMessage.success('取单成功！订单已移至“配送中”');
        await refreshOrderList(); // 操作成功后刷新列表
    } catch (error) {
        ElMessage.error("取单操作失败，请重试");
    }
};

/** 处理“已送达”操作 */
const handleDeliverOrder = async (orderId: string) => {
    try {
        await api.deliverOrderAPI(orderId);
        ElMessage.success('操作成功！订单已完成');
        
        // 刷新订单列表和今日收入
        await Promise.all([
            refreshOrderList(),
            refreshIncome()
        ]);
    } catch (error) {
        ElMessage.error("确认送达操作失败，请重试");
    }
};

// 刷新今日收入
const refreshIncome = async () => {
    try {
        const incomeRes = await api.fetchIncomeData() as any;
        const incomeData = incomeRes.data?.data;
        income.value = typeof incomeData === 'number' ? incomeData : (parseFloat(String(incomeData)) || 0);
    } catch (error) {
        console.error('刷新收入失败:', error);
    }
};


const refreshAvailableOrders = async () => {
    // 如果正在刷新，则阻止重复点击
    if (isRefreshing.value) return;

    isRefreshing.value = true;
    ElMessage.info('正在刷新订单...'); // 给出提示

    try {
        // 专门只调用获取可接订单的 API
        const res = await (api as any).fetchAvailableOrders() as any; 
        // 后端返回 camelCase 格式：{ success, code, message, data }
        availableOrders.value = res.data?.data ?? [];
        ElMessage.success('订单列表已更新！');
    } catch (error) {
        console.error("刷新可接订单失败:", error);
        ElMessage.error('刷新失败，请稍后重试');
    } finally {
        isRefreshing.value = false;
    }
};


const showNavigation = (order: Order) => {
    selectedOrder.value = order;
    showNavigationModal.value = true;
};

const closeNavigation = () => {
    showNavigationModal.value = false;
    selectedOrder.value = null;
};

// 打开菜品详情弹窗
const openDishDetailDialog = (order: Order) => {
    selectedDishOrder.value = order;
    showDishDetailDialog.value = true;
};

// 关闭菜品详情弹窗
const closeDishDetailDialog = () => {
    showDishDetailDialog.value = false;
    selectedDishOrder.value = null;
};


const startNavigation = () => {
    ElMessage.success('正在为您规划路线...');
    closeNavigation();
};

const loadDashboardData = async () => {
    isLoading.value = true;
    errorState.value = null;
    const loadingInstance = ElLoading.service({ fullscreen: true, text: '加载中...' });
    try {
        const [
            profileRes,
            statusRes,
            incomeRes,
            pendingOrdersRes,
            deliveringOrdersRes,
            completedOrdersRes,
            locationRes,
            complaintsRes
        ] = (await Promise.all([
            api.fetchUserProfile(),
            api.fetchWorkStatus(),
            api.fetchIncomeData(),
            api.fetchOrders('pending'),
            api.fetchOrders('delivering'),
            api.fetchOrders('completed'),
            api.fetchLocationInfo(),
            api.fetchComplaints()
        ])) as unknown as [
                { data: { success: boolean; code: number; message: string; data: any } },
                { data: { success: boolean; code: number; message: string; data: boolean } },
                { data: { success: boolean; code: number; message: string; data: number } },
                { data: { success: boolean; code: number; message: string; data: Order[] } },
                { data: { success: boolean; code: number; message: string; data: Order[] } },
                { data: { success: boolean; code: number; message: string; data: Order[] } },
                { data: { success: boolean; code: number; message: string; data: any } },
                { data: { success: boolean; code: number; message: string; data: Complaint[] } }
            ];
        
        // 提取用户资料
        if (profileRes.data?.success && profileRes.data?.data) {
            userProfile.value = profileRes.data.data;
        } else {
            console.warn('获取用户资料失败:', profileRes.data);
            userProfile.value = null;
        }
        
        // 提取工作状态
        workStatus.value = { isOnline: statusRes.data?.data ?? false };
        
        // 提取位置信息
        locationInfo.value = { area: locationRes.data?.data ?? '' };
        
        // 提取收入
        const incomeData = incomeRes.data?.data;
        income.value = typeof incomeData === 'number' ? incomeData : (parseFloat(String(incomeData)) || 0);

        const rawPendingOrders = pendingOrdersRes.data?.data ?? [];
        const rawDeliveringOrders = deliveringOrdersRes.data?.data ?? [];
        const rawCompletedOrders = completedOrdersRes.data?.data ?? [];
        
        const mapOrder = (order: any, status: OrderStatus) => ({
            id: order.id || '',
            status: status,
            restaurant: order.restaurant || '未知商家',
            pickupAddress: order.pickupAddress || order.address || '地址未提供',
            deliveryAddress: order.deliveryAddress || order.address || '地址未提供',
            customer: order.customer || '未知客户',
            fee: order.fee || '0.00',
            distance: order.distance || '0',
            time: order.time || '',
            completionTime: order.completionTime || undefined,
            customerPhone: order.customerPhone || undefined,
            restaurantPhone: order.restaurantPhone || undefined,
            isReadyForPickup: order.isReadyForPickup ?? false,
            remarks: order.remarks || '',
            dishDetails: order.dishDetails || []
        });
        
        const pendingOrders = rawPendingOrders.map((o: any) => mapOrder(o, 'pending'));
        const deliveringOrders = rawDeliveringOrders.map((o: any) => mapOrder(o, 'delivering'));
        const completedOrders = rawCompletedOrders.map((o: any) => mapOrder(o, 'completed'));
        
        pendingOrderCount.value = pendingOrders.length;
        deliveringOrderCount.value = deliveringOrders.length;
        completedOrderCount.value = completedOrders.length;

        // 合并所有状态的订单到 orders.value，这样切换标签时才能正确显示
        orders.value = [...pendingOrders, ...deliveringOrders, ...completedOrders];
        complaints.value = (complaintsRes.data?.data as Complaint[]) ?? [];

    } catch (error) {
        console.error("加载数据失败:", error);
        errorState.value = "数据加载失败，请检查网络或联系管理员。";
        ElMessage.error(errorState.value);
    } finally {
        isLoading.value = false;
        loadingInstance.close();
    }
};



const toggleWorkStatus = async () => {
    if (!workStatus.value) return;

    const newStatus = !workStatus.value.isOnline;

    try {
        await api.toggleWorkStatusAPI(newStatus);
        workStatus.value = {
            ...workStatus.value, // 先用展开运算符(...)复制旧对象的所有属性
            isOnline: newStatus, // 然后用新值覆盖 isOnline 属性
        };

        ElMessage.success(`状态已切换为: ${workStatus.value.isOnline ? '开工' : '已收工'}`);

    } catch (error) {
        console.error("状态切换失败:", error);
        ElMessage.error("状态切换失败，请重试");
    }
};







onMounted(() => {
    loadDashboardData();

    // 启动WebSocket监听器

});



watch(activeOrderTab, async () => {
    await refreshOrderList();
});

watch(currentTab, (newTab) => {
    if (newTab === 'available' && availableOrders.value.length === 0) {
        refreshAvailableOrders();
    }
}, { immediate: true });

// --- 计算属性和工具函数 ---
const filteredOrders = computed(() => {
    if (!orders.value) return [];
    return orders.value.filter(order => order.status === activeOrderTab.value);
});

const getOrderStatusClass = (status: string) => {
    switch (status) {
        case 'pending': return 'bg-orange-100 text-orange-600';
        case 'delivering': return 'bg-blue-100 text-blue-600';
        case 'completed': return 'bg-green-100 text-green-600';
        default: return 'bg-gray-100 text-gray-600';
    }
};

const getOrderStatusText = (status: string) => {
    switch (status) {
        case 'pending': return '待取单';
        case 'delivering': return '配送中';
        case 'completed': return '已送达';
        default: return '未知状态';
    }
};
const acceptAvailableOrder = async (order: Order) => {
    try {
        await api.acceptAvailableOrderAPI(order.id);
        ElMessage.success(`订单 #${order.id} 已接受！将移至"待取单"`);

        availableOrders.value = availableOrders.value.filter(o => o.id !== order.id);

        if (currentTab.value === 'orders' && activeOrderTab.value === 'pending') {
            await refreshOrderList();
        } else {
            const res = await api.fetchOrders('pending') as any;
            const rawOrders = res.data?.data ?? [];
            const pendingOrders = rawOrders.map((o: any) => {
                const statusLower = o.status?.toLowerCase();
                const orderStatus: OrderStatus = (statusLower === 'pending' || statusLower === 'delivering' || statusLower === 'completed') 
                    ? statusLower as OrderStatus 
                    : 'pending';
                return {
                    id: o.id || '',
                    status: orderStatus,
                    restaurant: o.restaurant || '未知商家',
                    pickupAddress: o.pickupAddress || o.address || '地址未提供',
                    deliveryAddress: o.deliveryAddress || o.address || '地址未提供',
                    customer: o.customer || '未知客户',
                    fee: o.fee || '0.00',
                    distance: o.distance || '0',
                    time: o.time || '',
                    completionTime: o.completionTime || undefined,
                    customerPhone: o.customerPhone || undefined,
                    restaurantPhone: o.restaurantPhone || undefined,
                    isReadyForPickup: o.isReadyForPickup ?? false,
                    remarks: o.remarks || '',
                    dishDetails: o.dishDetails || []
                };
            });
            orders.value = [...pendingOrders, ...orders.value.filter(o => o.status !== 'pending')];
            pendingOrderCount.value = pendingOrders.length;
        }

    } catch (error) {
        console.error("接单失败:", error);
        ElMessage.error("接单失败，可能已被他人抢走，请刷新");
    }
};

async function handleLogout() {
    try {
        await ElMessageBox.confirm(
            '您确定要退出当前账号吗？',
            '退出登录',
            {
                confirmButtonText: '确定退出',
                cancelButtonText: '取消',
                type: 'warning',
            }
        );

        await loginApi.logout();
        removeToken();
        ElMessage.success('您已成功退出登录');
        router.replace('/login');

    } catch (error: any) {
        if (error === 'cancel') { 
            ElMessage.info('已取消退出操作');
        } else {
            console.error('登出时发生错误:', error);
            ElMessage.warning('与服务器通信失败，但已在本地强制退出');
            removeToken();
            router.replace('/login');
        }
    }
}


</script>

<style scoped>
.rounded-button {
    border-radius: 8px;
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

::-webkit-scrollbar {
    width: 4px;
}

::-webkit-scrollbar-track {
    background: #f1f1f1;
}

::-webkit-scrollbar-thumb {
    background: #F9771C;
    border-radius: 2px;
}

::-webkit-scrollbar-thumb:hover {
    background: #e6691a;
}

.order-list-container {
    position: relative;
}

/* 用于ElLoading定位 */
</style>