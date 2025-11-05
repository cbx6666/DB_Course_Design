using BackEnd.DTOs.Common;
using BackEnd.DTOs.Customer;

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
        /// 用户信息
        /// </summary>
        public UserProfileDto User { get; set; } = null!;

        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; } = null!;

        /// <summary>
        /// 申请时间
        /// </summary>
        public string CreatedAt { get; set; } = null!;
    }

    /// <summary>
    /// 处理售后请求DTO（商家端处理）
    /// </summary>
    public class ProcessAfterSaleDto
    {
        /// <summary>
        /// 操作：approve/reject/negotiate
        /// </summary>
        public string Action { get; set; } = null!;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; } = null!;
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
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
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

        /// <summary>
        /// 处理备注
        /// </summary>
        public string ProcessingNote { get; set; } = string.Empty;
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
}
