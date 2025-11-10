import { getData } from '@/api/multiuse_function'

/**
 * 订单菜品详情
 */
export interface OrderDishItem {
    dishName: string
    dishImage: string
    quantity: number
    price: number
}

/**
 * 售后申请列表项
 */
export interface AfterSaleListItem {
    applicationId: number
    orderId: number
    storeName: string
    description: string
    images: string[]
    applicationTime: string
    status: string
    processingResult?: string
    processingReason?: string
    dishDetails: OrderDishItem[]
}

/**
 * 配送投诉列表项
 */
export interface DeliveryComplaintListItem {
    complaintId: number
    orderId: number
    deliveryTaskId: number
    complaintReason: string
    images: string[]
    complaintTime: string
    status: string
    processingResult?: string
    processingReason?: string
}

/**
 * 店铺举报列表项
 */
export interface StoreReportListItem {
    penaltyId: number
    storeId: number
    storeName: string
    content: string
    images: string[]
    reportTime: string
    status: string
    merchantPunishment?: string
    storePunishment?: string
    processingReason?: string
}

/**
 * 评论列表项
 */
export interface CommentListItem {
    commentId: number
    orderId?: number
    storeId: number
    storeName: string
    rating: number
    content: string
    images: string[]
    postedAt: string
    status: string
    dishDetails: OrderDishItem[]
}

/**
 * 获取用户的售后申请列表
 */
export async function getMyAfterSales(): Promise<AfterSaleListItem[]> {
    const response = await getData<AfterSaleListItem[]>('/customer/after-sales/mine')
    return response || []
}

/**
 * 获取用户的配送投诉列表
 */
export async function getMyDeliveryComplaints(): Promise<DeliveryComplaintListItem[]> {
    const response = await getData<DeliveryComplaintListItem[]>('/customer/delivery-complaints/mine')
    return response || []
}

/**
 * 获取用户的店铺举报列表
 */
export async function getMyStoreReports(): Promise<StoreReportListItem[]> {
    const response = await getData<StoreReportListItem[]>('/customer/store-reports/mine')
    return response || []
}

/**
 * 获取用户的评论列表
 */
export async function getMyComments(): Promise<CommentListItem[]> {
    const response = await getData<CommentListItem[]>('/customer/comments/mine')
    return response || []
}

