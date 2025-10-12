// src/config/index.ts

// 环境变量类型声明
interface ImportMetaEnv {
    readonly VITE_API_BASE_URL?: string;
    readonly MODE: string;
}

interface ImportMeta {
    readonly env: ImportMetaEnv;
}

// API配置
export const API_CONFIG = {
    // 基础URL，可以通过环境变量覆盖
    BASE_URL: import.meta.env.VITE_API_BASE_URL || 'http://localhost:5250',
    API_PREFIX: '/api',
    TIMEOUT: 10000,

    // 默认头像
    DEFAULT_AVATAR: '/images/default-avatar.jpg',

    // 文件上传
    UPLOAD_MAX_SIZE: 5 * 1024 * 1024, // 5MB
    ALLOWED_IMAGE_TYPES: ['image/jpeg', 'image/png', 'image/gif', 'image/webp'],
};

// 应用配置
export const APP_CONFIG = {
    NAME: '外卖管理系统',
    VERSION: '1.0.0',

    // 分页配置
    PAGE_SIZE: 10,
    PAGE_SIZE_OPTIONS: [10, 20, 50, 100],

    // 验证码倒计时
    CODE_COUNTDOWN: 60,

    // 自动保存间隔（毫秒）
    AUTO_SAVE_INTERVAL: 30000,
};

// 路由配置
export const ROUTE_CONFIG = {
    // 默认路由
    DEFAULT_ROUTE: '/',
    LOGIN_ROUTE: '/login',

    // 角色路由映射
    ROLE_ROUTES: {
        customer: '/',
        rider: '/courier',
        merchant: '/merchant',
        admin: '/admin',
    },
};

// 状态配置
export const STATUS_CONFIG = {
    // 订单状态
    ORDER_STATUS: {
        PENDING: 'pending',
        CONFIRMED: 'confirmed',
        PREPARING: 'preparing',
        READY: 'ready',
        DELIVERING: 'delivering',
        COMPLETED: 'completed',
        CANCELLED: 'cancelled',
    },

    // 用户角色
    USER_ROLES: {
        CUSTOMER: 'customer',
        RIDER: 'rider',
        MERCHANT: 'merchant',
        ADMIN: 'admin',
    },
};
