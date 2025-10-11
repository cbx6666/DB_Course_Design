namespace BackEnd.DTOs.User
{
    /// <summary>
    /// 用户个人资料数据传输对象
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// 用户姓名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 头像图片
        /// </summary>
        public string? Image { get; set; }
    }

    /// <summary>
    /// 用户地址数据传输对象
    /// </summary>
    public class UserAddressDto
    {
        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// 手机号码
        /// </summary>
        public long PhoneNumber { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { get; set; } = null!;
    }
}
