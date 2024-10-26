using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IUserRepository userRepository;
        private readonly IEmailService emailService;

        public AccountController(UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IConfiguration configuration, SignInManager<ApplicationUser> signInManager,
            IUserRepository userRepository, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.userRepository = userRepository;
            this.emailService = emailService;
        }

        [HttpPost("RegisterTrainee")]
        public async Task<ActionResult> RegisterTrainee(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByEmailAsync(model.Email);
                if (userFromDb != null)
                {
                    ModelState.AddModelError("Email", "This email is already taken, please choose another.");
                    return BadRequest(ModelState);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    Bio = model.Bio,
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    JoinedDate = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                var roleExists = await _roleManager.RoleExistsAsync("Trainee");
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(user, "Trainee");
                }
                else
                {
                    return BadRequest(new { message = "Trainee role does not exist." });
                }

                await SendConfirmationEmail(user);
                return Ok(new { message = "Trainee registered. Please check your email to confirm your account" });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("RegisterCoach")]
        public async Task<ActionResult> RegisterCoach(RegisterCoachDTO model)
        {
            if (ModelState.IsValid)
            {
                var userFromDb = await _userManager.FindByEmailAsync(model.Email);
                if (userFromDb != null)
                {
                    ModelState.AddModelError("Email", "This email is already taken, please choose another.");
                    return BadRequest(ModelState);
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
                    ProfilePictureUrl = model.ProfilePictureUrl,
                    JoinedDate = DateTime.Now,
                    AvailableForOnlineTraining = model.AvailableForOnlineTraining
                };

                var result = await _userManager.CreateAsync(coach, model.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return BadRequest(ModelState);
                }

                var roleExists = await _roleManager.RoleExistsAsync("Coach");
                if (roleExists)
                {
                    await _userManager.AddToRoleAsync(coach, "Coach");
                }
                else
                {
                    return BadRequest(new { message = "Coach role does not exist." });
                }

                await SendConfirmationEmail(coach);
                return Ok(new { message = "Coach registered. Please check your email to confirm your account" });
            }

            return BadRequest(ModelState);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

                if (user != null)
                {
                    if (!user.EmailConfirmed)
                    {
                        await SendConfirmationEmail(user);
                        return BadRequest("Please check your email to confirm your account");
                    }

                    bool result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                    if (result)
                    {
                        var token = await GenerateJwtToken(user);
                        var refreshToken = GenerateRefreshToken();
                        user.refreshTokens.Add(new RefreshToken()
                        {
                            Token = refreshToken,
                            Expires = DateTime.UtcNow.AddDays(3),
                            Created = DateTime.UtcNow
                        });

                        await userRepository.SaveAsync();

                        return Ok(new
                        {
                            token,
                            refreshToken,
                            exipiration = DateTime.Now.AddHours(1),
                        });
                    }
                }

                ModelState.AddModelError("", "The Email or pawword is invalid");
            }
            return BadRequest(ModelState);
        }

        [HttpGet("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await userRepository.GetAsync(e => e.Id == userId
            , includeProperties: "refreshTokens");

            if (user == null)
                return Unauthorized();


            foreach (var item in user.refreshTokens)
            {
                item.Revoked = DateTime.UtcNow;
            }

            await userRepository.SaveAsync();
            await signInManager.SignOutAsync();

            return Ok("SignOut Successfully");
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Invalid user ID");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok(new { message = "Email confirmed successfully!" });
            }

            return BadRequest("Email confirmation failed.");
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePaswwordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var oldresult = await _userManager.CheckPasswordAsync(user, dto.OldPassword);
            if (!oldresult)
            {
                return NotFound("The old password is incorrect");
            }

            var result = await _userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);

            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }
            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }
            return BadRequest(ModelState);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequestDTO request)
        {
            var user =await  userRepository
                .GetAsync(e => e.refreshTokens.Any(t => t.Token == request.RefreshToken));
            if (user == null)
                return Unauthorized("Invalid refresh token.");

            var refreshtoken = user.refreshTokens.Single(e => e.Token == request.RefreshToken);
            if (!refreshtoken.IsActive)
                return BadRequest("Refresh token has expired.");

            var newjwttoken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();
            refreshtoken.Revoked = DateTime.UtcNow;

            user.refreshTokens.Add(new RefreshToken
            {
                Token = newRefreshToken,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(3),
            });

            await userRepository.SaveAsync();

            TokenRequestDTO tokenRequest = new TokenRequestDTO()
            {
                Token = newjwttoken,
                RefreshToken = newRefreshToken,
            };

            return Ok(tokenRequest);
        }

        [HttpPost("RevokeAllTokens")]
        public async Task<ActionResult> RevokeAllTokens(TokenRequestDTO tokenRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await userRepository.GetAsync(e => e.Id == userId
                                     , includeProperties: "refreshTokens");
            if (user == null)
                return Unauthorized();

            foreach (var item in user.refreshTokens)
            {
                if(item.IsActive)
                    item.Revoked = DateTime.UtcNow;
            }

            await userRepository.SaveAsync();

            return Ok("All tokens have been revoked");
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            List<Claim> userclaims = new();
            userclaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            userclaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            userclaims.Add(new Claim(ClaimTypes.Name, user.UserName));
            var Roles = await _userManager.GetRolesAsync(user);
            foreach (var Role in Roles)
            {
                userclaims.Add(new Claim(ClaimTypes.Role, Role));
            }

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecritKey"]));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: configuration["JWT:IssuerIP"],
               audience: configuration["JWT:AudienceIP"],
               expires: DateTime.Now.AddHours(1),
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
        private async Task<IActionResult> SendConfirmationEmail(ApplicationUser user)
        {

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Account",
                new { userId = user.Id, token }, Request.Scheme);

            await emailService.SendEmailAsync(user.Email, "Confirm your email",
             $"Please confirm your account by clicking this link: <a href='{confirmationLink}'>link</a>");

            return Ok("Confirmation Email send");
        }
    }
}
