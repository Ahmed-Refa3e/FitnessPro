using Microsoft.AspNetCore.Http;

namespace Core.DTOs.GeneralDTO
{
    public class UpdateImageDTO
    {
        public IFormFile? Image { get; set; }
    }
}
