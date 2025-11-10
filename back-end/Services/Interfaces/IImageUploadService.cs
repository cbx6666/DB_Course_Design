namespace BackEnd.Services.Interfaces
{
    /// <summary>
    /// 图片上传服务接口
    /// </summary>
    public interface IImageUploadService
    {
        /// <summary>
        /// 上传图片文件
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <param name="subFolder">子文件夹（可选，默认为 "uploads"）</param>
        /// <param name="basePath">基础路径（可选，"images" 或 "avatars"，默认为 "images"）</param>
        /// <param name="maxFileSize">最大文件大小（可选，默认5MB）</param>
        /// <returns>图片相对URL</returns>
        Task<string> UploadImageAsync(Microsoft.AspNetCore.Http.IFormFile imageFile, string? subFolder = null, string? basePath = null, long? maxFileSize = null);

        /// <summary>
        /// 验证图片文件
        /// </summary>
        /// <param name="imageFile">图片文件</param>
        /// <param name="maxFileSize">最大文件大小（可选，默认5MB）</param>
        /// <returns>验证结果，如果通过返回null，否则返回错误消息</returns>
        string? ValidateImageFile(Microsoft.AspNetCore.Http.IFormFile imageFile, long? maxFileSize = null);
    }
}
