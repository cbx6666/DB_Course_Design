using BackEnd.DTOs.Dish;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 菜品服务实现（消费者侧）
    /// </summary>
    public class CustomerDishService : ICustomerDishService
    {
        private readonly IDishRepository _dishRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomerDishService(IDishRepository dishRepo)
        {
            _dishRepo = dishRepo;
        }

        /// <summary>
        /// 根据菜品种类ID获取菜品列表（消费者端查看）
        /// </summary>
        public async Task<IEnumerable<DishDto>> GetDishesByCategoryIdAsync(int categoryId)
        {
            var dishes = await _dishRepo.GetByCategoryIdAsync(categoryId);
            return dishes.Select(d => new DishDto
            {
                DishId = d.DishID,
                DishName = d.DishName,
                Price = d.Price,
                Description = d.Description,
                IsSoldOut = (int)d.IsSoldOut,
                CategoryID = d.CategoryID,
                DishImage = d.DishImage,
            });
        }

        /// <summary>
        /// 根据ID获取菜品详情（消费者端查看）
        /// </summary>
        public async Task<DishDto?> GetDishByIdAsync(int dishId)
        {
            var dish = await _dishRepo.GetByIdAsync(dishId);
            return dish == null ? null : new DishDto
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
