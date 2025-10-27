using BackEnd.Models.Enums;

namespace BackEnd.Models.Helpers
{
    /// <summary>
    /// 店铺种类辅助类
    /// </summary>
    public static class StoreCategoryHelper
    {
        /// <summary>
        /// 获取店铺种类的显示名称
        /// </summary>
        /// <param name="category">店铺种类枚举</param>
        /// <returns>显示名称</returns>
        public static string GetDisplayName(StoreCategory category)
        {
            return category switch
            {
                StoreCategory.Chinese => "中式菜品",
                StoreCategory.WesternFastFood => "西式快餐",
                StoreCategory.JapaneseKorean => "日韩料理",
                StoreCategory.DessertDrink => "甜品饮品",
                StoreCategory.HotpotBarbecue => "火锅烧烤",
                StoreCategory.Snacks => "小食零食",
                StoreCategory.HealthyLight => "健康轻食",
                StoreCategory.LocalSpecialty => "地方特色",
                _ => category.ToString()
            };
        }

        /// <summary>
        /// 根据显示名称转换为枚举值（用于数据库读取）
        /// </summary>
        /// <param name="displayName">显示名称</param>
        /// <returns>枚举值</returns>
        public static StoreCategory FromDisplayName(string displayName)
        {
            return displayName switch
            {
                "中式菜品" or "中式快餐" => StoreCategory.Chinese,
                "西式快餐" => StoreCategory.WesternFastFood,
                "日韩料理" => StoreCategory.JapaneseKorean,
                "甜品饮品" => StoreCategory.DessertDrink,
                "火锅烧烤" => StoreCategory.HotpotBarbecue,
                "小食零食" or "地方小吃" => StoreCategory.Snacks,
                "健康轻食" => StoreCategory.HealthyLight,
                "地方特色" => StoreCategory.LocalSpecialty,
                _ => Enum.TryParse<StoreCategory>(displayName, true, out var result) ? result : StoreCategory.Chinese
            };
        }

        /// <summary>
        /// 获取所有店铺种类选项
        /// </summary>
        /// <returns>店铺种类字典</returns>
        public static Dictionary<StoreCategory, string> GetCategoryOptions()
        {
            return Enum.GetValues<StoreCategory>()
                .ToDictionary(cat => cat, GetDisplayName);
        }
    }
}
