using Azure.Core;
using Core.DTOs.GymDTO;
using Core.DTOs.OnlineTrainingDTO;
using Core.DTOs.UserDTO;
using Core.Entities.GymEntities;
using Core.Entities.Identity;
using Core.Entities.OnlineTrainingEntities;
using Core.Helpers;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService emailService;
        private readonly IUserRepository repository;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IEmailService emailService, IUserRepository repository
                           , SignInManager<ApplicationUser> signInManager,
                           IOptions<JwtSettings> options)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.emailService = emailService;
            this.repository = repository;
            this.signInManager = signInManager;
            _jwtSettings = options.Value;
        }

        public async Task<Generalresponse> RegisterTraineeAsync(RegisterDTO model)
        {
            Generalresponse response = new Generalresponse();

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
                Bio = model.Bio,
                JoinedDate = DateTime.Now
            };

            if (model.ProfilePicture != null)
            {
                user.ProfilePictureUrl = await ImageHelper.SaveImageAsync(model.ProfilePicture, "Trainees");
            }

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

        public async Task<Generalresponse> RegisterCoachAsync(RegisterCoachDTO model)
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
                Bio = model.Bio,
                JoinedDate = DateTime.Now
            };

            if (model.ProfilePicture != null)
            {
                coach.ProfilePictureUrl = await ImageHelper.SaveImageAsync(model.ProfilePicture, "Coaches");
            }

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
                        exipiration = DateTime.Now.AddHours(1)
                    };
                    return response;
                }
            }

            response.IsSuccess = false;
            response.Data = "Invalid email or password";
            return response;
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

        public async Task<Generalresponse> ChangePasswordAsync(ChangePaswwordDTO dto)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(dto.Email);
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
            if (user != null)
            {
                var verificationCode = emailService.GenerateVerificatonCode();

                await _userManager.SetAuthenticationTokenAsync(user, "ResetPassword",
                    "ResetPasswordCode", verificationCode);

                await emailService.SendEmailAsync(email, "Password Reset Verification Code",
                $"Your verification code is: <b>{verificationCode}</b>");

                response.IsSuccess = true;
                response.Data = "Verification code sent for password reset";
                return response;

            }
            response.IsSuccess = false;
            response.Data = "User not found.";
            return response;
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

                var token = GenerateJwtToken(userModel, true);

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

        public async Task<Generalresponse> ResendConfirmationCodeAsync(ConfirmEmailDTO request)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByEmailAsync(request.Email);
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

        public async Task<Generalresponse> GetAllCoachesAsync()
        {
            Generalresponse response = new Generalresponse();
            var coaches = await repository.GetAllAsync(
                    expression: user => user is Coach,
                    includeProperties: "Gym,OnlineTrainings"
             );

            var coachDtos = coaches.Select(coach => new GetCoachDTO
            {
                Id = coach.Id,
                FirstName = coach.FirstName,
                LastName = coach.LastName,
                ProfilePictureUrl = coach.ProfilePictureUrl,
                Bio = coach.Bio,
                Gender = coach.Gender,
                JoinedDate = coach.JoinedDate,
                AvailableForOnlineTraining = ((Coach)coach).AvailableForOnlineTraining,
                Gym = coach is Coach returnCoach && returnCoach.Gym != null
                    ? new GymResponseDto
                    {
                        GymID = returnCoach.Gym.GymID,
                        GymName = returnCoach.Gym.GymName,
                        Address = returnCoach.Gym.Address,
                        City = returnCoach.Gym.City,
                        PictureUrl = returnCoach.Gym.PictureUrl,
                        Governorate = returnCoach.Gym.Governorate,
                        SubscriptionsCount = returnCoach.Gym.GymSubscriptions?.Count ?? 0,
                        AverageRating = (decimal)(returnCoach.Gym.Ratings != null &&
                                                  returnCoach.Gym.Ratings.Count != 0 ?
                                                  returnCoach.Gym.Ratings.Average(r => r.RatingValue) : 0)
                    }
                    : null,
                OnlineTrainingsGroup = ((Coach)coach).OnlineTrainings != null
                    ? ((Coach)coach).OnlineTrainings?.OfType<OnlineTrainingGroup>()
                    .Select(training => new GetOnlineTrainingGroupDTO
                    {
                        Id = training.Id,
                        Title = training.Title??"No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        NoOfSessionsPerWeek = training.NoOfSessionsPerWeek,
                        DurationOfSession = training.DurationOfSession,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null,
                OnlineTrainingsPrivate = ((Coach)coach).OnlineTrainings != null
                    ? ((Coach)coach).OnlineTrainings?.OfType<OnlineTrainingPrivate>()
                    .Select(training => new GetOnlineTrainingPrivateDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null
            });
            response.IsSuccess = true;
            response.Data = coachDtos;
            return response;
        }

        public async Task<Generalresponse> GetCoachDetailsAsync(string CoachId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == CoachId
                        , includeProperties: "Gym"
           );
            if (user == null)
            {
                response.IsSuccess = true;
                response.Data = "User Not Found.";
                return response;
            }

            var UserDto = new GetCoachDTO
            {
                Id = CoachId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Gender = user.Gender,
                JoinedDate = user.JoinedDate,
                AvailableForOnlineTraining = ((Coach)user).AvailableForOnlineTraining,
                Gym = user is Coach returnCoach && returnCoach.Gym != null
                    ? new GymResponseDto
                    {
                        GymID = returnCoach.Gym.GymID,
                        GymName = returnCoach.Gym.GymName,
                        Address = returnCoach.Gym.Address,
                        City = returnCoach.Gym.City,
                        PictureUrl = returnCoach.Gym.PictureUrl,
                        Governorate = returnCoach.Gym.Governorate,
                        SubscriptionsCount = returnCoach.Gym.GymSubscriptions?.Count ?? 0,
                        AverageRating = (decimal)(returnCoach.Gym.Ratings != null &&
                                                  returnCoach.Gym.Ratings.Count != 0 ?
                                                  returnCoach.Gym.Ratings.Average(r => r.RatingValue) : 0)
                    }
                    : null,
                OnlineTrainingsGroup = ((Coach)user).OnlineTrainings != null
                    ? ((Coach)user).OnlineTrainings?.OfType<OnlineTrainingGroup>()
                    .Select(training => new GetOnlineTrainingGroupDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        NoOfSessionsPerWeek = training.NoOfSessionsPerWeek,
                        DurationOfSession = training.DurationOfSession,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null,
                OnlineTrainingsPrivate = ((Coach)user).OnlineTrainings != null
                    ? ((Coach)user).OnlineTrainings?.OfType<OnlineTrainingPrivate>()
                    .Select(training => new GetOnlineTrainingPrivateDTO
                    {
                        Id = training.Id,
                        Title = training.Title ?? "No Title",
                        Description = training.Description ?? "No Description",
                        Price = training.Price,
                        OfferPrice = training.OfferPrice,
                        OfferEnded = training.OfferEnded,
                        SubscriptionClosed = training.SubscriptionClosed,
                        IsAvailable = training.IsAvailable
                    }).ToList()
                    : null
            };

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public async Task<Generalresponse> GetTraineeDetailsAsync(string TraineeId)
        {
            Generalresponse response = new Generalresponse();

            var user = await repository.GetAsync(e => e.Id == TraineeId);
            if (user == null)
            {
                response.IsSuccess = true;
                response.Data = "User Not Found.";
                return response;
            }

            var UserDto = new GetTraineeDTO
            {
                Id = TraineeId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Bio = user.Bio,
                Gender = user.Gender,
                JoinedDate = user.JoinedDate
            };

            response.IsSuccess = true;
            response.Data = UserDto;
            return response;
        }

        public async Task<Generalresponse> SetOnlineAvailabilityAsync(string userId, bool isAvailable)
        {
            Generalresponse response = new Generalresponse();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException();

            if (user is Coach coach)
            {
                coach.AvailableForOnlineTraining = isAvailable;

                await _userManager.UpdateAsync(coach);

                response.IsSuccess = true;
                response.Data = $"Availability status updated to: {isAvailable}";
                return response;
            }

            response.IsSuccess = false;
            response.Data = "Only coaches can set online availability.";
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

            response.IsSuccess = false;
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

            response.IsSuccess = false;
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
        private async Task<string> GenerateJwtToken(ApplicationUser user, bool? resetPassword = false)
        {
            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Name, user.UserName ?? string.Empty));

            if (resetPassword == true)
            {
                userclaims.Add(new Claim("Purpose", "ResetPassword"));
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
               expires: DateTime.Now.AddMinutes(30),
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
    }
}
