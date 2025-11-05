namespace BackEnd.DTOs.DeliveryTask
{
    // ========== 骑手侧 ==========

    /// <summary>
    /// 可接配送任务DTO（骑手端查看）
    /// </summary>
    public class CourierAvailableTaskDto
    {
        /// <summary>
        /// 配送任务ID（TaskID）
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = null!;

        /// <summary>
        /// 餐厅
        /// </summary>
        public string Restaurant { get; set; } = null!;

        /// <summary>
        /// 取餐地址
        /// </summary>
        public string PickupAddress { get; set; } = null!;

        /// <summary>
        /// 配送地址
        /// </summary>
        public string DeliveryAddress { get; set; } = null!;

        /// <summary>
        /// 客户
        /// </summary>
        public string Customer { get; set; } = null!;

        /// <summary>
        /// 配送费
        /// </summary>
        public string Fee { get; set; } = null!;

        /// <summary>
        /// 距离
        /// </summary>
        public string Distance { get; set; } = null!;

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = null!;
    }

    /// <summary>
    /// 配送任务列表项DTO（骑手端查看）
    /// </summary>
    public class CourierTaskListItemDto
    {
        /// <summary>
        /// 配送任务ID（TaskID）
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 餐厅名称
        /// </summary>
        public string Restaurant { get; set; } = string.Empty;

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 配送费
        /// </summary>
        public string Fee { get; set; } = string.Empty;

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 状态文本
        /// </summary>
        public string StatusText { get; set; } = string.Empty;

        /// <summary>
        /// 是否准备取餐
        /// </summary>
        public bool IsReadyForPickup { get; set; }
    }
}

