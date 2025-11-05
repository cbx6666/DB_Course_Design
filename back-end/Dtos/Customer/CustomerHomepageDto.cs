using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BackEnd.DTOs.Store;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.DTOs.Customer
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
    /// 店铺列表响应数据传输对象
    /// </summary>
    public class StoresResponseDto
    {
        public List<ShowStoreDto> AllStores { get; set; } = new List<ShowStoreDto>();
    }
}

