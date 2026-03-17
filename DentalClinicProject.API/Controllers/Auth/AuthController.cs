using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.DTOs.Auth;
using DentalClinicProject.Core.Interfaces.IRepository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers.Auth
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : BaseController
    {
        private readonly IValidator<VerifyLoginCodeDTO> _verifyLoginCodeValidator;
        private readonly IValidator<ResendEmailCodeDTO> _resendEmailCodeValidator;

        public AuthController(IUnitOfWork work, IMapper mapper,
            IValidator<VerifyLoginCodeDTO> verifyLoginCodeValidator,
            IValidator<ResendEmailCodeDTO> resendEmailCodeValidator) : base(work)
        {
            _verifyLoginCodeValidator = verifyLoginCodeValidator;
            _resendEmailCodeValidator = resendEmailCodeValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO dto)
        {
            var response = await work.AuthService.Register(dto);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDTO dto)
        {
            var response = await work.AuthService.Login(dto);

            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            if (!string.IsNullOrEmpty(response.Data?.Token) && !string.IsNullOrEmpty(response.Data?.RefreshToken))
            {
                await SetCookies(response.Data.Token, response.Data.RefreshToken);
            }

            return Ok(new
            {
                message = response.Message,
                data = new
                {
                    response.Data?.Succeeded,
                    response.Data?.UserId,
                    response.Data?.Email,
                    response.Data?.Username,
                    response.Data?.Role
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDTO dto)
        {
            var response = await work.AuthService.RefreshTokenAsync(dto);

            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            if (!string.IsNullOrEmpty(response.Data?.Token) && !string.IsNullOrEmpty(response.Data?.RefreshToken))
            {
                await SetCookies(response.Data.Token, response.Data.RefreshToken);
            }

            return Ok(new { message = "Token refreshed successfully" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var response = await work.AuthService.LogoutAsync(userId!, accessToken);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("verify-login-code")]
        public async Task<IActionResult> VerifyLoginCodeAsync(VerifyLoginCodeDTO dto)
        {
            var validationResult = await _verifyLoginCodeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { message = "Validation failed" });

            var response = await work.AuthService.VerifyLoginCode(dto.Identifier, dto.Code);
            if (!response.Success)
                return BadRequest(new { message = "Verification failed" });

            if (!string.IsNullOrEmpty(response.Data?.Token) && !string.IsNullOrEmpty(response.Data?.RefreshToken))
                await SetCookies(response.Data.Token, response.Data.RefreshToken);

            return Ok(new { message = "Login code verified successfully" });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
        {
            var response = await work.EmailVerificationService.VerifyEmailAsync(dto.Email, dto.Code);

            if (!response.Success)
                return BadRequest(new { message = "Email verification failed" });

            if (!string.IsNullOrEmpty(response.Data?.Token) && !string.IsNullOrEmpty(response.Data?.RefreshToken))
                await SetCookies(response.Data.Token, response.Data.RefreshToken);

            return Ok(new { message = "Email verified successfully" });
        }

        [HttpPost("verify-phone")]
        [Authorize]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneDTO dto)
        {
            var userId = User.FindFirst("uid")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await work.PhoneVerificationService.VerifyPhoneAsync(userId, dto.Code);
            if (!result.Success)
                return BadRequest(new { message = "Phone verification failed" });

            if (!string.IsNullOrEmpty(result.Data?.Token) && !string.IsNullOrEmpty(result.Data?.RefreshToken))
                await SetCookies(result.Data.Token, result.Data.RefreshToken);

            return Ok(new { message = "Phone verified successfully" });
        }

        [HttpPost("resend-phone-code")]
        public async Task<IActionResult> ResendPhoneCode()
        {
            var userId = User.FindFirst("uid")?.Value ??
                         User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var result = await work.PhoneVerificationService.ResendPhoneVerificationCodeAsync(userId);
            if (!result)
                return BadRequest(new { message = "Failed to resend verification code" });

            return Ok(new { message = "Verification code sent successfully" });
        }

        [HttpPost("resend-email-code")]
        public async Task<IActionResult> ResendEmailCode([FromBody] ResendEmailCodeDTO dto)
        {
            var validationResult = await _resendEmailCodeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
                return BadRequest(new { message = "Validation failed" });

            var result = await work.EmailVerificationService.ResendEmailVerificationCodeAsync(dto.SessionToken);
            if (!result)
                return BadRequest(new { message = "Failed to resend verification code" });

            return Ok(new { message = "Verification code sent successfully" });
        }

        // ==================== Utilities ====================

        private Task SetCookies(string accessToken, string refreshToken)
        {
            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(15)
            });

            return Task.CompletedTask;
        }
    }
}