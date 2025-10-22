using BackEnd.DTOs.Dish;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 菜品服务接口
    /// </summary>
    public interface IDishService
    {
        /// <summary>
        /// 获取所有菜品
        /// </summary>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<DishDto>> GetAllDishesAsync();

        /// <summary>
        /// 根据菜品种类ID获取菜品列表
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<DishDto>> GetDishesByCategoryIdAsync(int categoryId);

        /// <summary>
        /// 创建菜品
        /// </summary>
        /// <param name="dto">创建菜品请求</param>
        /// <returns>创建的菜品</returns>
        Task<DishDto> CreateDishAsync(CreateDishDto dto);

        /// <summary>
        /// 更新菜品
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="dto">更新菜品请求</param>
        /// <returns>更新后的菜品</returns>
        Task<DishDto?> UpdateDishAsync(int dishId, UpdateDishDto dto);

        /// <summary>
        /// 切换售罄状态
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="isSoldOut">是否售罄</param>
        /// <returns>切换结果</returns>
        Task<(bool Success, string? Message, DishDto? Data)> ToggleSoldOutAsync(int dishId, int isSoldOut);

        /// <summary>
        /// 根据ID获取菜品
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <returns>菜品详情</returns>
        Task<DishDto?> GetDishByIdAsync(int dishId);

        /// <summary>
        /// 上传菜品图片
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <returns>上传结果</returns>
        Task<UploadImageResultDto> UploadDishImageAsync(IFormFile imageFile);

        /// <summary>
        /// 删除菜品
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <returns>删除结果</returns>
        Task<bool> DeleteDishAsync(int dishId);
    }
}