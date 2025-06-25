using Core.DTOs.AuthDTO;
using Core.DTOs.GeneralDTO;
namespace Core.Interfaces.Services

{
    public interface IAuthService
    {
        Task<GeneralResponse> RegisterTraineeAsync(RegisterDTO model);
        Task<GeneralResponse> RegisterCoachAsync(RegisterDTO model);
        Task<GeneralResponse> LoginAsync(LoginDTO loginDTO);
        Task<GeneralResponse> GoogleLoginAsync(GoogleAuthDTO request);
        Task<GeneralResponse> SetUserRoleAsync(string userId, string role);
        Task<GeneralResponse> LogOutAsync(string userId);
        Task<GeneralResponse> ConfirmEmailAsync(ConfirmEmailDTO request);
        Task<GeneralResponse> ChangePasswordAsync(ChangePaswwordDTO dto,string userId);
        Task<GeneralResponse> ForgetPasswordAsync(string email);
        Task<GeneralResponse> VerifyResetCodeAsync(VerifyCodeDTO codeDTO);
        Task<GeneralResponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<GeneralResponse> ResendConfirmationCodeAsync(string Email);
        Task<GeneralResponse> ResendResetPasswordCodeAsync(string Email);
        Task<GeneralResponse> RefreshTokenAsync(TokenRequestDTO request);
        Task<GeneralResponse> RevokeAllTokensAsync(string userId);
    }
}
