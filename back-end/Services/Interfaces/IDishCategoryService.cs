using BackEnd.DTOs.DishCategory;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 菜品种类服务接口
    /// </summary>
    public interface IDishCategoryService
    {
        /// <summary>
        /// 根据菜单ID获取菜品种类列表
        /// </summary>
        /// <param name="menuId">菜单ID</param>
        /// <returns>菜品种类列表</returns>
        Task<DishCategoryListResponseDto> GetCategoriesByMenuIdAsync(int menuId);

        /// <summary>
        /// 创建菜品种类
        /// </summary>
        /// <param name="dto">创建菜品种类DTO</param>
        /// <returns>创建的菜品种类</returns>
        Task<DishCategoryDto> CreateCategoryAsync(CreateDishCategoryDto dto);

        /// <summary>
        /// 更新菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="dto">更新菜品种类DTO</param>
        /// <returns>更新后的菜品种类</returns>
        Task<DishCategoryDto?> UpdateCategoryAsync(int categoryId, CreateDishCategoryDto dto);

        /// <summary>
        /// 删除菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>是否删除成功</returns>
        Task<bool> DeleteCategoryAsync(int categoryId);

        /// <summary>
        /// 将菜品种类添加到菜单
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="menuId">菜单ID</param>
        /// <returns>是否添加成功</returns>
        Task<bool> AddCategoryToMenuAsync(int categoryId, int menuId);

        /// <summary>
        /// 从菜单中移除菜品种类
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <param name="menuId">菜单ID</param>
        /// <returns>是否移除成功</returns>
        Task<bool> RemoveCategoryFromMenuAsync(int categoryId, int menuId);
    }
}
