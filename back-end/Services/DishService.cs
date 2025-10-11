using BackEnd.DTOs.Dish;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;

namespace BackEnd.Services
{
    /// <summary>
    /// 菜品服务实现
    /// </summary>
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepo;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dishRepo">菜品仓储</param>
        public DishService(IDishRepository dishRepo)
        {
            _dishRepo = dishRepo;
        }

        /// <summary>
        /// 获取所有菜品
        /// </summary>
        /// <returns>菜品列表</returns>
        public async Task<IEnumerable<DishDto>> GetAllDishesAsync()
        {
            var dishes = await _dishRepo.GetAllAsync();
            return dishes.Select(d => new DishDto
            {
                DishId = d.DishID,
                DishName = d.DishName,
                Price = d.Price,
                Description = d.Description,
                IsSoldOut = (int)d.IsSoldOut,
            });
        }

        /// <summary>
        /// 创建菜品
        /// </summary>
        /// <param name="dto">创建菜品请求</param>
        /// <returns>创建的菜品</returns>
        public async Task<DishDto> CreateDishAsync(CreateDishDto dto)
        {
            var dish = new Dish
            {
                DishName = dto.DishName,
                Price = dto.Price,
                Description = dto.Description,
                IsSoldOut = (DishIsSoldOut)dto.IsSoldOut,
            };

            await _dishRepo.AddAsync(dish);
            return new DishDto
            {
                DishId = dish.DishID,
                DishName = dish.DishName,
                Price = dish.Price,
                Description = dish.Description,
                IsSoldOut = (int)dish.IsSoldOut,
            };
        }

        /// <summary>
        /// 更新菜品
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="dto">更新菜品请求</param>
        /// <returns>更新后的菜品</returns>
        public async Task<DishDto?> UpdateDishAsync(int dishId, UpdateDishDto dto)
        {
            var dish = await _dishRepo.GetByIdAsync(dishId);
            if (dish == null) return null;

            if (dto.DishName != null) dish.DishName = dto.DishName;
            if (dto.Price.HasValue) dish.Price = dto.Price.Value;
            if (dto.Description != null) dish.Description = dto.Description;
            if (dto.IsSoldOut.HasValue) dish.IsSoldOut = (DishIsSoldOut)dto.IsSoldOut.Value;

            await _dishRepo.UpdateAsync(dish);
            return new DishDto
            {
                DishId = dish.DishID,
                DishName = dish.DishName,
                Price = dish.Price,
                Description = dish.Description,
                IsSoldOut = (int)dish.IsSoldOut,
            };
        }

        /// <summary>
        /// 切换售罄状态
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <param name="isSoldOut">是否售罄</param>
        /// <returns>切换结果</returns>
        public async Task<(bool Success, string? Message, DishDto? Data)> ToggleSoldOutAsync(int dishId, int isSoldOut)
        {
            if (isSoldOut != 0 && isSoldOut != 2)
                return (false, "售罄状态错误", null);

            var dish = await _dishRepo.GetByIdAsync(dishId);
            if (dish == null)
                return (false, "菜品不存在", null);

            dish.IsSoldOut = (DishIsSoldOut)isSoldOut;
            await _dishRepo.UpdateAsync(dish);

            return (true, null, new DishDto
            {
                DishId = dish.DishID,
                DishName = dish.DishName,
                Price = dish.Price,
                Description = dish.Description,
                IsSoldOut = (int)dish.IsSoldOut,
            });
        }

        /// <summary>
        /// 根据ID获取菜品
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <returns>菜品详情</returns>
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
            };
        }
    }
}