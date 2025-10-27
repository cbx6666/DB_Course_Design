using BackEnd.DTOs.AuthRequest;
using BackEnd.Models.Enums;
using BackEnd.Models.Helpers;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 用户注册控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerService;

        public RegisterController(IRegisterService registerService)
        {
            _registerService = registerService;
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="request">注册请求</param>
        /// <returns>注册结果</returns>
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _registerService.RegisterAsync(request);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// 获取店铺种类选项列表
        /// </summary>
        /// <returns>店铺种类选项列表</returns>
        [HttpGet("store-categories")]
        public IActionResult GetStoreCategories()
        {
            var categories = StoreCategoryHelper.GetCategoryOptions()
                .Select(kvp => new { value = (int)kvp.Key, label = kvp.Value })
                .ToList();

            return Ok(categories);
        }
    }
}
