using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;
using BackEnd.DTOs.Dish;

namespace BackEnd.DTOs.AfterSaleApplication
{
    // ========== 消费者侧 ==========

    /// <summary>
    /// 创建售后申请请求DTO（消费者端）
    /// </summary>
    public class CreateAfterSaleApplicationDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 申请图片URL列表（多个图片用逗号分隔）
        /// </summary>
        public string? Images { get; set; }
    }

    /// <summary>
    /// 创建售后申请响应DTO（带申请ID）
    /// </summary>
    public class CreateAfterSaleApplicationResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public int? ApplicationId { get; set; }
    }

    /// <summary>
    /// 用户售后申请列表项DTO（消费者端查看自己的申请）
    /// </summary>
    public class CustomerAfterSaleListItemDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public int ApplicationId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 申请图片URL列表
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime ApplicationTime { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处理结果
        /// </summary>
        public string? ProcessingResult { get; set; }

        /// <summary>
        /// 处理原因
        /// </summary>
        public string? ProcessingReason { get; set; }

        /// <summary>
        /// 商家回复
        /// </summary>
        public string? MerchantReply { get; set; }

        /// <summary>
        /// 消费者评分（0-5）
        /// </summary>
        public int? ConsumerRating { get; set; }

        /// <summary>
        /// 订单菜品列表
        /// </summary>
        public List<OrderDishDto> DishDetails { get; set; } = new List<OrderDishDto>();
    }

    // ========== 商家侧 ==========

    /// <summary>
    /// 售后申请列表项DTO（商家端展示）
    /// </summary>
    public class AfterSaleApplicationListItemDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; } = null!;

        /// <summary>
        /// 订单ID
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public UserProfileDto User { get; set; } = null!;

        /// <summary>
        /// 账户用户名（与收货人真实姓名区分）
        /// </summary>
        public string? AccountUserName { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 申请图片URL列表
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 申请时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;

        /// <summary>
        /// 订单菜品详情
        /// </summary>
        public List<OrderDishDto> DishDetails { get; set; } = new List<OrderDishDto>();

        /// <summary>
        /// 状态（待处理/商家反馈/已完成）
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// 商家回复
        /// </summary>
        public string? MerchantReply { get; set; }

        /// <summary>
        /// 管理员处理措施（已完成时返回）
        /// </summary>
        public string? Punishment { get; set; }

        /// <summary>
        /// 管理员处理原因（已完成时返回）
        /// </summary>
        public string? PunishmentReason { get; set; }
    }

    /// <summary>
    /// 商家回复请求 DTO（仅支持在待处理状态下填写商家回复）
    /// </summary>
    public class MerchantReplyDto
    {
        /// <summary>
        /// 商家回复内容
        /// </summary>
        public string Remark { get; set; } = string.Empty;
    }

    // ========== 管理员侧 ==========

    /// <summary>
    /// 售后申请详情DTO（管理员端展示）
    /// </summary>
    public class AfterSaleApplicationDetailDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;

        /// <summary>
        /// 订单ID
        /// </summary>
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplicationTime { get; set; } = string.Empty;

        /// <summary>
        /// 申请描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 处理状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;

        /// <summary>
        /// 用户信息（用于头像与姓名、电话展示）
        /// </summary>
        public UserProfileDto? User { get; set; }

        /// <summary>
        /// 申请图片URL列表
        /// </summary>
        public string[] Images { get; set; } = Array.Empty<string>();

        /// <summary>
        /// 订单菜品列表
        /// </summary>
        public List<OrderDishDto> DishDetails { get; set; } = new List<OrderDishDto>();

        /// <summary>
        /// 商家回复
        /// </summary>
        public string? MerchantReply { get; set; }

        /// <summary>
        /// 消费者评分（0-5）
        /// </summary>
        public int? ConsumerRating { get; set; }
    }

    /// <summary>
    /// 更新售后申请请求DTO（管理员端处理）
    /// </summary>
    public class UpdateAfterSaleApplicationDto
    {
        /// <summary>
        /// 申请ID
        /// </summary>
        public string ApplicationId { get; set; } = string.Empty;

        /// <summary>
        /// 处理状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 处罚措施
        /// </summary>
        public string Punishment { get; set; } = "-";

        /// <summary>
        /// 处罚原因
        /// </summary>
        public string PunishmentReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// 更新售后申请响应DTO（带详情数据）
    /// </summary>
    public class UpdateAfterSaleApplicationResponseDto : ApiResponseDto
    {
        /// <summary>
        /// 更新后的售后申请详情
        /// </summary>
        public AfterSaleApplicationDetailDto? Data { get; set; }
    }

    /// <summary>
    /// 提交售后申请评分请求 DTO（消费者端）
    /// </summary>
    public class SubmitAfterSaleRatingDto
    {
        /// <summary>
        /// 评分（0-5）
        /// </summary>
        public int Score { get; set; }
    }
}
