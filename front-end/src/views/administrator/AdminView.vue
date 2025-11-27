<!-- The exported code uses Tailwind CSS. Install Tailwind CSS in your dev environment to ensure all styles work. -->
<template>
    <div class="min-h-screen bg-gray-50">
        <!-- 顶部导航栏 -->
        <!-- 顶部导航栏 (修改后) -->
        <header class="fixed top-0 left-0 right-0 bg-white border-b border-gray-200 z-50">
            <div class="flex items-center justify-between px-6 h-16">
                <!-- Logo 和标题 (这部分没问题) -->
                <div class="flex items-center space-x-4">
                    <div class="flex items-center space-x-3">
                        <div class="w-8 h-8 bg-orange-500 rounded-lg flex items-center justify-center">
                            <i class="fas fa-utensils text-white text-sm"></i>
                        </div>
                        <h1 class="text-xl font-semibold text-gray-800">{{ projectName }}</h1>
                    </div>
                </div>

                <!-- 用户信息和登出按钮的区块 -->
                <div class="flex items-center space-x-4">
                    <!-- 【修改】用 v-if 包裹用户信息，防止数据加载完成前出错 -->
                    <div v-if="currentUser" class="flex items-center space-x-3">
                        <img :src="'https://s1.aigei.com/src/img/png/f7/f734d8198b614d0a9356196cd83c6758.png?imageMogr2/auto-orient/thumbnail/!282x282r/gravity/Center/crop/282x282/quality/85/%7CimageView2/2/w/282&e=2051020800&token=P7S2Xpzfz11vAkASLTkfHN7Fw-oOZBecqeJaxypL:FldJin-4wd319skieoNSW_v2zAY='"
                             :alt="currentUser.username + '的头像'"
                            class="w-8 h-8 rounded-full object-cover">
                        <span class="text-sm text-gray-700">{{ getDisplayName() }}</span>
                    </div>
                    <!-- 【修改】这是现在唯一的一个登出按钮，并绑定了点击事件 -->
                    <button @click="handleLogout" class="text-gray-500 hover:text-gray-700 cursor-pointer">
                        <i class="fas fa-sign-out-alt text-lg"></i>
                    </button>
                </div>
            </div>
        </header>
        <div class="flex pt-16">
            <!-- 左侧导航菜单 -->
            <aside class="fixed left-0 top-16 bottom-0 w-64 bg-white border-r border-gray-200 overflow-y-auto">
                <nav class="p-4">
                    <ul class="space-y-2">
                        <li>
                            <a href="#"
                                :class="{ 'bg-orange-500 text-white': activeMenu === 'admin', 'text-gray-700 hover:bg-gray-100': activeMenu !== 'admin' }"
                                class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors cursor-pointer"
                                @click="activeMenu = 'admin'">
                                <i class="fas fa-user-shield text-lg"></i>
                                <span>管理员信息</span>
                            </a>
                        </li>
                        <li>
                            <a href="#"
                                :class="{ 'bg-orange-500 text-white': activeMenu === 'afterSales', 'text-gray-700 hover:bg-gray-100': activeMenu !== 'afterSales' }"
                                class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors cursor-pointer"
                                @click.prevent="activeMenu = 'afterSales'">
                                <i class="fas fa-headset text-lg"></i>
                                <span>售后处理中心</span>
                            </a>
                        </li>
                        <li>
                            <a href="#"
                                :class="{ 'bg-orange-500 text-white': activeMenu === 'complaints', 'text-gray-700 hover:bg-gray-100': activeMenu !== 'complaints' }"
                                class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors cursor-pointer"
                                @click.prevent="activeMenu = 'complaints'">
                                <i class="fas fa-exclamation-triangle text-lg"></i>
                                <span>投诉处理中心</span>
                            </a>
                        </li>
                        <li>
                            <a href="#"
                                :class="{ 'bg-orange-500 text-white': activeMenu === 'violations', 'text-gray-700 hover:bg-gray-100': activeMenu !== 'violations' }"
                                class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors cursor-pointer"
                                @click="activeMenu = 'violations'">
                                <i class="fas fa-gavel text-lg"></i>
                                <span>违规举报处理</span>
                            </a>
                        </li>
                        <li>
                            <a href="#"
                                :class="{ 'bg-orange-500 text-white': activeMenu === 'reviews', 'text-gray-700 hover:bg-gray-100': activeMenu !== 'reviews' }"
                                class="flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors cursor-pointer"
                                @click="activeMenu = 'reviews'">
                                <i class="fas fa-comment-dots text-lg"></i>
                                <span>评论审核管理</span>
                            </a>
                        </li>
                    </ul>
                </nav>
            </aside>
            <!-- 右侧内容区域 -->
            <main class="flex-1 ml-64 p-6">
                <!-- 面包屑导航 -->
                <div class="mb-6">
                    <nav class="flex items-center space-x-2 text-sm text-gray-600">
                        <span>首页</span>
                        <i class="fas fa-chevron-right text-xs"></i>
                        <span class="text-orange-500">{{ getBreadcrumb() }}</span>
                    </nav>
                </div>
                <!-- 售后处理中心页面 -->
                <div v-if="activeMenu === 'afterSales'">
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100">
                        <div class="p-6 border-b border-gray-100">
                            <div class="flex items-center justify-between mb-4">
                                <h2 class="text-lg font-semibold text-gray-900">售后处理中心</h2>
                            </div>
                            <!-- 搜索和筛选区域 -->
                            <div class="flex items-center space-x-4 mb-6">
                                <div class="flex-1 max-w-md">
                                    <div class="relative">
                                        <input type="text" placeholder="搜索售后申请编号..."
                                            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm"
                                            v-model="searchQuery">
                                        <i
                                            class="fas fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 text-sm"></i>
                                    </div>
                                </div>
                                <div class="flex space-x-2">
                                    <button v-for="status in afterSalesStatuses" :key="status.value"
                                        :class="{ 'bg-orange-500 text-white': selectedAfterSalesStatus === status.value, 'bg-gray-100 text-gray-700 hover:bg-gray-200': selectedAfterSalesStatus !== status.value }"
                                        class="px-4 py-2 rounded-lg text-sm font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap"
                                        @click="selectedAfterSalesStatus = status.value">
                                        {{ status.label }}
                                    </button>
                                </div>
                            </div>
                        </div>
                        <!-- 售后申请列表 -->
                        <div class="overflow-x-auto">
                            <table class="w-full">
                                <thead class="bg-gray-50">
                                    <tr>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            订单编号</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            申请时间</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            情况说明</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            处理措施</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            状态</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            操作</th>
                                    </tr>
                                </thead>
                                <tbody class="bg-white divide-y divide-gray-200">
                                    <tr v-for="item in filteredAfterSales" :key="item.applicationId"
                                        class="hover:bg-gray-50">
                                        <td class="pl-10 pr-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{ item.orderId }}
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{
                                            item.applicationTime }}</td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{
                                            item.description }}</td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900">{{ item.punishment || '-' }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap">
                                            <span :class="getStatusClass(getAfterSaleStatusText(item.status))"
                                                class="inline-block px-2 py-1 text-xs rounded-full">
                                                {{ getAfterSaleStatusText(item.status) }}
                                            </span>
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm font-medium space-x-2">
                                            <button @click="openAfterSaleDetail(item)"
                                                class="text-orange-600 hover:text-orange-900 cursor-pointer !rounded-button whitespace-nowrap">
                                                <span class="flex items-center">
                                                    <i class="fas fa-eye mr-1"></i>
                                                    查看详情
                                                </span>
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 分页 -->
                        <div class="px-6 py-4 border-t border-gray-200">
                            <div class="flex items-center justify-between">
                                <div class="text-sm text-gray-700">
                                    显示第 1-10 条，共 {{ afterSalesList.length }} 条记录
                                </div>
                                <div class="flex space-x-2">
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">上一页</button>
                                    <button
                                        class="px-3 py-1 bg-orange-500 text-white rounded text-sm cursor-pointer !rounded-button whitespace-nowrap">1</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">2</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">下一页</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 投诉处理中心页面 -->
                <div v-if="activeMenu === 'complaints'">
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100">
                        <div class="p-6 border-b border-gray-100">
                            <div class="flex items-center justify-between mb-4">
                                <h2 class="text-lg font-semibold text-gray-900">投诉处理中心</h2>
                            </div>
                            <!-- 搜索和筛选区域 -->
                            <div class="flex items-center space-x-4 mb-6">
                                <div class="flex-1 max-w-md">
                                    <div class="relative">
                                        <input type="text" placeholder="搜索投诉编号..."
                                            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm"
                                            v-model="complaintSearchQuery">
                                        <i
                                            class="fas fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 text-sm"></i>
                                    </div>
                                </div>
                                <div class="flex space-x-2">
                                    <button v-for="status in complaintStatuses" :key="status.value"
                                        :class="{ 'bg-orange-500 text-white': selectedComplaintStatus === status.value, 'bg-gray-100 text-gray-700 hover:bg-gray-200': selectedComplaintStatus !== status.value }"
                                        class="px-4 py-2 rounded-lg text-sm font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap"
                                        @click="selectedComplaintStatus = status.value">
                                        {{ status.label }}
                                    </button>
                                </div>
                            </div>
                        </div>
                        <!-- 投诉列表 -->
                        <div class="overflow-x-auto">
                            <table class="w-full">
                                <thead class="bg-gray-50">
                                    <tr>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            投诉编号</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            投诉对象</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            投诉内容</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            申请时间</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            处理措施</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            状态</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            操作</th>
                                    </tr>
                                </thead>
                                <tbody class="bg-white divide-y divide-gray-200">
                                    <tr v-for="item in filteredComplaints" :key="item.complaintId"
                                        class="hover:bg-gray-50">
                                        <td class="pl-10 pr-6 py-4 text-left whitespace-nowrap text-sm font-medium text-gray-900">{{
                                            item.complaintId }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{ item.target }}
                                        </td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{ item.content }}
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{
                                            item.applicationTime }}</td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900">{{ item.punishment || '-' }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap">
                                            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm">
                                                {{ item.status }}
                                            </span>
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm font-medium space-x-2">
                                            <button @click="openComplaintDetail(item)"
                                                class="text-orange-600 hover:text-orange-900 cursor-pointer !rounded-button whitespace-nowrap">查看详情</button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 分页 -->
                        <div class="px-6 py-4 border-t border-gray-200">
                            <div class="flex items-center justify-between">
                                <div class="text-sm text-gray-700">
                                    显示第 1-10 条，共 {{ complaintsList.length }} 条记录
                                </div>
                                <div class="flex space-x-2">
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">上一页</button>
                                    <button
                                        class="px-3 py-1 bg-orange-500 text-white rounded text-sm cursor-pointer !rounded-button whitespace-nowrap">1</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">2</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">下一页</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 店铺违规处理页面 -->
                <div v-if="activeMenu === 'violations'">
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100">
                        <div class="p-6 border-b border-gray-100">
                            <div class="flex items-center justify-between mb-4">
                                <h2 class="text-lg font-semibold text-gray-900">店铺违规处理</h2>
                            </div>
                            <!-- 搜索和筛选区域 -->
                            <div class="flex items-center space-x-4 mb-6">
                                <div class="flex-1 max-w-md">
                                    <div class="relative">
                                        <input type="text" placeholder="搜索处罚编号或店铺名称..."
                                            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm"
                                            v-model="violationSearchQuery">
                                        <i
                                            class="fas fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 text-sm"></i>
                                    </div>
                                </div>
                                <div class="flex space-x-2">
                                    <button v-for="status in violationStatuses" :key="status.value"
                                        :class="{ 'bg-orange-500 text-white': selectedViolationStatus === status.value, 'bg-gray-100 text-gray-700 hover:bg-gray-200': selectedViolationStatus !== status.value }"
                                        class="px-4 py-2 rounded-lg text-sm font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap"
                                        @click="selectedViolationStatus = status.value">
                                        {{ status.label }}
                                    </button>
                                </div>
                            </div>
                        </div>
                        <!-- 违规处理列表 -->
                        <div class="overflow-x-auto">
                            <table class="w-full">
                                <thead class="bg-gray-50">
                                    <tr>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            处罚编号</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            店铺名称</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            处罚原因</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            商家处罚措施</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            店铺处罚措施</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            处罚时间</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            状态</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            操作</th>
                                    </tr>
                                </thead>
                                <tbody class="bg-white divide-y divide-gray-200">
                                    <tr v-for="item in filteredViolations" :key="item.punishmentId"
                                        class="hover:bg-gray-50">
                                        <td class="pl-10 pr-6 py-4 text-left whitespace-nowrap text-sm font-medium text-gray-900">{{
                                            item.punishmentId }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{ item.storeName
                                            }}</td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{ item.reason }}
                                        </td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{
                                            punishmentOptions.violations.merchant.find(option => option.value === item.merchantPunishment)?.label || item.merchantPunishment }}
                                        </td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{
                                            punishmentOptions.violations.store.find(option => option.value === item.storePunishment)?.label || item.storePunishment }}
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{
                                            item.punishmentTime }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap">
                                            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm">
                                                {{ item.status }}
                                            </span>
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm font-medium space-x-2">
                                            <button @click="openViolationDetail(item)"
                                                class="text-orange-600 hover:text-orange-900 cursor-pointer !rounded-button whitespace-nowrap">
                                                <span class="flex items-center">
                                                    <i class="fas fa-eye mr-1"></i>
                                                    查看详情
                                                </span>
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 分页 -->
                        <div class="px-6 py-4 border-t border-gray-200">
                            <div class="flex items-center justify-between">
                                <div class="text-sm text-gray-700">
                                    显示第 1-10 条，共 {{ violationsList.length }} 条记录
                                </div>
                                <div class="flex space-x-2">
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">上一页</button>
                                    <button
                                        class="px-3 py-1 bg-orange-500 text-white rounded text-sm cursor-pointer !rounded-button whitespace-nowrap">1</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">2</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">下一页</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- 评论审核管理页面 -->
                <div v-if="activeMenu === 'reviews'">
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100">
                        <div class="p-6 border-b border-gray-100">
                            <div class="flex items-center justify-between mb-4">
                                <h2 class="text-lg font-semibold text-gray-900">评论审核管理</h2>
                            </div>
                            <!-- 搜索和筛选区域 -->
                            <div class="flex items-center space-x-4 mb-6">
                                <div class="flex-1 max-w-md">
                                    <div class="relative">
                                        <input type="text" placeholder="搜索评论内容..."
                                            class="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm"
                                            v-model="reviewSearchQuery">
                                        <i
                                            class="fas fa-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 text-sm"></i>
                                    </div>
                                </div>
                                <div class="flex space-x-2">
                                    <button v-for="status in reviewStatuses" :key="status.value"
                                        :class="{ 'bg-orange-500 text-white': selectedReviewStatus === status.value, 'bg-gray-100 text-gray-700 hover:bg-gray-200': selectedReviewStatus !== status.value }"
                                        class="px-4 py-2 rounded-lg text-sm font-medium transition-colors cursor-pointer !rounded-button whitespace-nowrap"
                                        @click="selectedReviewStatus = status.value">
                                        {{ status.label }}
                                    </button>
                                </div>
                            </div>
                        </div>
                        <!-- 评论审核列表 -->
                        <div class="overflow-x-auto">
                            <table class="w-full">
                                <thead class="bg-gray-50">
                                    <tr>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            评论编号</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            用户名</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            评论类型 <!-- 【修改】这里由"店铺名称"改为"评论类型" -->
                                        </th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            评论内容</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            评分</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            提交时间</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            状态</th>
                                        <th
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            操作</th>
                                    </tr>
                                </thead>
                                <tbody class="bg-white divide-y divide-gray-200">
                                    <tr v-for="item in filteredReviews" :key="item.reviewId" class="hover:bg-gray-50">
                                        <td class="pl-10 pr-6 py-4 text-left whitespace-nowrap text-sm font-medium text-gray-900">{{
                                            item.reviewId }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{ item.username
                                            }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{
                                            item.type
                                            }}</td>
                                        <td class="px-6 py-4 text-left text-sm text-gray-900 max-w-xs truncate">{{ item.content }}
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">
                                            <span v-if="item.type === '回复评论'" class="text-gray-500">无</span>
                                            <div v-else class="flex items-center">
                                                <span class="mr-1">{{ item.rating }}</span>
                                                <div class="flex text-yellow-400">
                                                    <i v-for="star in 5" :key="star"
                                                        :class="star <= item.rating ? 'fas fa-star' : 'far fa-star'"
                                                        class="text-xs"></i>
                                                </div>
                                            </div>
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm text-gray-900">{{ item.submitTime
                                            }}</td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap">
                                            <span :class="getStatusClass(item.status)" class="px-3 py-1 rounded-full text-sm">
                                                {{ item.status }}
                                            </span>
                                        </td>
                                        <td class="px-6 py-4 text-left whitespace-nowrap text-sm font-medium space-x-2">
                                            <button @click="openReviewDetail(item)"
                                                class="text-orange-600 hover:text-orange-900 cursor-pointer !rounded-button whitespace-nowrap">
                                                <span class="flex items-center">
                                                    <i class="fas fa-eye mr-1"></i>
                                                    查看详情
                                                </span>
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- 分页 -->
                        <div class="px-6 py-4 border-t border-gray-200">
                            <div class="flex items-center justify-between">
                                <div class="text-sm text-gray-700">
                                    显示第 1-10 条，共 {{ reviewsList.length }} 条记录
                                </div>
                                <div class="flex space-x-2">
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">上一页</button>
                                    <button
                                        class="px-3 py-1 bg-orange-500 text-white rounded text-sm cursor-pointer !rounded-button whitespace-nowrap">1</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">2</button>
                                    <button
                                        class="px-3 py-1 border border-gray-300 rounded text-sm hover:bg-gray-50 cursor-pointer !rounded-button whitespace-nowrap">下一页</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-if="activeMenu === 'admin' && currentUser">
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100">
                        <div class="p-6 border-b border-gray-100">
                            <h2 class="text-lg font-semibold text-gray-900 text-center">管理员信息管理</h2>
                        </div>
                        <div class="p-6">
                            <div class="max-w-4xl mx-auto">
                            <!-- 头像和基本信息 -->
                            <div class="flex items-center justify-between mb-8">
                                <div class="flex items-center space-x-6">
                                    <img :src="'https://s1.aigei.com/src/img/png/f7/f734d8198b614d0a9356196cd83c6758.png?imageMogr2/auto-orient/thumbnail/!282x282r/gravity/Center/crop/282x282/quality/85/%7CimageView2/2/w/282&e=2051020800&token=P7S2Xpzfz11vAkASLTkfHN7Fw-oOZBecqeJaxypL:FldJin-4wd319skieoNSW_v2zAY='"
                                        class="w-24 h-24 rounded-full object-cover border-4 border-gray-100">
                                    <div>
                                        <p class="text-gray-600 text-lg font-semibold">系统管理员 {{ currentUser.id }}号</p>
                                        <p class="text-sm text-gray-500">注册时间：{{ currentUser.registrationDate }}</p>
                                    </div>
                                </div>
                                <div class="text-right">
                                    <div class="text-center">
                                        <p class="text-lg font-semibold text-gray-700 mb-1">事务评分</p>
                                        <p class="text-3xl font-extrabold text-orange-600">
                                            {{ currentUser.averageRating && currentUser.averageRating > 0 ? currentUser.averageRating : '暂未获得评分' }}
                                        </p>
                                    </div>
                                </div>
                            </div>

                                <!-- 信息表单 -->
                                <div>
                                    <div class="mb-6">
                                        <label class="block text-sm font-medium text-gray-700 mb-2">管理对象</label>
                                        <div class="flex flex-wrap gap-5 justify-center">
                                            <button 
                                                type="button"
                                                v-for="option in managementOptions" 
                                                :key="option"
                                                @click="toggleOption(option)"
                                                :class="{
                                                    'bg-orange-500 text-white': isSelected(option),
                                                    'bg-gray-200 text-gray-700 hover:bg-gray-300': !isSelected(option)
                                                }"
                                                class="px-8 py-2 rounded-lg text-sm font-medium transition-colors">
                                                {{ option }}
                                            </button>
                                        </div>
                                    </div>
                                    <div class="grid grid-cols-2 gap-6 mb-6">
                                        <div>
                                            <label class="block text-sm font-medium text-gray-700 mb-2">用户名</label>
                                            <input type="text" v-model="currentUser.username"
                                                class="w-full px-3 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 text-sm bg-white">
                                        </div>
                                        <div>
                                            <label class="block text-sm font-medium text-gray-700 mb-2">真实姓名</label>
                                            <input type="text" :value="currentUser.realName" readonly
                                                class="w-full px-3 py-2 border border-gray-200 rounded-lg bg-gray-100 text-gray-600 text-sm cursor-not-allowed">
                                        </div>
                                    </div>
                                    <div class="grid grid-cols-2 gap-6 mb-6">
                                        <div>
                                            <label class="block text-sm font-medium text-gray-700 mb-2">手机号</label>
                                            <input type="text" :value="currentUser.phone" readonly
                                                class="w-full px-3 py-2 border border-gray-200 rounded-lg bg-gray-100 text-gray-600 text-sm cursor-not-allowed">
                                        </div>
                                        <div>
                                            <label class="block text-sm font-medium text-gray-700 mb-2">电子邮箱</label>
                                            <input type="email" v-model="currentUser.email" readonly
                                                class="w-full px-3 py-2 border border-gray-200 rounded-lg bg-gray-100 text-gray-600 text-sm cursor-not-allowed">
                                        </div>
                                    </div>

                                    <!-- 操作按钮 -->
                                    <div class="mt-12 pt-6 border-t border-gray-200 flex justify-center space-x-4">
                                    <button @click="handleSaveChanges" :disabled="!hasChanges || isSaving"
                                        class="px-6 py-2 bg-orange-500 text-white rounded-lg hover:bg-orange-600 transition-colors cursor-pointer !rounded-button whitespace-nowrap disabled:opacity-50 disabled:cursor-not-allowed">
                                        {{ isSaving ? '保存中...' : '保存修改' }}
                                    </button>
                                    <button @click="resetForm" :disabled="!hasChanges || isSaving"
                                        class="px-6 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors cursor-pointer !rounded-button whitespace-nowrap disabled:opacity-50 disabled:cursor-not-allowed">
                                        重置
                                    </button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


            </main>

        </div>
        <!-- 售后详情弹窗 -->
        <el-dialog v-model="showAfterSaleDetail" title="售后详情" width="800px" class="after-sale-detail-dialog">
            <div v-if="currentAfterSale" class="after-sale-detail space-y-6 bg-gray-50/50 p-1">
                <!-- 顶部信息卡片：申请基础信息 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100 relative overflow-hidden">
                    <!-- 状态标签 (绝对定位到右上角) -->
                    <div class="absolute top-0 right-0">
                        <span :class="getStatusClass(getAfterSaleStatusText(currentAfterSale.status))" class="px-4 py-1.5 rounded-bl-xl text-xs font-medium shadow-sm block">
                            {{ getAfterSaleStatusText(currentAfterSale.status) }}
                        </span>
                    </div>

                    <div class="flex justify-between items-start mb-4 pb-4 border-b border-gray-100 pt-6">
                        <div class="flex flex-col">
                            <div class="flex items-center gap-3 mb-2">
                                <span class="text-sm font-normal text-gray-500 bg-gray-100 px-2 py-0.5 rounded">申请编号</span>
                                <span class="text-lg font-bold text-gray-900 font-mono">{{ currentAfterSale.applicationId }}</span>
                            </div>
                            <p class="text-xs text-gray-500">申请时间：{{ currentAfterSale.applicationTime }}</p>
                            </div>
                        <div class="text-right">
                            <div class="flex items-center justify-end gap-2">
                                <span class="text-xs text-gray-400">订单</span>
                                <span class="text-sm font-medium text-gray-900 font-mono bg-blue-50 text-blue-700 px-2 py-0.5 rounded border border-blue-100">
                                    {{ currentAfterSale.orderId }}
                        </span>
                    </div>
                            <div class="flex items-center justify-end gap-2 mt-2" v-if="currentAfterSale.user">
                                <span class="text-xs text-gray-500">{{ currentAfterSale.user.name }}</span>
                                <span class="text-xs text-gray-300">|</span>
                                <span class="text-xs text-gray-500 font-mono">{{ currentAfterSale.user.phoneNumber }}</span>
                </div>
                        </div>
                    </div>

                    <!-- 申请内容 -->
                    <div class="space-y-4">
                    <div>
                            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">申请描述</h4>
                            <p class="text-sm text-gray-800 bg-gray-50 p-3 rounded-lg border border-gray-100 leading-relaxed">
                                {{ currentAfterSale.description }}
                            </p>
                        </div>
                        
                        <!-- 图片 -->
                        <div v-if="(currentAfterSale.images?.length || 0) > 0">
                            <h4 class="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">凭证图片</h4>
                            <div class="flex justify-center gap-3">
                                <div v-for="(img, idx) in currentAfterSale.images" :key="idx" class="relative w-20 h-20">
                                    <img :src="normalizeImageUrl(img)" class="w-full h-full object-cover rounded-lg border border-gray-200 cursor-zoom-in hover:opacity-90" @click="previewImage(normalizeImageUrl(img))" />
                    </div>
                </div>
                    </div>

                        <!-- 菜品 -->
                        <div v-if="(currentAfterSale.dishDetails?.length || 0) > 0">
                            <div class="mb-2 flex items-center gap-2">
                                <span class="text-xs font-semibold text-gray-700">菜品信息</span>
                                <div class="flex-1 h-px bg-gradient-to-r from-gray-200 to-transparent"></div>
                </div>
                            <div class="flex justify-between items-start p-3 bg-gradient-to-br from-gray-50 to-white rounded-lg border border-gray-100">
                                <div class="flex gap-2 overflow-x-auto scrollbar-hide pb-1 flex-1 min-w-0">
                                    <div v-for="(dish, idx) in (currentAfterSale.dishDetails || []).slice(0, 8)" :key="idx" class="flex flex-col items-center min-w-[4.5rem]">
                                        <div class="relative w-16 h-16 rounded-lg bg-white flex items-center justify-center overflow-hidden border border-gray-200 shadow-sm">
                                            <img :src="normalizeImageUrl(dish.dishImage)" :alt="dish.dishName"
                                                class="w-full h-full object-cover" @error="handleImageError" />
                                            <span v-if="dish.quantity > 1" class="absolute top-0 right-0 bg-red-500 text-white text-[10px] px-1 rounded-bl-lg font-bold">x{{ dish.quantity }}</span>
                        </div>
                                        <div class="w-16 mt-1.5 text-center">
                                            <p class="text-xs text-gray-800 truncate w-full font-medium" :title="dish.dishName">{{ dish.dishName }}</p>
                                            <p class="text-[10px] text-gray-500 mt-0.5 font-mono">
                                                ¥{{ Number.isInteger(dish.price) ? dish.price : Number(dish.price).toFixed(2) }}
                                            </p>
                    </div>
                </div>
                                    <div v-if="(currentAfterSale.dishDetails?.length || 0) > 8"
                                        class="w-16 h-16 flex flex-col items-center justify-center rounded-lg bg-white text-gray-500 text-xs border border-gray-200 min-w-[4.5rem] shadow-sm">
                                        <span class="text-lg font-bold">+{{ (currentAfterSale.dishDetails?.length || 0) - 8 }}</span>
                                        <span class="text-[10px]">更多</span>
                                </div>
                                </div>
                                <div class="ml-3 flex h-16 items-center shrink-0">
                                    <span class="text-xs text-gray-600 bg-white px-2.5 py-1.5 rounded-lg border border-gray-200 shadow-sm font-medium">共 {{ (currentAfterSale.dishDetails || []).reduce((acc, d) => acc + d.quantity, 0) }} 件</span>
                            </div>
                        </div>
                    </div>
                </div>
                     </div>

                <!-- 状态分流逻辑 -->
                <div v-if="getAfterSaleStatusText(currentAfterSale.status) === '商家未回复'" class="bg-orange-50 border border-orange-100 rounded-xl p-6 text-center">
                    <div class="flex flex-col items-center gap-3">
                        <div class="w-12 h-12 bg-orange-100 text-orange-500 rounded-full flex items-center justify-center text-xl">
                            <i class="fas fa-hourglass-half"></i>
                 </div>
                        <h3 class="text-base font-medium text-gray-900">等待商家回复</h3>
                        <p class="text-sm text-gray-500 max-w-md">
                            商家需要在规定时间内对售后申请进行反馈。请在商家回复后再进行平台介入处理。
                        </p>
                 </div>
                </div>

                <template v-else>
                    <!-- 商家回复卡片 -->
                    <div class="bg-white rounded-xl shadow-sm border border-blue-100 overflow-hidden">
                        <div class="bg-blue-50/50 px-5 py-3 border-b border-blue-100 flex justify-between items-center">
                            <h4 class="text-sm font-semibold text-blue-800 flex items-center gap-2">
                                <i class="fas fa-store"></i> 商家回复
                            </h4>
                        </div>
                        <div class="p-5">
                            <p class="text-sm text-gray-800 leading-relaxed">
                                {{ currentAfterSale.merchantReply || '商家暂无详细文字回复' }}
                            </p>
                        </div>
                    </div>

                    <!-- 平台处理卡片 -->
                    <div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden" :class="{'border-orange-200 ring-4 ring-orange-50': isAfterSaleEditable}">
                        <div class="px-5 py-3 border-b border-gray-100 flex justify-between items-center" :class="isAfterSaleEditable ? 'bg-orange-50/50' : 'bg-gray-50/50'">
                            <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
                                <i class="fas fa-gavel" :class="isAfterSaleEditable ? 'text-orange-500' : 'text-gray-400'"></i>
                                平台处理
                            </h4>
                            <span v-if="isAfterSaleEditable" class="text-xs bg-orange-100 text-orange-700 px-2 py-0.5 rounded font-medium">待处理</span>
                            <span v-else class="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded">已归档</span>
                        </div>
                        
                        <div class="p-5 space-y-5">
                            <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">处理结果</label>
                    <el-select 
                        v-model="selectedPunishment" 
                                    class="w-full" 
                        :class="{ 'select-readonly': !isAfterSaleEditable }"
                        placeholder="请选择处理措施"
                        :disabled="!isAfterSaleEditable"
                    >
                        <el-option 
                            v-for="option in punishmentOptions.afterSales" 
                            :key="option.value"
                            :label="option.label" 
                            :value="option.value" 
                        />
                    </el-select>
                            </div>

                            <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">处理原因 / 备注</label>
                    <el-input 
                                    v-if="isAfterSaleEditable"
                        v-model="punishmentReason" 
                        type="textarea" 
                        :rows="3" 
                                    placeholder="请输入详细的处理原因..." 
                        maxlength="500"
                        show-word-limit 
                    />
                                <div v-else class="bg-gray-50 p-3 rounded-lg border border-gray-200 text-sm text-gray-600 min-h-[60px]">
                                    {{ currentAfterSale.punishmentReason || '无详细原因' }}
                </div>
                            </div>

                            <div v-if="!isAfterSaleEditable" class="pt-4 border-t border-gray-100">
                                <div class="flex items-center gap-3">
                                    <span class="text-xs font-medium text-gray-500">用户评分</span>
                                    <el-rate :model-value="currentAfterSale.consumerRating || 0" :max="5" disabled size="small" />
                                    <span class="text-xs text-gray-400">{{ currentAfterSale.consumerRating != null ? `${currentAfterSale.consumerRating}分` : '暂无评分' }}</span>
                                </div>
                            </div>
                        </div>

                        <div v-if="isAfterSaleEditable" class="px-5 py-4 bg-gray-50 border-t border-gray-100 flex justify-end gap-3">
                    <el-button @click="showAfterSaleDetail = false">取消</el-button>
                            <el-button type="primary" @click="handleAfterSaleAction" :disabled="!selectedPunishment">
                                确认处理
                    </el-button>
                </div>
                        <div v-else class="px-5 py-3 bg-gray-50 border-t border-gray-100 flex justify-end">
                            <el-button @click="showAfterSaleDetail = false" size="small">关闭</el-button>
                        </div>
                    </div>
                </template>
            </div>
        </el-dialog>
        <!-- 投诉详情弹窗 -->
        <el-dialog v-model="showComplaintDetail" title="投诉详情" width="800px" class="complaint-detail-dialog">
            <div v-if="currentComplaint" class="complaint-detail space-y-6 bg-gray-50/50 p-1">
                <!-- 顶部卡片 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100 relative overflow-hidden">
                    <!-- 状态标签 -->
                    <div class="absolute top-0 right-0">
                        <span :class="getStatusClass(currentComplaint.status)" class="px-4 py-1.5 rounded-bl-xl text-xs font-medium shadow-sm block">
                            {{ currentComplaint.status }}
                        </span>
                            </div>

                    <div class="flex justify-between items-start mb-6 pt-6">
                        <div class="flex flex-col">
                            <div class="flex items-center gap-3 mb-2">
                                <span class="text-sm font-normal text-gray-500 bg-gray-100 px-2 py-0.5 rounded">投诉编号</span>
                                <span class="text-lg font-bold text-gray-900 font-mono">{{ currentComplaint.complaintId }}</span>
                            </div>
                            <p class="text-xs text-gray-500">提交时间：{{ currentComplaint.applicationTime }}</p>
                        </div>
                        
                        <!-- 投诉对象信息 -->
                        <div class="text-right">
                            <div class="inline-block text-left bg-blue-50 rounded-lg px-3 py-2 border border-blue-100">
                                <p class="text-xs text-blue-500 mb-0.5">投诉对象</p>
                                <p class="text-sm font-bold text-blue-900">{{ currentComplaint.target }}</p>
                            </div>
                        </div>
                    </div>

                    <!-- 骑手信息 -->
                    <div class="bg-gray-50 rounded-lg border border-gray-100 overflow-hidden">
                        <div class="grid grid-cols-2 divide-x divide-gray-200">
                            <div class="p-4">
                                <p class="text-xs text-gray-400 mb-2 flex items-center gap-1">
                                    <i class="fas fa-motorcycle"></i> 配送骑手
                                </p>
                                <div class="flex flex-col items-start gap-1">
                                    <span class="font-bold text-gray-900 text-sm">{{ currentComplaint.courierName || '未知骑手' }}</span>
                                    <span class="text-xs text-gray-500 font-mono bg-white px-1.5 py-0.5 rounded border border-gray-200">
                                        {{ currentComplaint.courierPhone || '无联系方式' }}
                        </span>
                    </div>
                </div>
                            <div class="p-4">
                                <p class="text-xs text-gray-400 mb-2 flex items-center gap-1">
                                    <i class="fas fa-clock"></i> 配送时间
                                </p>
                                <div class="space-y-1.5">
                                    <div class="flex items-center text-xs">
                                        <span class="w-8 text-gray-400 shrink-0">接单</span>
                                        <span class="text-gray-700 font-mono">{{ currentComplaint.acceptTime || '-' }}</span>
                        </div>
                                    <div class="flex items-center text-xs">
                                        <span class="w-8 text-gray-400 shrink-0">送达</span>
                                        <span class="text-gray-700 font-mono">{{ currentComplaint.completionTime || '-' }}</span>
                    </div>
                        </div>
                    </div>
                </div>
                    </div>
                </div>

                <!-- 投诉内容 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                    <h4 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i class="fas fa-align-left text-orange-500"></i>
                        投诉详情
                    </h4>
                    <div class="space-y-4">
                        <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-3 rounded-lg border border-gray-100">
                            {{ currentComplaint.content }}
                        </p>
                        
                        <div v-if="(currentComplaint.images?.length || 0) > 0">
                            <p class="text-xs text-gray-400 mb-2">凭证图片</p>
                            <div class="flex justify-center gap-3">
                                <div v-for="(img, idx) in currentComplaint.images" :key="idx" class="relative w-20 h-20">
                            <img 
                                :src="normalizeImageUrl(img)" 
                                alt="投诉图片" 
                                        class="w-full h-full object-cover rounded-lg border border-gray-200 cursor-zoom-in hover:opacity-90"
                                @error="handleImageError"
                                @click="previewImage(normalizeImageUrl(img))"
                            />
                        </div>
                    </div>
                </div>
                    </div>
                </div>

                <!-- 平台处理 -->
                <div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden" :class="{'border-orange-200 ring-4 ring-orange-50': currentComplaint.status === '待处理'}">
                    <div class="px-5 py-3 border-b border-gray-100 flex justify-between items-center" :class="currentComplaint.status === '待处理' ? 'bg-orange-50/50' : 'bg-gray-50/50'">
                        <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-gavel" :class="currentComplaint.status === '待处理' ? 'text-orange-500' : 'text-gray-400'"></i>
                            处理结果
                        </h4>
                        <span v-if="currentComplaint.status === '待处理'" class="text-xs bg-orange-100 text-orange-700 px-2 py-0.5 rounded font-medium">待处理</span>
                        <span v-else class="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded">已归档</span>
                    </div>

                    <div class="p-5 space-y-5">
                        <div class="grid grid-cols-2 gap-5">
                            <!-- 处理措施 -->
                            <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">处罚类型</label>
                    <el-select 
                                    v-if="currentComplaint.status === '待处理'"
                        v-model="selectedComplaintPunishment" 
                                    class="w-full" 
                                    placeholder="请选择"
                    >
                                    <el-option v-for="option in punishmentOptions.complaints" :key="option.value" :label="option.label" :value="option.value" />
                    </el-select>
                                <div v-else class="bg-gray-50 p-2.5 rounded-lg border border-gray-200 text-sm text-gray-900">
                                    {{ getPunishmentLabel(currentComplaint.punishment, 'complaint') }}
                                </div>
                            </div>
                    
                            <!-- 罚款金额 -->
                            <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">罚款金额 (元)</label>
                    <el-input-number 
                                    v-if="currentComplaint.status === '待处理'"
                        v-model="complaintFine" 
                                    :min="0" :precision="2" :step="10" 
                        controls-position="right"
                                    class="w-full"
                    />
                                <div v-else class="bg-gray-50 p-2.5 rounded-lg border border-gray-200 text-sm text-gray-900 font-mono">
                                    ¥{{ currentComplaint.fine || '0.00' }}
                                </div>
                            </div>
                        </div>
                    
                        <!-- 处理原因 -->
                        <div>
                            <label class="block text-xs font-medium text-gray-500 mb-1.5">处理原因 / 备注</label>
                    <el-input 
                                v-if="currentComplaint.status === '待处理'"
                        v-model="complaintPunishmentReason" 
                        type="textarea" 
                        :rows="3" 
                                placeholder="请输入详细的处理原因..." 
                        maxlength="500" 
                        show-word-limit 
                    />
                            <div v-else class="bg-gray-50 p-3 rounded-lg border border-gray-200 text-sm text-gray-600 min-h-[60px]">
                                {{ currentComplaint.punishmentReason || '无详细原因' }}
                </div>
                        </div>
                    </div>

                    <!-- 操作栏 -->
                    <div v-if="currentComplaint.status === '待处理'" class="px-5 py-4 bg-gray-50 border-t border-gray-100 flex justify-end gap-3">
                    <el-button @click="showComplaintDetail = false">取消</el-button>
                        <el-button type="primary" @click="handleComplaintProcess">
                            确认处理
                        </el-button>
                    </div>
                    <div v-else class="px-5 py-3 bg-gray-50 border-t border-gray-100 flex justify-end">
                        <el-button @click="showComplaintDetail = false" size="small">关闭</el-button>
                    </div>
                </div>
            </div>
        </el-dialog>
        <!-- 违规举报详情弹窗 -->
        <el-dialog v-model="showViolationDetail" title="违规举报详情" width="800px" class="violation-detail-dialog">
            <div v-if="currentViolation" class="violation-detail space-y-6 bg-gray-50/50 p-1">
                <!-- 顶部卡片 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100 relative overflow-hidden">
                    <!-- 状态标签 -->
                    <div class="absolute top-0 right-0">
                        <span :class="getStatusClass(currentViolation.status)" class="px-4 py-1.5 rounded-bl-xl text-xs font-medium shadow-sm block">
                            {{ currentViolation.status }}
                        </span>
                            </div>

                    <div class="flex justify-between items-start mb-6 pt-6">
                        <div class="flex flex-col">
                            <div class="flex items-center gap-3 mb-2">
                                <span class="text-sm font-normal text-gray-500 bg-gray-100 px-2 py-0.5 rounded">处罚编号</span>
                                <span class="text-lg font-bold text-gray-900 font-mono">{{ currentViolation.punishmentId }}</span>
                            </div>
                            <p class="text-xs text-gray-500">处罚时间：{{ currentViolation.punishmentTime }}</p>
                        </div>
                        
                        <div class="text-right">
                            <div class="inline-block text-left bg-purple-50 rounded-lg px-3 py-2 border border-purple-100">
                                <p class="text-xs text-purple-500 mb-0.5">违规店铺</p>
                                <p class="text-sm font-bold text-purple-900 flex items-center gap-1">
                                    <i class="fas fa-store text-xs"></i>
                                    {{ currentViolation.storeName }}
                                </p>
                    </div>
                </div>
                        </div>
                </div>

                <!-- 违规详情 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100">
                    <h4 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i class="fas fa-exclamation-circle text-red-500"></i>
                        违规详情
                    </h4>
                    <div class="space-y-4">
                        <div class="flex items-center gap-2">
                            <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">违规类型</span>
                            <span class="text-sm font-medium text-gray-900">食品安全</span>
                    </div>
                    <div>
                            <p class="text-xs text-gray-400 mb-1.5">违规原因说明</p>
                            <p class="text-sm text-gray-800 leading-relaxed bg-red-50 p-3 rounded-lg border border-red-100">
                                {{ currentViolation.reason }}
                            </p>
                        </div>
                    </div>
                </div>

                <!-- 处罚执行 -->
                <div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden" :class="{'border-orange-200 ring-4 ring-orange-50': currentViolation.status === '待处理'}">
                    <div class="px-5 py-3 border-b border-gray-100 flex justify-between items-center" :class="currentViolation.status === '待处理' ? 'bg-orange-50/50' : 'bg-gray-50/50'">
                        <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-gavel" :class="currentViolation.status === '待处理' ? 'text-orange-500' : 'text-gray-400'"></i>
                            处罚执行
                        </h4>
                        <span v-if="currentViolation.status === '待处理'" class="text-xs bg-orange-100 text-orange-700 px-2 py-0.5 rounded font-medium">待处理</span>
                        <span v-else class="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded">已执行</span>
                    </div>

                    <div class="p-5 space-y-5">
                        <div class="grid grid-cols-2 gap-5">
                            <!-- 商家处罚 -->
                    <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">商家处罚</label>
                        <el-select 
                                    v-if="currentViolation.status === '待处理'"
                            v-model="selectedMerchantPunishment"
                            class="w-full" 
                                    placeholder="请选择"
                        >
                                    <el-option v-for="option in punishmentOptions.violations.merchant" :key="option.value" :label="option.label" :value="option.value" />
                        </el-select>
                                <div v-else class="bg-gray-50 p-3 rounded-lg border border-gray-200 text-sm text-gray-900 font-medium">
                                    {{ getPunishmentLabel(currentViolation.merchantPunishment, 'merchant') }}
                    </div>
                            </div>

                            <!-- 店铺处罚 -->
                    <div>
                                <label class="block text-xs font-medium text-gray-500 mb-1.5">店铺处罚</label>
                        <el-select 
                                    v-if="currentViolation.status === '待处理'"
                            v-model="selectedStorePunishment"
                            class="w-full" 
                                    placeholder="请选择"
                        >
                                    <el-option v-for="option in punishmentOptions.violations.store" :key="option.value" :label="option.label" :value="option.value" />
                        </el-select>
                                <div v-else class="bg-gray-50 p-3 rounded-lg border border-gray-200 text-sm text-gray-900 font-medium">
                                    {{ getPunishmentLabel(currentViolation.storePunishment, 'store') }}
                    </div>
                </div>
                        </div>
                    </div>

                    <!-- 操作栏 -->
                    <div v-if="currentViolation.status === '待处理'" class="px-5 py-4 bg-gray-50 border-t border-gray-100 flex justify-end gap-3">
                    <el-button @click="showViolationDetail = false">取消</el-button>
                        <el-button type="primary" @click="handleViolationAction('complete')">
                            执行处罚
                        </el-button>
                    </div>
                    <div v-else class="px-5 py-3 bg-gray-50 border-t border-gray-100 flex justify-end">
                        <el-button @click="showViolationDetail = false" size="small">关闭</el-button>
                    </div>
                </div>
            </div>
        </el-dialog>
        <!-- 评论详情弹窗 -->
        <el-dialog v-model="showReviewDetail" title="评论详情" width="800px" class="review-detail-dialog">
            <div v-if="currentReview" class="review-detail space-y-6 bg-gray-50/50 p-1">
                <!-- 顶部卡片：用户评分 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100 relative overflow-hidden">
                    <!-- 状态标签 -->
                    <div class="absolute top-0 right-0">
                        <span :class="getStatusClass(currentReview.status)" class="px-4 py-1.5 rounded-bl-xl text-xs font-medium shadow-sm block">
                            {{ currentReview.status }}
                        </span>
                    </div>

                    <div class="flex items-start justify-between mb-4 pt-6">
                        <div class="flex items-center gap-4">
                            <!-- 商家回复：显示店铺信息 -->
                            <template v-if="currentReview.type === '回复评论'">
                                <img 
                                    v-if="currentReview.storeImage"
                                    :src="normalizeImageUrl(currentReview.storeImage)" 
                                    :alt="currentReview.storeName || '店铺'"
                                    class="w-12 h-12 rounded-lg object-cover border border-gray-200 shadow-sm"
                                    @error="handleImageError"
                                />
                                <div v-else class="w-12 h-12 bg-orange-100 rounded-lg flex items-center justify-center border border-orange-200">
                                    <i class="fas fa-store text-orange-500"></i>
                                </div>
                                <div>
                                    <h3 class="text-base font-bold text-gray-900">{{ currentReview.storeName || '未知店铺' }}</h3>
                                    <p class="text-xs text-gray-500 mt-0.5">{{ currentReview.submitTime }}</p>
                                </div>
                            </template>
                            <!-- 普通评论：显示用户信息 -->
                            <template v-else>
                                <img 
                                    v-if="currentReview.avatar"
                                    :src="normalizeImageUrl(currentReview.avatar)" 
                                    :alt="currentReview.username"
                                    class="w-12 h-12 rounded-full object-cover border border-gray-200 shadow-sm"
                                    @error="handleImageError"
                                />
                                <div v-else class="w-12 h-12 bg-gray-100 rounded-full flex items-center justify-center">
                                    <i class="fas fa-user text-gray-400"></i>
                                </div>
                                <div>
                                    <h3 class="text-base font-bold text-gray-900">{{ currentReview.username }}</h3>
                                    <p class="text-xs text-gray-500 mt-0.5">{{ currentReview.submitTime }}</p>
                                </div>
                            </template>
                        </div>
                        </div>
                    
                    <!-- 评分展示（仅普通评论显示） -->
                    <div v-if="currentReview.type !== '回复评论'" class="flex items-center gap-3 bg-yellow-50/50 p-3 rounded-lg border border-yellow-100">
                        <span class="text-xs font-medium text-gray-600">综合评分</span>
                        <div class="flex items-center gap-1 text-yellow-400">
                            <i v-for="star in 5" :key="star" :class="star <= currentReview.rating ? 'fas fa-star' : 'far fa-star'"></i>
                    </div>
                        <span class="text-sm font-bold text-yellow-600">{{ currentReview.rating }} 分</span>
                </div>
                        </div>

                <!-- 原评论信息（仅商家回复显示） -->
                <div v-if="currentReview.type === '回复评论' && currentReview.originalCommentContent" class="bg-white rounded-xl p-5 shadow-sm border border-blue-200">
                    <h4 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i class="fas fa-comment text-blue-500"></i>
                        原评论信息
                    </h4>
                    <div class="space-y-3">
                        <div class="flex items-center gap-2 mb-2">
                            <span class="text-xs text-gray-500 bg-blue-100 px-2 py-1 rounded">评论用户</span>
                            <span class="text-sm font-medium text-gray-900">{{ currentReview.originalCommentUsername || '未知用户' }}</span>
                            <span v-if="currentReview.originalCommentTime" class="text-xs text-gray-400 ml-auto">{{ currentReview.originalCommentTime }}</span>
                        </div>
                        <p class="text-sm text-gray-800 leading-relaxed bg-blue-50 p-3 rounded-lg border border-blue-100">
                            {{ currentReview.originalCommentContent }}
                        </p>
                    </div>
                </div>

                <!-- 评论内容 -->
                <div class="bg-white rounded-xl p-5 shadow-sm border border-gray-100" :class="{'border-orange-200': currentReview.type === '回复评论'}">
                    <h4 class="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                        <i :class="currentReview.type === '回复评论' ? 'fas fa-reply text-orange-500' : 'fas fa-comment-dots text-blue-500'"></i>
                        {{ currentReview.type === '回复评论' ? '商家回复内容' : '评论内容' }}
                    </h4>
                    <div class="space-y-4">
                        <div class="flex items-center gap-2 mb-2">
                            <span class="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded">类型</span>
                            <span class="text-sm font-medium text-gray-900">{{ currentReview.type }}</span>
                    </div>
                        <p class="text-sm text-gray-800 leading-relaxed bg-gray-50 p-3 rounded-lg border border-gray-100">
                            {{ currentReview.content }}
                        </p>
                        
                        <div v-if="reviewImages.length > 0">
                            <p class="text-xs text-gray-400 mb-2">评论配图</p>
                            <div class="flex justify-center gap-3">
                                <div v-for="(img, idx) in reviewImages" :key="idx" class="relative w-20 h-20">
                            <img 
                                        :src="normalizeImageUrl(img)" 
                                        class="w-full h-full object-cover rounded-lg border border-gray-200 cursor-zoom-in hover:opacity-90"
                                @error="handleImageError"
                                        @click="previewImage(normalizeImageUrl(img))"
                            />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- 平台审核 -->
                <div class="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden" :class="{'border-orange-200 ring-4 ring-orange-50': currentReview.status === '待处理'}">
                    <div class="px-5 py-3 border-b border-gray-100 flex justify-between items-center" :class="currentReview.status === '待处理' ? 'bg-orange-50/50' : 'bg-gray-50/50'">
                        <h4 class="text-sm font-semibold text-gray-900 flex items-center gap-2">
                            <i class="fas fa-check-circle" :class="currentReview.status === '待处理' ? 'text-orange-500' : 'text-gray-400'"></i>
                            审核处理
                        </h4>
                        <span v-if="currentReview.status === '待处理'" class="text-xs bg-orange-100 text-orange-700 px-2 py-0.5 rounded font-medium">待处理</span>
                        <span v-else-if="currentReview.status === '通过'" class="text-xs bg-green-100 text-green-700 px-2 py-0.5 rounded font-medium">通过</span>
                        <span v-else class="text-xs bg-gray-100 text-gray-600 px-2 py-0.5 rounded">{{ currentReview.status }}</span>
                    </div>

                    <!-- 操作栏 -->
                    <div v-if="currentReview.status === '待处理'" class="px-5 py-4 bg-gray-50 flex justify-end gap-3">
                    <el-button @click="showReviewDetail = false">取消</el-button>
                        <el-button type="danger" @click="processReview('reject')">判定违规</el-button>
                        <el-button type="success" @click="processReview('approve')">审核通过</el-button>
                    </div>
                    <div v-else class="px-5 py-3 bg-gray-50 flex justify-end">
                        <el-button @click="showReviewDetail = false" size="small">关闭</el-button>
                    </div>
                </div>
            </div>
        </el-dialog>
        
        <!-- 图片预览弹窗 -->
        <div v-if="previewImageUrl" 
             class="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center z-50"
             @click.self="closePreview">
            <div class="relative max-w-4xl max-h-full p-4">
                <img 
                    :src="previewImageUrl" 
                    alt="预览图片" 
                    class="max-w-full max-h-[90vh] rounded-lg shadow-lg"
                />
                <button 
                    @click="closePreview"
                    class="absolute top-2 right-2 text-white text-3xl font-bold hover:text-gray-300 transition-colors"
                >
                    &times;
                </button>
            </div>
        </div>
    </div>

</template>
<script lang="ts" setup>
import { getProjectName } from '@/stores/name';
import { normalizeImageUrl, handleImageError } from '@/utils/imageUtils';

// =================================================================
// 步骤 1: 导入必要的模块
// =================================================================
import { ref, computed, onMounted, readonly } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import { useRouter } from 'vue-router';
import axios from 'axios'; // 导入axios用于真实API请求

const useProjectName = getProjectName();
const projectName = useProjectName.projectName;

// =================================================================
// 步骤 2: 定义数据类型
// =================================================================
interface AdminInfo {
    id: string; // e.g., "ADM001"
    username: string;
    realName: string; // e.g., "张伟"
    registrationDate: string; // e.g., "2023-01-15"
    avatarUrl: string;
    phone: string;
    email: string;
    gender: '男' | '女';
    managementScope: string;
    averageRating: number;
}


interface AfterSaleItem {
    applicationId: string;
    orderId: string;
    applicationTime: string;
    description: string;
    status: string;
    punishment: string;
    punishmentReason?: string;
    // 新增：管理员详情所需的字段
    user?: { name: string; phoneNumber: number; avatar?: string };
    images?: string[];
    dishDetails?: Array<{ dishName: string; dishImage: string; quantity: number; price: number }>;
    merchantReply?: string;
    consumerRating?: number;
}

interface ComplaintItem {
    complaintId: string;
    target: string;
    content: string;
    applicationTime: string;
    status: string;
    punishment: string;
    fine: string;
    punishmentReason?: string;
    // 新增：投诉图片与骑手信息、时间
    images?: string[];
    courierName?: string;
    courierPhone?: string;
    acceptTime?: string | null;
    pickupTime?: string | null;
    completionTime?: string | null;
}

interface ViolationItem {
    punishmentId: string;
    storeName: string;
    reason: string;
    merchantPunishment: string;
    storePunishment: string;
    punishmentTime: string;
    status: string;
}

interface ReviewItem {
    reviewId: string;
    username: string;
    avatar?: string; // 用户头像
    type: string;
    content: string;
    rating: number;
    submitTime: string;
    status: string;
    image?: string; // 评论图片（逗号分隔的字符串）
    originalCommentContent?: string; // 原评论内容（如果是回复评论）
    originalCommentUsername?: string; // 原评论用户名（如果是回复评论）
    originalCommentTime?: string; // 原评论时间（如果是回复评论）
    storeName?: string; // 店铺名称（如果是商家回复）
    storeImage?: string; // 店铺图片（如果是商家回复）
}

// =================================================================
// 步骤 3: API定义和切换逻辑
// =================================================================

// 3.1 ----------------- API实现 -----------------

// 3.2 ----------------- API实现 -----------------
import apiClient from '@/api/client';






// 3.2 ----------------- API实现 (修改后) -----------------

const api = {
    // --- 获取列表 (GET请求) - 这部分保持不变，因为它们返回的是数组 ---
    getAfterSalesList: () => apiClient.get<AfterSaleItem[]>('/admin/after-sales/mine').then(res => res.data),
    getComplaintsList: () => apiClient.get<ComplaintItem[]>('/admin/delivery-complaints/mine').then(res => res.data),
    getViolationsList: () => apiClient.get<ViolationItem[]>('/admin/penalties/mine').then(res => res.data),
    getReviewsList: () => apiClient.get<ReviewItem[]>('/admin/comments/mine').then(res => res.data),
    getAdminInfo: async () => {
        try {
            const response = await apiClient.get<any>('/admin/info');
            // 后端直接返回 GetAdminInfo 对象（camelCase），不是包装在 ApiResponseDto 中
            // 如果响应是 ApiResponseDto 格式，则提取 data 字段；否则直接返回响应数据
            const data = response.data;
            if (data && typeof data === 'object' && 'data' in data && 'success' in data) {
                // 如果包装在 ApiResponseDto 中，提取 data
                return data.data as AdminInfo;
            }
            // 直接返回响应数据
            return data as AdminInfo;
        } catch (error) {
            console.error('获取管理员信息失败:', error);
            throw error;
        }
    },

    // --- 更新数据 (PUT请求) ---
    // 【核心修改】将所有更新函数的返回值包装成 { success: boolean, data: T } 的格式

    updateAfterSale: async (item: AfterSaleItem) => {
        try {
            const response = await apiClient.put<AfterSaleItem>(`/admin/after-sales/update`, item);
            return { success: true, data: response.data }; // 成功时，包装成礼盒返回
        } catch (error) {
            console.error('更新售后失败:', error);
            // 失败时，也返回一个礼盒，但 success 为 false，data 可以是原始数据或 null
            return { success: false, data: item };
        }
    },

    updateComplaint: async (item: ComplaintItem) => {
        try {
            const response = await apiClient.put<ComplaintItem>(`/admin/delivery-complaints/update`, item);
            return { success: true, data: response.data };
        } catch (error) {
            console.error('更新投诉失败:', error);
            return { success: false, data: item };
        }
    },

    updateViolation: async (item: ViolationItem) => {
        try {
            const response = await apiClient.put<ViolationItem>(`/admin/penalties/update`, item);
            return { success: true, data: response.data };
        } catch (error) {
            console.error('更新违规失败:', error);
            return { success: false, data: item };
        }
    },

    updateReview: async (item: ReviewItem) => {
        try {
            const response = await apiClient.put<ReviewItem>(`/admin/comments/update`, item);
            return { success: true, data: response.data };
        } catch (error) {
            console.error('更新评论失败:', error);
            return { success: false, data: item };
        }
    },

    updateAdminInfo: async (data: Partial<AdminInfo>) => {
        try {
            const response = await apiClient.put<AdminInfo>('/admin/info', data);
            return { success: true, data: response.data };
        } catch (error) {
            console.error('更新管理员信息失败:', error);
            return { success: false, data: data as AdminInfo };
        }
    },
};

// 3.3 ----------------- API切换器 -----------------


// =================================================================
// 步骤 4: 组件状态和逻辑
// =================================================================

// 4.1 ----------------- 组件状态定义 -----------------
const activeMenu = ref('admin');

// 搜索和筛选的状态
const searchQuery = ref('');
const complaintSearchQuery = ref('');
const violationSearchQuery = ref('');
const reviewSearchQuery = ref('');
const selectedAfterSalesStatus = ref('all');
const selectedComplaintStatus = ref('all');
const selectedViolationStatus = ref('all');
const selectedReviewStatus = ref('all');

// 数据列表 (初始化为空数组，由API填充)
const afterSalesList = ref<AfterSaleItem[]>([]);
const complaintsList = ref<ComplaintItem[]>([]);
const violationsList = ref<ViolationItem[]>([]);
const reviewsList = ref<ReviewItem[]>([]);

// 弹窗和当前选中项的状态
const showAfterSaleDetail = ref(false);
const currentAfterSale = ref<AfterSaleItem | null>(null);
const showComplaintDetail = ref(false);
const currentComplaint = ref<ComplaintItem | null>(null);
const showViolationDetail = ref(false);
const currentViolation = ref<ViolationItem | null>(null);
const showReviewDetail = ref(false);
const currentReview = ref<ReviewItem | null>(null);

// 计算属性：从评论数据中提取图片数组
const reviewImages = computed(() => {
    if (!currentReview.value?.image) return [];
    return currentReview.value.image
        .split(',')
        .map(img => img.trim())
        .filter(img => img.length > 0);
});

// 图片预览
const previewImageUrl = ref<string | null>(null);
const previewImage = (url: string) => {
    previewImageUrl.value = url;
};
const closePreview = () => {
    previewImageUrl.value = null;
};
// 处罚措施选项的状态
const selectedPunishment = ref('');
const punishmentReason = ref('');
const selectedComplaintPunishment = ref('');
const complaintPunishmentReason = ref('');
const selectedMerchantPunishment = ref('');
const selectedStorePunishment = ref('');
const complaintFine = ref<number | undefined>(undefined);

const router = useRouter(); // 【新增】获取 router 实例

// 【新增】创建一个响应式变量来存储当前管理员的信息
const currentUser = ref<AdminInfo | null>(null);
// 【新增】创建一个响应式变量来备份初始数据，用于"重置"功能和修改检测
const originalAdminInfo = ref<AdminInfo | null>(null);
// 【新增】一个加载状态，提升用户体验
const isSaving = ref(false);

// 【新增】计算属性：检查是否有修改
const hasChanges = computed(() => {
    if (!currentUser.value || !originalAdminInfo.value) {
        return false;
    }
    
    return (
        currentUser.value.username !== originalAdminInfo.value.username ||
        currentUser.value.managementScope !== originalAdminInfo.value.managementScope ||
        currentUser.value.gender !== originalAdminInfo.value.gender
    );
});


// 静态数据
const commonStatuses = [{ label: '全部', value: 'all' }, { label: '待处理', value: '待处理' }, { label: '已完成', value: '已完成' }];
const afterSalesStatuses = [
    { label: '全部', value: 'all' },
    { label: '商家未回复', value: '未回复' },
    { label: '商家已回复', value: '已回复' },
    { label: '已完成', value: '已完成' }
];
const complaintStatuses = commonStatuses;
const violationStatuses = [{ label: '全部', value: 'all' }, { label: '待处理', value: '待处理' }, { label: '已完成', value: '已完成' }];
const reviewStatuses = [{ label: '全部', value: 'all' }, { label: '待处理', value: '待处理' }, { label: '通过', value: '通过' }];
const punishmentOptions = { afterSales: [{ label: '全额退款', value: 'full_refund' }, { label: '部分退款', value: 'partial_refund' }, { label: '重新配送', value: 'redelivery' }, { label: '商家道歉', value: 'apology' }, { label: '赔偿用户', value: 'compensation' }], complaints: [{ label: '警告处分', value: 'warning' }, { label: '暂停接单3天', value: 'suspend_3days' }, { label: '暂停接单7天', value: 'suspend_7days' }, { label: '罚款处理', value: 'fine' }, { label: '终止合作', value: 'terminate' }], violations: { merchant: [{ label: '口头警告', value: 'verbal_warning' }, { label: '书面警告', value: 'written_warning' }, { label: '罚款500元', value: 'fine_500' }, { label: '罚款1000元', value: 'fine_1000' }], store: [{ label: '限期整改', value: 'correction' }, { label: '暂停营业3天', value: 'suspend_3days' }, { label: '暂停营业7天', value: 'suspend_7days' }, { label: '永久下架', value: 'permanent_removal' }] }, reviews: [{ label: '通过审核', value: 'approve' }, { label: '删除评论', value: 'delete' }, { label: '禁止评论7天', value: 'ban_7days' }, { label: '禁止评论30天', value: 'ban_30days' }, { label: '永久禁言', value: 'permanent_ban' }] };

// 辅助函数：将处罚 value 转换为 label
const getPunishmentLabel = (value: string | undefined, type: 'merchant' | 'store' | 'complaint' | 'afterSale'): string => {
    if (!value) return '未记录';
    
    let options: Array<{ label: string; value: string }> = [];
    if (type === 'merchant') options = punishmentOptions.violations.merchant;
    else if (type === 'store') options = punishmentOptions.violations.store;
    else if (type === 'complaint') options = punishmentOptions.complaints;
    else if (type === 'afterSale') options = punishmentOptions.afterSales;
    
    const found = options.find(opt => opt.value === value);
    return found?.label || value;
};

// 4.2 ----------------- 数据获取 -----------------
onMounted(async () => {
    console.log('API 模式: 真实 (Real)');
    try {
        // 【修改】使用 Promise.all 并行加载所有数据，包括管理员信息
        const [
            adminInfo,
            afterSales,
            complaints,
            violations,
            reviews
        ] = await Promise.all([
            api.getAdminInfo(), // <--- 调用获取管理员信息的 API
            api.getAfterSalesList(),
            api.getComplaintsList(),
            api.getViolationsList(),
            api.getReviewsList(),
        ]);

        // 【新增】填充管理员信息数据模型
        console.log('获取到的管理员信息:', adminInfo);
        if (adminInfo) {
            // 确保性别字段格式正确（后端可能返回 'M'/'F'，前端期望 '男'/'女'）
            const genderMap: Record<string, '男' | '女'> = {
                'M': '男',
                'F': '女',
                '男': '男',
                '女': '女',
                '': '男' // 空字符串默认为男
            };
            if (!adminInfo.gender || !['男', '女'].includes(adminInfo.gender)) {
                adminInfo.gender = genderMap[adminInfo.gender] || '男';
            }
            currentUser.value = adminInfo;
            originalAdminInfo.value = JSON.parse(JSON.stringify(adminInfo)); // 深拷贝备份，用于重置
        } else {
            ElMessage.warning('未能获取管理员信息');
        }

        // 填充其他列表数据
        afterSalesList.value = afterSales;
        complaintsList.value = complaints;
        violationsList.value = violations;
        reviewsList.value = reviews;
    } catch (error: any) {
        const errorMsg = error?.response?.data?.message || error?.message || '数据加载失败';
        ElMessage.error(`数据加载失败: ${errorMsg}`);
        console.error('数据加载失败:', error);
        if (error?.response) {
            console.error('响应状态:', error.response.status);
            console.error('响应数据:', error.response.data);
        }
    }
});

// 4.3 ----------------- 计算属性和工具函数 (不变) -----------------
const getAfterSaleStatusText = (status: string) => {
    // 统一映射为三种：商家未回复 / 商家已回复 / 已完成
    if (status === '商家反馈' || status === '待审核') return '商家已回复';
    if (status === '待处理') return '商家未回复';
    return status; // 已完成 或 其它保持原样（默认后端已是"已完成"）
};

const getAfterSaleFilterKey = (status: string) => {
    const t = getAfterSaleStatusText(status);
    if (t === '商家未回复') return '未回复';
    if (t === '商家已回复') return '已回复';
    if (t === '已完成') return '已完成';
    return '其它';
};

const filteredAfterSales = computed(() =>
    afterSalesList.value.filter(item => {
        const matchSearch = (item.applicationId?.toLowerCase() || '').includes((searchQuery.value?.toLowerCase() || '')) ||
            (item.orderId?.toLowerCase() || '').includes((searchQuery.value?.toLowerCase() || ''));
        if (selectedAfterSalesStatus.value === 'all') return matchSearch;
        return matchSearch && (getAfterSaleFilterKey(item.status) === selectedAfterSalesStatus.value);
    })
);

const filteredComplaints = computed(() =>
    complaintsList.value.filter(item =>
        (selectedComplaintStatus.value === 'all' || item.status === selectedComplaintStatus.value) &&
        ((item.complaintId?.toLowerCase() || '').includes((complaintSearchQuery.value?.toLowerCase() || '')) ||
            (item.target?.toLowerCase() || '').includes((complaintSearchQuery.value?.toLowerCase() || '')))
    )
);

const filteredViolations = computed(() =>
    violationsList.value.filter(item =>
        (selectedViolationStatus.value === 'all' || item.status === selectedViolationStatus.value) &&
        ((item.punishmentId?.toLowerCase() || '').includes((violationSearchQuery.value?.toLowerCase() || '')) ||
            (item.storeName?.toLowerCase() || '').includes((violationSearchQuery.value?.toLowerCase() || '')))
    )
);

const filteredReviews = computed(() =>
    reviewsList.value.filter(item =>
        (selectedReviewStatus.value === 'all' || item.status === selectedReviewStatus.value) &&
        ((item.content?.toLowerCase() || '').includes((reviewSearchQuery.value?.toLowerCase() || '')) ||
            (item.username?.toLowerCase() || '').includes((reviewSearchQuery.value?.toLowerCase() || '')))
    )
);

const getBreadcrumb = () => ({ admin: '管理员信息', afterSales: '售后处理中心', complaints: '投诉处理中心', violations: '违规举报处理', reviews: '评论审核管理' })[activeMenu.value] || '控制台';
const getStatusClass = (status: string) => ({
    '商家未回复': 'bg-gray-100 text-gray-800',
    '商家已回复': 'bg-yellow-100 text-yellow-800',
    '已完成': 'bg-green-100 text-green-800',
    '通过': 'bg-green-100 text-green-800',
    '待处理': 'bg-yellow-100 text-yellow-800',
    '待审核': 'bg-yellow-100 text-yellow-800',
    '审核通过': 'bg-green-100 text-green-800',
    '违规': 'bg-red-100 text-red-800',
})[status] || 'bg-gray-100 text-gray-800';

const openAfterSaleDetail = async (item: AfterSaleItem) => {
    // 先用列表数据占位，避免空白闪烁
    currentAfterSale.value = { ...item };
    showAfterSaleDetail.value = true;

    // 拉取完整详情（含用户、图片、菜品）
    try {
        const detailRes = await apiClient.get<AfterSaleItem>(`/admin/after-sales/${Number(item.applicationId)}`);
        const detail = detailRes.data;
        if (detail) {
            currentAfterSale.value = { ...currentAfterSale.value, ...detail };
        }
    } catch (e) {
        // 忽略详情加载失败，仍然显示基础信息
        console.error('加载售后详情失败', e);
    }

    if (getAfterSaleStatusText(currentAfterSale.value.status) === '商家已回复') {
        selectedPunishment.value = '';
        punishmentReason.value = '';
    } else {
        selectedPunishment.value = currentAfterSale.value.punishment || '';
        punishmentReason.value = currentAfterSale.value.punishmentReason || '';
    }
};

const openComplaintDetail = (item: ComplaintItem) => {
    currentComplaint.value = { ...item };
    if (item.status === '待处理') {
        selectedComplaintPunishment.value = '';
        complaintPunishmentReason.value = '';
        complaintFine.value = 0.00; 
    } else {
        selectedComplaintPunishment.value = item.punishment || '';
        complaintPunishmentReason.value = item.punishmentReason || '';
        complaintFine.value = item.fine ? Number(item.fine) : 0.00;
    }
    showComplaintDetail.value = true;
};

const openViolationDetail = (item: ViolationItem) => {
    currentViolation.value = { ...item };
    
    if (item.status === '待处理') {
        // 待处理：清空输入框，准备填写
        selectedMerchantPunishment.value = '';
        selectedStorePunishment.value = '';
    } else {
        // 已完成：填充已保存的数据，供查看
        console.log('原始数据:', item);
        selectedMerchantPunishment.value = item.merchantPunishment === '-' ? '' : item.merchantPunishment;
        selectedStorePunishment.value = item.storePunishment === '-' ? '' : item.storePunishment;
    }
    
    showViolationDetail.value = true;
};


const openReviewDetail = (item: ReviewItem) => { currentReview.value = { ...item }; showReviewDetail.value = true; };

// 获取显示名称（姓氏+先生/女士）
const getDisplayName = () => {
    if (!currentUser.value) return '管理员';
    const realName = currentUser.value.realName || '';
    const gender = currentUser.value.gender || '男';
    
    if (realName) {
        const surname = realName.charAt(0); // 取第一个字符作为姓氏
        const honorific = gender === '女' ? '女士' : '先生';
        return `${surname}${honorific}`;
    }
    
    return currentUser.value.username || '管理员';
}

// 下拉框相关的数据和方法
const dropdownVisible = ref(false)
const managementOptions = ['售后处理', '配送投诉', '商家举报', '评论审核']

const toggleDropdown = () => {
  dropdownVisible.value = !dropdownVisible.value
}

const getSelectedText = () => {
  if (!currentUser.value?.managementScope) return '请选择管理对象'
  
  // 将用户的管理范围按分隔符拆分
  const userSelections = currentUser.value.managementScope.split('、')
  
  // 只保留在当前选项列表中的项目
  const validSelections = userSelections.filter(selection => 
    managementOptions.includes(selection)
  )
  
  // 如果没有有效选项，显示默认文本
  if (validSelections.length === 0) return '请选择管理对象'
  
  // 返回有效的选项，用顿号连接
  return validSelections.join('、')
}

const isSelected = (option: string) => {
  if (!currentUser.value?.managementScope) return false
  return currentUser.value.managementScope.split('、').includes(option)
}

const toggleOption = (option: string) => {
  if (!currentUser.value) return
  
  let selected = currentUser.value.managementScope ? currentUser.value.managementScope.split('、') : []
  const index = selected.indexOf(option)
  
  if (index > -1) {
    selected.splice(index, 1)
  } else {
    selected.push(option)
  }
  
  currentUser.value.managementScope = selected.join('、')
}

// 4.4 ----------------- 修改数据处理函数 (全部完成) -----------------

const handleAfterSaleAction = async () => {
    if (!selectedPunishment.value || !punishmentReason.value.trim()) {
        return ElMessage.warning('请填写完整的处理信息和处罚原因');
    }
    if (!currentAfterSale.value) return;

    try {
        await ElMessageBox.confirm('确定要执行选定的处罚措施吗？', '确认操作', { type: 'warning' });

        const punishmentLabel = punishmentOptions.afterSales.find(o => o.value === selectedPunishment.value)?.label || selectedPunishment.value;
        const updatedItem: AfterSaleItem = {
            ...currentAfterSale.value,
            status: '已完成',
            punishment: punishmentLabel,
            punishmentReason: punishmentReason.value
        };

        // 【核心修改】接收API的返回结果
        const response = await api.updateAfterSale(updatedItem);

        // 【核心修改】检查返回结果的 success 字段
        if (response.success) {
            // 只有在后端确认成功后，才更新前端的UI
            const index = afterSalesList.value.findIndex(item => item.applicationId === updatedItem.applicationId);
            if (index !== -1) {
                // 使用从后端返回的最新数据来更新列表，这是最佳实践
                afterSalesList.value[index] = { ...updatedItem, ...response.data };
            }
            ElMessage.success('处理完成，处罚措施已执行');
            showAfterSaleDetail.value = false;
        } else {
            // 如果后端返回失败，则提示用户
            ElMessage.error('操作失败，数据未能成功保存到服务器');
        }

    } catch (error) {
        // 这个 catch 现在主要捕获 ElMessageBox 的 "cancel" 行为
        if (error !== 'cancel') {
            console.error("处理售后时发生未知错误:", error);
            ElMessage.error('操作失败');
        } else {
            ElMessage.info('操作已取消');
        }
    }
};


/**
 * 【新增】处理保存修改的函数
 */
/**
 * 【新增】处理保存修改的函数
 */
const handleSaveChanges = async () => {
    if (!currentUser.value) return;

    isSaving.value = true;
    try {
        // 只发送后端需要的字段
        const updateData = {
            username: currentUser.value.username,
            managementScope: currentUser.value.managementScope,
            gender: currentUser.value.gender
        };
        const response = await api.updateAdminInfo(updateData);

        if (response.success) {
            // 【核心修改】使用 Object.assign 来更新现有响应式对象的属性
            // 这样做可以更稳定地触发视图更新
            Object.assign(currentUser.value, response.data);

            originalAdminInfo.value = JSON.parse(JSON.stringify(response.data));

            ElMessage.success('信息更新成功！');
            console.log('管理员信息已更新:', response.data);
        } else {
            ElMessage.error('信息更新失败，未能成功保存到服务器');
            console.error('信息更新失败:', response);
        }

    } catch (error) {
        console.error('更新失败:', error);
        ElMessage.error('信息更新失败，请稍后再试。');
    } finally {
        isSaving.value = false;
    }
};

/**
 * 【新增】重置表单的函数
 */
const resetForm = () => {
    if (originalAdminInfo.value) {
        currentUser.value = JSON.parse(JSON.stringify(originalAdminInfo.value)); // 从备份恢复
        ElMessage.info('表单已重置');
    }
};

/**
 * 【新增】处理登出的函数
 */
const handleLogout = () => {
    // 弹出确认框，防止误触
    ElMessageBox.confirm('您确定要退出登录吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning',
    }).then(() => {
        // 1. 清除本地存储的所有认证信息
        localStorage.removeItem('authToken');
        localStorage.removeItem('userInfo'); // 如果你存了用户信息，也要一并清除

        // 2. 显示成功提示
        ElMessage.success('您已成功退出登录');

        // 3. 跳转回登录页面
        router.push('/login'); // 假设你的登录页路由是 '/login'
    }).catch(() => {
        // 用户点击了取消，不做任何事
        ElMessage.info('已取消退出');
    });
};


const handleComplaintProcess = async () => {
    if (!selectedComplaintPunishment.value || !complaintPunishmentReason.value.trim()) {
        return ElMessage.warning('请填写完整的处理信息和处罚原因');
    }
    if (!currentComplaint.value) return;

    try {
        await ElMessageBox.confirm('确定要处理该投诉吗？', '确认操作', { type: 'warning' });

        const punishmentLabel = punishmentOptions.complaints.find(o => o.value === selectedComplaintPunishment.value)?.label || selectedComplaintPunishment.value;
        const updatedItem: ComplaintItem = {
            ...currentComplaint.value,
            status: '已完成',
            punishment: punishmentLabel,
            punishmentReason: complaintPunishmentReason.value,
            fine: complaintFine.value !== undefined ? String(complaintFine.value) : ''
        };

        // 【核心修改】接收API的返回结果
        const response = await api.updateComplaint(updatedItem);

        // 【核心修改】检查返回结果的 success 字段
        if (response.success) {
            // 只有在后端确认成功后，才更新前端的UI
            const index = complaintsList.value.findIndex(item => item.complaintId === updatedItem.complaintId);
            if (index !== -1) {
                // 使用从后端返回的最新数据来更新列表
                complaintsList.value[index] = { ...updatedItem, ...response.data };
            }
            ElMessage.success('投诉已处理完成');
            showComplaintDetail.value = false;
        } else {
            // 如果后端返回失败，则提示用户
            ElMessage.error('操作失败，数据未能成功保存到服务器');
        }

    } catch (error) {
        // 这个 catch 主要捕获 ElMessageBox 的 "cancel" 行为
        if (error !== 'cancel') {
            console.error("处理投诉时发生未知错误:", error);
            ElMessage.error('操作失败');
        } else {
            ElMessage.info('操作已取消');
        }
    }
};

const handleViolationAction = async (action: 'complete') => {
    if (!currentViolation.value) return;
    if (!selectedMerchantPunishment.value || !selectedStorePunishment.value) {
        return ElMessage.warning('请选择商家和店铺的处罚措施');
    }

    try {
        await ElMessageBox.confirm('确定要完成处理该处罚吗？', '确认操作', { type: 'warning' });

        const updatedItem: ViolationItem = {
            ...currentViolation.value,
            status: '已完成',
            merchantPunishment: selectedMerchantPunishment.value,
            storePunishment: selectedStorePunishment.value
        };

        // 【核心修改】接收API的返回结果
        const response = await api.updateViolation(updatedItem);

        // 【核心修改】检查返回结果的 success 字段
        if (response.success) {
            // 只有在后端确认成功后，才更新前端的UI
            const index = violationsList.value.findIndex(item => item.punishmentId === updatedItem.punishmentId);
            if (index !== -1) {
                // 使用从后端返回的最新数据来更新列表
                violationsList.value[index] = { ...updatedItem, ...response.data };
                console.log('更新后的数据:', violationsList.value[index]);
            }
            ElMessage.success('处罚已执行完成');
            showViolationDetail.value = false;
        } else {
            // 如果后端返回失败，则提示用户
            ElMessage.error('操作失败，数据未能成功保存到服务器');
        }

    } catch (error) {
        if (error !== 'cancel') {
            console.error("处理违规时发生未知错误:", error);
            ElMessage.error('操作失败');
        } else {
            ElMessage.info('操作已取消');
        }
    }
};

const processReview = async (decision: 'approve' | 'reject') => {
    if (!currentReview.value) return;

    // 根据决定设置提示文字和最终的处罚说明
    const newStatus = decision === 'approve' ? '通过' : '违规';
    const actionText = decision === 'approve' ? '审核通过' : '判定违规'

    try {
        // 弹出确认框
        await ElMessageBox.confirm(`确定要将此评论标记为"${actionText}"吗？`, '确认操作', { type: 'warning' });

        // 准备要发送到后端的数据
        const updatedItem: ReviewItem = {
            ...currentReview.value,
            status: newStatus
        };

        // 调用API
        const response = await api.updateReview(updatedItem);

        // 处理API返回结果
        if (response.success) {
            const index = reviewsList.value.findIndex(item => item.reviewId === updatedItem.reviewId);
            if (index !== -1) {
                reviewsList.value[index] = { ...updatedItem, ...response.data };
            }
            ElMessage.success(`操作成功，评论已${actionText}`);
            showReviewDetail.value = false; // 关闭弹窗
        } else {
            ElMessage.error('操作失败，数据未能成功保存到服务器');
        }

    } catch (error) {
        if (error !== 'cancel') {
            console.error(`处理评论(${actionText})时发生未知错误:`, error);
            ElMessage.error('操作失败');
        } else {
            ElMessage.info('操作已取消');
        }
    }
};

const isAfterSaleEditable = computed(() => getAfterSaleStatusText(currentAfterSale.value?.status || '') === '商家已回复');

</script>
<style scoped>
.\!rounded-button {
    border-radius: 0.5rem;
}

/* 隐藏数字输入框的箭头 */
input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
}

input[type="number"] {
    -moz-appearance: textfield;
    appearance: textfield;
}

/* 滚动条样式 */
::-webkit-scrollbar {
    width: 6px;
    height: 6px;
}

::-webkit-scrollbar-track {
    background: #f1f1f1;
    border-radius: 3px;
}

::-webkit-scrollbar-thumb {
    background: #c1c1c1;
    border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
    background: #a8a8a8;
}
</style>

<style scoped>
/* 取消禁用时的"变暗"效果（仅用于处罚措施选择框） */
.no-dim-disabled :deep(.el-select.is-disabled .el-input__wrapper) {
  opacity: 1 !important;
  background-color: rgba(255, 255, 255, 0.8) !important;
  filter: none !important;
}
.no-dim-disabled :deep(.el-select.is-disabled .el-input__inner) {
  -webkit-text-fill-color: inherit !important;
  color: inherit !important;
}

.no-dim-disabled.select-readonly :deep(.el-input__wrapper),
.no-dim-disabled :deep(.el-select.is-disabled .el-input__wrapper) {
   opacity: 1 !important;
   background-color: rgba(255, 255, 255, 0.8) !important;
   filter: none !important;
 }
.no-dim-disabled.select-readonly :deep(.el-input__inner),
.no-dim-disabled :deep(.el-select.is-disabled .el-input__inner) {
   -webkit-text-fill-color: inherit !important;
   color: inherit !important;
 }
/* 兼容文本域禁用/只读的不变暗（仅在容器带有 no-dim-disabled 时生效） */
.no-dim-disabled :deep(.el-textarea.is-disabled .el-textarea__inner),
.no-dim-disabled :deep(.el-textarea .el-textarea__inner[readonly]) {
  opacity: 1 !important;
  background-color: rgba(255, 255, 255, 0.8) !important;
  color: inherit !important;
  -webkit-text-fill-color: inherit !important;
}

/* 非编辑态下隐藏下拉箭头 */
.select-readonly :deep(.el-input__suffix),
.select-readonly :deep(.el-select__caret) {
  display: none !important;
}

</style>