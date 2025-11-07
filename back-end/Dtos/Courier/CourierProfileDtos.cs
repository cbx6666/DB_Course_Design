using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace BackEnd.DTOs.Courier
{
    /// <summary>
    /// 配送员个人资料数据传输对象（展示用）
    /// </summary>
    public class CourierProfileDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 配送员ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 注册日期
        /// </summary>
        public string RegisterDate { get; set; } = string.Empty;

        /// <summary>
        /// 评分
        /// </summary>
        public decimal Rating { get; set; }

        /// <summary>
        /// 信用分数
        /// </summary>
        public int CreditScore { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string? Gender { get; set; }
    }

    /// <summary>
    /// 更新配送员资料请求DTO（用于编辑页面）
    /// </summary>
    public class UpdateProfileDto
    {
        /// <summary>
        /// 用户名（可修改）
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(15, ErrorMessage = "用户名长度不能超过15个字符")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// 真实姓名（只读，不可修改）
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// 性别（代码形式）
        /// </summary>
        [StringLength(2, ErrorMessage = "性别代码长度不能超过2个字符")]
        public string? Gender { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(1000)]
        public string? Avatar { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        [Required(ErrorMessage = "车辆类型不能为空")]
        [StringLength(20)]
        public string VehicleType { get; set; } = null!;
    }

    /// <summary>
    /// 配送员摘要信息DTO（用于订单配送信息展示）
    /// </summary>
    public class CourierSummaryDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// 配送员注册时间
        /// </summary>
        public string? CourierRegistrationTime { get; set; }
        /// <summary>
        /// 交通工具类型
        /// </summary>
        public string VehicleType { get; set; } = null!;
        /// <summary>
        /// 信誉积分
        /// </summary>
        public decimal ReputationPoints { get; set; }
        /// <summary>
        /// 总配送次数
        /// </summary>
        public int TotalDeliveries { get; set; }
        /// <summary>
        /// 平均配送时间
        /// </summary>
        public int AvgDeliveryTime { get; set; }
        /// <summary>
        /// 平均评分
        /// </summary>
        public decimal AverageRating { get; set; }
        /// <summary>
        /// 月薪
        /// </summary>
        public decimal MonthlySalary { get; set; }
        /// <summary>
        /// 全名
        /// </summary>
        public string? FullName { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        public long? PhoneNumber { get; set; }
        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }
        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }
    }

    /// <summary>
    /// 更新配送员头像请求
    /// </summary>
    public class UpdateCourierAvatarDto
    {
        /// <summary>
        /// 头像文件
        /// </summary>
        [Required]
        public IFormFile AvatarFile { get; set; } = null!;
    }
}

