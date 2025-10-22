using BackEnd.DTOs.DishCategory;
using BackEnd.Models;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 菜品种类服务实现
    /// </summary>
    public class DishCategoryService : IDishCategoryService
    {
        private readonly IDishCategoryRepository _dishCategoryRepository;
        private readonly IMenuRepository _menuRepository;

        public DishCategoryService(IDishCategoryRepository dishCategoryRepository, IMenuRepository menuRepository)
        {
            _dishCategoryRepository = dishCategoryRepository;
            _menuRepository = menuRepository;
        }

        /// <summary>
        /// 根据菜单ID获取菜品种类列表
        /// </summary>
        public async Task<DishCategoryListResponseDto> GetCategoriesByMenuIdAsync(int menuId)
        {
            var categories = await _dishCategoryRepository.GetByMenuIdAsync(menuId);
            var categoryDtos = categories.Select(MapToDishCategoryDto).ToList();

            return new DishCategoryListResponseDto
            {
                List = categoryDtos,
                Total = categoryDtos.Count
            };
        }

        /// <summary>
        /// 创建菜品种类
        /// </summary>
        public async Task<DishCategoryDto> CreateCategoryAsync(CreateDishCategoryDto dto)
        {
            // 创建菜品种类
            var category = new DishCategory
            {
                CategoryName = dto.CategoryName
            };

            await _dishCategoryRepository.AddAsync(category);

            // 将菜品种类添加到菜单
            var menuCategory = new Menu_DishCategory
            {
                MenuID = dto.MenuId,
                CategoryID = category.CategoryID
            };

            await _dishCategoryRepository.AddMenuDishCategoryAsync(menuCategory);

            return new DishCategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                DishCount = 0
            };
        }

        /// <summary>
        /// 更新菜品种类
        /// </summary>
        public async Task<DishCategoryDto?> UpdateCategoryAsync(int categoryId, CreateDishCategoryDto dto)
        {
            var category = await _dishCategoryRepository.GetByIdAsync(categoryId);

            if (category == null)
                return null;

            category.CategoryName = dto.CategoryName;
            await _dishCategoryRepository.UpdateAsync(category);

            return MapToDishCategoryDto(category);
        }

        /// <summary>
        /// 删除菜品种类
        /// </summary>
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _dishCategoryRepository.GetByIdAsync(categoryId);

            if (category == null)
                return false;

            // 检查是否有菜品关联
            if (category.Dishes.Any())
            {
                throw new InvalidOperationException("该菜品种类下还有菜品，无法删除");
            }

            // 删除菜单关联
            var menuCategories = await _dishCategoryRepository.GetMenuDishCategoriesByMenuIdAsync(0);
            foreach (var menuCategory in menuCategories.Where(mdc => mdc.CategoryID == categoryId))
            {
                await _dishCategoryRepository.RemoveMenuDishCategoryAsync(menuCategory);
            }

            // 删除菜品种类
            await _dishCategoryRepository.DeleteAsync(category);
            return true;
        }

        /// <summary>
        /// 将菜品种类添加到菜单
        /// </summary>
        public async Task<bool> AddCategoryToMenuAsync(int categoryId, int menuId)
        {
            // 检查关联是否已存在
            var existingRelation = await _dishCategoryRepository.GetMenuDishCategoryAsync(menuId, categoryId);

            if (existingRelation != null)
                return false; // 关联已存在

            var menuCategory = new Menu_DishCategory
            {
                MenuID = menuId,
                CategoryID = categoryId
            };

            await _dishCategoryRepository.AddMenuDishCategoryAsync(menuCategory);
            return true;
        }

        /// <summary>
        /// 从菜单中移除菜品种类
        /// </summary>
        public async Task<bool> RemoveCategoryFromMenuAsync(int categoryId, int menuId)
        {
            var menuCategory = await _dishCategoryRepository.GetMenuDishCategoryAsync(menuId, categoryId);

            if (menuCategory == null)
                return false;

            await _dishCategoryRepository.RemoveMenuDishCategoryAsync(menuCategory);
            return true;
        }

        /// <summary>
        /// 将DishCategory实体映射为DishCategoryDto
        /// </summary>
        /// <param name="category">DishCategory实体</param>
        /// <returns>DishCategoryDto</returns>
        private static DishCategoryDto MapToDishCategoryDto(DishCategory category)
        {
            return new DishCategoryDto
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName,
                DishCount = category.Dishes.Count
            };
        }
    }
}