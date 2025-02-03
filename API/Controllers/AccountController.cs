using Core.DTOs.UserDTO;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {

        private readonly IAuthService service;

        public AccountController(IAuthService service)
        {
            this.service = service;
        }

        [HttpPost("RegisterTrainee")]
        public async Task<ActionResult> RegisterTrainee([FromForm]RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await service.RegisterTraineeAsync(model);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);

        }

        [HttpPost("RegisterCoach")]
        public async Task<ActionResult> RegisterCoach([FromForm]RegisterCoachDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await service.RegisterCoachAsync(model);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await service.LoginAsync(loginDTO);
            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpGet("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await service.LogOutAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO request)
        {
            var result = await service.ConfirmEmailAsync(request);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePaswwordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await service.ChangePasswordAsync(dto);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] string email)
        {
            var result = await service.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);

        }
        [HttpPost("VerifyResetCode")]
        public async Task<IActionResult> VerifyResetCode(VerifyCodeDTO codeDTO)
        {
            var result = await service.VerifyResetCodeAsync(codeDTO);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            var emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;
            var purposeClaim = User.FindFirst("Purpose")?.Value;

            if (string.IsNullOrEmpty(emailClaim) || purposeClaim != "ResetPassword")
            {
                return Unauthorized(new Generalresponse
                {
                    IsSuccess = false,
                    Data = "Unauthorized access or invalid token purpose."
                });
            }


            var result = await service.ResetPasswordAsync(resetPassword);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);

        }

        [HttpPost("resend-Confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] ConfirmEmailDTO request)
        {
            var result = await service.ResendConfirmationCodeAsync(request);

            if (result.IsSuccess)
                return Ok(result);
            else
                return BadRequest(result);
        }

        [HttpPost("resend-reset-password-code")]
        public async Task<IActionResult> ResendResetPasswordCode([FromBody] string Email)
        {

            var result = await service.ResendResetPasswordCodeAsync(Email);

            if (result.IsSuccess)
                return Ok(result);
            else
                return NotFound(result);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken(TokenRequestDTO request)
        {
            try
            {
                var result = await service.RefreshTokenAsync(request);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("RevokeAllTokens")]
        public async Task<ActionResult> RevokeAllTokens()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized();

            try
            {
                var result = await service.RevokeAllTokensAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
