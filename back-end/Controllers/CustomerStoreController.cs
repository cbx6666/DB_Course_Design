using BackEnd.DTOs.Store;
using BackEnd.DTOs.Comment;
using BackEnd.DTOs.Penalty;
using BackEnd.DTOs.Common;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 店铺管理控制器（消费者侧）
    /// </summary>
    [ApiController]
    [Route("api/customer/stores")]
    [Authorize]
    public class CustomerStoreController : BaseController
    {
        private readonly ICustomerStoreService _customerStoreService;
        private readonly ICustomerPenaltyService _customerPenaltyService;

        public CustomerStoreController(
            ICustomerStoreService customerStoreService,
            ICustomerPenaltyService customerPenaltyService)
        {
            _customerStoreService = customerStoreService;
            _customerPenaltyService = customerPenaltyService;
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        [HttpGet("{storeId}/info")]
        public async Task<ActionResult<StoreResponseDto>> GetStoreInfo([FromRoute] int storeId)
        {
            if (storeId <= 0)
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = "店铺编号无效" });

            var result = await _customerStoreService.GetStoreInfoAsync(storeId);
            if (result == null) return NotFound("店铺不存在");

            return Ok(result);
        }

        /// <summary>
        /// 获取店铺的菜品种类列表
        /// </summary>
        [HttpGet("{storeId}/categories")]
        public async Task<ActionResult> GetStoreCategories([FromRoute] int storeId)
        {
            if (storeId <= 0)
                return BadRequest("店铺编号无效");

            var result = await _customerStoreService.GetStoreCategoriesAsync(storeId);
            return Ok(result);
        }

        /// <summary>
        /// 获取菜单（平铺菜品）
        /// </summary>
        [HttpGet("{storeId}/menu")]
        public async Task<ActionResult> GetMenu([FromRoute] int storeId)
        {
            if (storeId <= 0)
                return BadRequest("参数无效");

            var result = await _customerStoreService.GetMenuAsync(storeId);
            if (result == null) return NotFound("当前无菜品");

            return Ok(result);
        }

        /// <summary>
        /// 用户投诉店铺
        /// </summary>
        [HttpPost("{storeId}/report")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReportStore([FromRoute] int storeId, [FromBody] ReportStoreDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized("无效的Token");
                }

                dto.StoreId = storeId;
                dto.UserId = userId.Value;

                await _customerPenaltyService.SubmitStoreReportAsync(dto);
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "投诉已提交，等待管理员审核" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponseDto { Success = false, Code = 500, Message = "提交投诉时发生错误" });
            }
        }
    }
}
