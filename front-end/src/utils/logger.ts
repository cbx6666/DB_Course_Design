// src/utils/logger.ts

// 日志级别
export enum LogLevel {
    DEBUG = 0,
    INFO = 1,
    WARN = 2,
    ERROR = 3,
}

// 当前日志级别（生产环境可以设置为WARN或ERROR）
const CURRENT_LOG_LEVEL = process.env.NODE_ENV === 'development' ? LogLevel.DEBUG : LogLevel.WARN;

/**
 * 统一的日志工具
 */
export const logger = {
    debug: (message: string, ...args: any[]) => {
        if (CURRENT_LOG_LEVEL <= LogLevel.DEBUG) {
            console.debug(`[DEBUG] ${message}`, ...args);
        }
    },

    info: (message: string, ...args: any[]) => {
        if (CURRENT_LOG_LEVEL <= LogLevel.INFO) {
            console.info(`[INFO] ${message}`, ...args);
        }
    },

    warn: (message: string, ...args: any[]) => {
        if (CURRENT_LOG_LEVEL <= LogLevel.WARN) {
            console.warn(`[WARN] ${message}`, ...args);
        }
    },

    error: (message: string, ...args: any[]) => {
        if (CURRENT_LOG_LEVEL <= LogLevel.ERROR) {
            console.error(`[ERROR] ${message}`, ...args);
        }
    },
};

/**
 * 开发环境专用的调试日志
 */
export const devLog = {
    api: (message: string, ...args: any[]) => {
        if (process.env.NODE_ENV === 'development') {
            console.log(`[API] ${message}`, ...args);
        }
    },

    component: (componentName: string, message: string, ...args: any[]) => {
        if (process.env.NODE_ENV === 'development') {
            console.log(`[${componentName}] ${message}`, ...args);
        }
    },

    user: (message: string, ...args: any[]) => {
        if (process.env.NODE_ENV === 'development') {
            console.log(`[USER] ${message}`, ...args);
        }
    },

    error: (message: string, ...args: any[]) => {
        if (process.env.NODE_ENV === 'development') {
            console.error(`[ERROR] ${message}`, ...args);
        }
    },
};
