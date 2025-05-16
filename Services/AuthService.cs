using Core.DTOs.AuthDTO;
using Core.DTOs.GeneralDTO;
using Core.Entities.Identity;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService emailService;
        private readonly IUserRepository repository;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly GoogleSettings _googleSettings;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IEmailService emailService, IUserRepository repository
                           , SignInManager<ApplicationUser> signInManager,
                           IOptions<JwtSettings> options, IOptions<GoogleSettings> GoogleOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.emailService = emailService;
            this.repository = repository;
            this.signInManager = signInManager;
            _googleSettings = GoogleOptions.Value;
            _jwtSettings = options.Value;
        }

        public async Task<Generalresponse> RegisterTraineeAsync(RegisterDTO model)
        {
            Generalresponse response = new();

            var userFromDb = await _userManager.FindByEmailAsync(model.Email);
            if (userFromDb != null)
            {
                response.IsSuccess = false;
                response.Data = "This email is already taken, please choose another.";
                return response;
            }

            var user = new Trainee
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                JoinedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Data = result.Errors.Select(e => e.Description).ToList();
                return response;
            }

            var roleExists = await _roleManager.RoleExistsAsync("Trainee");
            if (roleExists)
            {
                await _userManager.AddToRoleAsync(user, "Trainee");
            }
            else
            {
                response.IsSuccess = false;
                response.Data = "Trainee role does not exist.";
                return response;
            }

            await SendConfirmationEmail(user);
            response.IsSuccess = true;
            response.Data = "Registration successful. Check your email for the verification code.";
            return response;
        }

        public async Task<Generalresponse> RegisterCoachAsync(RegisterDTO model)
        {
            Generalresponse response = new Generalresponse();

            var userFromDb = await _userManager.FindByEmailAsync(model.Email);
            if (userFromDb != null)
            {
                response.IsSuccess = false;
                response.Data = "This email is already taken, please choose another.";
                return response;
            }

            Coach coach = new Coach
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                JoinedDate = DateTime.Now
            };

            var result = await _userManager.CreateAsync(coach, model.Password);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Data = result.Errors.Select(e => e.Description).ToList();
                return response;
            }

            var roleExists = await _roleManager.RoleExistsAsync("Coach");
            if (roleExists)
            {
                await _userManager.AddToRoleAsync(coach, "Coach");
            }
            else
            {
                response.IsSuccess = false;
                response.Data = "Coach role does not exist.";
                return response;
            }

            await SendConfirmationEmail(coach);
            response.IsSuccess = true;
            response.Data = "Registration successful. Check your email for the verification code.";
            return response;
        }

        public async Task<Generalresponse> LoginAsync(LoginDTO loginDTO)
        {
            Generalresponse response = new Generalresponse();

            ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    await SendConfirmationEmail(user);
                    response.IsSuccess = false;
                    response.Data = "Please check your email to confirm your account";
                    return response;
                }

                bool result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (result)
                {
                    var token = await GenerateJwtToken(user);
                    var refreshToken = GenerateRefreshToken();
                    user.refreshTokens?.Add(new RefreshToken()
                    {
                        Token = refreshToken,
                        Expires = DateTime.UtcNow.AddDays(3),
                        Created = DateTime.UtcNow
                    });

                    await repository.SaveAsync();

                    response.IsSuccess = true;
                    response.Data = new
                    {
                        token,
                        refreshToken,
                        exipiration = DateTime.UtcNow.AddMinutes(30)
                    };
                    return response;
                }
            }

            response.IsSuccess = false;
            response.Data = "Invalid email or password";
            return response;
        }
        public async Task<Generalresponse> SetUserRoleAsync(string userId, string role)
        {
            Generalresponse response = new Generalresponse();
            var oldUser = await _userManager.FindByIdAsync(userId);
            if (oldUser == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            ApplicationUser newUser;
            if (role == "Coach")
            {
                newUser = new Coach
                {
                    UserName = oldUser.UserName,
                    Email = oldUser.Email,
                    PhoneNumber = oldUser.PhoneNumber,
                    FirstName = oldUser.FirstName,
                    LastName = oldUser.LastName,
                    DateOfBirth = oldUser.DateOfBirth,
                    Gender = oldUser.Gender,
                    ProfilePictureUrl = oldUser.ProfilePictureUrl,
                    JoinedDate = oldUser.JoinedDate,
                    EmailConfirmed = oldUser.EmailConfirmed,
                };
            }
            else if (role == "Trainee")
            {
                newUser = new Trainee
                {
                    UserName = oldUser.UserName,
                    Email = oldUser.Email,
                    PhoneNumber = oldUser.PhoneNumber,
                    FirstName = oldUser.FirstName,
                    LastName = oldUser.LastName,
                    DateOfBirth = oldUser.DateOfBirth,
                    Gender = oldUser.Gender,
                    ProfilePictureUrl = oldUser.ProfilePictureUrl,
                    JoinedDate = oldUser.JoinedDate,
                    EmailConfirmed = oldUser.EmailConfirmed,
                };
            }
            else
            {
                response.IsSuccess = false;
                response.Data = "InValid Role";
                return response;
            }

            await _userManager.DeleteAsync(oldUser);

            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                response.IsSuccess = false;
                response.Data = result.Errors.Select(e => e.Description).ToList();
                return response;
            }

            await _userManager.AddToRoleAsync(newUser, role);

            var token = await GenerateJwtToken(newUser);
            var refreshToken = GenerateRefreshToken();
            newUser.refreshTokens?.Add(new RefreshToken()
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(3),
                Created = DateTime.UtcNow
            });

            await repository.SaveAsync();
            return new Generalresponse
            {
                IsSuccess = true,
                Data = new
                {
                    token,
                    refreshToken,
                    exipiration = DateTime.Now.AddHours(1)
                }
            };
        }
        public async Task<Generalresponse> GoogleLoginAsync(GoogleAuthDTO request)
        {
            try
            {
                var payload = await VerifyGoogleIdToken(request.IdToken);
                if (payload == null)
                    return new Generalresponse { IsSuccess = false, Data = "Invalid Google token" };

                var user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    var userInfo = await GetGoogleUserInfo(request.AccessToken);
                    if (userInfo == null)
                        return new Generalresponse { IsSuccess = false, Data = "Invalid Access Token or missing permissions." };

                    user = new ApplicationUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        FirstName = userInfo.FirstName,
                        LastName = userInfo.LastName,
                        ProfilePictureUrl = userInfo.Picture,
                        Gender = userInfo.Gender,
                        DateOfBirth = userInfo.Birthdate,
                    };
                    user.EmailConfirmed = true;
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        return new Generalresponse
                        {
                            IsSuccess = false,
                            Data = result.Errors.Select(e => e.Description).ToList()
                        };
                    }

                    var Checktoken = await GenerateJwtToken(user, setRole: true);

                    await repository.SaveAsync();
                    return new Generalresponse
                    {
                        IsSuccess = true,
                        Data = new
                        {
                            Checktoken,
                            exipiration = DateTime.Now.AddHours(1)
                        }
                    };
                }

                var token = await GenerateJwtToken(user);
                var refreshToken = GenerateRefreshToken();
                user.refreshTokens?.Add(new RefreshToken()
                {
                    Token = refreshToken,
                    Expires = DateTime.UtcNow.AddDays(3),
                    Created = DateTime.UtcNow
                });

                await repository.SaveAsync();
                return new Generalresponse
                {
                    IsSuccess = true,
                    Data = new
                    {
                        token,
                        refreshToken,
                        exipiration = DateTime.Now.AddHours(1)
                    }
                };
            }
            catch (Exception ex)
            {
                return new Generalresponse
                {
                    IsSuccess = false,
                    Data = $"An error occurred: {ex.Message}"
                };
            }
        }

        public async Task<Generalresponse> LogOutAsync(string userId)
        {
            Generalresponse response = new Generalresponse();
            var user = await repository.GetAsync(e => e.Id == userId
           , includeProperties: "refreshTokens");

            if (user == null)
                throw new UnauthorizedAccessException();

            if (user.refreshTokens != null)
            {
                foreach (var item in user.refreshTokens)
                {
                    item.Revoked = DateTime.UtcNow;
                }
            }

            await repository.SaveAsync();
            await signInManager.SignOutAsync();

            response.IsSuccess = true;
            response.Data = "SignOut Successfully";

            return response;
        }

        public async Task<Generalresponse> ConfirmEmailAsync(ConfirmEmailDTO request)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            var storedCode = await _userManager.GetAuthenticationTokenAsync(user,
                "EmailVerification", "VerificationCode");

            if (storedCode != request.VerificationCode)
            {
                response.IsSuccess = false;
                response.Data = "Invalid verification code.";
                return response;
            }

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = "Email successfully confirmed.";
                return response;
            }
            response.IsSuccess = false;
            response.Data = "Email confirmation failed.";
            return response;
        }

        public async Task<Generalresponse> ChangePasswordAsync(ChangePaswwordDTO dto, string userId)
        {
            Generalresponse response = new Generalresponse();


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found";
                return response;
            }

            var oldresult = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!oldresult)
            {
                response.IsSuccess = false;
                response.Data = "The old password is incorrect";
                return response;
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = "Password changed successfully";
                return response;
            }
            response.IsSuccess = false;
            response.Data = result.Errors.Select(e => e.Description).ToList();
            return response;
        }

        public async Task<Generalresponse> ForgetPasswordAsync(string email)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            try
            {
                var verificationCode = emailService.GenerateVerificatonCode();

                await _userManager.SetAuthenticationTokenAsync(user, "ResetPassword",
                    "ResetPasswordCode", verificationCode);

                await emailService.SendEmailAsync(email, "Password Reset Verification Code",
                $"Your verification code is: <b>{verificationCode}</b>");

                response.IsSuccess = true;
                response.Data = "If this email is registered, a verification code has been sent.";
                return response;
            }
            catch (Exception)
            {
                response.IsSuccess = false;
                response.Data = "An error occurred while sending the verification code. Please try again later.";
                return response;
            }

        }

        public async Task<Generalresponse> VerifyResetCodeAsync(VerifyCodeDTO codeDTO)
        {
            Generalresponse response = new Generalresponse();

            var userModel = await _userManager.FindByEmailAsync(codeDTO.Email);
            if (userModel == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            var savedCode = await _userManager.GetAuthenticationTokenAsync(userModel,
                    "ResetPassword", "ResetPasswordCode");

            if (savedCode == codeDTO.verificationCode)
            {
                await _userManager.RemoveAuthenticationTokenAsync(userModel, "ResetPassword", "ResetPasswordCode");

                var token = await GenerateJwtToken(userModel, true);

                response.IsSuccess = true;
                response.Data = token;
                return response;
            }
            response.IsSuccess = false;
            response.Data = "Invalid verification code.";
            return response;
        }

        public async Task<Generalresponse> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            Generalresponse response = new Generalresponse();
            var userModel = await _userManager.FindByEmailAsync(resetPasswordDTO.Email);
            if (userModel == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            var removePasswordResult = await _userManager.RemovePasswordAsync(userModel);

            if (!removePasswordResult.Succeeded)
            {
                response.IsSuccess = false;
                response.Data = "Failed to reset password.";
                return response;
            }

            var addPasswordResult = await _userManager.AddPasswordAsync(userModel, resetPasswordDTO.NewPassword);

            if (addPasswordResult.Succeeded)
            {
                response.IsSuccess = true;
                response.Data = "Password has been changed successfully.";
                return response;
            }

            response.IsSuccess = false;
            response.Data = "Invalid password format.";
            return response;
        }

        public async Task<Generalresponse> ResendConfirmationCodeAsync(string Email)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                response.IsSuccess = false;
                response.Data = "User email is not set.";
                return response;
            }

            if (user.EmailConfirmed)
            {
                response.IsSuccess = false;
                response.Data = "Email already confirmed.";
                return response;
            }
            var verificationCode = emailService.GenerateVerificatonCode();
            await _userManager.SetAuthenticationTokenAsync(user, "EmailVerification", "VerificationCode", verificationCode);

            var emailBody = $"Your verification code is: <b>{verificationCode}</b>";
            await emailService.SendEmailAsync(user.Email, "Resend Verification Code", emailBody);

            response.IsSuccess = true;
            response.Data = "Verification code sent.";
            return response;
        }

        public async Task<Generalresponse> ResendResetPasswordCodeAsync(string Email)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Data = "User not found.";
                return response;
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                response.IsSuccess = false;
                response.Data = "User email is not set.";
                return response;
            }

            var verificationCode = emailService.GenerateVerificatonCode();
            await _userManager.SetAuthenticationTokenAsync(user, "ResetPassword", "ResetPasswordCode", verificationCode);

            var emailBody = $"Your password reset verification code is: <b>{verificationCode}</b>";
            await emailService.SendEmailAsync(user.Email, "Password Reset Verification Code", emailBody);

            response.IsSuccess = true;
            response.Data = "Verification code sent for password reset.";
            return response;
        }

        public async Task<Generalresponse> RefreshTokenAsync(TokenRequestDTO request)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository
                .GetAsync(e => e.refreshTokens != null
                                    && e.refreshTokens.Any(t => t.Token == request.RefreshToken));
            if (user == null)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            var refreshtoken = user.refreshTokens?.Single(e => e.Token == request.RefreshToken);
            if (refreshtoken == null || !refreshtoken.IsActive)
                throw new UnauthorizedAccessException("Refresh token has expired.");

            var newjwttoken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            refreshtoken.Revoked = DateTime.UtcNow;

            user.refreshTokens?.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(3),
            });

            await repository.SaveAsync();

            TokenRequestDTO tokenRequest = new TokenRequestDTO()
            {
                Token = newjwttoken,
                RefreshToken = newRefreshToken,
            };

            response.IsSuccess = true;
            response.Data = tokenRequest;
            return response;
        }

        public async Task<Generalresponse> RevokeAllTokensAsync(string userId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == userId
                                     , includeProperties: "refreshTokens");
            if (user == null)
                throw new UnauthorizedAccessException();

            if (user.refreshTokens != null)
            {
                foreach (var item in user.refreshTokens)
                {
                    if (item.IsActive)
                        item.Revoked = DateTime.UtcNow;
                }
            }

            await repository.SaveAsync();

            response.IsSuccess = true;
            response.Data = "All tokens have been revoked";
            return response;
        }


        private async Task<Generalresponse> SendConfirmationEmail(ApplicationUser user)
        {
            Generalresponse response = new Generalresponse();

            if (string.IsNullOrEmpty(user.Email))
            {
                response.IsSuccess = false;
                response.Data = "User email is not set.";
                return response;
            }

            var verificationCode = emailService.GenerateVerificatonCode();
            user.EmailConfirmed = false;

            await _userManager.SetAuthenticationTokenAsync(user, "EmailVerification", "VerificationCode", verificationCode);

            var emailBody = $"Your verification code is: <b>{verificationCode}</b>";

            await emailService.SendEmailAsync(user.Email, "Verify your email", emailBody);

            response.IsSuccess = true;
            response.Data = "Confirmation Email send";
            return response;
        }
        private async Task<string> GenerateJwtToken(ApplicationUser user, bool? resetPassword = false, bool? setRole = false)
        {
            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty));
            userclaims.Add(new Claim(ClaimTypes.Email, user.Email ?? string.Empty));

            if (resetPassword == true)
            {
                userclaims.Add(new Claim("Purpose", "ResetPassword"));
            }
            else if (setRole == true)
            {
                userclaims.Add(new Claim("CheckRole", "NoRole"));
            }
            else
            {
                var Roles = await _userManager.GetRolesAsync(user);
                foreach (var Role in Roles)
                {
                    userclaims.Add(new Claim(ClaimTypes.Role, Role));
                }
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecritKey));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: _jwtSettings.IssuerIP,
               audience: _jwtSettings.AudienceIP,
               expires: DateTime.UtcNow.AddMinutes(30),
               claims: userclaims,
               signingCredentials: credentials
            );

            string result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleIdToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { _googleSettings.ClientID }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch
            {
                return null;
            }
        }

        public async Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://people.googleapis.com/v1/people/me?personFields=names,birthdays,emailAddresses,genders,photos");
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var googleData = JsonSerializer.Deserialize<GooglePeopleApiResponse>(
                 content,
                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (googleData == null)
                return null;

            var fullName = googleData.Names?.FirstOrDefault()?.DisplayName?.Split(" ") ?? Array.Empty<string>();
            string firstName = fullName?.Length > 0 ? fullName[0] : "";
            string lastName = fullName?.Length > 1 ? string.Join(" ", fullName.Skip(1)) : "";

            var birthDate = googleData.Birthdays?.FirstOrDefault()?.Date;
            DateTime? birthdate = (birthDate != null) ? new DateTime(birthDate.Year, birthDate.Month, birthDate.Day) : null;

            return new GoogleUserInfo
            {
                Email = googleData.EmailAddresses?.FirstOrDefault()?.Value ?? string.Empty,
                FirstName = firstName,
                LastName = lastName,
                Picture = googleData.Photos?.FirstOrDefault()?.Url ?? string.Empty,
                Gender = googleData.Genders?.FirstOrDefault()?.Value,
                Birthdate = birthdate
            };
        }
    }
}
