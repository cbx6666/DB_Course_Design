namespace BackEnd.DTOs.Customer
{
    /// <summary>
    /// 收藏项DTO
    /// </summary>
    public class FavoriteItemDto
    {
        /// <summary>
        /// 收藏项ID
        /// </summary>
        public int ItemID { get; set; }

        /// <summary>
        /// 店铺ID
        /// </summary>
        public int StoreID { get; set; }

        /// <summary>
        /// 店铺名称
        /// </summary>
        public string StoreName { get; set; } = string.Empty;

        /// <summary>
        /// 店铺图片
        /// </summary>
        public string? StoreImage { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        public DateTime FavoritedAt { get; set; }

        /// <summary>
        /// 收藏原因
        /// </summary>
        public string FavoriteReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// 收藏夹DTO
    /// </summary>
    public class FavoritesFolderDto
    {
        /// <summary>
        /// 收藏夹ID
        /// </summary>
        public int FolderID { get; set; }

        /// <summary>
        /// 收藏夹名称
        /// </summary>
        public string FolderName { get; set; } = string.Empty;

        /// <summary>
        /// 收藏项列表
        /// </summary>
        public List<FavoriteItemDto> FavoriteItems { get; set; } = new List<FavoriteItemDto>();
    }

    /// <summary>
    /// 新建收藏夹请求
    /// </summary>
    public class CreateFavoritesFolderDto
    {
        public string FolderName { get; set; } = string.Empty;
    }

    /// <summary>
    /// 添加收藏项请求
    /// </summary>
    public class AddFavoriteItemDto
    {
        public int StoreId { get; set; }
        public string? FavoriteReason { get; set; }
    }

    /// <summary>
    /// 删除收藏项请求
    /// </summary>
    public class RemoveFavoriteItemDto
    {
        public int StoreId { get; set; }
    }
}

