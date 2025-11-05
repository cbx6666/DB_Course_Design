import apiClient from '../client';

/**
 * 发布配送任务
 * @param orderId 订单ID
 * @param estimatedArrivalTime 预计到达时间
 * @param estimatedDeliveryTime 预计配送时间
 */
export const publishDeliveryTask = async (orderId: number, estimatedArrivalTime: string, estimatedDeliveryTime: string) => {
    const response = await apiClient.post('/merchant/delivery-tasks/publish', {
        OrderId: orderId,
        EstimatedArrivalTime: estimatedArrivalTime,
        EstimatedDeliveryTime: estimatedDeliveryTime
    });
    return response.data;
};

/**
 * 获取订单配送信息
 * @param orderId 订单ID
 */
export const getOrderDeliveryInfo = async (orderId: number) => {
    const response = await apiClient.get(`/merchant/delivery-tasks/order/${orderId}`);
    return response.data;
};

