using BackEnd.DTOs.Menu;
using BackEnd.DTOs.DishCategory;
using BackEnd.DTOs.Store;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Models.Helpers;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 店铺服务实现（消费者侧）
    /// </summary>
    public class CustomerStoreService : ICustomerStoreService
    {
        private readonly IStoreRepository _storeRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerStoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        /// <summary>
        /// 获取店铺信息
        /// </summary>
        public async Task<StoreResponseDto?> GetStoreInfoAsync(int storeId)
        {
            var store = await _storeRepository.GetStoreInfoForUserAsync(storeId);
            if (store == null) return null;

            return new StoreResponseDto
            {
                Id = store.StoreID,
                Name = store.StoreName,
                Image = store.StoreImage ?? string.Empty,
                Address = store.StoreAddress,
                OpenTime = store.OpenTime,
                CloseTime = store.CloseTime,
                BusinessHours = $"{store.OpenTime:hh\\:mm}-{store.CloseTime:hh\\:mm}",
                Rating = store.AverageRating,
                MonthlySales = store.MonthlySales,
                Description = store.StoreFeatures ?? string.Empty,
                Category = GetCategoryDisplayName(store.StoreCategory),
                CreateTime = store.StoreCreationTime
            };
        }

        /// <summary>
        /// 获取店铺种类的显示名称
        /// </summary>
        private string GetCategoryDisplayName(StoreCategory category)
        {
            return StoreCategoryHelper.GetDisplayName(category);
        }

        /// <summary>
        /// 获取店铺的菜品种类列表
        /// </summary>
        public async Task<List<CategoryResponseDto>> GetStoreCategoriesAsync(int storeId)
        {
            var store = await _storeRepository.GetByIdAsync(storeId);
            if (store == null || store.Menus == null) return new List<CategoryResponseDto>();

            var allCategories = new List<CategoryResponseDto>();

            foreach (var menu in store.Menus.Where(m => m.IsActive))
            {
                if (menu.MenuDishCategories != null)
                {
                    foreach (var mdc in menu.MenuDishCategories)
                    {
                        if (!allCategories.Any(c => c.Id == mdc.DishCategory.CategoryID))
                        {
                            allCategories.Add(new CategoryResponseDto
                            {
                                Id = mdc.DishCategory.CategoryID,
                                Name = mdc.DishCategory.CategoryName
                            });
                        }
                    }
                }
            }

            return allCategories.OrderBy(c => c.Name).ToList();
        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        public async Task<List<MenuResponseDto>> GetMenuAsync(int storeId)
        {
            var dishes = await _storeRepository.GetDishesByStoreIdAsync(storeId);

            if (dishes == null || !dishes.Any()) return new List<MenuResponseDto>();

            return dishes.Select(d => new MenuResponseDto
            {
                Id = d.DishID,
                Name = d.DishName,
                Description = d.Description,
                Price = d.Price,
                Image = d.DishImage ?? string.Empty,
                IsSoldOut = (int)d.IsSoldOut,
                CategoryId = d.CategoryID
            }).ToList();
        }
    }
}
