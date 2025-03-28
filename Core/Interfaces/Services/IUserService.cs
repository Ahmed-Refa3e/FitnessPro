using Core.DTOs.GeneralDTO;
using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Core.Helpers;
using Microsoft.AspNetCore.Http;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<CoachResponseDTO>> GetAllCoachesAsync(GetCoachesDTO getCoachesDTO);
        Task<Generalresponse> GetCoachDetailsAsync(string CoachId);
        GetProfileDTO GetProfileDetails(ApplicationUser user);
        Task<bool> CheckUserStatusAsync(ApplicationUser user);
        Task<Generalresponse> ChangeProfilePictureAsync(IFormFile profilePicture, ApplicationUser user);
        Generalresponse DeleteProfilePictureAsync(ApplicationUser user);
        Task<Generalresponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO, ApplicationUser user);
    }
}
