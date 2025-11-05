using BackEnd.DTOs.Coupon;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 商家优惠券管理控制器
    /// </summary>
    [ApiController]
    [Route("api/merchant/coupons")]
    [Authorize]
    public class MerchantCouponController : BaseController
    {
        private readonly IMerchantCouponService _merchantCouponService;

        /// <summary>
        /// 初始化商家优惠券管理控制器
        /// </summary>
        /// <param name="merchantCouponService">优惠券服务（商家侧）</param>
        public MerchantCouponController(IMerchantCouponService merchantCouponService)
        {
            _merchantCouponService = merchantCouponService;
        }

        /// <summary>
        /// 获取优惠券列表（分页）
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns>优惠券列表</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDto<PageResultDto<MerchantCouponDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<PageResultDto<MerchantCouponDto>>>> GetCoupons(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1 || pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new ApiResponseDto<PageResultDto<MerchantCouponDto>>
                    {
                        Success = false,
                        Code = 400,
                        Message = "页码必须大于0，页大小必须在1-100之间"
                    });
                }

                var sellerId = GetCurrentUserId();
                var result = await _merchantCouponService.GetCouponsAsync(sellerId, page, pageSize);

                return Ok(new ApiResponseDto<PageResultDto<MerchantCouponDto>>
                {
                    Success = true,
                    Code = 200,
                    Message = "获取优惠券列表成功",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<PageResultDto<MerchantCouponDto>>
                {
                    Success = false,
                    Code = 500,
                    Message = "获取优惠券列表失败，请稍后重试"
                });
            }
        }

        /// <summary>
        /// 获取优惠券统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponseDto<CouponStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<CouponStatsDto>>> GetCouponStats()
        {
            try
            {
                var sellerId = GetCurrentUserId();
                var result = await _merchantCouponService.GetStatsAsync(sellerId);

                return Ok(new ApiResponseDto<CouponStatsDto>
                {
                    Success = true,
                    Code = 200,
                    Message = "获取统计信息成功",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<CouponStatsDto>
                {
                    Success = false,
                    Code = 500,
                    Message = "获取统计信息失败，请稍后重试"
                });
            }
        }

        /// <summary>
        /// 创建优惠券
        /// </summary>
        /// <param name="request">创建优惠券请求</param>
        /// <returns>创建结果</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseDto<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<int>>> CreateCoupon([FromBody] CreateCouponRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResponseDto<int>
                    {
                        Success = false,
                        Code = 400,
                        Message = $"请求数据验证失败: {string.Join(", ", errors)}"
                    });
                }

                // 自定义验证：根据优惠券类型验证value字段
                if (request.Type == "fixed")
                {
                    // 满减券：优惠金额必须在0.01-999999.99之间
                    if (request.Value < 0.01m || request.Value > 999999.99m)
                    {
                        return BadRequest(new ApiResponseDto<int>
                        {
                            Success = false,
                            Code = 400,
                            Message = "满减券的优惠金额必须在0.01-999999.99之间"
                        });
                    }
                }
                else if (request.Type == "discount")
                {
                    // 折扣券：折扣比例必须在0.01-1之间
                    if (request.Value < 0.01m || request.Value > 1m)
                    {
                        return BadRequest(new ApiResponseDto<int>
                        {
                            Success = false,
                            Code = 400,
                            Message = "折扣券的折扣比例必须在0.01-1之间"
                        });
                    }
                }

                var sellerId = GetCurrentUserId();
                var couponId = await _merchantCouponService.CreateCouponAsync(sellerId, request);

                return Ok(new ApiResponseDto<int>
                {
                    Success = true,
                    Code = 200,
                    Message = "优惠券创建成功",
                    Data = couponId
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponseDto<int>
                {
                    Success = false,
                    Code = 400,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<int>
                {
                    Success = false,
                    Code = 500,
                    Message = "创建优惠券失败，请稍后重试"
                });
            }
        }

        /// <summary>
        /// 更新优惠券
        /// </summary>
        /// <param name="id">优惠券ID</param>
        /// <param name="request">更新请求</param>
        /// <returns>更新结果</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<object>>> UpdateCoupon(int id, [FromBody] CreateCouponRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Code = 400,
                        Message = $"请求数据验证失败: {string.Join(", ", errors)}"
                    });
                }

                request.Id = id;
                var sellerId = GetCurrentUserId();
                await _merchantCouponService.UpdateCouponAsync(sellerId, request);

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Code = 200,
                    Message = "优惠券更新成功"
                });
            }
            catch (ArgumentException ex)
            {
                if (ex.Message.Contains("不存在"))
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Code = 404,
                        Message = ex.Message
                    });
                }
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Code = 400,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Code = 500,
                    Message = "更新优惠券失败，请稍后重试"
                });
            }
        }

        /// <summary>
        /// 删除优惠券
        /// </summary>
        /// <param name="id">优惠券ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<object>>> DeleteCoupon(int id)
        {
            try
            {
                var sellerId = GetCurrentUserId();
                await _merchantCouponService.DeleteCouponAsync(sellerId, id);

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Code = 200,
                    Message = "优惠券删除成功"
                });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Code = 404,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Code = 500,
                    Message = "删除优惠券失败，请稍后重试"
                });
            }
        }

        /// <summary>
        /// 批量删除优惠券
        /// </summary>
        /// <param name="request">批量删除请求</param>
        /// <returns>删除结果</returns>
        [HttpDelete("batch")]
        [ProducesResponseType(typeof(ApiResponseDto<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponseDto<int>>> BatchDeleteCoupons([FromBody] BatchDeleteCouponsRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return BadRequest(new ApiResponseDto<int>
                    {
                        Success = false,
                        Code = 400,
                        Message = $"请求数据验证失败: {string.Join(", ", errors)}"
                    });
                }

                var sellerId = GetCurrentUserId();
                var deletedCount = await _merchantCouponService.BatchDeleteCouponsAsync(sellerId, request);

                return Ok(new ApiResponseDto<int>
                {
                    Success = true,
                    Code = 200,
                    Message = $"成功删除 {deletedCount} 张优惠券",
                    Data = deletedCount
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponseDto<int>
                {
                    Success = false,
                    Code = 400,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<int>
                {
                    Success = false,
                    Code = 500,
                    Message = "批量删除优惠券失败，请稍后重试"
                });
            }
        }
    }
}
