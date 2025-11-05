using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Store;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 店铺管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/store")]
    [Authorize]
    public class MerchantStoreController : BaseController
    {
        private readonly IMerchantStoreService _storeService;

        public MerchantStoreController(IMerchantStoreService storeService)
        {
            _storeService = storeService;
        }

        /// <summary>
        /// 获取店铺概览
        /// </summary>
        /// <returns>店铺概览信息</returns>
        [HttpGet("overview")]
        public async Task<ActionResult<ShopOverviewResponseDto>> GetShopOverview()
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _storeService.GetShopOverviewAsync(sellerId);
                return Ok(new ApiResponseDto<ShopOverviewResponseDto> { Success = true, Code = 200, Message = "获取成功", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <returns>店铺信息</returns>
        [HttpGet("info")]
        public async Task<ActionResult<ShopInfoResponseDto>> GetShopInfo()
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _storeService.GetShopInfoAsync(sellerId);
                if (result == null)
                {
                    return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "店铺不存在" });
                }
                return Ok(new ApiResponseDto<ShopInfoResponseDto> { Success = true, Code = 200, Message = "获取成功", Data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 切换营业状态
        /// </summary>
        /// <param name="request">状态切换请求</param>
        /// <returns>操作结果</returns>
        [HttpPatch("status")]
        public async Task<ActionResult<ApiResponseDto>> ToggleBusinessStatus([FromBody] ToggleBusinessStatusRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数无效" });
                }

                var sellerId = GetCurrentUserId();
                var result = await _storeService.ToggleBusinessStatusAsync(sellerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 更新店铺字段
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>操作结果</returns>
        [HttpPatch("field")]
        public async Task<ActionResult<ApiResponseDto>> UpdateShopField([FromBody] UpdateShopFieldRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数无效" });
                }

                var sellerId = GetCurrentUserId();
                var result = await _storeService.UpdateShopFieldAsync(sellerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 上传并更新店铺图片
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <returns>操作结果</returns>
        [HttpPut("image")]
        public async Task<IActionResult> UploadStoreImage([FromForm] IFormFile imageFile)
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _storeService.UploadStoreImageAsync(sellerId, imageFile);
                return result.Success
                    ? Ok(new ApiResponseDto<string> { Success = true, Code = 200, Message = "图片上传成功", Data = result.ImageUrl ?? string.Empty })
                    : BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = result.Message ?? "上传失败" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 更新店铺种类
        /// </summary>
        /// <param name="request">更新店铺种类请求</param>
        /// <returns>操作结果</returns>
        [HttpPatch("category")]
        public async Task<ActionResult<ApiResponseDto>> UpdateStoreCategory([FromBody] UpdateStoreCategoryDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "请求参数无效" });
                }

                var sellerId = GetCurrentUserId();
                var result = await _storeService.UpdateStoreCategoryAsync(sellerId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }

        /// <summary>
        /// 获取店铺种类选项
        /// </summary>
        /// <returns>店铺种类选项</returns>
        [HttpGet("category-options")]
        public ActionResult GetStoreCategoryOptions()
        {
            try
            {
                var options = BackEnd.Models.Helpers.StoreCategoryHelper.GetCategoryOptions()
                    .Select(kvp => new { value = kvp.Key, label = kvp.Value })
                    .Cast<object>()
                    .ToList();
                return Ok(new ApiResponseDto<List<object>> { Success = true, Code = 200, Message = "获取成功", Data = options });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = ex.Message });
            }
        }
    }
}
