// src/api/client.ts
import axios from 'axios';
import { API_CONFIG } from '@/config';

// 统一的API配置
const CLIENT_CONFIG = {
  baseURL: `${API_CONFIG.BASE_URL}${API_CONFIG.API_PREFIX}`,
  timeout: API_CONFIG.TIMEOUT,
  headers: {
    'Content-Type': 'application/json'
  }
};

// --- Axios 实例和拦截器 ---
const apiClient = axios.create(CLIENT_CONFIG);

/**
 * 请求拦截器: 在每个请求发送前自动添加 Token
 */
apiClient.interceptors.request.use(
  config => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => {
    return Promise.reject(error);
  }
);

/**
 * 响应拦截器: 统一处理错误响应
 */
apiClient.interceptors.response.use(
  response => response,
  error => {
    // 统一错误处理
    if (error.response?.status === 401) {
      // Token过期，清除本地存储并跳转到登录页
      localStorage.removeItem('authToken');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// 导出配置好的客户端
export default apiClient;
export { CLIENT_CONFIG };