using Core.DTOs.GeneralDTO;
using Microsoft.AspNetCore.Http;

namespace Core.Utilities
{
    public static class AddImageHelper
    {
        public static IntResult CheckImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new IntResult { Massage = "Please upload a valid image file." };

            if (!file.ContentType.StartsWith("image/"))
                return new IntResult { Massage = "Only image files are allowed." };


            const long maxSize = 5 * 1024 * 1024; // 5MB
            if (file.Length > maxSize)
                return new IntResult { Massage = "Image size must not exceed 5MB." };
            return new IntResult { Id = 1 };

        }
    }
}
