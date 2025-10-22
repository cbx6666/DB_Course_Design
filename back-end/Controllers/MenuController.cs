using Microsoft.AspNetCore.Mvc;
using BackEnd.DTOs.Menu;
using BackEnd.Services.Interfaces;

namespace BackEnd.Controllers
{
    /// <summary>
    /// 菜单管理控制器
    /// </summary>
    [ApiController]
    [Route("api/menus")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="sellerId">商家ID</param>
        /// <returns>菜单列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetMenus([FromQuery] int sellerId)
        {
            try
            {
                var response = await _menuService.GetMenusBySellerIdAsync(sellerId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="dto">创建菜单DTO</param>
        /// <returns>创建的菜单</returns>
        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { code = 400, message = "请求参数无效", errors = ModelState });
                }

                var menu = await _menuService.CreateMenuAsync(dto);
                return CreatedAtAction(nameof(GetMenus), new { sellerId = dto.SellerId }, menu);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="dto">更新菜单DTO</param>
        /// <returns>更新后的菜单</returns>
        [HttpPut("{menuId}")]
        public async Task<IActionResult> UpdateMenu(int menuId, [FromBody] CreateMenuDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { code = 400, message = "请求参数无效", errors = ModelState });
                }

                var menu = await _menuService.UpdateMenuAsync(menuId, dto);
                if (menu == null)
                {
                    return NotFound(new { code = 404, message = "菜单不存在" });
                }

                return Ok(menu);
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>操作结果</returns>
        [HttpDelete("{menuId}")]
        public async Task<IActionResult> DeleteMenu(int menuId)
        {
            try
            {
                var result = await _menuService.DeleteMenuAsync(menuId);
                if (!result)
                {
                    return NotFound(new { code = 404, message = "菜单不存在" });
                }

                return Ok(new { code = 200, message = "菜单删除成功" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }

        /// <summary>
        /// 设置菜单为活跃状态
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <param name="sellerId">商家ID</param>
        /// <returns>操作结果</returns>
        [HttpPut("{menuId}/set-active")]
        public async Task<IActionResult> SetActiveMenu(int menuId, [FromQuery] int sellerId)
        {
            try
            {
                var result = await _menuService.SetActiveMenuAsync(menuId, sellerId);
                if (!result)
                {
                    return NotFound(new { code = 404, message = "菜单不存在或不属于该商家" });
                }

                return Ok(new { code = 200, message = "菜单已设置为活跃状态" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { code = 400, message = ex.Message });
            }
        }
    }
}
