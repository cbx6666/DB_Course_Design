import { RouteRecordRaw } from 'vue-router';
import UserHomeView from '@/views/user/Homepage/UserHome.vue'
import UserRestaurantsView from '@/views/user/Homepage/UserRestaurants.vue'
import UserOrderView from '@/views/user/Homepage/UserOrders.vue'
import UserCouponsView from '@/views/user/Homepage/UserCoupons.vue'
import UserAfterSaleView from '@/views/user/Homepage/UserAfterSale.vue'
import HomeLayout from '@/views/user/Homepage/HomeLayout.vue';

const userRoutes: Array<RouteRecordRaw> = [
  {
    path: '/home',
    redirect: '/home/intro'
  },

  {
    path: '/home',
    component: HomeLayout,
    children: [
      {
        path: 'intro',
        name: 'Intro',
        component: UserHomeView,
        meta: { title: '首页' }
      },
      {
        path: 'restaurants',
        name: 'Restaurant',
        component: UserRestaurantsView,
        meta: { title: '商家' }
      },
      {
        path: 'orders',
        name: 'Order',
        component: UserOrderView,
        meta: { title: '订单' }
      },
      {
        path: 'coupons',
        name: 'Coupons',
        component: UserCouponsView,
        meta: { title: '优惠' }
      },
      {
        path: 'after-sale',
        name: 'AfterSale',
        component: UserAfterSaleView,
        meta: { title: '售后' }
      }
    ]
  }
];

export default userRoutes;
