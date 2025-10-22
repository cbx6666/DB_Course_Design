using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Dish;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 菜品管理控制器
    /// </summary>
    [ApiController]
    [Route("api/dishes")]
    public class DishesController : ControllerBase
    {
        private readonly IDishService _dishService;

        public DishesController(IDishService dishService)
        {
            _dishService = dishService;
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
                    var dishes = await _dishService.GetDishesByCategoryIdAsync(categoryId.Value);
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
                return BadRequest(new { code = 400, message = ex.Message });
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
                var dish = await _dishService.CreateDishAsync(dto);
                return CreatedAtAction(nameof(GetDishById), new { dishId = dish.DishId }, dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var dish = await _dishService.GetDishByIdAsync(dishId);
                return dish == null ? NotFound(new { code = 404, message = "菜品不存在" }) : Ok(dish);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var updated = await _dishService.UpdateDishAsync(dishId, dto);
                return updated == null ? NotFound(new { code = 404, message = "菜品不存在" }) : Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                var result = await _dishService.ToggleSoldOutAsync(dishId, dto.IsSoldOut);
                return !result.Success ? BadRequest(new { code = 400, message = result.Message }) : Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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
                if (imageFile == null || imageFile.Length == 0)
                {
                    return BadRequest(new { code = 400, message = "请选择要上传的图片" });
                }

                // 验证文件类型
                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png" };
                if (!allowedTypes.Contains(imageFile.ContentType))
                {
                    return BadRequest(new { code = 400, message = "只支持 JPG、JPEG、PNG 格式的图片" });
                }

                // 验证文件大小 (2MB)
                if (imageFile.Length > 2 * 1024 * 1024)
                {
                    return BadRequest(new { code = 400, message = "图片大小不能超过 2MB" });
                }

                var result = await _dishService.UploadDishImageAsync(imageFile);
                return result.Success
                    ? Ok(new { code = 200, success = true, imageUrl = result.ImageUrl })
                    : BadRequest(new { code = 400, success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, message = ex.Message });
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
                var result = await _dishService.DeleteDishAsync(id);
                if (!result)
                {
                    return NotFound(new { code = 404, message = "菜品不存在" });
                }
                return Ok(new { code = 200, message = "菜品删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
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