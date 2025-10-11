namespace BackEnd.Models.Enums
{
    /// <summary>
    /// 用户身份枚举
    /// </summary>
    public enum UserIdentity
    {
        /// <summary>
        /// 消费者
        /// </summary>
        Customer = 0,

        /// <summary>
        /// 配送员
        /// </summary>
        Courier = 1,

        /// <summary>
        /// 管理员
        /// </summary>
        Administrator = 2,

        /// <summary>
        /// 商家
        /// </summary>
        Merchant = 3
    }

    /// <summary>
    /// 消费者会员状态枚举
    /// </summary>
    public enum MembershipStatus
    {
        /// <summary>
        /// 非会员
        /// </summary>
        NotMember = 0,

        /// <summary>
        /// 会员
        /// </summary>
        Member = 1
    }

    /// <summary>
    /// 用户信息隐私级别枚举
    /// </summary>
    public enum ProfilePrivacyLevel
    {
        /// <summary>
        /// 不公开
        /// </summary>
        Private = 0,

        /// <summary>
        /// 公开
        /// </summary>
        Public = 1,

        /// <summary>
        /// 仅好友
        /// </summary>
        FriendsOnly = 2
    }

    /// <summary>
    /// 菜品售罄状态枚举
    /// </summary>
    public enum DishIsSoldOut
    {
        /// <summary>
        /// 售罄
        /// </summary>
        IsSoldOut = 0,

        /// <summary>
        /// 未售罄
        /// </summary>
        IsNotSoldOut = 2
    }

    /// <summary>
    /// 优惠券状态枚举
    /// </summary>
    public enum CouponState
    {
        /// <summary>
        /// 未使用
        /// </summary>
        Unused = 0,

        /// <summary>
        /// 已使用
        /// </summary>
        Used = 1,

        /// <summary>
        /// 已过期
        /// </summary>
        Expired = 2
    }

    /// <summary>
    /// 店铺状态枚举
    /// </summary>
    public enum StoreState
    {
        /// <summary>
        /// 营业中
        /// </summary>
        IsOperation = 0,

        /// <summary>
        /// 休息中
        /// </summary>
        Closing = 1,

        /// <summary>
        /// 已封禁
        /// </summary>
        Banned = 2
    }

    /// <summary>
    /// 商家状态枚举
    /// </summary>
    public enum SellerState
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,

        /// <summary>
        /// 已封禁
        /// </summary>
        Banned = 1
    }

    /// <summary>
    /// 订单状态枚举
    /// </summary>
    public enum FoodOrderState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 准备中
        /// </summary>
        Preparing = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 2
    }

    /// <summary>
    /// 评论类型枚举
    /// </summary>
    public enum CommentType
    {
        /// <summary>
        /// 普通评论
        /// </summary>
        Comment = 0,

        /// <summary>
        /// 店铺评论
        /// </summary>
        Store = 1,

        /// <summary>
        /// 订单评论
        /// </summary>
        FoodOrder = 2
    }

    /// <summary>
    /// 配送员在线状态枚举
    /// </summary>
    public enum CourierIsOnline
    {
        /// <summary>
        /// 在线
        /// </summary>
        Online = 0,

        /// <summary>
        /// 离线
        /// </summary>
        Offline = 1
    }

    /// <summary>
    /// 售后申请状态枚举
    /// </summary>
    public enum AfterSaleState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 1
    }

    /// <summary>
    /// 配送投诉状态枚举
    /// </summary>
    public enum ComplaintState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 1
    }

    /// <summary>
    /// 违规处罚状态枚举
    /// </summary>
    public enum ViolationPenaltyState
    {
        /// <summary>
        /// 待处理
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        Processing = 1,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 2
    }

    /// <summary>
    /// 评论状态枚举
    /// </summary>
    public enum CommentState
    {
        /// <summary>
        /// 待审核
        /// </summary>
        Pending = 0,

        /// <summary>
        /// 已通过
        /// </summary>
        Completed = 1,

        /// <summary>
        /// 违规
        /// </summary>
        Illegal = 2
    }

    /// <summary>
    /// 优惠券类型枚举
    /// </summary>
    public enum CouponType
    {
        /// <summary>
        /// 满减券
        /// </summary>
        Fixed = 0,

        /// <summary>
        /// 折扣券
        /// </summary>
        Discount = 1
    }

    /// <summary>
    /// 购物车状态枚举
    /// </summary>
    public enum ShoppingCartState
    {
        /// <summary>
        /// 活跃
        /// </summary>
        Active = 0,

        /// <summary>
        /// 已完成
        /// </summary>
        Done = 1
    }

    /// <summary>
    /// 配送状态枚举
    /// </summary>
    public enum DeliveryStatus
    {
        /// <summary>
        /// 待取件
        /// </summary>
        To_Be_Taken,

        /// <summary>
        /// 待取单
        /// </summary>
        Pending,

        /// <summary>
        /// 配送中
        /// </summary>
        Delivering,

        /// <summary>
        /// 已完成
        /// </summary>
        Completed,

        /// <summary>
        /// 已取消
        /// </summary>
        Cancelled
    }
}
