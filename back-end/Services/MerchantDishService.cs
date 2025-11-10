using BackEnd.DTOs.Dish;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 菜品服务实现（商家侧）
    /// </summary>
    public class MerchantDishService : IMerchantDishService
    {
        private readonly IDishRepository _dishRepo;
        private readonly IImageUploadService _imageUploadService;

        /// <summary>
        /// 构造函数
        /// </summary>
        public MerchantDishService(IDishRepository dishRepo, IImageUploadService imageUploadService)
        {
            _dishRepo = dishRepo;
            _imageUploadService = imageUploadService;
        }

        /// <summary>
        /// 获取所有菜品
        /// </summary>
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
                CategoryID = d.CategoryID,
                DishImage = d.DishImage,
            });
        }

        /// <summary>
        /// 根据菜品种类ID获取菜品列表
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
        /// 创建菜品
        /// </summary>
        public async Task<DishDto> CreateDishAsync(CreateDishDto dto)
        {
            var dish = new Dish
            {
                DishName = dto.DishName,
                Price = dto.Price,
                Description = dto.Description,
                IsSoldOut = (DishIsSoldOut)dto.IsSoldOut,
                CategoryID = dto.CategoryID,
                DishImage = dto.DishImage,
            };

            await _dishRepo.AddAsync(dish);
            await _dishRepo.SaveAsync();

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

        /// <summary>
        /// 更新菜品
        /// </summary>
        public async Task<DishDto?> UpdateDishAsync(int dishId, UpdateDishDto dto)
        {
            var dish = await _dishRepo.GetByIdAsync(dishId);
            if (dish == null) return null;

            if (dto.DishName != null) dish.DishName = dto.DishName;
            if (dto.Price.HasValue) dish.Price = dto.Price.Value;
            if (dto.Description != null) dish.Description = dto.Description;
            if (dto.IsSoldOut.HasValue) dish.IsSoldOut = (DishIsSoldOut)dto.IsSoldOut.Value;
            if (dto.DishImage != null) dish.DishImage = dto.DishImage;

            await _dishRepo.UpdateAsync(dish);
            await _dishRepo.SaveAsync();

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

        /// <summary>
        /// 切换售罄状态
        /// </summary>
        public async Task<(bool Success, string? Message, DishDto? Data)> ToggleSoldOutAsync(int dishId, int isSoldOut)
        {
            if (isSoldOut != 0 && isSoldOut != 2)
                return (false, "售罄状态错误", null);

            var dish = await _dishRepo.GetByIdAsync(dishId);
            if (dish == null)
                return (false, "菜品不存在", null);

            dish.IsSoldOut = (DishIsSoldOut)isSoldOut;
            await _dishRepo.UpdateAsync(dish);
            await _dishRepo.SaveAsync();

            return (true, null, new DishDto
            {
                DishId = dish.DishID,
                DishName = dish.DishName,
                Price = dish.Price,
                Description = dish.Description,
                IsSoldOut = (int)dish.IsSoldOut,
                CategoryID = dish.CategoryID,
                DishImage = dish.DishImage,
            });
        }

        /// <summary>
        /// 根据ID获取菜品
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

        /// <summary>
        /// 上传菜品图片
        /// </summary>
        public async Task<UploadImageResultDto> UploadDishImageAsync(IFormFile imageFile)
        {
            try
            {
                // 使用统一的图片上传服务，限制大小为2MB
                var imageUrl = await _imageUploadService.UploadImageAsync(imageFile, "dishes", "images", 2 * 1024 * 1024);

                return new UploadImageResultDto
                {
                    Success = true,
                    Message = "图片上传成功",
                    ImageUrl = imageUrl
                };
            }
            catch (ArgumentException ex)
            {
                return new UploadImageResultDto
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                return new UploadImageResultDto
                {
                    Success = false,
                    Message = $"图片上传失败: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// 删除菜品
        /// </summary>
        public async Task<bool> DeleteDishAsync(int dishId)
        {
            try
            {
                var dish = await _dishRepo.GetByIdAsync(dishId);
                if (dish == null)
                {
                    return false;
                }

                await _dishRepo.DeleteAsync(dish);
                await _dishRepo.SaveAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
