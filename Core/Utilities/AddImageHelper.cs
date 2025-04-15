using Core.DTOs.GeneralDTO;
using Microsoft.AspNetCore.Http;

namespace Core.Utilities
{
    public static class AddImageHelper
    {
        public static StringResult chickImagePath(IFormFile file, string storagePath)
        {
            if (file is null)
            {
                return new StringResult();
            }
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var contentType = file.ContentType.ToLower();
            if (!allowedExtensions.Contains(fileExtension) || !contentType.StartsWith("image/"))
            {
                return new StringResult { Massage = "Invalid file type, Only images are allowed" };
            }
            var fileName = Path.GetRandomFileName() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(storagePath, fileName);
            return new StringResult { Id = filePath };
        }
    }
}
