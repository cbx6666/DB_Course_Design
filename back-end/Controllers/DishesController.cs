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
        /// <param name="sellerId">商家ID</param>
        /// <returns>菜品列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetDishes([FromQuery] int? sellerId)
        {
            try
            {
                if (sellerId.HasValue && sellerId == 3)
                {
                    var dishes = await _dishService.GetAllDishesAsync();
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
            };
        }
    }
}