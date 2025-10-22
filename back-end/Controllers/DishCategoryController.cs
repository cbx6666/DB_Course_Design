using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.DishCategory;
using BackEnd.Services.Interfaces;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 菜品种类管理控制器
    /// </summary>
    [ApiController]
    [Route("api/dish-categories")]
    public class DishCategoryController : ControllerBase
    {
        private readonly IDishCategoryService _dishCategoryService;

        public DishCategoryController(IDishCategoryService dishCategoryService)
        {
            _dishCategoryService = dishCategoryService;
        }

        /// <summary>
        /// 根据菜单ID获取菜品种类列表
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜品种类列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int menuId)
        {
            try
            {
                var response = await _dishCategoryService.GetCategoriesByMenuIdAsync(menuId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 创建菜品种类
        /// </summary>
        /// <param name="dto">创建菜品种类DTO</param>
        /// <returns>创建的菜品种类</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateDishCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { code = 400, message = "请求参数无效", errors = ModelState });
                }

                var category = await _dishCategoryService.CreateCategoryAsync(dto);
                return CreatedAtAction(nameof(GetCategories), new { menuId = dto.MenuId }, category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="dto">更新菜品种类DTO</param>
        /// <returns>更新后的菜品种类</returns>
        [HttpPut("{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CreateDishCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { code = 400, message = "请求参数无效", errors = ModelState });
                }

                var category = await _dishCategoryService.UpdateCategoryAsync(categoryId, dto);
                if (category == null)
                {
                    return NotFound(new { code = 404, message = "菜品种类不存在" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 删除菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var result = await _dishCategoryService.DeleteCategoryAsync(categoryId);
                if (!result)
                {
                    return NotFound(new { code = 404, message = "菜品种类不存在" });
                }

                return Ok(new { code = 200, message = "菜品种类删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 将菜品种类添加到菜单
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="menuId">菜单ID</param>
        /// <returns>操作结果</returns>
        [HttpPost("{categoryId}/add-to-menu")]
        public async Task<IActionResult> AddToMenu(int categoryId, [FromQuery] int menuId)
        {
            try
            {
                var result = await _dishCategoryService.AddCategoryToMenuAsync(categoryId, menuId);
                if (!result)
                {
                    return BadRequest(new { code = 400, message = "菜品种类已存在于该菜单中" });
                }

                return Ok(new { code = 200, message = "菜品种类已添加到菜单" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 从菜单中移除菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="menuId">菜单ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{categoryId}/remove-from-menu")]
        public async Task<IActionResult> RemoveFromMenu(int categoryId, [FromQuery] int menuId)
        {
            try
            {
                var result = await _dishCategoryService.RemoveCategoryFromMenuAsync(categoryId, menuId);
                if (!result)
                {
                    return BadRequest(new { code = 400, message = "菜品种类不在该菜单中" });
                }

                return Ok(new { code = 200, message = "菜品种类已从菜单中移除" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }
    }
}
