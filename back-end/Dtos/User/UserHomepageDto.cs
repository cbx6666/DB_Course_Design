using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 首页推荐数据传输对象
    /// </summary>
    public class HomeRecmDto
    {
        /// <summary>
        /// 推荐店铺列表
        /// </summary>
        [Required]
        public IEnumerable<ShowStoreDto> RecomStore { get; set; } = Array.Empty<ShowStoreDto>();
    }

    /// <summary>
    /// 首页搜索请求数据传输对象
    /// </summary>
    public class HomeSearchDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [Required]
        [FromQuery(Name = "address")]
        public string Address { get; set; } = null!;

        /// <summary>
        /// 关键词
        /// </summary>
        [Required]
        [FromQuery(Name = "keyword")]
        public string Keyword { get; set; } = null!;
    }

    /// <summary>
    /// 首页搜索结果数据传输对象
    /// </summary>
    public class HomeSearchGetDto
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// 店铺图片
        /// </summary>
        [Required]
        public string Image { get; set; } = null!;

        /// <summary>
        /// 平均评分
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageRating { get; set; } = 0.00m;

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 月销量
        /// </summary>
        [Required]
        public int MonthlySales { get; set; }
    }

    /// <summary>
    /// 用户ID数据传输对象
    /// </summary>
    public class UserIdDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public int UserId { get; set; }
    }

    /// <summary>
    /// 用户信息数据传输对象
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [MaxLength(15)]
        public string Username { get; set; } = null!;

        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(255)]
        public string? Avatar { get; set; }
    }

    /// <summary>
    /// 订单中的单个菜品数据传输对象
    /// </summary>
    public class DishDto
    {
        /// <summary>
        /// 菜品ID
        /// </summary>
        public int DishID { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        public string DishName { get; set; } = null!;

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 菜品图片
        /// </summary>
        public string? DishImage { get; set; }
    }

    /// <summary>
    /// 历史订单数据传输对象
    /// </summary>
    public class HistoryOrderDto
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        [Required]
        public int OrderID { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>
        [Required]
        public string PaymentTime { get; set; } = string.Empty;

        /// <summary>
        /// 购物车ID
        /// </summary>
        [Required]
        public int CartID { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        [Required]
        public int StoreID { get; set; }

        /// <summary>
        /// 店铺图片
        /// </summary>
        [Required]
        public string StoreImage { get; set; } = string.Empty;

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 菜品图片列表
        /// </summary>
        [Required]
        public List<string> DishImage { get; set; } = new List<string>();

        /// <summary>
        /// 总金额
        /// </summary>
        [Required]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Required]
        public FoodOrderState OrderStatus { get; set; }
    }

    /// <summary>
    /// 历史订单列表数据传输对象
    /// </summary>
    public class HistoryOrderGetDto
    {
        /// <summary>
        /// 订单列表
        /// </summary>
        [Required]
        public List<HistoryOrderDto> Orders { get; set; } = new List<HistoryOrderDto>();
    }

    /// <summary>
    /// 用户信息响应数据传输对象
    /// </summary>
    public class UserInfoResponse
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 手机号
        /// </summary>
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Image { get; set; } = null!;

        /// <summary>
        /// 默认地址
        /// </summary>
        public string DefaultAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// 优惠券数据传输对象
    /// </summary>
    public class CouponDto
    {
        /// <summary>
        /// 优惠券ID
        /// </summary>
        [Key]
        public int CouponID { get; set; }

        /// <summary>
        /// 优惠券状态
        /// </summary>
        public CouponState CouponState { get; set; } = CouponState.Unused;

        /// <summary>
        /// 关联订单ID
        /// </summary>
        public int? OrderID { get; set; }

        /// <summary>
        /// 优惠券管理ID
        /// </summary>
        [Required]
        public int CouponManagerID { get; set; }

        /// <summary>
        /// 最低消费金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MinimumSpend { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 有效期至
        /// </summary>
        [Required]
        public DateTime ValidTo { get; set; }
    }

    /// <summary>
    /// 展示店铺数据传输对象
    /// </summary>
    public class ShowStoreDto
    {
        /// <summary>
        /// 店铺ID
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// 店铺图片
        /// </summary>
        [Required]
        public string Image { get; set; } = null!;

        /// <summary>
        /// 店铺名称
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 平均评分
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal AverageRating { get; set; } = 0.00m;

        /// <summary>
        /// 月销量
        /// </summary>
        [Required]
        public int MonthlySales { get; set; }
    }

    /// <summary>
    /// 店铺列表响应数据传输对象
    /// </summary>
    public class StoresResponseDto
    {
        /// <summary>
        /// 所有店铺列表
        /// </summary>
        public List<ShowStoreDto> AllStores { get; set; } = new List<ShowStoreDto>();
    }
}