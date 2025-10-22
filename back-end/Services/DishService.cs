using BackEnd.DTOs.Dish;
using BackEnd.Models;
using BackEnd.Models.Enums;
using BackEnd.Repositories.Interfaces;
using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Http;

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
                CategoryID = d.CategoryID,
                DishImage = d.DishImage,
            });
        }

        /// <summary>
        /// 根据菜品种类ID获取菜品列表
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
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
                CategoryID = dto.CategoryID,
                DishImage = dto.DishImage,
            };

            await _dishRepo.AddAsync(dish);
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

        /// <summary>
        /// 上传菜品图片
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <returns>上传结果</returns>
        public async Task<UploadImageResultDto> UploadDishImageAsync(IFormFile imageFile)
        {
            try
            {
                if (imageFile == null || imageFile.Length == 0)
                {
                    return new UploadImageResultDto
                    {
                        Success = false,
                        Message = "请选择要上传的图片"
                    };
                }

                // 创建菜品图片目录
                var dishImagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "dishes");
                if (!Directory.Exists(dishImagesFolder))
                {
                    Directory.CreateDirectory(dishImagesFolder);
                }

                // 生成唯一文件名
                var fileExtension = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(dishImagesFolder, fileName);

                // 保存文件
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // 返回图片URL
                var imageUrl = $"/images/dishes/{fileName}";

                return new UploadImageResultDto
                {
                    Success = true,
                    Message = "图片上传成功",
                    ImageUrl = imageUrl
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
        /// <param name="dishId">菜品ID</param>
        /// <returns>删除结果</returns>
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
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}