using Core.DTOs.AuthDTO;
using Core.DTOs.GeneralDTO;
using Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace API.Controllers
{
    public class AccountController(IAuthService service) : BaseApiController
    {

        [HttpPost("RegisterTrainee")]
        public async Task<ActionResult> RegisterTrainee(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await service.RegisterTraineeAsync(model);

            if (result.IsSuccess)
                return Created("", result);
            else if (result.Data is string message && message.Contains("already taken"))
                return Conflict(result);
            else if (result.Data is string roleMessage && roleMessage.Contains("does not exist"))
                return StatusCode(500, result);
            else
                return BadRequest(result);

        }

        [HttpPost("RegisterCoach")]
        public async Task<ActionResult> RegisterCoach(RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await service.RegisterCoachAsync(model);

            if (result.IsSuccess)
                return Created("", result);
            else if (result.Data is string message && message.Contains("already taken"))
                return Conflict(result);
            else if (result.Data is string roleMessage && roleMessage.Contains("does not exist"))
                return StatusCode(500, result);
            else
                return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await service.LoginAsync(loginDTO);

            if (result.IsSuccess)
                return Ok(result);
            else if (result.Data is string message && message.Contains("confirm your account"))
                return StatusCode(StatusCodes.Status403Forbidden, new { Message = message });
            else
                return BadRequest(result);

        }

        [HttpPost("GoogleLogin")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await service.GoogleLoginAsync(request);
            if (result.IsSuccess)
                return Ok(result);
            if (result.Data is string errorMessage)
            {
                if (errorMessage.Contains("Invalid Google token"))
                    return Unauthorized(result);
                if (errorMessage.Contains("Invalid Access Token"))
                    return Unauthorized(result);
            }

            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }

        [Authorize]
        [HttpPost("SetUserRole")]
        public async Task<IActionResult> SetUserRole([FromBody] SetRoleDTO request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var purposeClaim = User.FindFirst("CheckRole")?.Value;

            if (purposeClaim != "NoRole")
            {
                return Unauthorized(new Generalresponse
                {
                    IsSuccess = false,
                    Data = "Unauthorized access or invalid token."
                });
            }
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "Invalid token" });

            var result = await service.SetUserRoleAsync(userId, request.Role);
            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }
        [HttpPost("LogOut")]
        public async Task<ActionResult> LogOut()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("User is not authenticated");

            try
            {
                var result = await service.LogOutAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid user or session expired");
            }
        }

        [HttpPost("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDTO request)
        {
            var result = await service.ConfirmEmailAsync(request);

            if (result.IsSuccess)
                return Ok(result);
            else if (result.Data is string message && message.Contains("User not found"))
                return NotFound(result);
            else if (result.Data is string newMessage && newMessage.Contains("Invalid verification code"))
                return BadRequest(result);
            else
                return StatusCode(500, result);
        }
        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<ActionResult> ChangePassword(ChangePaswwordDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(string.IsNullOrEmpty(userId))
                return Unauthorized();

            var result = await service.ChangePasswordAsync(dto,userId);

            if (result.IsSuccess)
                return Ok(result);
            else if (result.Data is string message && message.Contains("User not found"))
                return NotFound(result);
            else if (result.Data is string newMessage && newMessage.Contains("The old password is incorrect"))
                return BadRequest(result);
            else
                return StatusCode(500, result);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] string email)
        {
            var result = await service.ForgetPasswordAsync(email);

            if (result.IsSuccess)
                return Ok(result);
            else if (result.Data is string message && message.Contains("User not found"))
                return NotFound(result);
            else
                return StatusCode(500, result);
        }

        [HttpPost("VerifyResetCode")]
        public async Task<IActionResult> VerifyResetCode(VerifyCodeDTO codeDTO)
        {
            var result = await service.VerifyResetCodeAsync(codeDTO);

            if (result.IsSuccess)
                return Ok(result);
            else if (result.Data is string message && message.Contains("User not found"))
                return NotFound(result);
            else
                return Unauthorized(result);
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
            else if (result.Data == "User not found.")
                return NotFound(result);
            else if (result.Data == "Failed to reset password.")
                return Conflict(result);
            else
                return BadRequest(result);

        }

        [HttpPost("resend-Confirmation-code")]
        public async Task<IActionResult> ResendConfirmationCode([FromBody] string Email)
        {
            var result = await service.ResendConfirmationCodeAsync(Email);

            if (result.IsSuccess)
                return Ok(result);
            if (result.Data == "User not found.")
                return NotFound(result);
            if (result.Data == "User email is not set.")
                return Conflict(result);
            if (result.Data == "Email already confirmed.")
                return BadRequest(result);

            return BadRequest(result);
        }

        [HttpPost("resend-reset-password-code")]
        public async Task<IActionResult> ResendResetPasswordCode([FromBody] string Email)
        {

            var result = await service.ResendResetPasswordCodeAsync(Email);

            if (result.IsSuccess)
                return Ok(result);
            if (result.Data == "User not found.")
                return NotFound(result);
            if (result.Data == "User email is not set.")
                return Conflict(result);
            return BadRequest(result);
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
                return Unauthorized(new Generalresponse
                {
                    IsSuccess = false,
                    Data = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("RevokeAllTokens")]
        public async Task<ActionResult> RevokeAllTokens()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new Generalresponse
                {
                    IsSuccess = false,
                    Data = "User ID is missing."
                });

            try
            {
                var result = await service.RevokeAllTokensAsync(userId);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new Generalresponse
                {
                    IsSuccess = false,
                    Data = "User not found or not allowed."
                });
            }
        }
    }
}
