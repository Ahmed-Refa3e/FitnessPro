using Core.DTOs.AuthDTO;
using Core.DTOs.GeneralDTO;
namespace Core.Interfaces.Services

{
    public interface IAuthService
    {
        Task<Generalresponse> RegisterTraineeAsync(RegisterDTO model);
        Task<Generalresponse> RegisterCoachAsync(RegisterDTO model);
        Task<Generalresponse> LoginAsync(LoginDTO loginDTO);
        Task<Generalresponse> GoogleLoginAsync(GoogleAuthDTO request);
        Task<Generalresponse> LogOutAsync(string userId);
        Task<Generalresponse> ConfirmEmailAsync(ConfirmEmailDTO request);
        Task<Generalresponse> ChangePasswordAsync(ChangePaswwordDTO dto);
        Task<Generalresponse> ForgetPasswordAsync(string email);
        Task<Generalresponse> VerifyResetCodeAsync(VerifyCodeDTO codeDTO);
        Task<Generalresponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<Generalresponse> ResendConfirmationCodeAsync(string Email);
        Task<Generalresponse> ResendResetPasswordCodeAsync(string Email);
        Task<Generalresponse> RefreshTokenAsync(TokenRequestDTO request);
        Task<Generalresponse> RevokeAllTokensAsync(string userId);
    }
}
