using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackEnd.DTOs.Merchant;
using BackEnd.Services.Interfaces;
using BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 商家管理控制器
    /// </summary>
    [ApiController]
    [Route("api/merchant")]
    [Authorize]
    public class MerchantController : ControllerBase
    {
        private readonly IMerchantService _merchantService;
        private readonly IMerchantInformationService _merchantInformationService;
        private readonly AppDbContext _context;

        public MerchantController(IMerchantService merchantService, IMerchantInformationService merchantInformationService, AppDbContext context)
        {
            _merchantService = merchantService;
            _merchantInformationService = merchantInformationService;
            _context = context;
        }

        /// <summary>
        /// 获取店铺概览
        /// </summary>
        /// <returns>店铺概览信息</returns>
        [HttpGet("/api/shop/overview")]
        public async Task<ActionResult<ShopOverviewResponseDto>> GetShopOverview()
        {
            try
            {
                var sellerId = GetCurrentSellerId();
                var result = await _merchantService.GetShopOverviewAsync(sellerId);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        /// <returns>店铺信息</returns>
        [HttpGet("/api/shop/info")]
        public async Task<ActionResult<ShopInfoResponseDto>> GetShopInfo()
        {
            try
            {
                var sellerId = GetCurrentSellerId();
                var result = await _merchantService.GetShopInfoAsync(sellerId);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 获取商家信息
        /// </summary>
        /// <returns>商家信息</returns>
        [HttpGet("info")]
        public async Task<ActionResult<MerchantInfoResponseDto>> GetMerchantInfo()
        {
            try
            {
                var sellerId = GetCurrentSellerId();
                // 统一由 MerchantInformationService 提供商家个人信息，避免与店铺服务重复职责
                var profile = await _merchantInformationService.GetMerchantInfoAsync(sellerId);
                if (!profile.Success || profile.Data == null)
                {
                    return StatusCode(500, new { error = profile.Message ?? "获取商家信息失败" });
                }

                var dto = new MerchantInfoResponseDto
                {
                    Username = profile.Data.Username,
                    SellerId = sellerId,
                    Avatar = profile.Data.Avatar
                };

                return Ok(new { data = dto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 切换营业状态
        /// </summary>
        /// <param name="request">状态切换请求</param>
        /// <returns>操作结果</returns>
        [HttpPatch("/api/shop/status")]
        public async Task<ActionResult<CommonResponseDto>> ToggleBusinessStatus([FromBody] ToggleBusinessStatusRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = GetCurrentSellerId();
                var result = await _merchantService.ToggleBusinessStatusAsync(sellerId, request);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 更新店铺字段
        /// </summary>
        /// <param name="request">更新请求</param>
        /// <returns>操作结果</returns>
        [HttpPatch("/api/shop/update-field")]
        public async Task<ActionResult<CommonResponseDto>> UpdateShopField([FromBody] UpdateShopFieldRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var sellerId = GetCurrentSellerId();
                var result = await _merchantService.UpdateShopFieldAsync(sellerId, request);
                return Ok(new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 上传并更新店铺图片
        /// </summary>
        [HttpPut("/api/shop/image")]
        public async Task<IActionResult> UploadStoreImage([FromForm] IFormFile imageFile)
        {
            try
            {
                var sellerId = GetCurrentSellerId();
                var result = await _merchantService.UploadStoreImageAsync(sellerId, imageFile);
                return result.Success
                    ? Ok(new { code = 200, success = true, image = result.ImageUrl })
                    : BadRequest(new { code = 400, success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.StackTrace });
            }
        }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <returns>连接测试结果</returns>
        [HttpGet("test-db-connection")]
        public async Task<ActionResult> TestDatabaseConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    var storeCount = await _context.Stores.CountAsync();
                    var sellerCount = await _context.Sellers.CountAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "数据库连接成功",
                        storeCount = storeCount,
                        sellerCount = sellerCount
                    });
                }
                else
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "无法连接到数据库"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "数据库连接错误",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// 获取当前商家ID
        /// </summary>
        /// <returns>商家ID</returns>
        private int GetCurrentSellerId()
        {
            var sellerIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(sellerIdString, out int sellerId))
            {
                throw new UnauthorizedAccessException("无效的 Token，无法获取商家 ID");
            }
            return sellerId;
        }
    }
}