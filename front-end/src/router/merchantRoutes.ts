import { RouteRecordRaw } from 'vue-router'
import MerchantHomeView from '@/features/merchant/views/MerchantHomeView.vue'
import MerchantOrdersView from '@/features/merchant/views/MerchantOrdersView.vue'
import MerchantMenuView from '@/features/merchant/views/MerchantMenuView.vue'
import MerchantCouponsView from '@/features/merchant/views/MerchantCouponsView.vue'
import MerchantAftersaleView from '@/features/merchant/views/MerchantAftersaleView.vue'
import MerchantProfileView from '@/features/merchant/views/MerchantProfileView.vue'

const merchantRoutes: Array<RouteRecordRaw> = [
    {
        path: '/MerchantHome',
        name: 'MerchantHome',
        component: MerchantHomeView,
        meta: { title: '商家主页' }
    },
    {
        path: '/MerchantOrders',
        name: 'MerchantOrders',
        component: MerchantOrdersView,
        meta: { title: '商家订单页' }
    },
    {
        path: '/MerchantMenu',
        name: 'MerchantMenu',
        component: MerchantMenuView,
        meta: { title: '商家菜品管理页' }
    },
    {
        path: '/MerchantCoupons',
        name: 'MerchantCoupons',
        component: MerchantCouponsView,
        meta: { title: '商家配券页' }
    },
    {
        path: '/MerchantAftersale',
        name: 'MerchantAftersale',
        component: MerchantAftersaleView,
        meta: { title: '商家售后页' }
    },
    {
        path: '/MerchantProfile',
        name: 'MerchantProfile',
        component: MerchantProfileView,
        meta: { title: '商家个人页' }
    }
];

export default merchantRoutes;