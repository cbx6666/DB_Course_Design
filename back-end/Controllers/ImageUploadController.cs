using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 通用图片上传控制器
    /// </summary>
    [ApiController]
    [Route("api/upload")]
    [Authorize]
    public class ImageUploadController : BaseController
    {
        private readonly IImageUploadService _imageUploadService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="imageUploadService">图片上传服务</param>
        public ImageUploadController(IImageUploadService imageUploadService)
        {
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// 上传通用图片
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <param name="subFolder">子文件夹（可选）</param>
        /// <returns>上传结果</returns>
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile imageFile, [FromForm] string? subFolder = null)
        {
            try
            {
                var imageUrl = await _imageUploadService.UploadImageAsync(imageFile, subFolder);
                return Ok(new ApiResponseDto<string> { Success = true, Code = 200, Message = "图片上传成功", Data = imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = $"图片上传失败: {ex.Message}" });
            }
        }
    }
}

