using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services
{
    public interface IBlobService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}
