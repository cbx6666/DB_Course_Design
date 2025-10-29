export * from './account';
export * from './address';
export * from './checkout';
export * from './store-info';
export * from './home';
export * from './store';
export * from './coupon';
export * from './comment';
export * from './report';
export * from './account-utils';

// 导出优惠券相关的API
export { getAvailableCoupons, claimCoupon, type AvailableCoupon, type ClaimCouponResponse } from './coupon';

// Legacy modules no longer exported; all users should import from '@/api/user'.
