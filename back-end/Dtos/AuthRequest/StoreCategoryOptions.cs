using BackEnd.Models.Enums;
using BackEnd.Models.Helpers;

namespace BackEnd.DTOs.AuthRequest
{
    /// <summary>
    /// 店铺种类选项（已废弃，请使用 StoreCategoryHelper）
    /// </summary>
    [Obsolete("请使用 StoreCategoryHelper.GetCategoryOptions()")]
    public static class StoreCategoryOptions
    {
        /// <summary>
        /// 获取所有店铺种类及其显示名称
        /// </summary>
        public static Dictionary<StoreCategory, string> GetCategoryOptions()
        {
            return StoreCategoryHelper.GetCategoryOptions();
        }
    }
}
