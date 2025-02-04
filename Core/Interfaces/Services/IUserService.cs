using Core.DTOs.UserDTO;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<Generalresponse> GetAllCoachesAsync();
        Task<Generalresponse> GetCoachDetailsAsync(string CoachId);
        Task<Generalresponse> GetTraineeDetailsAsync(string TraineeId);
        Task<Generalresponse> SetOnlineAvailabilityAsync(string userId, bool isAvailable);
        Task<Generalresponse> ChangeProfilePictureAsync(IFormFile profilePicture,string userId);
        Task<Generalresponse> UpdateProfileDetailsAsync(UpdateProfileDTO profileDTO,string userId);
    }
}
