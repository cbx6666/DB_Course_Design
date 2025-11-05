using BackEnd.DTOs.Dish;

namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 菜品服务接口（消费者侧）
    /// </summary>
    public interface ICustomerDishService
    {
        /// <summary>
        /// 根据菜品种类ID获取菜品列表（消费者端查看）
        /// </summary>
        /// <param name="categoryId">菜品种类ID</param>
        /// <returns>菜品列表</returns>
        Task<IEnumerable<DishDto>> GetDishesByCategoryIdAsync(int categoryId);

        /// <summary>
        /// 根据ID获取菜品详情（消费者端查看）
        /// </summary>
        /// <param name="dishId">菜品ID</param>
        /// <returns>菜品详情</returns>
        Task<DishDto?> GetDishByIdAsync(int dishId);
    }
}
