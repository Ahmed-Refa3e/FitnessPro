using Core.DTOs.GeneralDTO;
using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Core.Helpers;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<PagedResult<CoachResponseDTO>> GetAllCoachesAsync(GetCoachesDTO getCoachesDTO);
        Task<GeneralResponse> GetCoachDetailsAsync(string CoachId);
        GetProfileDTO GetProfileDetails(ApplicationUser user);
        Task<GeneralResponse> CheckUserStatusAsync(ApplicationUser user);
        Task<GeneralResponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO, ApplicationUser user);
        Task<GeneralResponse> GetProfilePictureAsync(string userId);
    }
}