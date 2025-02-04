using Core.DTOs.UserDTO;
using Core.Entities.Identity;

namespace Core.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Generalresponse> RegisterTraineeAsync(RegisterDTO model);
        Task<Generalresponse> RegisterCoachAsync(RegisterCoachDTO model);
        Task<Generalresponse> LoginAsync(LoginDTO loginDTO);
        Task<Generalresponse> LogOutAsync(string userId);
        Task<Generalresponse> ConfirmEmailAsync(ConfirmEmailDTO request);
        Task<Generalresponse> ChangePasswordAsync(ChangePaswwordDTO dto);
        Task<Generalresponse> ForgetPasswordAsync(string email);
        Task<Generalresponse> VerifyResetCodeAsync(VerifyCodeDTO codeDTO);
        Task<Generalresponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<Generalresponse> ResendConfirmationCodeAsync(ConfirmEmailDTO request);
        Task<Generalresponse> ResendResetPasswordCodeAsync(string Email);
        Task<Generalresponse> RefreshTokenAsync(TokenRequestDTO request);
        Task<Generalresponse> RevokeAllTokensAsync(string userId);
    }
}
