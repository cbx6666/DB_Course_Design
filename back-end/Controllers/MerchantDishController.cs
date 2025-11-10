using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Dish;
using BackEnd.Services.Interfaces;
using BackEnd.DTOs.Common;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 菜品管理控制器（商家侧）
    /// </summary>
    [ApiController]
    [Route("api/merchant/dishes")]
    [Authorize]
    public class MerchantDishController : BaseController
    {
        private readonly IMerchantDishService _merchantDishService;

        public MerchantDishController(IMerchantDishService merchantDishService)
        {
            _merchantDishService = merchantDishService;
        }

        /// <summary>
        /// 获取菜品列表
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetDishes([FromQuery] int? categoryId)
        {
            try
            {
                if (categoryId.HasValue)
                {
                    var dishes = await _merchantDishService.GetDishesByCategoryIdAsync(categoryId.Value);
                    var dishDtos = dishes ?? new List<DishDto>();
                    return Ok(dishDtos);
                }
                else
                {
                    return Ok(new List<DishDto>());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 创建新菜品
        /// </summary>
        /// <param name="dto">菜品创建信息</param>
        /// <returns>创建的菜品</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDish([FromBody] CreateDishDto dto)
        {
            try
            {
                var dish = await _merchantDishService.CreateDishAsync(dto);
                return CreatedAtAction(nameof(GetDishById), new { dishId = dish.DishId }, dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 根据ID获取菜品详情
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <returns>菜品详情</returns>
        [HttpGet("{dishId}")]
        public async Task<IActionResult> GetDishById(int dishId)
        {
            try
            {
                var dish = await _merchantDishService.GetDishByIdAsync(dishId);
                return dish == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "菜品不存在" }) : Ok(dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 更新菜品信息
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="dto">更新信息</param>
        /// <returns>更新后的菜品</returns>
        [HttpPatch("{dishId}")]
        public async Task<IActionResult> UpdateDish(int dishId, [FromBody] UpdateDishDto dto)
        {
            try
            {
                var updated = await _merchantDishService.UpdateDishAsync(dishId, dto);
                return updated == null ? NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "菜品不存在" }) : Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 切换菜品售罄状态
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="dto">售罄状态</param>
        /// <returns>操作结果</returns>
        [HttpPatch("{dishId}/soldout")]
        public async Task<IActionResult> ToggleSoldOut(int dishId, [FromBody] ToggleSoldOutDto dto)
        {
            try
            {
                var result = await _merchantDishService.ToggleSoldOutAsync(dishId, dto.IsSoldOut);
                return !result.Success ? BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = result.Message ?? "操作失败" }) : Ok(result.Data ?? new object());
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 上传菜品图片
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <returns>上传结果</returns>
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadDishImage([FromForm] IFormFile imageFile)
        {
            try
            {
                var result = await _merchantDishService.UploadDishImageAsync(imageFile);
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
        /// 删除菜品
        /// </summary>
        /// <param name="id">菜品ID</param>
        /// <returns>删除结果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDish(int id)
        {
            try
            {
                var result = await _merchantDishService.DeleteDishAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponseDto { Success = false, Code = 404, Message = "菜品不存在" });
                }
                return Ok(new ApiResponseDto { Success = true, Code = 200, Message = "菜品删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseDto { Success = false, Code = 400, Message = ex.Message });
            }
        }

        /// <summary>
        /// 将菜品实体映射为DTO
        /// </summary>
        /// <param name="dish">菜品实体</param>
        /// <returns>菜品DTO</returns>
        private static DishDto MapToDishDto(Models.Dish dish)
        {
            return new DishDto
            {
                DishId = dish.DishID,
                DishName = dish.DishName,
                Price = dish.Price,
                Description = dish.Description,
                IsSoldOut = (int)dish.IsSoldOut,
                CategoryID = dish.CategoryID,
                DishImage = dish.DishImage,
            };
        }
    }
}
