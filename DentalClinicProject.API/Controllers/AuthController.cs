using AutoMapper;
using DentalClinicProject.Core.DTOs;
using DentalClinicProject.Core.Interfaces.IRepository;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace DentalClinicProject.API.Controllers
{
    [EnableRateLimiting("AuthLimiter")]
    public class AuthController : BaseController
    {
        private readonly IValidator<VerifyLoginCodeDTO> _verifyLoginCodeValidator;
        private readonly IValidator<ResendEmailCodeDTO> _resendEmailCodeValidator;

        public AuthController(IUnitOfWork work, IMapper mapper,
            IValidator<VerifyLoginCodeDTO> verifyLoginCodeValidator,
            IValidator<ResendEmailCodeDTO> resendEmailCodeValidator) : base(work, mapper)
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

            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(LoginDTO dto)
        {
            var response = await work.AuthService.Login(dto);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-login-code")]
        public async Task<IActionResult> VerifyLoginCodeAsync(VerifyLoginCodeDTO dto)
        {
            var validationResult = await _verifyLoginCodeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    statusCode = 400,
                    message = "Validation failed",
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var response = await work.AuthService.VerifyLoginCode(dto.Identifier, dto.Code);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            var userId = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var response = await work.AuthService.LogoutAsync(userId!, accessToken);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }
        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAllAsync()
        {
            var userId = User.FindFirst("uid")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var response = await work.AuthService.LogoutAllAsync(userId!, accessToken);
            if (!response.Success)
                return BadRequest(new { errors = response.Errors ?? "Unknown", message = response.Message });

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO dto)
        {
            var response = await work.EmailVerificationService.VerifyEmailAsync(dto.Email, dto.Code);

            return StatusCode(response.StatusCode, response);
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
                return BadRequest(result);

            return Ok(result);
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
                return BadRequest(new { message = "Failed to resend verification code. Please try again later or check rate limit." });

            return Ok(new { message = "Verification code sent successfully" });
        }

        [HttpPost("resend-email-code")]
        public async Task<IActionResult> ResendEmailCode([FromBody] ResendEmailCodeDTO dto)
        {
            var validationResult = await _resendEmailCodeValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => e.ErrorMessage)
                });
            }

            var result = await work.EmailVerificationService.ResendEmailVerificationCodeAsync(dto.SessionToken);

            if (!result)
                return BadRequest(new { message = "Failed to resend verification code. Invalid session or rate limit exceeded." });

            return Ok(new { message = "Verification code sent successfully" });
        }
    }
}