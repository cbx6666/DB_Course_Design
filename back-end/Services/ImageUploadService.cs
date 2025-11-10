using BackEnd.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace BackEnd.Services
{
    /// <summary>
    /// 图片上传服务实现
    /// </summary>
    public class ImageUploadService : IImageUploadService
    {
        private readonly IWebHostEnvironment _env;
        private const long DefaultMaxFileSize = 5 * 1024 * 1024; // 5MB
        private readonly string[] AllowedTypes = { "image/jpeg", "image/jpg", "image/png" };

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="env">Web主机环境</param>
        public ImageUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        /// <summary>
        /// 验证图片文件
        /// </summary>
        public string? ValidateImageFile(IFormFile imageFile, long? maxFileSize = null)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return "请选择要上传的图片";
            }

            // 验证文件类型
            if (!AllowedTypes.Contains(imageFile.ContentType))
            {
                return "只支持 JPG、JPEG、PNG 格式的图片";
            }

            // 验证文件大小
            var maxSize = maxFileSize ?? DefaultMaxFileSize;
            if (imageFile.Length > maxSize)
            {
                var maxSizeMB = maxSize / (1024 * 1024);
                return $"图片大小不能超过 {maxSizeMB}MB";
            }

            return null; // 验证通过
        }

        /// <summary>
        /// 上传图片文件
        /// </summary>
        public async Task<string> UploadImageAsync(IFormFile imageFile, string? subFolder = null, string? basePath = null, long? maxFileSize = null)
        {
            // 验证文件
            var validationError = ValidateImageFile(imageFile, maxFileSize);
            if (validationError != null)
            {
                throw new ArgumentException(validationError);
            }

            // 确定基础路径和子文件夹
            var baseFolder = string.IsNullOrWhiteSpace(basePath) ? "images" : basePath;
            var folderName = string.IsNullOrWhiteSpace(subFolder) ? "uploads" : subFolder;
            
            // 构建完整路径
            var fullPath = baseFolder == "avatars" 
                ? Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "avatars")
                : Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, baseFolder, folderName);
            
            // 创建目录（如果不存在）
            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            // 生成唯一文件名
            var fileExtension = Path.GetExtension(imageFile.FileName);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(fullPath, fileName);

            // 保存文件
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // 返回图片相对URL
            return baseFolder == "avatars" 
                ? $"/avatars/{fileName}"
                : $"/{baseFolder}/{folderName}/{fileName}";
        }
    }
}

